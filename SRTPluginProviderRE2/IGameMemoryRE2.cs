using SRTPluginProviderRE2.Structs;
using SRTPluginProviderRE2.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE2
{
    public interface IGameMemoryRE2
    {
        string GameName { get; }

        string VersionInfo { get; }

        GameTimer Timer { get; }

        RankManager RankManager { get; }

        Player PlayerManager { get; }

        int InventoryCount { get; }

        InventoryEntry[] Items { get; }

        int EnemyCount { get; }

        int EnemyKillCount { get; }
    }
}
