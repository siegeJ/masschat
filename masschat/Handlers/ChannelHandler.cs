using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib;
using TwitchLib.Models.API.v3.Streams;

namespace masschat.Handlers
{

    public interface IChannelHandler
    {
        Task<ConcurrentDictionary<string, Stream>> GetStreams();
    }

    public class ChannelHandler : IChannelHandler
    {
        private static TwitchAPI api;
        private string clientId;

        private ConcurrentDictionary<string, Stream> streams;
    

        public ChannelHandler(string clientId, int refreshChannelMinutes)
        {
            this.clientId = clientId;
            streams = new ConcurrentDictionary<string, Stream>();
            var channelRefreshTimer = new Timer(TimeSpan.FromMinutes(refreshChannelMinutes).TotalMilliseconds);
            channelRefreshTimer.Elapsed += async (sender, args) => await PopulateChannels();
            channelRefreshTimer.Start();

        }

        public async Task<bool> PopulateChannels()
        {
            Console.WriteLine($"Getting Channels From Twitch");
            api = new TwitchAPI(clientId);
            var livestreams = await api.Streams.v3.GetStreamsAsync(null, null, 100, 0, clientId, TwitchLib.Enums.StreamType.Live);

            streams.Clear();

            foreach (var livestream in livestreams.Streams)
            {
                streams.TryAdd(livestream.Channel.Name, livestream);
            }
    
            return true;
        }

        public async Task<ConcurrentDictionary<string, Stream>> GetStreams()
        {
            if (!streams.Any())
            {
                await PopulateChannels();
            }

            return streams;
        }
    }
}
