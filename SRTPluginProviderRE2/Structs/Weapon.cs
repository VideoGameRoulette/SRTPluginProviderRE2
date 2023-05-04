using System;
using SRTPluginProviderRE2.Structs.GameStructs;

namespace SRTPluginProviderRE2.Structs
{
    public struct Weapon : IEquatable<Weapon>
    {
        public WeaponType WeaponID;
        public WeaponParts Attachments;

        public bool Equals(Weapon other) => (int)this.WeaponID == (int)other.WeaponID && (int)this.Attachments == (int)other.Attachments;
    }
}