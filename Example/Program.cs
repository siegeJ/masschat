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
using masschat.Models;
using TwitchLib.Client.Models;

namespace Example
{
    public class ExampleMessageHandler : IMessageHandler
    {
        private string toFind => "My first IMessageHandler!";

        public void Handle(ChatMessage message)
        {
            if (message.Message.Contains(toFind))
            {
                Console.WriteLine($"We found it in {message.Channel}, thanks {message.Username}: {message.Message}");
            }
        }
    }

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

            System.Timers.Timer cleartimer = new System.Timers.Timer(60000);
            cleartimer.Elapsed += (sender, stuff) => ClearRecentClipChannels();
            cleartimer.Start();
            Console.WriteLine($"MASS CHAT");


            var clientid = "";
            var nick = "";
            var password = "oauth:";
            var accessToken = "";

            var exampleHandler = new ExampleMessageHandler();

            ChannelHandler channelhandler = new ChannelHandler(clientid, accessToken, 5);
            ChatHandler chatHandler = new ChatHandler(nick, password, channelhandler, new IMessageHandler[] {exampleHandler});


            recentClipChannels = new List<string>();

            while (true)
            {
                try
                {

                    var messageHandlers = chatHandler.AverageTokensMessageHandlers;
                    var channels = messageHandlers.SelectMany(c => c.Channels).ToList();

                    foreach (var channel in channels)
                    {
                        var count30 = channel.Value.CountLast(30);


                        if ((count30 > (channel.Value.AveragePerMinuteAllTime * 3)) &&
                            channel.Value.AveragePerMinuteAllTime > 2)
                        {

                            var streams = channelhandler.GetStreams().ConfigureAwait(false).GetAwaiter()
                                .GetResult();

                            if (streams.ContainsKey(channel.Key) && !recentClipChannels.Contains(channel.Key))
                            {
                                var id = streams[channel.Key].Channel.Id;
                                Console.WriteLine($"Clip being created in {channel.Key} for {channel.Value.HandlerName}: Last 30: {count30}. Average: {channel.Value.AveragePerMinuteAllTime}");

                                var createdClips = channelhandler.CreateClip(id).ConfigureAwait(false).GetAwaiter()
                                    .GetResult().CreatedClips;

                                recentClipChannels.Add(channel.Key);

                                foreach (var clip in createdClips)
                                {
                                    Console.WriteLine("--------------");
                                    Console.WriteLine(clip.Id);
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
