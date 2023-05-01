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

        public int EnemyType { get => enemyType; set => enemyType = value; }
        public int CurrentHP { get => currentHP; set => currentHP = value; }
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public bool IsAlive => CurrentHP > 0;

        public void SetValues(int et, HitPointController? hpc)
        {
            EnemyType = et;
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
}