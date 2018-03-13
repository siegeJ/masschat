using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masschat.Handlers;
using masschat.Models;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace masschat
{
    public class ChatHandler
    {
        private IList<TwitchClient> _clients;
        private readonly ConnectionCredentials _credentials;
        private readonly IChannelHandler _channelHandler;
        private int _messagesProcessed = 0;

        public List<IMessageHandler> MessageHandlers { get; set; }

        public List<AverageTokensMessageHandler> AverageTokensMessageHandlers
        {
            get
            {
                var averagehandlers = MessageHandlers
                    .OfType<AverageTokensMessageHandler>().ToList();

                return averagehandlers;
            }
        }

        /// <summary>
        /// Reads messages from chat then filters them
        /// </summary>
        /// <param name="nick">irc nick</param>
        /// <param name="password">irc password</param>
        /// <param name="channelHandler">channel handler that gets channels</param>
        /// <param name="messageHandlers">If not supplied a defualt list of message handlers will be provided</param>
        public ChatHandler(string nick, string password, IChannelHandler channelHandler, IList<IMessageHandler> messageHandlers = null)
        {
            _credentials = new ConnectionCredentials(nick, password);
            _clients = new List<TwitchClient>();
            this._channelHandler = channelHandler;

            MessageHandlers = new List<IMessageHandler> { new KappaAverageTokensMessageHandler(), new LolAverageTokensMessageHandler() };
            
            if (messageHandlers != null)
                MessageHandlers.AddRange(messageHandlers);
        }

        /// <summary>
        /// Returns true when all channels are joined
        /// </summary>
        /// <returns></returns>
        public async Task<bool> JoinAllChannels()
        {

            var streams = _channelHandler.GetStreams().ConfigureAwait(false);

            var client = new TwitchClient(_credentials);
            client.OnJoinedChannel += onJoinedChannel;
            client.OnMessageReceived += onMessageReceived;
            client.Connect();

            foreach (var stream in await streams)
            {
                client.JoinChannel(stream.Channel.Name);
            }

            return true;
        }


        private void onJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Joined " + e.Channel);
        }

        private void onMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            try
            {
                _messagesProcessed++;

                foreach (var messageHandler in MessageHandlers)
                {
                    if (messageHandler.Handle(e.ChatMessage))
                    {
                        //Console.WriteLine(e.ChatMessage.Message);
                    }
                }

                if (_messagesProcessed % 250 == 0)
                {
                    Console.WriteLine(_messagesProcessed + " messages processed.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("WAT HAPPENED??" + exception.StackTrace);
            }

        }

    }
}
