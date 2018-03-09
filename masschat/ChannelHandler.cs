using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.API.v3.Streams;


namespace masschat
{
    public class ChannelHandler
    {
        private static TwitchAPI api;
        private string clientId;

        public List<Stream> streams = new List<Stream>();

        public ChannelHandler(string clientId)
        {
            this.clientId = clientId;
        }

        public async Task<bool> PopulateChannels()
        {
            api = new TwitchAPI(clientId);
            var livestreams = await api.Streams.v3.GetStreamsAsync(null, null, 10, 0, clientId, TwitchLib.Enums.StreamType.Live);

            streams.Clear();
            streams.AddRange(livestreams.Streams);
            
            return true;
        }

        public async Task<IList<Stream>> GetStreams()
        {
            if (!streams.Any())
            {
                await PopulateChannels();
            }

            return streams;
        }

    }
}
