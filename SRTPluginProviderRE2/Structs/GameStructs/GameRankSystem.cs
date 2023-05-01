using System;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2.Structs.GameStructs
{
    public class RankManager
    {
        private int gameRank;
        private float rankPoint;
        private DifficultyParamClass[] gameRankParameterData;

        public int GameRank { get => gameRank; set => gameRank = value; }
        public float RankPoint { get => rankPoint; set => rankPoint = value; }
        public DifficultyParamClass[] GameRankParameterData { get => gameRankParameterData; set => gameRankParameterData = value; }

        public RankManager()
        {
            gameRank = 0;
            rankPoint = 0;
            gameRankParameterData = new DifficultyParamClass[3];
        }

        public void SetValues(GameRankSystem grs, DifficultyParamClass[] dpc)
        {
            gameRank = grs.GameRank;
            rankPoint = grs.RankPoint;
            gameRankParameterData = dpc;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x88)]
    public struct GameRankSystem
    {
        [FieldOffset(0x58)] private int gameRank;
        [FieldOffset(0x5C)] private float rankPoint;
        [FieldOffset(0x70)] private nint gameRankParameter;

        public int GameRank => gameRank;
        public float RankPoint => rankPoint;
        public IntPtr GameRankParameter => IntPtr.Add(gameRankParameter, 0x0);
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x88)]
    public struct GameRankParameterDataDX11
    {
        [FieldOffset(0x30)] private nint difficultyParamEasy;
        [FieldOffset(0x38)] private nint difficultyParamNormal;
        [FieldOffset(0x40)] private nint difficultyParamHard;

        public IntPtr DifficultyParamEasy => IntPtr.Add(difficultyParamEasy, 0x0);
        public IntPtr DifficultyParamNormal => IntPtr.Add(difficultyParamNormal, 0x0);
        public IntPtr DifficultyParamHard => IntPtr.Add(difficultyParamHard, 0x0);
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x88)]
    public struct GameRankParameterData
    {
        [FieldOffset(0x20)] private nint difficultyParamEasy;
        [FieldOffset(0x28)] private nint difficultyParamNormal;
        [FieldOffset(0x30)] private nint difficultyParamHard;

        public IntPtr DifficultyParamEasy => IntPtr.Add(difficultyParamEasy, 0x0);
        public IntPtr DifficultyParamNormal => IntPtr.Add(difficultyParamNormal, 0x0);
        public IntPtr DifficultyParamHard => IntPtr.Add(difficultyParamHard, 0x0);
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x1C)]
    public struct DifficultyParamClass
    {
        [FieldOffset(0x10)] private float defPoint;
        [FieldOffset(0x14)] private float minPoint;
        [FieldOffset(0x18)] private float maxPoint;

        public float DefPoint => defPoint;
        public float MinPoint => minPoint;
        public float MaxPoint => maxPoint;
    }
}