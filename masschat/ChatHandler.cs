using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace masschat
{
    public class ChatHandler
    {
        private IList<TwitchClient> clients;
        private ConnectionCredentials credentials;
        private ChannelHandler channelHandler;

        public ChatHandler(string nick, string password, ChannelHandler channelHandler)
        {
            credentials = new ConnectionCredentials(nick, password);
            clients = new List<TwitchClient>();
            this.channelHandler = channelHandler;


        }

        /// <summary>
        /// Returns true when all channels are joined
        /// </summary>
        /// <returns></returns>
        public async Task<bool> JoinAllChannels()
        {

            var streams = await channelHandler.GetStreams();

            foreach (var stream in streams)
            {
                var client = new TwitchClient(credentials, stream.Channel.Name);
                client.OnJoinedChannel += onJoinedChannel;
                client.OnMessageReceived += onMessageReceived;
                client.Connect();
            }

            return true;
        }


        private void onJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Joined " + e.Channel);
        }

        private void onMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine(e.ChatMessage.Message);
        }

    }
}
