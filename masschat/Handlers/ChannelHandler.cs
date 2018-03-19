using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib;
using TwitchLib.Models.API.Helix.Clips.CreateClip;
using TwitchLib.Models.API.v3.Streams;

namespace masschat.Handlers
{

    public interface IChannelHandler
    {
        Task<Dictionary<string, Stream>> GetStreams();
    }

    public class ChannelHandler : IChannelHandler
    {
        private static TwitchAPI api;
        private string clientId;
        private string accessToken;

        private ConcurrentDictionary<string, Stream> streams;
    

        public ChannelHandler(string clientId, string accessToken, int refreshChannelMinutes = 1)
        {
            this.clientId = clientId;
            this.accessToken = accessToken;
            streams = new ConcurrentDictionary<string, Stream>();
            var channelRefreshTimer = new Timer(TimeSpan.FromMinutes(refreshChannelMinutes).TotalMilliseconds);
            channelRefreshTimer.Elapsed += async (sender, args) => await PopulateChannels();
            channelRefreshTimer.Start();

        }

        public async Task<bool> PopulateChannels()
        {
            Console.WriteLine($"Getting Channels From Twitch");
            api = new TwitchAPI(clientId, accessToken);
            var livestreams = await api.Streams.v3.GetStreamsAsync(null, null, 100, 0, clientId, TwitchLib.Enums.StreamType.Live);

            streams.Clear();

            foreach (var livestream in livestreams.Streams)
            {
                streams.TryAdd(livestream.Channel.Name, livestream);
            }
    
            return true;
        }

        public async Task<Dictionary<string, Stream>> GetStreams()
        {
            if (!streams.Any())
            {
                await PopulateChannels();
            }



            return streams.ToDictionary(s => s.Key, s=> s.Value);
        }

        public async Task<CreatedClipResponse> CreateClip(string channel)
        {
            
            //var authToken = await api.Auth.v5.RefreshAuthTokenAsync()
            return await api.Clips.helix.CreateClipAsync(channel);
        }
    }
}
