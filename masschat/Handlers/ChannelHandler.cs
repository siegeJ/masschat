using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Clips.CreateClip;
using TwitchLib.Api.V5.Models.Streams;

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
            api = new TwitchAPI();
            api.Settings.ClientId = clientId;
            api.Settings.AccessToken = accessToken;
            var livestreams = await api.V5.Streams.GetLiveStreamsAsync(null, null, null, null, 100, 100);
            var livestreams2 = await api.V5.Streams.GetLiveStreamsAsync(null, null, null, null, 100, 100);
            var livestreams3 = await api.V5.Streams.GetLiveStreamsAsync(null, null, null, null, 100, 200);

            streams.Clear();

            var allStreams = livestreams.Streams.ToList();
            allStreams.AddRange(livestreams2.Streams);
            allStreams.AddRange(livestreams3.Streams);

            foreach (var livestream in allStreams)
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
            return await api.Helix.Clips.CreateClipAsync(channel);
        }
    }
}
