using System;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;

namespace PinionCore.Showcases.Lobby.Client
{
    internal class Program
    {
        public class Options
        {
            [Option("servicefile", Required = false, HelpText = "Path to server assembly for standalone mode.")]
            public string ServiceFile { get; set; }

            [Option("tcp", Required = false, HelpText = "Remote TCP endpoint, format host:port.")]
            public string Tcp { get; set; }

            [Option("ws", Required = false, HelpText = "Remote WebSocket URL, e.g. ws://127.0.0.1:8080/")]
            public string Ws { get; set; }

            [Option("username", Required = false, HelpText = "Login username.")]
            public string Username { get; set; } = "player";
        }

        static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<Options>(args)
                .MapResult(
                    (Options opts) => Run(opts),
                    _ => 1);
        }

        static int Run(Options opts)
        {
            if (!string.IsNullOrWhiteSpace(opts.ServiceFile))
            {
                return RunStandalone(opts);
            }
            return RunRemote(opts);
        }

        static int RunStandalone(Options opts)
        {
            var protocol = PinionCore.Showcases.Protocol.ProtocolCreater.Create();

            var asm = Assembly.LoadFrom(opts.ServiceFile);
            PinionCore.Remote.IEntry entry = null;
            foreach (var t in asm.GetExportedTypes())
            {
                if (typeof(PinionCore.Remote.IEntry).IsAssignableFrom(t))
                {
                    entry = (PinionCore.Remote.IEntry)Activator.CreateInstance(t);
                    break;
                }
            }
            if (entry == null)
            {
                Console.WriteLine("No IEntry implementation found in service assembly.");
                return 2;
            }

            var service = PinionCore.Remote.Standalone.Provider.CreateService(entry, protocol);
            var stream = new PinionCore.Remote.Standalone.Stream();
            var agent = service.Create(stream);

            return RunClientLoop(agent, opts, onExit: () =>
            {
                service.Destroy(agent);
                service.Dispose();
            });
        }

        static int RunRemote(Options opts)
        {
            var protocol = PinionCore.Showcases.Protocol.ProtocolCreater.Create();
            PinionCore.Remote.Ghost.IAgent agent = null;

            Func<Task> disconnect = async () => { await Task.CompletedTask; };

            if (!string.IsNullOrWhiteSpace(opts.Tcp))
            {
                if (!TryParseHostPort(opts.Tcp, out var host, out var port))
                {
                    Console.WriteLine("Invalid --tcp, expected host:port.");
                    return 3;
                }
                var set = PinionCore.Remote.Client.Provider.CreateTcpAgent(protocol);
                var peerTask = set.Connector.Connect(new IPEndPoint(ResolveIp(host), port));
                var peer = peerTask.GetAwaiter().GetResult();
                if (peer == null)
                {
                    Console.WriteLine($"Failed to connect TCP {host}:{port}");
                    return 4;
                }
                agent = set.Agent;
                agent.Enable(peer);
                disconnect = async () => { await set.Connector.Disconnect(); };
            }
            else if (!string.IsNullOrWhiteSpace(opts.Ws))
            {
                var conn = new PinionCore.Network.Web.Connecter(new ClientWebSocket());
                var ok = conn.ConnectAsync(opts.Ws).GetAwaiter().GetResult();
                if (!ok)
                {
                    Console.WriteLine($"Failed to connect WebSocket {opts.Ws}");
                    return 5;
                }
                agent = PinionCore.Remote.Client.Provider.CreateAgent(protocol);
                agent.Enable(conn);
                disconnect = async () => { await conn.DisconnectAsync(); };
            }
            else
            {
                Console.WriteLine("Specify --tcp host:port or --ws url, or --servicefile for standalone.");
                return 1;
            }

            return RunClientLoop(agent, opts, onExit: () =>
            {
                disconnect().GetAwaiter().GetResult();
                agent.Disable();
            });
        }

        static int RunClientLoop(PinionCore.Remote.Ghost.IAgent agent, Options opts, Action onExit)
        {
            var cts = new System.Threading.CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            // Wire up lobby flow
            agent.QueryNotifier<PinionCore.Showcases.Protocol.Lobby.IUser>().Supply += user =>
            {
                Console.WriteLine("IUser supplied. Subscribing to login notifier...");
                user.AlreadyLoggedInElsewhereEvent += () => Console.WriteLine("Already logged in elsewhere.");
                user.InLobbyEvent += () => Console.WriteLine("Entered lobby.");
                user.Notifier.Base.Supply += login =>
                {
                    Console.WriteLine($"ILogin supplied. Logging in as '{opts.Username}'...");
                    login.Login(opts.Username);
                };
                user.Notifier.Base.Unsupply += login => Console.WriteLine("ILogin removed.");
            };

            Console.WriteLine("Client running. Press Ctrl+C to stop.");
            try
            {
                var ar = new PinionCore.Utility.AutoPowerRegulator(new PinionCore.Utility.PowerRegulator());
                while (!cts.IsCancellationRequested)
                {
                    agent.HandleMessage();
                    agent.HandlePackets();
                    ar.Operate(new System.Threading.CancellationTokenSource());
                }
            }
            finally
            {
                onExit();
            }
            return 0;
        }

        static bool TryParseHostPort(string input, out string host, out int port)
        {
            host = null; port = 0;
            var idx = input.LastIndexOf(':');
            if (idx <= 0 || idx == input.Length - 1)
                return false;
            host = input.Substring(0, idx);
            return int.TryParse(input.Substring(idx + 1), out port);
        }

        static IPAddress ResolveIp(string host)
        {
            if (IPAddress.TryParse(host, out var ip))
                return ip;
            var addrs = Dns.GetHostAddresses(host);
            foreach (var a in addrs)
            {
                if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return a;
            }
            return addrs.Length > 0 ? addrs[0] : IPAddress.Loopback;
        }
    }
}
