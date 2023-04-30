using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2.Structs.GameStructs
{
    public class GameTimer
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss";
        private bool measureGameElapsedTime;
        private bool measureDemoSpendingTime;
        private bool measureInventorySpendingTime;
        private bool measurePauseSpendingTime;
        private GameClockGameSaveData gameSaveData;

        public bool MeasureGameElapsedTime { get => measureGameElapsedTime; set => measureGameElapsedTime = value; }
        public bool MeasureDemoSpendingTime { get => measureDemoSpendingTime; set => measureDemoSpendingTime = value; }
        public bool MeasureInventorySpendingTime { get => measureInventorySpendingTime; set => measureInventorySpendingTime = value; }
        public bool MeasurePauseSpendingTime { get => measurePauseSpendingTime; set => measurePauseSpendingTime = value; }
        public GameClockGameSaveData GameSaveData { get => gameSaveData; set => gameSaveData = value; }
        private long IGTCalculated => unchecked(GameSaveData.GameElapsedTime - GameSaveData.DemoSpendingTime - GameSaveData.PauseSpendingTime);
        private long IGTCalculatedTicks => unchecked(IGTCalculated * 10L);
        private TimeSpan IGTTimeSpan
        {
            get
            {
                TimeSpan timespanIGT;

                if (IGTCalculatedTicks <= TimeSpan.MaxValue.Ticks)
                    timespanIGT = new TimeSpan(IGTCalculatedTicks);
                else
                    timespanIGT = new TimeSpan();

                return timespanIGT;
            }
        }
        public string IGTFormattedString => IGTTimeSpan.ToString(IGT_TIMESPAN_STRING_FORMAT, CultureInfo.InvariantCulture);

        public GameTimer()
        {
            measureGameElapsedTime = false;
            measureDemoSpendingTime = false;
            measureInventorySpendingTime = false;
            measurePauseSpendingTime = false;
            gameSaveData = default(GameClockGameSaveData);
        }

        public void SetValues(GameClock gc, GameClockGameSaveData gsd)
        {
            measureGameElapsedTime = gc.MeasureGameElapsedTime;
            measureDemoSpendingTime = gc.MeasureDemoSpendingTime;
            measureInventorySpendingTime = gc.MeasureInventorySpendingTime;
            measurePauseSpendingTime = gc.MeasurePauseSpendingTime;
            gameSaveData = gsd;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x80)]
    public struct GameClock
    {
        [FieldOffset(0x50)] private byte measureGameElapsedTime;
        [FieldOffset(0x51)] private byte measureDemoSpendingTime;
        [FieldOffset(0x52)] private byte measureInventorySpendingTime;
        [FieldOffset(0x53)] private byte measurePauseSpendingTime;
        [FieldOffset(0x60)] private nint gameSaveData;

        public bool MeasureGameElapsedTime => measureGameElapsedTime != 0;
        public bool MeasureDemoSpendingTime => measureDemoSpendingTime != 0;
        public bool MeasureInventorySpendingTime => measureInventorySpendingTime != 0;
        public bool MeasurePauseSpendingTime => measurePauseSpendingTime != 0;
        public IntPtr GameSaveData => IntPtr.Add(gameSaveData, 0x0);
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x38)]
    public struct GameClockGameSaveData
    {
        [FieldOffset(0x18)] private long gameElapsedTime;
        [FieldOffset(0x20)] private long demoSpendingTime;
        [FieldOffset(0x28)] private long inventorySpendingTime;
        [FieldOffset(0x30)] private long pauseSpendingTime;

        public long GameElapsedTime => gameElapsedTime;
        public long DemoSpendingTime => demoSpendingTime;
        public long InventorySpendingTime => inventorySpendingTime;
        public long PauseSpendingTime => pauseSpendingTime;
    }
}