//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Reflection;
//using Terraria;
//using Terraria.ID;
//using Terraria.IO;
//using Terraria.ModLoader;

//namespace SkipSelect
//{
//    public class InvestigateMultiplayer : ModSystem
//    {
//        public override void Load() => Mod.Logger.Info("InvestigateMultiplayer.Load() called");

//        public override void PostSetupContent()
//        {
//            Mod.Logger.Info("InvestigateMultiplayer.PostSetupContent() called");
//            EnterMultiplayerWorld();
//        }

//        private void EnterMultiplayerWorld()
//        {
//            // Load worlds and players.
//            Main.LoadWorlds();
//            Main.LoadPlayers();
//            var player = Main.PlayerList.First(p => p.IsFavorite);
//            var world = Main.WorldList.First(w => w.IsFavorite);

//            Main.SelectPlayer(player);
//            Main.ActivePlayerFileData = player;
//            string worldPath = Main.GetWorldPathFromName(world.Name, false);
//            Main.UpdateWorldPreparationState();
//            Mod.Logger.Info($"Player: {player.Name}  |  World: {world.Name}  |  Path: {worldPath}");

//            // Switch to server mode.
//            Main.ActiveWorldFileData = world;
//            Main.SwitchNetMode(NetmodeID.Server);
//            // Set server IP (avoids potential null issues later)
//            Netplay.ServerIP = "0.0.0.0";
//            Netplay.ServerIPText = "0.0.0.0";

//            // Setup the TCP connection (before Netplay.Initialize)
//            EnsureTcpListener();
//            DumpTcpListenerInternalState();

//            // Initialize network.
//            Netplay.Initialize();
//            DumpNetplayInternalState();

//            // Call SetOngoingToTemps (assumed to now work)
//            WorldFile.SetOngoingToTemps();

//            // Invoke StartListening (using our bound callback)
//            CallPrivateStartListening();

//            // Finally start the server.
//            Netplay.StartServer();
//        }

//        private void DumpNetplayInternalState()
//        {
//            var fields = typeof(Netplay).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
//            foreach (var field in fields)
//            {
//                Mod.Logger.Info($"Netplay.{field.Name} = {field.GetValue(null) ?? "null"}");
//            }
//        }

//        private void DumpTcpListenerInternalState()
//        {
//            if (Netplay.TcpListener == null)
//            {
//                Mod.Logger.Info("TcpListener is null.");
//                return;
//            }
//            var type = Netplay.TcpListener.GetType();
//            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            foreach (var field in fields)
//            {
//                Mod.Logger.Info($"{type.Name}.{field.Name} = {field.GetValue(Netplay.TcpListener) ?? "null"}");
//            }
//            // Dump underlying socket info if available.
//            var innerField = type.GetField("_listener", BindingFlags.NonPublic | BindingFlags.Instance);
//            if (innerField != null)
//            {
//                TcpListener innerListener = innerField.GetValue(Netplay.TcpListener) as TcpListener;
//                if (innerListener != null)
//                {
//                    Socket s = innerListener.Server;
//                    Mod.Logger.Info($"Internal Socket.LocalEndPoint = {s.LocalEndPoint}");
//                    Mod.Logger.Info($"Internal Socket.RemoteEndPoint = {s.RemoteEndPoint}");
//                    Mod.Logger.Info($"Internal Socket.Connected = {s.Connected}");
//                }
//            }
//        }

//        /// <summary>
//        /// Ensures that Netplay.TcpListener is instantiated and its internal listener is started.
//        /// </summary>
//        private void EnsureTcpListener()
//        {
//            // Get the TcpListener field from Netplay.
//            FieldInfo tcpListenerField = typeof(Netplay)
//                .GetField("TcpListener", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
//            // If null, create an instance.
//            if (tcpListenerField.GetValue(null) == null)
//            {
//                Type targetType = tcpListenerField.FieldType;
//                Mod.Logger.Info($"TcpListener field type: {targetType.FullName}");
//                // If targetType is ISocket, force usage of TcpSocket.
//                object newListener = (targetType.Name == "ISocket")
//                    ? Activator.CreateInstance(Assembly.GetAssembly(typeof(Netplay))
//                        .GetType("Terraria.Net.Sockets.TcpSocket"), true)
//                    : Activator.CreateInstance(targetType, true);
//                tcpListenerField.SetValue(null, newListener);
//                Mod.Logger.Info($"TcpListener set to instance of {newListener.GetType().FullName}");
//            }
//            // Now set up the internal _listener.
//            var listener = tcpListenerField.GetValue(null);
//            FieldInfo innerField = listener.GetType()
//                .GetField("_listener", BindingFlags.NonPublic | BindingFlags.Instance);
//            if (innerField != null && innerField.GetValue(listener) == null)
//            {
//                TcpListener newInner = new TcpListener(IPAddress.Any, Netplay.ListenPort);
//                newInner.Start();
//                innerField.SetValue(listener, newInner);
//                Mod.Logger.Info("Internal _listener assigned and started.");
//                // Optionally, set _isListening to true.
//                FieldInfo isListeningField = listener.GetType().GetField("_isListening", BindingFlags.NonPublic | BindingFlags.Instance);
//                if (isListeningField != null)
//                {
//                    isListeningField.SetValue(listener, true);
//                    Mod.Logger.Info("Internal _isListening set to true.");
//                }
//            }
//        }

//        /// <summary>
//        /// Invokes the private Netplay.StartListening method via reflection.
//        /// </summary>
//        private void CallPrivateStartListening()
//        {
//            MethodInfo m = typeof(Netplay).GetMethod("StartListening", BindingFlags.NonPublic | BindingFlags.Static);
//            var parameters = m.GetParameters();
//            if (parameters.Length > 0)
//            {
//                // Assume it expects a callback delegate (e.g. one that takes an int).
//                Type delegateType = parameters[0].ParameterType;
//                MethodInfo callbackMethod = GetType().GetMethod("OnConnectionAccepted", BindingFlags.NonPublic | BindingFlags.Instance);
//                Delegate callbackDelegate = Delegate.CreateDelegate(delegateType, this, callbackMethod);
//                m.Invoke(null, new object[] { callbackDelegate });
//                Mod.Logger.Info($"Invoked StartListening with delegate {delegateType.FullName}");
//            }
//            else
//            {
//                m.Invoke(null, null);
//                Mod.Logger.Info("Invoked StartListening with no parameters.");
//            }
//        }

//        /// <summary>
//        /// Callback for when a connection is accepted.
//        /// </summary>
//        private void OnConnectionAccepted(int socketIndex)
//        {
//            Mod.Logger.Info($"OnConnectionAccepted: {socketIndex}");
//            if (socketIndex >= 0 && socketIndex < Netplay.Clients.Length)
//            {
//                Mod.Logger.Info($"RemoteClient[{socketIndex}].State = {Netplay.Clients[socketIndex].State}");
//            }
//        }
//    }
//}
