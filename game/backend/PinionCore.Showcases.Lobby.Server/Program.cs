using CommandLine;

namespace PinionCore.Showcases.Lobby.Server
{
    internal class Program
    {
        public class Options
        {
            [Option("tcp-port", Required = false, HelpText = "TCP listen port. Omit to disable TCP.")]
            public int? TcpPort { get; set; }

            [Option("tcp-backlog", Required = false, HelpText = "TCP listen backlog. Default: 10")]
            public int TcpBacklog { get; set; } = 10;

            [Option("ws-prefix", Required = false, HelpText = "HttpListener prefix for WebSocket, e.g. http://+:8080/. Omit to disable WebSocket.")]
            public string WsPrefix { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    (Options opts) => Run(opts),
                    errs => 1
                );
        }

        static int Run(Options opts)
        {
            if (opts.TcpPort is null && string.IsNullOrWhiteSpace(opts.WsPrefix))
            {
                System.Console.WriteLine("No listener specified. Use --tcp-port or --ws-prefix.");
                return 1;
            }

            var protocol = PinionCore.Showcases.Protocol.ProtocolCreater.Create();
            var entry = new PinionCore.Showcases.Lobby.Entry();

            var mux = new MultiListener();
            var closers = new List<Action>();
            IDisposable service = null;

            if (opts.TcpPort is not null)
            {
                var tcp = new PinionCore.Remote.Server.Tcp.Listener();
                tcp.Bind(opts.TcpPort.Value, opts.TcpBacklog);
                mux.Add(tcp);
                closers.Add(() => tcp.Close());
                System.Console.WriteLine($"TCP listening on :{opts.TcpPort.Value} (backlog {opts.TcpBacklog}).");
            }

            if (!string.IsNullOrWhiteSpace(opts.WsPrefix))
            {
                var web = new PinionCore.Remote.Server.Web.Listener();
                web.Bind(opts.WsPrefix);
                mux.Add(web);
                closers.Add(() => web.Close());
                System.Console.WriteLine($"WebSocket listening at {opts.WsPrefix}");
            }

            service = PinionCore.Remote.Server.Provider.CreateService(entry, protocol, mux);

            using var cts = new System.Threading.CancellationTokenSource();
            System.Console.WriteLine("Press Ctrl+C to stop.");
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                var mre = new System.Threading.ManualResetEventSlim(false);
                using (cts.Token.Register(() => mre.Set()))
                {
                    mre.Wait();
                }
            }
            finally
            {
                foreach (var close in closers)
                {
                    try { close(); } catch { }
                }
                try { service?.Dispose(); } catch { }
                mux.Dispose();
            }

            return 0;
        }
    }
}
