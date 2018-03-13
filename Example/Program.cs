using masschat;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using masschat.Handlers;

namespace Example
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            var clientid = "";
            var nick = "";
            var password = "";

            Console.WriteLine($"MASS CHAT");
            ChannelHandler channelhandler = new ChannelHandler(clientid);
            ChatHandler chatHandler = new ChatHandler(nick, password, channelhandler);
            Task.Run(() => chatHandler.JoinAllChannels().ConfigureAwait(false));

            Task.Run(() =>
            {
                while (true)
                {
                    var stuff = chatHandler.AverageTokensMessageHandlers;

                    foreach (var channels in stuff.SelectMany(c => c.Channels))
                    {
                        Console.WriteLine($"Average messages per minute of handler name {channels.Value.HandlerName.ToString()} in channel {channels.Key} is {channels.Value.AveragePerMinuteAllTime}");
                    }

                    Thread.Sleep(5000);
                }
            });

           

            //chatHandler.MessageHandlers[0].

    
            _quitEvent.WaitOne();
        }
    }
}
