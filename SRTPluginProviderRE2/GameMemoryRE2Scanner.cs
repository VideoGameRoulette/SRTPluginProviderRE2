using ProcessMemory;
using SRTPluginProviderRE2.Structs.GameStructs;
using System;
using System.Diagnostics;
using Windows.Win32;

namespace SRTPluginProviderRE2
{
    internal class GameMemoryRE2Scanner : IDisposable
    {
        private readonly int MAX_ENTITES = 32;
        private readonly int MAX_ITEMS = 20;
        private readonly int mSize = 0x18;

        // Variables
        private GameVersion gv;
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public uint ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

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
            gameMemoryValues._items = new InventoryEntry[MAX_ITEMS];
            gameMemoryValues._enemies = new Enemy[MAX_ENTITES];
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            gv = SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));
            if (gv == GameVersion.Unknown)
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler((uint)pid);
            if (ProcessRunning)
            {
                BaseAddress = process?.MainModule?.BaseAddress ?? IntPtr.Zero; // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                InitGMV();
                PointerGameClock = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paGameClock));
                PointerGameRankSystem = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paGameRankSystem));
                PointerPlayerCondition = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paPlayerManager), 0x50, 0x10, 0x20);
                PointerInventoryManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x58);
                PointerEnemyManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paEnemyManager));
            }
        }

        private GameVersion SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE2_WW_11026357:
                    {
                        paEnemyManager = 0x091A6A08;
                        paGameClock = 0x091AEC78;
                        paGameRankSystem = 0x09184EC0;
                        paPlayerManager = 0x091AD1F0;
                        paInventoryManager = 0x091A6CD0;
                        return GameVersion.RE2_WW_11026357;
                    }
                case GameVersion.RE2_WW_11055033:
                    {
                        paEnemyManager = 0x070A69E0;
                        paGameClock = 0x070AEBB8;
                        paGameRankSystem = 0x070B8528;
                        paInventoryManager = 0x070B23A8;
                        paPlayerManager = 0x070AA850;
                        return GameVersion.RE2_WW_11055033;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return GameVersion.Unknown;
        }

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
            bool isDX12 = (gv == GameVersion.RE2_WW_11026357);
            // GameClock
            var gc = PointerGameClock.Deref<GameClock>(0x0);
            var gsd = memoryAccess.GetAt<GameClockGameSaveData>((nuint*)gc.GameSaveData);
            gameMemoryValues._timer.SetValues(gc, gsd);

            // GameRankSystem
            var grs = PointerGameRankSystem.Deref<GameRankSystem>(0x0);

            GameRankParameterData grpd;
            GameRankParameterDataDX11 grpdx11;
            
            if (isDX12)
            {
                grpd = memoryAccess.GetAt<GameRankParameterData>((nuint*)grs.GameRankParameter);
                dpc[0] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamEasy);
                dpc[1] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamNormal);
                dpc[2] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamHard);
            }
            else if (!isDX12)
            {
                grpdx11 = memoryAccess.GetAt<GameRankParameterDataDX11>((nuint*)grs.GameRankParameter);
                dpc[0] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpdx11.DifficultyParamEasy);
                dpc[1] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpdx11.DifficultyParamNormal);
                dpc[2] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpdx11.DifficultyParamHard);
            }
            
            gameMemoryValues._rankManager.SetValues(grs, dpc);

            // PlayerManager
            var pc = PointerPlayerCondition.Deref<PlayerCondition>(0x0);
            var cc = memoryAccess.GetAt<CostumeChanger>((nuint*)pc.CostumeChanger);
            var hpc = memoryAccess.GetAt<HitPointController>((nuint*)pc.HitPointController);
            gameMemoryValues._playerManager.SetValues(pc, cc, hpc);

            // InventoryManager
            var im = PointerInventoryManager.Deref<InventoryManager>(0x0);
            var invArray = memoryAccess.GetAt<ListInventory>((nuint*)im.Inventory);
            var inv = memoryAccess.GetAt<Inventory>((nuint*)invArray._ListInventory);
            var slots = memoryAccess.GetAt<Slots>((nuint*)inv.ListSlots);
            gameMemoryValues._inventoryCount = inv.CurrentSlotSize;
            gameMemoryValues._inventoryMaxCount = slots.Count;

            for (int i = 0; i < MAX_ITEMS; i++)
            {
                var position = (i * 0x8) + 0x20;
                var slotAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slots._Slots, position));
                var slot = memoryAccess.GetAt<Slot>(slotAddress);
                var itemAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slot._Slot, 0x10));
                var item = memoryAccess.GetAt<PrimitiveItem>(itemAddress);
                gameMemoryValues._items[i].SetValues(slot.Index, item);
            }

            // EnemyManager
            var em = PointerEnemyManager.Deref<EnemyManager>(0x0);
            gameMemoryValues._enemyKillCount = em.TotalEnemyKillCount;
            var el = memoryAccess.GetLongAt((nuint*)IntPtr.Add(em.EnemyList, 0x10));
            var ael = memoryAccess.GetLongAt((nuint*)IntPtr.Add(em.ActiveEnemyList, 0x10));
            gameMemoryValues._enemyCount = memoryAccess.GetIntAt((nuint*)IntPtr.Add(em.ActiveEnemyList, mSize));
            for (int i = 0; i < MAX_ENTITES; i++)
            {
                if (i > gameMemoryValues.EnemyCount)
                {
                    gameMemoryValues._enemies[i].SetValues(0, null);
                    continue;
                }
                var position = (i * 0x8) + 0x20;
                // EnemyList
                var eAddress = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)el, position));
                var ec1 = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)eAddress, 0x18));
                var ecc1 = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ec1, 0x20));
                var enemyType = memoryAccess.GetIntAt((nuint*)IntPtr.Add((IntPtr)ecc1, 0x54));
                // ActiveEnemyList
                var aeAddress = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ael, position));
                var hc = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)aeAddress, 0x218));
                var ehpc = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)hc, 0xB8));
                var enemyHP = memoryAccess.GetAt<HitPointController>(ehpc);
                gameMemoryValues._enemies[i].SetValues(enemyType, enemyHP);
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
