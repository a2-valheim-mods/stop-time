using Jotunn.Entities;
using Jotunn.Managers;
using System.Linq;

namespace A2.StopTime.Commands
{
    internal class TimeStopCommand : ConsoleCommand
    {
        public override string Name => "timestop";
        public override string Help => "Stops the flow of time.";

        public override bool IsCheat => true;
        public override bool IsNetwork => true;
        public override bool IsSecret => false;
        public override bool OnlyServer => false;

        public override void Run(string[] args)
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Console.instance.Print("You must be an admin to use this command.");
                return;
            }

            Plugin.CommandUsed = true;
            if (ZNet.instance.IsServer())
            {
                var targets = ZNet.instance.m_peers.Where(x => !x.m_server).ToList();
                Console.instance.Print($"Sending timestop command to clients. Targets: {targets.Count}");
                Plugin.TimeStopRPCSendPackage(targets);
                Plugin.IsTimeStopped = true;
            }
            else
            {
                var targets = ZNet.instance.m_peers.Where(x => x.m_server).ToList();
                Console.instance.Print($"Sending timestop command to server. Targets: {targets.Count}");
                Plugin.TimeStopRPCSendPackage(targets);
            }
        }
    }
}
