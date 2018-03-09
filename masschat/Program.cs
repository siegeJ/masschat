using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using System.Threading;
using System.Threading.Tasks;

namespace masschat
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine($"MASS CHAT");
            Console.WriteLine($"Nick: {Configuration.Instance.Nick}");
    
            ChannelHandler channelhandler = new ChannelHandler(Configuration.Instance.ClientId);
            ChatHandler chatHandler = new ChatHandler(Configuration.Instance.Nick, Configuration.Instance.Password, channelhandler);

            var task = Task.Run(() => chatHandler.JoinAllChannels().ConfigureAwait(false));

            while(true)
            {
                Console.WriteLine($"we be waiting");
                Thread.Sleep(5000);
            }
        }

    }
}
