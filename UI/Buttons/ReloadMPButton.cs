using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadMPButton : BaseButton
    {
        // Constructor
        public ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : base(spritesheet, buttonText, hoverText)
        {
            // deactived by default since the SP button is active
            Active = false;
            buttonUIText.Active = false;
        }

        // Set custom animation dimensions
        protected override float SpriteScale => 1.25f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            // If alt+click, toggle the mode and return
            bool altClick = Main.keyState.IsKeyDown(Keys.LeftAlt);
            if (altClick)
            {
                Active = false;
                buttonUIText.Active = false;

                // set MP active
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                foreach (var btn in sys?.mainState?.AllButtons)
                {
                    if (btn is ReloadSPButton spBtn)
                    {
                        spBtn.Active = true;
                        spBtn.buttonUIText.Active = true;
                    }
                }
                return;
            }

            ReloadUtilities.PrepareClient(ClientModes.MPMain);

            // we must be in multiplayer for this to have an effect
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();

                int clientCount = Main.player.Where((p) => p.active).Count() - 1;
                List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                List<Task<string?>> clientResponses = new List<Task<string?>>();

                Log.Info($"Waiting for {clientCount} clients...");

                // Wait for conecting all clients
                for (int i = 0; i < clientCount; i++)
                {
                    var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeName, PipeDirection.InOut, clientCount);
                    await pipeServer.WaitForConnectionAsync();
                    clients.Add(pipeServer);
                    Log.Info($"Client {i + 1} connected!");
                }

                foreach (var client in clients)
                {
                    clientResponses.Add(ReloadUtilities.ReadPipeMessage(client));
                }

                // Чекаємо, поки всі клієнти надішлють свої повідомлення
                await Task.WhenAll(clientResponses);

                ReloadUtilities.BuildAndReloadMod();

                Log.Info("Mod Builded");
                // After building - reload all other clients
                for (int i = 0; i < clients.Count; i++)
                {
                    using StreamWriter writer = new StreamWriter(clients[i]) { AutoFlush = true };
                    await writer.WriteLineAsync($"yes, go reload yourself now");
                }

                foreach (var client in clients)
                {
                    client.Close();
                    client.Dispose();
                }
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorldOrServer();
                ReloadUtilities.BuildAndReloadMod();
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // If right click, toggle the mode and return
            Active = false;
            buttonUIText.Active = false;

            // set MP active
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            foreach (var btn in sys?.mainState?.AllButtons)
            {
                if (btn is ReloadSPButton spBtn)
                {
                    spBtn.Active = true;
                    spBtn.buttonUIText.Active = true;
                }
            }
            return;
        }
    }
}