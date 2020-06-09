using ProcessMemory;
using SRTPluginProviderRE2.Structures;
using System;
using System.Diagnostics;

namespace SRTPluginProviderRE2
{
    internal class GameMemoryRE2Scanner : IDisposable
    {
        // Variables
        private ProcessMemory.ProcessMemory memoryAccess;
        private GameMemoryRE2 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private long pointerAddressIGT;
        private long pointerAddressRank;
        private long pointerAddressHP;
        private long pointerAddressInventory;
        private long pointerAddressEnemy;

        // Pointer Classes
        private long BaseAddress { get; set; }
        private MultilevelPointer PointerIGT { get; set; }
        private MultilevelPointer PointerRank { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer[] PointerEnemyEntries { get; set; }
        private MultilevelPointer[] PointerInventoryEntries { get; set; }
        //private MultilevelPointer PointerInventoryCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        internal GameMemoryRE2Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE2();
            if (process != null)
                Initialize(process);
        }
        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            SelectPointerAddresses();

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemory.ProcessMemory(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT).ToInt64(); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                PointerIGT = new MultilevelPointer(memoryAccess, BaseAddress + pointerAddressIGT, 0x2E0L, 0x218L, 0x610L, 0x710L, 0x60L);
                PointerRank = new MultilevelPointer(memoryAccess, BaseAddress + pointerAddressRank);
                PointerPlayerHP = new MultilevelPointer(memoryAccess, BaseAddress + pointerAddressHP, 0x50L, 0x20L);
                GenerateEnemyEntries();

                PointerInventoryEntries = new MultilevelPointer[20];
                for (long i = 0; i < PointerInventoryEntries.Length; ++i)
                    PointerInventoryEntries[i] = new MultilevelPointer(memoryAccess, BaseAddress + pointerAddressInventory, 0x50L, 0x98L, 0x10L, 0x20L + (i * 0x08L), 0x18L);
            }
        }

        private void SelectPointerAddresses()
        {
            pointerAddressIGT = 0x07097EF8;
            pointerAddressRank = 0x070A7C88;
            pointerAddressHP = 0x070A17E0;
            pointerAddressInventory = 0x070A17E0;
            pointerAddressEnemy = 0x070960E0;
        }

        /// <summary>
        /// Dereferences a 4-byte signed integer via the PointerEnemyEntryCount pointer to detect how large the enemy pointer table is and then create the pointer table entries if required.
        /// </summary>
        private void GenerateEnemyEntries()
        {
            PointerEnemyEntries = new MultilevelPointer[32]; // Create a new enemy pointer table array with the detected size.
            for (long i = 0; i < PointerEnemyEntries.Length; ++i) // Loop through and create all of the pointers for the table.
                PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, BaseAddress + pointerAddressEnemy, 0x80L + (i * 0x08L), 0x88L, 0x18L, 0x1A0L);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerIGT.UpdatePointers();
            PointerPlayerHP.UpdatePointers();
            PointerRank.UpdatePointers();

            GenerateEnemyEntries(); // This has to be here for the next part.

            for (int i = 0; i < PointerEnemyEntries.Length; ++i)
                PointerEnemyEntries[i].UpdatePointers();

            //PointerInventoryCount.UpdatePointers();
            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
                PointerInventoryEntries[i].UpdatePointers();

        }

        internal IGameMemoryRE2 Refresh()
        {
            // IGT
            gameMemoryValues.IGTRunningTimer = PointerIGT.DerefLong(0x18);
            gameMemoryValues.IGTCutsceneTimer = PointerIGT.DerefLong(0x20);
            gameMemoryValues.IGTMenuTimer = PointerIGT.DerefLong(0x28);
            gameMemoryValues.IGTPausedTimer = PointerIGT.DerefLong(0x30);

            // Player HP
            gameMemoryValues.PlayerMaxHealth = PointerPlayerHP.DerefInt(0x54);
            gameMemoryValues.PlayerCurrentHealth = PointerPlayerHP.DerefInt(0x58);
            gameMemoryValues.Rank = PointerRank.DerefInt(0x58);
            gameMemoryValues.RankScore = PointerRank.DerefFloat(0x5C);

            // Enemy HP
            GenerateEnemyEntries();
            if (gameMemoryValues.EnemyHealth == null)
            {
                gameMemoryValues.EnemyHealth = new EnemyHP[32];
                for (int i = 0; i < gameMemoryValues.EnemyHealth.Length; ++i)
                    gameMemoryValues.EnemyHealth[i] = new EnemyHP();
            }
            for (int i = 0; i < gameMemoryValues.EnemyHealth.Length; ++i)
            {
                if (i < PointerEnemyEntries.Length)
                { // While we're within the size of the enemy table, set the values.
                    gameMemoryValues.EnemyHealth[i].MaximumHP = PointerEnemyEntries[i].DerefInt(0x54);
                    gameMemoryValues.EnemyHealth[i].CurrentHP = PointerEnemyEntries[i].DerefInt(0x58);
                }
                else
                { // We're beyond the current size of the enemy table. It must have shrunk because it was larger before but for the sake of performance, we're not going to constantly recreate the array any time the size doesn't match. Just blank out the remaining array values.
                    gameMemoryValues.EnemyHealth[i].MaximumHP = 0;
                    gameMemoryValues.EnemyHealth[i].CurrentHP = 0;
                }
            }

            // Inventory
            //gameMemoryValues.PlayerInventoryCount = PointerInventoryCount.DerefInt(0x90);
            if (gameMemoryValues.PlayerInventory == null)
            {
                gameMemoryValues.PlayerInventory = new InventoryEntry[20];
                for (int i = 0; i < gameMemoryValues.PlayerInventory.Length; ++i)
                    gameMemoryValues.PlayerInventory[i] = new InventoryEntry();
            }
            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
            {
                //if (i < gameMemoryValues.PlayerInventoryCount)
                //{
                    long invDataOffset = PointerInventoryEntries[i].DerefLong(0x10) - PointerInventoryEntries[i].Address;
                    gameMemoryValues.PlayerInventory[i].SetValues(PointerInventoryEntries[i].DerefInt(0x28), PointerInventoryEntries[i].DerefByteArray(invDataOffset + 0x10, 0x14));
                //}
                //else
                    //gameMemoryValues.PlayerInventory[i].SetValues(PointerInventoryEntries[i].DerefInt(0x28), null);
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
