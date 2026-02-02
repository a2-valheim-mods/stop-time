using HarmonyLib;
#pragma warning disable IDE0060
namespace A2.StopTime.Patches
{
    public static class ZNetPatch
    {
        [HarmonyPatch(typeof(ZNet), "UpdateNetTime")]
        public static class InputText
        {
            public static bool Prefix(ZNet __instance, float dt)
            {
                return !Plugin.IsTimeStopped;
            }
        }
    }
}
#pragma warning restore IDE0060