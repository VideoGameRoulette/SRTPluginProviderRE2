using ProcessMemory;
using SRTPluginProviderRE2.Structs.GameStructs;
using System;
using System.Diagnostics;

namespace SRTPluginProviderRE2
{
    internal class GameMemoryRE2Scanner : IDisposable
    {
        // private readonly int MAX_ENTITES = 32;
        private readonly int MAX_ITEMS = 20;
        private readonly int mSize = 0x18;
        private long _address;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int paGameClock;
        private int paGameRankSystem;
        private int paPlayerManager;
        private int paInventoryManager;
        private int paEnemyManager;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameClock { get; set; }
        private MultilevelPointer PointerGameRankSystem { get; set; }
        private MultilevelPointer PointerPlayerCondition { get; set; }
        private MultilevelPointer PointerInventoryManager { get; set; }
        private MultilevelPointer PointerEnemyManager { get; set; }

        // private InventoryEntry EmptySlot = new InventoryEntry();
        private DifficultyParamClass[] dpc;

        internal GameMemoryRE2Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE2();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void InitGMV()
        {
            dpc = new DifficultyParamClass[3];
            gameMemoryValues._timer = new GameTimer();
            gameMemoryValues._rankManager = new RankManager();
            gameMemoryValues._playerManager = new Player();
            gameMemoryValues._items = new InventoryEntry[20];
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                InitGMV();
                PointerGameClock = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, paGameClock));
                PointerGameRankSystem = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, paGameRankSystem));
                PointerPlayerCondition = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, paPlayerManager), 0x50, 0x10, 0x20);
                PointerInventoryManager = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, paInventoryManager), 0x58);
                PointerEnemyManager = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, paEnemyManager), 0x50);
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE2_WW_20230418_1:
                    {
                        paGameRankSystem = 0x09184EC0;
                        paPlayerManager = 0x091AD1F0;
                        paGameClock = 0x091AEC78;
                        paInventoryManager = 0x091A6CD0;
                        paEnemyManager = 0x091A6A08;
                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerGameClock.UpdatePointers();
            PointerGameRankSystem.UpdatePointers();
            PointerPlayerCondition.UpdatePointers();
            PointerInventoryManager.UpdatePointers();
            PointerEnemyManager.UpdatePointers();
        }

        internal unsafe IGameMemoryRE2 Refresh()
        {
            // GameClock
            var gc = PointerGameClock.Deref<GameClock>(0x0);
            var gsd = memoryAccess.GetAt<GameClockGameSaveData>(gc.GameSaveData);
            gameMemoryValues._timer.SetValues(gc, gsd);

            // GameRankSystem
            var grs = PointerGameRankSystem.Deref<GameRankSystem>(0x0);
            var grpd = memoryAccess.GetAt<GameRankParameterData>(grs.GameRankParameter);
            dpc[0] = memoryAccess.GetAt<DifficultyParamClass>(grpd.DifficultyParamEasy);
            dpc[1] = memoryAccess.GetAt<DifficultyParamClass>(grpd.DifficultyParamNormal);
            dpc[2] = memoryAccess.GetAt<DifficultyParamClass>(grpd.DifficultyParamHard);
            gameMemoryValues._rankManager.SetValues(grs, dpc);

            // PlayerManager
            var pc = PointerPlayerCondition.Deref<PlayerCondition>(0x0);
            var cc = memoryAccess.GetAt<CostumeChanger>(pc.CostumeChanger);
            var hpc = memoryAccess.GetAt<HitPointController>(pc.HitPointController);
            gameMemoryValues._playerManager.SetValues(pc, cc, hpc);

            // InventoryManager
            var im = PointerInventoryManager.Deref<InventoryManager>(0x0);
            var invArray = memoryAccess.GetAt<ListInventory>(im.Inventory);
            var inv = memoryAccess.GetAt<Inventory>(invArray._ListInventory);
            var slots = memoryAccess.GetAt<Slots>(inv.ListSlots);
            gameMemoryValues._inventoryCount = inv.CurrentSlotSize;
            gameMemoryValues._inventoryMaxCount = slots.Count;

            for (int i = 0; i < slots.Count; i++)
            {
                var position = (i * 0x8) + 0x20;
                var slotAddress = (long*)memoryAccess.GetLongAt(IntPtr.Add(slots._Slots, position));
                var slot = memoryAccess.GetAt<Slot>(slotAddress);
                var itemAddress = (long*)memoryAccess.GetLongAt(IntPtr.Add(slot._Slot, 0x10));
                var item = memoryAccess.GetAt<PrimitiveItem>(itemAddress);
                gameMemoryValues._items[i].SetValues(slot.Index, item);
            }
            
            // TO-DO LOOP THRU INVENTROY ARRAY AND UPDATE GAME MEMORY VALUES FOR DATA

            // EnemyManager
            gameMemoryValues._enemyCount = PointerEnemyManager.DerefInt(mSize);
            var enemies = PointerEnemyManager.Deref<EnemyManager>(0x0);
            gameMemoryValues._enemyKillCount = enemies.TotalEnemyKillCount;

            // TO-DO LOOP THRU ENEMY ARRAY AND UPDATE GAME MEMORY VALUES FOR DATA

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
