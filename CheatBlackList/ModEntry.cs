using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using xTile.Dimensions;

namespace CheatBlackList
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private Timer timer;
        private readonly List<Farmer> cheatingFarmers = new List<Farmer>();

        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<ModConfig>();
            timer = new Timer(2000);
            timer.Elapsed += KickPlayer;
            timer.Stop();
            Helper.Events.Multiplayer.PeerContextReceived += Multiplayer_PeerContextReceived;
            Helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected; ;
        }

        private void KickPlayer(object sender, ElapsedEventArgs e)
        {
            if (cheatingFarmers.Count == 0)
            {
                timer.Stop();
                return;
            }
            foreach (Farmer cheater in cheatingFarmers)
            {
                try
                {
                    Game1.server.sendMessage(cheater.uniqueMultiplayerID, new StardewValley.Network.OutgoingMessage(Multiplayer.forceKick, cheater.uniqueMultiplayerID));
                }
                catch { }
                Game1.server.playerDisconnected(cheater.uniqueMultiplayerID);
                Game1.otherFarmers.Remove(cheater.uniqueMultiplayerID);
            }
        }

        private void Multiplayer_PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            if (!e.Peer.HasSmapi || e.Peer.IsHost)
            {
                return;
            }
            foreach (IMultiplayerPeerMod mod in e.Peer.Mods)
            {
                if (Config.ModBlackListIds.Any(mod.ID.Contains) || Config.ModBlackListNames.Any(mod.Name.Contains))
                {
                    var cheater = Game1.getAllFarmhands().First(x => x.UniqueMultiplayerID == e.Peer.PlayerID);
                    Monitor.Log("Cheater has joined: " + cheater.name, LogLevel.Warn);
                    cheatingFarmers.Add(cheater);
                    if (!timer.Enabled)
                    {
                        timer.Start();
                    }
                    return;
                }
            }
        }

        private void Multiplayer_PeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            if (!e.Peer.HasSmapi || e.Peer.IsHost)
            {
                return;
            }
            var cheater = cheatingFarmers.Find(x => x.UniqueMultiplayerID == e.Peer.PlayerID);
            cheatingFarmers.Remove(cheater);
        }
    }
}
