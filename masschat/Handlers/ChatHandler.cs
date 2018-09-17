using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace masschat.Handlers
{
    public class ChatHandler
    {
        private readonly ConnectionCredentials _credentials;
        private readonly IChannelHandler _channelHandler;
        private int _messagesProcessed = 0;
        private TwitchClient client;

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
            this._channelHandler = channelHandler;

            MessageHandlers = new List<IMessageHandler> { new KappaAverageTokensMessageHandler(), new LolAverageTokensMessageHandler(), new PogChampAverageTokensMessageHandler() };
            
            if (messageHandlers != null)
                MessageHandlers.AddRange(messageHandlers);

            Start();

            var channelRefreshTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            channelRefreshTimer.Elapsed += async (sender, args) => await JoinAllChannels();
            channelRefreshTimer.Start();
        }

        private async void Start()
        {
            await JoinAllChannels().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns true when all channels are joined
        /// </summary>
        /// <returns></returns>
        public async Task<bool> JoinAllChannels()
        {
            try
            {

                if (client == null || !client.IsConnected)
                {
                    client = new TwitchClient();
                    client.Initialize(_credentials);
                    client.OnJoinedChannel += onJoinedChannel;
                    client.OnMessageReceived += onMessageReceived;
                    client.Connect();
                }
                else
                {
                    Console.WriteLine($"Refreshing IRC Channels");
                }

                var streams = (await _channelHandler.GetStreams()).ToList();
                var joinedChannels = client.JoinedChannels.ToList();
                foreach (var stream in streams)
                {
                    //join if not in already
                    if (!joinedChannels.Select(j => j.Channel).Contains(stream.Value.Channel.Name))
                    {
                        Console.WriteLine("Joined " + stream.Value.Channel.Name);
                        client.JoinChannel(stream.Value.Channel.Name);
                    }
                }


                //Leave channels that we didnt get back (no longer live)
                foreach (var joinedChannel in joinedChannels)
                {
                    if (!streams.Select(s => s.Value.Channel.Name).Contains(joinedChannel.Channel))
                    {
                        Console.WriteLine($"Leaving {joinedChannel.Channel}");
                        client.LeaveChannel(joinedChannel);
                    }
                }

                Console.WriteLine($"Currently in {streams.Count} channels.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return true;

        }


        private void onJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            //Console.WriteLine("Joined " + e.Channel);
        }

        private void onMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            try
            {
                _messagesProcessed++;


                //simple check for testing to see how fast its getting messages
                if (e.ChatMessage.Message.Contains("this is a test"))
                {
                    Console.WriteLine($"{e.ChatMessage.Message} {e.ChatMessage.Channel} {e.ChatMessage.Username}");
                }

                foreach (var messageHandler in MessageHandlers)
                {
                    messageHandler.Handle(e.ChatMessage);
                }

                if (_messagesProcessed % 1000 == 0)
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
