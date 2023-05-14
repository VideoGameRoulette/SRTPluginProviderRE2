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
        public string CurrentSurvivorString => CurrentSurvivor.ToString();
        public Costume CurrentCostume { get => currentCostume; set => currentCostume = value; }
        public string CurrentCostumeString => CurrentCostume.ToString();
        public HitPointController Health { get => hitPointController; set => hitPointController = value; }
        public bool IsPoisoned { get => isPoisoned; set => isPoisoned = value; }
        public bool IsLoaded => (int)CurrentSurvivor <= 55 && Health.MaxHP >= 1200;
        public Vec3 Position { get => position; set => position = value; }
        public PlayerState HealthState
        {
            get =>
                !Health.IsAlive ? PlayerState.Dead :
                IsPoisoned ? PlayerState.Poisoned :
                Health.Percentage >= 0.66f ? PlayerState.Fine :
                Health.Percentage >= 0.33f ? PlayerState.Caution :
                PlayerState.Danger;
        }
        public string CurrentHealthState => HealthState.ToString();
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
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
    }

    public enum MapID
    {
        None = 0,
        st0_101_0 = 1,
        st0_102_0 = 2,
        st1_101_0 = 3,
        st1_102_0 = 4,
        st1_103_0 = 5,
        st1_104_0 = 6,
        st1_105_0 = 7,
        st1_106_0 = 8,
        st1_107_0 = 9,
        st1_108_0 = 10,
        st1_109_0 = 11,
        st1_110_0 = 12,
        st1_111_0 = 13,
        st1_201_0 = 14,
        st1_202_0 = 15,
        st1_203_0 = 16,
        st1_204_0 = 17,
        st1_205_0 = 18,
        st1_206_0 = 19,
        st1_207_0 = 20,
        st1_208_0 = 21,
        st1_209_0 = 22,
        st1_210_0 = 23,
        st1_211_0 = 24,
        st2_101_0 = 25,
        st2_102_0 = 26,
        st2_103_0 = 27,
        st2_201_0 = 28,
        st2_202_0 = 29,
        st2_203_0 = 30,
        st2_204_0 = 31,
        st2_205_0 = 32,
        st2_206_0 = 33,
        st2_207_0 = 34,
        st2_208_0 = 35,
        st2_209_0 = 36,
        st2_210_0 = 37,
        st2_211_0 = 38,
        st2_212_0 = 39,
        st2_213_0 = 40,
        st2_214_0 = 41,
        st2_215_0 = 42,
        st2_216_0 = 43,
        st2_301_0 = 44,
        st2_302_0 = 45,
        st2_303_0 = 46,
        st2_304_0 = 47,
        st2_305_0 = 48,
        st2_306_0 = 49,
        st2_307_0 = 50,
        st2_308_0 = 51,
        st2_309_0 = 52,
        st2_401_0 = 53,
        st2_401_1 = 54,
        st2_402_0 = 55,
        st2_403_0 = 56,
        st2_404_0 = 57,
        st2_405_0 = 58,
        st2_406_0 = 59,
        st2_407_0 = 60,
        st2_408_0 = 61,
        st2_501_0 = 62,
        st2_502_0 = 63,
        st2_502_0b = 64,
        st2_502_0c = 65,
        st2_503_0 = 66,
        st2_504_0 = 67,
        st2_505_0 = 68,
        st2_506_0 = 69,
        st2_507_0 = 70,
        st2_508_0 = 71,
        st2_601_0 = 72,
        st2_602_0 = 73,
        st2_603_0 = 74,
        st2_604_0 = 75,
        st2_605_0 = 76,
        st2_606_0 = 77,
        st2_607_0 = 78,
        st2_608_0 = 79,
        st3_101_0 = 80,
        st3_102_0 = 81,
        st3_103_0 = 82,
        st3_104_0 = 83,
        st3_105_0 = 84,
        st3_106_0 = 85,
        st3_107_0 = 86,
        st3_108_0 = 87,
        st3_109_0 = 88,
        st3_110_0 = 89,
        st3_111_0 = 90,
        st3_112_0 = 91,
        st3_113_0 = 92,
        st3_114_0 = 93,
        st3_201_0 = 94,
        st3_202_0 = 95,
        st3_203_0 = 96,
        st3_204_0 = 97,
        st3_205_0 = 98,
        st3_206_0 = 99,
        st3_207_0 = 100,
        st3_208_0 = 101,
        st3_209_0 = 102,
        st3_210_0 = 103,
        st3_301_0 = 104,
        st3_302_0 = 105,
        st3_303_0 = 106,
        st3_304_0 = 107,
        st3_305_0 = 108,
        st3_306_0 = 109,
        st3_307_0 = 110,
        st3_308_0 = 111,
        st4_101_0 = 112,
        st4_102_0 = 113,
        st4_103_0 = 114,
        st4_104_0 = 115,
        st4_105_0 = 116,
        st4_106_0 = 117,
        st4_207_0 = 118,
        st4_208_0 = 119,
        st4_209_0 = 120,
        st4_210_0 = 121,
        st4_211_0 = 122,
        st4_212_0 = 123,
        st4_213_0 = 124,
        st4_214_0 = 125,
        st5_101_0 = 126,
        st5_102_0 = 127,
        st5_103_0 = 128,
        st5_104_0 = 129,
        st5_105_0 = 130,
        st5_106_0 = 131,
        st5_107_0 = 132,
        st5_201_0 = 133,
        st5_202_0 = 134,
        st5_203_0 = 135,
        st5_204_0 = 136,
        st5_205_0 = 137,
        st5_206_0 = 138,
        st5_207_0 = 139,
        st5_301_0 = 140,
        st5_302_0 = 141,
        st5_303_0 = 142,
        st6_101_0 = 143,
        st6_102_0 = 144,
        st6_103_0 = 145,
        st6_104_0 = 146,
        st6_105_0 = 147,
        st6_106_0 = 148,
        st6_107_0 = 149,
        st6_201_0 = 150,
        st6_202_0 = 151,
        st6_203_0 = 152,
        st6_204_0 = 153,
        st6_205_0 = 154,
        st6_206_0 = 155,
        st6_207_0 = 156,
        st6_208_0 = 157,
        st6_209_0 = 158,
        st6_210_0 = 159,
        st6_211_0 = 160,
        st6_212_0 = 161,
        st7_101_0 = 162,
        st7_102_0 = 163,
        st7_103_0 = 164,
        st7_104_0 = 165,
        st7_105_0 = 166,
        st7_106_0 = 167,
        st7_107_0 = 168,
        st7_108_0 = 169,
        st7_109_0 = 170,
        st7_110_0 = 171,
        st7_111_0 = 172,
        st7_112_0 = 173,
        st8_101_0 = 174,
        st8_102_0 = 175,
        st8_103_0 = 176,
        st8_104_0 = 177,
        st8_105_0 = 178,
        st8_106_0 = 179,
        st8_107_0 = 180,
        st8_108_0 = 181,
        st8_109_0 = 182,
        st8_110_0 = 183,
        st8_111_0 = 184,
        st8_112_0 = 185,
        st8_113_0 = 186,
        st8_114_0 = 187,
        st8_115_0 = 188,
        st8_201_0 = 189,
        st8_202_0 = 190,
        st8_203_0 = 191,
        st8_301_0 = 192,
        st8_302_0 = 193,
        st8_303_0 = 194,
        st8_304_0 = 195,
        st8_305_0 = 196,
        st8_306_0 = 197,
        st8_307_0 = 198,
        st8_308_0 = 199,
        st8_309_0 = 200,
        st8_310_0 = 201,
        st8_311_0 = 202,
        st8_401_0 = 203,
        st8_402_0 = 204,
        st8_403_0 = 205,
        st8_404_0 = 206,
        st8_405_0 = 207,
        st8_406_0 = 208,
        st8_407_0 = 209,
        st9_101_0 = 210,
        st9_102_0 = 211,
        st9_103_0 = 212,
        st9_201_0 = 213,
        st9_202_0 = 214,
        st9_203_0 = 215,
        st9_301_0 = 216,
        st1_301_0 = 217,
        st1_401_0 = 218,
        st4_201_0 = 219,
        st4_202_0 = 220,
        st4_203_0 = 221,
        st4_204_0 = 222,
        st4_205_0 = 223,
        st4_206_0 = 224,
        st4_215_0 = 225,
        st4_216_0 = 226,
        st4_301_0 = 227,
        st4_302_0 = 228,
        st4_303_0 = 229,
        st4_304_0 = 230,
        st4_305_0 = 231,
        st4_306_0 = 232,
        st4_307_0 = 233,
        st4_308_0 = 234,
        st4_309_0 = 235,
        st4_310_0 = 236,
        st4_311_0 = 237,
        st4_312_0 = 238,
        st4_313_0 = 239,
        st4_314_0 = 240,
        st4_401_0 = 241,
        st4_401_1 = 242,
        st4_402_0 = 243,
        st4_403_0 = 244,
        st4_404_0 = 245,
        st4_405_0 = 246,
        st4_406_0 = 247,
        st4_407_0 = 248,
        st4_408_0 = 249,
        st4_409_0 = 250,
        st4_410_0 = 251,
        st4_411_0 = 252,
        st4_412_0 = 253,
        st4_501_0 = 254,
        st4_502_0 = 255,
        st4_503_0 = 256,
        st4_504_0 = 257,
        st4_505_0 = 258,
        st4_506_0 = 259,
        st4_507_0 = 260,
        st4_508_0 = 261,
        st4_601_0 = 262,
        st4_602_0 = 263,
        st4_603_0 = 264,
        st4_604_0 = 265,
        st4_605_0 = 266,
        st4_606_0 = 267,
        st4_607_0 = 268,
        st4_608_0 = 269,
        st4_609_0 = 270,
        st4_610_0 = 271,
        st4_701_0 = 272,
        st4_702_0 = 273,
        st4_703_0 = 274,
        st4_704_0 = 275,
        st4_705_0 = 276,
        st4_708_0 = 277,
        st4_709_0 = 278,
        st4_710_0 = 279,
        st4_711_0 = 280,
        st4_712_0 = 281,
        st4_714_0 = 282,
        st1_501_0 = 283,
        st1_502_0 = 284,
        st1_503_0 = 285,
        st1_504_0 = 286,
        st1_505_0 = 287,
        st1_506_0 = 288,
        st4_650_0 = 289,
        st3_401_0 = 290,
        st3_402_0 = 291,
        st3_403_0 = 292,
        st3_404_0 = 293,
        st3_405_0 = 294,
        st3_406_0 = 295,
        st3_407_0 = 296,
        st3_408_0 = 297,
        st3_409_0 = 298,
        st3_410_0 = 299,
        st8_501_0 = 300,
        st8_601_0 = 301,
        st8_602_0 = 302,
        st8_603_0 = 303,
        st8_604_0 = 304,
        st8_605_0 = 305,
        st8_606_0 = 306,
        st8_607_0 = 307,
        st8_608_0 = 308,
        st8_609_0 = 309,
        st3_600_0 = 310,
        st3_601_0 = 311,
        st3_602_0 = 312,
        st3_603_0 = 313,
        st3_610_0 = 314,
        st3_611_0 = 315,
        st3_612_0 = 316,
        st3_613_0 = 317,
        st3_614_0 = 318,
        st3_615_0 = 319,
        st3_616_0 = 320,
        st3_617_0 = 321,
        st3_620_0 = 322,
        st3_621_0 = 323,
        st3_622_0 = 324,
        st3_623_0 = 325,
        st3_624_0 = 326,
        st3_625_0 = 327,
        st3_626_0 = 328,
        st3_627_0 = 329,
        st3_630_0 = 330,
        st3_631_0 = 331,
        st3_632_0 = 332,
        st3_633_0 = 333,
        st3_634_0 = 334,
        st3_635_0 = 335,
        st3_636_0 = 336,
        st3_637_0 = 337,
        st3_638_0 = 338,
        st3_640_0 = 339,
        st3_641_0 = 340,
        st3_642_0 = 341,
        st3_643_0 = 342,
        st3_644_0 = 343,
        st3_650_0 = 344,
        st3_651_0 = 345,
        st3_652_0 = 346,
        st1_411_0 = 347,
        st4_715_0 = 348,
        st4_716_0 = 349,
        st4_717_0 = 350,
        st8_408_0 = 351,
        st4_750_0 = 352,
        st4_751_0 = 353,
        st4_752_0 = 354,
        st4_753_0 = 355,
        st4_754_0 = 356,
        st8_409_0 = 357,
        st8_410_0 = 358,
        st1_601_0 = 359,
        st1_602_0 = 360,
        st1_603_0 = 361,
        st1_604_0 = 362,
        st1_605_0 = 363,
        st1_606_0 = 364,
        st1_607_0 = 365,
        st1_608_0 = 366,
        st1_609_0 = 367,
        st8_650_0 = 368,
        st8_411_0 = 369,
        st5_111_0 = 370,
        st5_121_0 = 371,
        st5_131_0 = 372,
        st5_211_0 = 373,
        st5_221_0 = 374,
        st0_103_0 = 375,
        st8_660_0 = 376,
        st1_620_0 = 377,
        st1_621_0 = 378,
        st1_622_0 = 379,
        st1_623_0 = 380,
        st1_610_0 = 381,
        st1_611_0 = 382,
        st1_612_0 = 383,
        st1_613_0 = 384,
        st1_614_0 = 385,
        st5_112_0 = 386,
        st5_110_0 = 387,
        st5_113_0 = 388,
        st5_114_0 = 389,
        st5_115_0 = 390,
        st5_307_0 = 391,
        st5_308_0 = 392,
        st5_309_0 = 393,
        st5_310_0 = 394,
        st5_401_0 = 395,
        st5_402_0 = 396,
        st5_403_0 = 397,
        st5_404_0 = 398,
        st5_405_0 = 399,
        st5_406_0 = 400,
        st5_407_0 = 401,
        st5_408_0 = 402,
        st4_800_0 = 403,
        st1_615_0 = 404,
        st4_755_0 = 405,
        st1_630_0 = 406,
        st1_631_0 = 407,
        st1_632_0 = 408,
        st4_713_0 = 409,
        st5_122_0 = 410,
        st5_123_0 = 411,
        st5_124_0 = 412,
        st5_125_0 = 413,
        st5_126_0 = 414,
        st5_127_0 = 415,
        st5_128_0 = 416,
        st5_129_0 = 417,
        st5_132_0 = 418,
        st5_133_0 = 419,
        st5_134_0 = 420,
        st5_212_0 = 421,
        st5_222_0 = 422,
        st5_223_0 = 423,
        st8_651_0 = 424,
        st8_610_0 = 425,
        st8_611_0 = 426,
        st8_612_0 = 427,
        st8_613_0 = 428,
        st3_604_0 = 429,
        st3_605_0 = 430,
        st4_217_0 = 431,
        st5_122_1 = 432,
        st5_122_2 = 433,
        st8_614_0 = 434,
        MAP_NUM = 435,
    };

    public enum LocationID
    {
        None = 0,
        CityArea = 1,
        Factory = 2,
        Laboratory = 3,
        Mountain = 4,
        Opening = 5,
        Orphanage = 6,
        Police = 7,
        SewagePlant = 8,
        Sewer = 9,
        Playground = 10,
        DLC_Laboratory = 11,
        DLC_Aida = 12,
        DLC_Hunk = 13,
        Opening2 = 14,
        GasStation = 15,
        RPD = 16,
        WasteWater = 17,
        WaterPlant = 18,
        EV011 = 19,
        EV050 = 20,
        LaboratoryUndermost = 21,
        Transportation = 22,
        GasStation2 = 23,
        OrphanAsylum = 24,
        OrphanApproach = 25,
        CrocodiliaArea = 26,
        Title = 27,
        Movie = 28,
        RPD_B1 = 29,
        Opening3 = 30,
        GameOver = 31,
        Result = 32,
        Ending = 33,
        LOCATION_NUM = 34,
    };

    public enum PlayerState : int
    {
        Dead,
        Fine,
        Caution,
        Danger,
        Poisoned
    }

    public enum SurvivorType : int
    {
        Invalid = -1,
        Leon = 0,
        Claire = 1,
        Ada = 2,
        Sherry = 3,
        Hunk = 4,
        Tofu = 5,
        Kendo = 6,
        PL5100 = 7,
        PL5200 = 8,
        PL5300 = 9,
        PL5400 = 10,
        PL5500 = 11,
        Ghost = 12,
        PL5700 = 13,
        PL5800 = 14,
        PL5900 = 15,
        PL6000 = 16,
        PL6100 = 17,
        PL6200 = 18,
        PL6300 = 19,
        Katherine = 20,
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