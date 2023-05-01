using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2.Structs.GameStructs
{
    public struct Enemy
    {
        private int enemyType;
        private int currentHP;
        private int maxHP;

        public EnemyType EnemyType { get => (EnemyType)enemyType; set => enemyType = (int)value; }
        public string EnemyTypeString => EnemyType.ToString();
        public int CurrentHP { get => currentHP; set => currentHP = value; }
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP > 0;

        public void SetValues(int et, HitPointController? hpc)
        {
            EnemyType = (EnemyType)et;
            CurrentHP = hpc?.CurrentHP ?? 0;
            MaxHP = hpc?.MaxHP ?? 0;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x180)]
    public struct EnemyManager
    {
        [FieldOffset(0x50)] private nint enemyList;
        [FieldOffset(0x58)] private nint activeEnemyList;
        [FieldOffset(0x150)] private int totalEnemyKillCount;
        public IntPtr EnemyList => IntPtr.Add(enemyList, 0x0);
        public IntPtr ActiveEnemyList => IntPtr.Add(activeEnemyList, 0x0);
        public int TotalEnemyKillCount => totalEnemyKillCount;
    }

    public enum EnemyType : int
    {
        ZOMBIE = 0,
        EM3000 = 1,
        EM4000 = 2,
        EM4100 = 3,
        EM4400 = 4,
        EM5000 = 5,
        EM6000 = 6,
        EM6100 = 7,
        EM6200 = 8,
        EM6300 = 9,
        EM7000 = 10,
        EM7100 = 11,
        EM7110 = 12,
        EM7200 = 13,
        EM7300 = 14,
        EM7400 = 15,
        EM9000 = 16,
    };
}