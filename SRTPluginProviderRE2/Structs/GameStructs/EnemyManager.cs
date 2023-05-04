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

        public KindID EnemyID { get => (KindID)enemyType; set => enemyType = (int)value; }
        public string EnemyTypeString => EnemyID.ToString();
        public int CurrentHP { get => currentHP; set => currentHP = value; }
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP > 0;

        public void SetValues(int et, HitPointController? hpc)
        {
            EnemyID = (KindID)et;
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

    public enum KindID : int
    {
        em0000 = 0,
        em0100 = 1,
        em0200 = 2,
        em3000 = 3,
        em4000 = 4,
        em4100 = 5,
        em4400 = 6,
        em5000 = 7,
        em6000 = 8,
        em6100 = 9,
        em6200 = 10,
        em6300 = 11,
        em7000 = 12,
        em7100 = 13,
        em7110 = 14,
        em7200 = 15,
        em7300 = 16,
        em7400 = 17,
        em9000 = 18,
        em8000 = 19,
        em8100 = 20,
        em8200 = 21,
        em8300 = 22,
        em8400 = 23,
        em8500 = 24,
        em9999 = 25,
        MAX = 26,
        Invalid = -1,
    };
}