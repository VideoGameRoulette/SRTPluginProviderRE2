using SRTPluginProviderRE2.Structures;
using System;

namespace SRTPluginProviderRE2
{
    public interface IGameMemoryRE2
    {
        int PlayerCurrentHealth { get; set; }
        int PlayerMaxHealth { get; set; }
        InventoryEntry[] PlayerInventory { get; set; }
        EnemyHP[] EnemyHealth { get; set; }
        long IGTRunningTimer { get; set; }
        long IGTCutsceneTimer { get; set; }
        long IGTMenuTimer { get; set; }
        long IGTPausedTimer { get; set; }
        int Rank { get; set; }
        float RankScore { get; set; }
        bool IsRunning { get; set; }
        bool IsCutscene { get; set; }
        bool IsMenu { get; set; }
        bool IsPaused { get; set; }
        long IGTCalculated { get; }
        long IGTCalculatedTicks { get; }
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
    }
}
