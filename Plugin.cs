using A2.StopTime.Commands;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace A2.StopTime
{
    [BepInPlugin(PluginInfo.PluginGUID, PluginInfo.PluginName, PluginInfo.PluginSemanticVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Plugin : BaseUnityPlugin
    {
        private static readonly Harmony _harmony = new(PluginInfo.PluginGUID);
        private static ConfigEntry<bool>? IsTimeStopped_ConfigEntry;
        internal static bool IsTimeStopped
        {
            get
            {
                if (IsTimeStopped_ConfigEntry is null) return false;
                return IsTimeStopped_ConfigEntry.Value;
            }
            set
            {
                if (IsTimeStopped_ConfigEntry is not null)
                {
                    IsTimeStopped_ConfigEntry.Value = value;
                }
            }
        }
        internal static bool CommandUsed = false;

        private static CustomRPC? TimeStopRPC;
        private static CustomRPC? TimeResumeRPC;

        public void Awake()
        {
            ConfigurationManagerAttributes isAdminOnly = new() { IsAdminOnly = true };
            IsTimeStopped_ConfigEntry = Config.Bind("Stop time", "StopTime", false, new ConfigDescription("Stops the flow of time.", null, isAdminOnly));
            IsTimeStopped_ConfigEntry.SettingChanged += OnIsTimeStoppedChanged;

            TimeStopRPC = NetworkManager.Instance.AddRPC("TimeStopRPC", ServerReceiveTimeStop, ClientReceiveTimeStop);
            TimeResumeRPC = NetworkManager.Instance.AddRPC("TimeResumeRPC", ServerReceiveTimeResume, ClientReceiveTimeResume);

            CommandManager.Instance.AddConsoleCommand(new TimeStopCommand());
            CommandManager.Instance.AddConsoleCommand(new TimeResumeCommand());

            _harmony.PatchAll();
        }

        internal static void TimeStopRPCSendPackage(List<ZNetPeer> peers)
        {
            if (TimeStopRPC is null) return;
            TimeStopRPC.SendPackage(peers, new ZPackage());
        }
        internal static void TimeResumeRPCSendPackage(List<ZNetPeer> peers)
        {
            if (TimeResumeRPC is null) return;
            TimeResumeRPC.SendPackage(peers, new ZPackage());
        }

        private static IEnumerator ServerReceiveTimeStop(long sender, ZPackage package)
        {
            Console.instance.Print("Time stop command received from client.");
            yield return null;

            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (!ZNet.instance.IsAdmin(peer.m_socket.GetHostName()))
            {
                ZNet.instance.RemotePrint(peer.m_rpc, "You are not admin");
            }
            else
            {
                IsTimeStopped = true;
            }
            yield return null;
        }
        private static IEnumerator ServerReceiveTimeResume(long sender, ZPackage package)
        {
            Console.instance.Print("Time resume command received from client.");
            yield return null;

            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (!ZNet.instance.IsAdmin(peer.m_socket.GetHostName()))
            {
                ZNet.instance.RemotePrint(peer.m_rpc, "You are not admin");
            }
            else
            {
                IsTimeStopped = false;
            }
            yield return null;
        }

        private static IEnumerator ClientReceiveTimeStop(long sender, ZPackage package)
        {
            Console.instance.Print("Time stop command received from server.");
            yield return null;

            CommandUsed = true;
            IsTimeStopped = true;
            yield return null;
        }
        private static IEnumerator ClientReceiveTimeResume(long sender, ZPackage package)
        {
            Console.instance.Print("Time resume command received from server.");
            yield return null;

            CommandUsed = true;
            IsTimeStopped = false;
            yield return null;
        }

        private static void OnIsTimeStoppedChanged(object sender, System.EventArgs args)
        {
            if (IsTimeStopped)
            {
                Console.instance.Print("Time stopped.");
            }
            else
            {
                Console.instance.Print("Time resumed.");
            }
            if (CommandUsed)
            {
                CommandUsed = false;
                return;
            }

            // Send the command to the other players
            if (ZNet.instance.IsServer())
            {
                var targets = ZNet.instance.m_peers.Where(x => !x.m_server).ToList();
                if (IsTimeStopped)
                {
                    Console.instance.Print($"Sending timestop command to clients. Targets: {targets.Count}");
                    TimeStopRPCSendPackage(targets);
                }
                else
                {
                    Console.instance.Print($"Sending timeresume command to clients. Targets: {targets.Count}");
                    TimeResumeRPCSendPackage(targets);
                }
            }
            else
            {
                var targets = ZNet.instance.m_peers.Where(x => x.m_server).ToList();
                if (IsTimeStopped)
                {
                    Console.instance.Print($"Sending timestop command to server. Targets: {targets.Count}");
                    TimeStopRPCSendPackage(targets);
                }
                else
                {
                    Console.instance.Print($"Sending timeresume command to server. Targets: {targets.Count}");
                    TimeResumeRPCSendPackage(targets);
                }
            }
        }

        public void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }
    }
}
