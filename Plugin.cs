using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Template
{
    [BepInPlugin(PluginInfo.PluginGUID, PluginInfo.PluginName, PluginInfo.PluginSemanticVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.NotEnforced, VersionStrictness.None)]
    internal class Plugin : BaseUnityPlugin
    {
        private static readonly Harmony _harmony = new(PluginInfo.PluginGUID);
        private static bool _isPatched = false;

        public void Awake()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                // do not run on dedicated server
                return;
            }
            _isPatched = true;
            _harmony.PatchAll();
        }

        public void OnDestroy()
        {
            if (!_isPatched)
            {
                return;
            }
            _harmony.UnpatchSelf();
        }
    }
}
