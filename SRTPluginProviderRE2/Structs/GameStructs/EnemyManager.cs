using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2.Structs.GameStructs
{
    public class Enemy
    {
        public Enemy()
        {
        }

        public void SetValues()
        {
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x2A0)]
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