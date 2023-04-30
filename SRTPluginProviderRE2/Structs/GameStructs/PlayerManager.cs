using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2.Structs.GameStructs
{
    public class Player
    {
        private SurvivorType currentSurvivor;
        private Costume currentCostume;
        private HitPointController hitPointController;
        private bool isPoisoned;
        private Vec3 position;

        public SurvivorType CurrentSurvivor { get => currentSurvivor; set => currentSurvivor = value; }
        public Costume CurrentCostume { get => currentCostume; set => currentCostume = value; }
        public HitPointController Health { get => hitPointController; set => hitPointController = value; }
        public bool IsPoisoned { get => isPoisoned; set => isPoisoned = value; }
        public Vec3 Position { get => position; set => position = value; }

        public Player()
        {
            currentSurvivor = default(SurvivorType);
            currentCostume = default(Costume);
            hitPointController = default(HitPointController);
            isPoisoned = false;
            position = new Vec3(0,0,0);
        }

        public void SetValues(PlayerCondition pc, CostumeChanger cc, HitPointController hpc)
        {
            currentSurvivor = pc.SurvivorType;
            currentCostume = cc.CurrentCostume;
            hitPointController = hpc;
            isPoisoned = pc.Poison;
            position.Update(pc.X, pc.Y, pc.Z);
        }
    }

    public class Vec3
    {
        private float x;
        private float y;
        private float z;
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Z { get => z; set => z = value; }

        public Vec3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Update(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x2A0)]
    public struct PlayerCondition
    {
        [FieldOffset(0x54)] private int survivorType;
        [FieldOffset(0x1F8)] private nint costumeChanger;
        [FieldOffset(0x230)] private nint hitPointController;
        [FieldOffset(0x258)] private byte poison;
        [FieldOffset(0x290)] private Vector3 pastPosition;

        public SurvivorType SurvivorType => (SurvivorType)survivorType;
        public string SurvivorTypeString => SurvivorType.ToString();
        public IntPtr CostumeChanger => IntPtr.Add(costumeChanger, 0x0);
        public IntPtr HitPointController => IntPtr.Add(hitPointController, 0x0);
        public bool Poison => poison != 0;
        public float X => pastPosition.X;
        public float Y => pastPosition.Y;
        public float Z => pastPosition.Z;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xD0)]
    public struct CostumeChanger
    {
        [FieldOffset(0x54)] private int currentCostume;

        public Costume CurrentCostume => (Costume)currentCostume;
        public string CurrentCostumeString => CurrentCostume.ToString();
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x60)]
    public struct HitPointController
    {
        [FieldOffset(0x54)] private int defaultHitPoint;
        [FieldOffset(0x58)] private int currentHitPoint;
        [FieldOffset(0x5C)] private byte invincible;
        [FieldOffset(0x5D)] private byte noDamage;

        public int MaxHP => defaultHitPoint;
        public int CurrentHP => currentHitPoint;
        public bool Invincible => invincible != 0;
        public bool NoDamage => noDamage != 0;
    }

    public enum SurvivorType : int
    {
        Invalid = -1,
        Leon = 0,
        Claire = 1,
        PL2000 = 2,
        PL3000 = 3,
        PL4000 = 4,
        PL4100 = 5,
        PL5000 = 6,
        PL5100 = 7,
        PL5200 = 8,
        PL5300 = 9,
        PL5400 = 10,
        PL5500 = 11,
        PL5600 = 12,
        PL5700 = 13,
        PL5800 = 14,
        PL5900 = 15,
        PL6000 = 16,
        PL6100 = 17,
        PL6200 = 18,
        PL6300 = 19,
        PL6400 = 20,
        PL6500 = 21,
        PL6600 = 22,
        PL6700 = 23,
        PL6800 = 24,
        PL6900 = 25,
        PL7000 = 26,
        PL7100 = 27,
        PL7200 = 28,
        PL7300 = 29,
        PL7400 = 30,
        PL7500 = 31,
        PL7600 = 32,
        PL7700 = 33,
        PL7800 = 34,
        PL7900 = 35,
        PL8000 = 36,
        PL8100 = 37,
        PL8200 = 38,
        PL8300 = 39,
        PL8400 = 40,
        PL8500 = 41,
        PL8600 = 42,
        PL8700 = 43,
        PL8800 = 44,
        PL8900 = 45,
        PL9000 = 46,
        PL9100 = 47,
        PL9200 = 48,
        PL9300 = 49,
        PL9400 = 50,
        PL9500 = 51,
        PL9600 = 52,
        PL9700 = 53,
        PL9800 = 54,
        PL9900 = 55,
    };

    public enum Costume : int
    {
        Invalid = -1,
        Default = 0,
        Costume_1 = 1,
        Costume_2 = 2,
        Costume_3 = 3,
        Costume_4 = 4,
        Costume_5 = 5,
        Costume_6 = 6,
        Costume_7 = 7,
        Costume_8 = 8,
        Costume_9 = 9,
        Costume_A = 10,
        Costume_B = 11,
        Costume_C = 12,
        Costume_D = 13,
        Costume_E = 14,
        Costume_F = 15,
        Scenario = 16,
        Classic = 17,
    };
}