using masschat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using masschat.Handlers;

namespace Example
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        private static List<string> recentClipChannels;

        static void ClearRecentClipChannels()
        {
            Console.WriteLine("Clearing recent clip channels");
            recentClipChannels.Clear();
        }

        static void Main(string[] args)
        {
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            SetConsoleMode(handle, ENABLE_EXTENDED_FLAGS);

            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            System.Timers.Timer cleartimer = new System.Timers.Timer(30000);
            cleartimer.Elapsed += (sender, stuff) => ClearRecentClipChannels();
            cleartimer.Start();


            var clientid = "";
            var nick = "";
            var password = "oauth:";
            var accessToken = "";

            recentClipChannels = new List<string>();

            Console.WriteLine($"MASS CHAT");
            ChannelHandler channelhandler = new ChannelHandler(clientid, accessToken);
            ChatHandler chatHandler = new ChatHandler(nick, password, channelhandler);



            while (true)
            {
                try
                {

                    var stuff = chatHandler.AverageTokensMessageHandlers;

                    foreach (var channel in stuff.SelectMany(c => c.Channels).ToList())
                    {
                        var count30 = channel.Value.CountLast(30);

                        if ((count30 > (channel.Value.AveragePerMinuteAllTime * 3)) &&
                            channel.Value.AveragePerMinuteAllTime > 0)
                        {
                            //Console.WriteLine(
                            //    $" {channel.Value.HandlerName.ToString()} handler average in channel {channel.Key} is {channel.Value.AveragePerMinuteAllTime} Per minute");
                            //Console.WriteLine($"Last 30 seconds was {count30}");

                            var streams = channelhandler.GetStreams().ConfigureAwait(false).GetAwaiter()
                                .GetResult();

                            if (streams.ContainsKey(channel.Key) && !recentClipChannels.Contains(channel.Key))
                            {
                                var id = streams[channel.Key].Channel.Id;
                                var clip = channelhandler.CreateClip(id).ConfigureAwait(false).GetAwaiter()
                                    .GetResult();

                                recentClipChannels.Add(channel.Key);

                                foreach (var coolBean in clip.CreatedClips)
                                {
                                    Console.WriteLine("--------------");
                                    Console.WriteLine(coolBean.Id);
                                    Console.WriteLine("--------------");
                                }
                            }

                        }
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }



        }
    }
}
