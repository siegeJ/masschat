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
            var password = "oauth:";

            Console.WriteLine($"MASS CHAT");
            ChannelHandler channelhandler = new ChannelHandler(clientid, 1);
            ChatHandler chatHandler = new ChatHandler(nick, password, channelhandler);

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {

                        var stuff = chatHandler.AverageTokensMessageHandlers;

                        foreach (var channel in stuff.SelectMany(c => c.Channels))
                        {
                            var count30 = channel.Value.CountLast(30);

                            if ((count30 > (channel.Value.AveragePerMinuteAllTime * 3)) &&
                                channel.Value.AveragePerMinuteAllTime > 0)
                            {
                                Console.WriteLine(
                                    $" {channel.Value.HandlerName.ToString()} average in channel {channel.Key} is {channel.Value.AveragePerMinuteAllTime} Per minute");
                                Console.WriteLine($"Last 30 seconds was {count30}");

                                var streams = channelhandler.GetStreams().ConfigureAwait(false).GetAwaiter()
                                    .GetResult();

                                Console.WriteLine(streams.ContainsKey(channel.Key)
                                    ? $"Number of viewers is {streams[channel.Key].Viewers}"
                                    : $"{channel.Key} not found");
                            }
                        }

                        Thread.Sleep(10000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
     
                    }
                }
            });

           

            //chatHandler.MessageHandlers[0].

    
            _quitEvent.WaitOne();
        }
    }
}
