using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SRTPluginProviderRE2
{
    /// <summary>
    /// SHA256 hashes for the RE2/BIO2 REmake game executables.
    /// 
    /// Resident Evil 2 (WW): https://steamdb.info/app/883710/ / https://steamdb.info/depot/883711/
    /// Biohazard 2 (CERO Z): https://steamdb.info/app/895950/ / https://steamdb.info/depot/895951/
    /// </summary>
    public static class GameHashes
    {
        
        
        // Latest RT DX12 Build
        private static readonly byte[] re2WW_11636119 = new byte[32] { 0xA6, 0xE7, 0x20, 0xDB, 0xA2, 0x01, 0xAB, 0xEA, 0x9A, 0xCC, 0x05, 0xCF, 0xAF, 0x1E, 0xDA, 0x46, 0x0C, 0xA7, 0xF7, 0x29, 0x25, 0x57, 0xF5, 0x71, 0x85, 0x7A, 0xD2, 0xB0, 0xE5, 0x54, 0x13, 0x54 };
        // Latest DX11 Build
        private static readonly byte[] re2WW_11055033 = new byte[32] { 0xF2, 0x6C, 0xDE, 0xBF, 0xFE, 0x66, 0x55, 0xD0, 0x5B, 0xF0, 0x04, 0x7F, 0x79, 0x39, 0xEA, 0x5E, 0x38, 0x36, 0x08, 0xAF, 0xF4, 0x60, 0x3C, 0xA3, 0xF0, 0xF3, 0x6A, 0x12, 0xDC, 0x56, 0x23, 0xCA };
        // Latest DX11 Japanese Build
        private static readonly byte[] re2ceroz_11055259 = new byte[32] { 0x42, 0x6A, 0x81, 0xD0, 0x16, 0x22, 0x2D, 0xB7, 0x0F, 0x13, 0xB6, 0x1B, 0x31, 0x10, 0xF9, 0x4C, 0x5E, 0xCA, 0x2A, 0xD3, 0x19, 0x48, 0xBE, 0x82, 0xAF, 0xFC, 0x82, 0x24, 0x10, 0x69, 0xC0, 0x5F };
        // Latest RT Japanese Build
        private static readonly byte[] re2ceroz_11636615 = new byte[32] { 0x81, 0x2E, 0x11, 0xDA, 0xFE, 0xD8, 0x5C, 0x96, 0x04, 0x6E, 0x52, 0x26, 0xAE, 0x98, 0x4B, 0xBE, 0xE6, 0x20, 0x3D, 0x7A, 0x18, 0x5A, 0x5F, 0xF4, 0xBA, 0x1B, 0xD3, 0x1E, 0xE4, 0x2F, 0x96, 0xD7 };



        private static void OutputVersionString(byte[] cs)
        {
            StringBuilder sb = new StringBuilder("private static readonly byte[] re2??_00000000 = new byte[32] { ");

            for (int i = 0; i < cs.Length; i++)
            {
                sb.AppendFormat("0x{0:X2}", cs[i]);

                if (i < cs.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(" };");
            Console.WriteLine("Please DM VideoGameRoulette or Squirrelies with the version.log");
            // write output to file
            string filename = "version.log";
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(sb.ToString());
            }
        }

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);
            
            if (checksum.SequenceEqual(re2WW_11636119))
            {
                Console.WriteLine("Game Detected! Version: World public Release");
                return GameVersion.RE2_WW_11636119;
            }
            
            else if (checksum.SequenceEqual(re2WW_11055033))
            {
                Console.WriteLine("Game Detected! Version: World dx11_non-rt Release");
                return GameVersion.RE2_WW_11055033;
            }
            
            else if (checksum.SequenceEqual(re2ceroz_11636615))
            {
                Console.WriteLine("Game Detected! Version: CeroZ public Release");
                return GameVersion.RE2_CEROZ_11636615;
            }
            else if (checksum.SequenceEqual(re2ceroz_11055259))
            {
                Console.WriteLine("Game Detected! Version: CeroZ dx11_non-rt Release");
                return GameVersion.RE2_CEROZ_11055259;
            }
            else
            {
                Console.WriteLine("Unknown Version");
                OutputVersionString(checksum);
                return GameVersion.Unknown;
            }
                
        }
    }
}
