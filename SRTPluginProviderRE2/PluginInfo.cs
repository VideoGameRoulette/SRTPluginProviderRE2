using SRTPluginBase;
using System;

namespace SRTPluginProviderRE2
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider (Resident Evil 2 (2019))";

        public string Description => "A game memory provider plugin for Resident Evil 2 (2019).";

        public string Author => "Squirrelies";

        public Uri MoreInfoURL => new Uri("https://github.com/SpeedrunTooling/SRTPluginProviderRE2");

        public int VersionMajor => assemblyVersion.Major;

        public int VersionMinor => assemblyVersion.Minor;

        public int VersionBuild => assemblyVersion.Build;

        public int VersionRevision => assemblyVersion.Revision;

        private Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    }
}
