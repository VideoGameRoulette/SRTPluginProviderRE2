using SRTPluginProviderRE2.Structs.GameStructs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace SRTPluginProviderRE2
{
    public struct GameMemoryRE2 : IGameMemoryRE2
    {
        public string GameName => "RE2R";

        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public GameTimer Timer { get => _timer; set => _timer = value; }
        internal GameTimer _timer;

        public RankManager RankManager { get => _rankManager; set => _rankManager = value; }
        internal RankManager _rankManager;

        public Player PlayerManager { get => _playerManager; set => _playerManager = value; }
        internal Player _playerManager;

        public int InventoryCount { get => _inventoryCount; set => _inventoryCount = value; }
        internal int _inventoryCount;
        public int InventoryMaxCount { get => _inventoryMaxCount; set => _inventoryMaxCount = value; }
        internal int _inventoryMaxCount;

        public int EnemyCount { get => _enemyCount; set => _enemyCount = value; }
        internal int _enemyCount;

        public int EnemyKillCount { get => _enemyKillCount; set => _enemyKillCount = value; }
        internal int _enemyKillCount;
    }
}
