using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using masschat.Enums;
using masschat.Models;
using TwitchLib.Client.Models;

namespace masschat.Handlers
{

    public static class AverageHandlerHelper
    {
        public static List<IMessageHandler> Handlers => new List<IMessageHandler>()
        {
            new KappaAverageTokensMessageHandler(),
            new LolAverageTokensMessageHandler(),
            new PogChampAverageTokensMessageHandler(),
            new KreygasmTokensMessageHandler(),
            new MonkaSTokensMessageHandler(),
            new DansGameAverageTokensMessageHandler()
        };
    }

    public abstract class AverageTokensMessageHandler : IMessageHandler
    {
        public abstract IList<Token> Tokens { get; }

        public abstract AverageTokensMessageHandlerName MessageHandlerName { get; }

        /// <summary>
        /// Channel with matched messages
        /// </summary>
        public ConcurrentDictionary<string, AverageTokensChannelInfo> Channels { get; set; }

        protected AverageTokensMessageHandler()
        {
            Channels = new ConcurrentDictionary<string, AverageTokensChannelInfo>();
        }

        public virtual void Handle(ChatMessage message)
        {
            foreach (var token in Tokens)
            {
                if (token.Check(message.Message))
                {

                    if (Channels.ContainsKey(message.Channel))
                    {
                        Channels[message.Channel].MessageCount = Channels[message.Channel].MessageCount + 1;
                    }
                    else
                    {
                        Channels.TryAdd(message.Channel, new AverageTokensChannelInfo()
                        {
                            Channel = message.Channel,
                            HandlerName = MessageHandlerName,
                            MessageCount = 1,
                            StartDate = DateTime.UtcNow
                        });
                    }

                }
            }

        }
    }



    public class KappaAverageTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "Kappa" )
        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.Kappa;
    }


    public class DansGameAverageTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new ContainsTokenChecker(), "DansGame" ),
            new Token(new ContainsTokenChecker(), "WutFace" ),
            new Token(new ContainsTokenChecker(), "WTF" ),
        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.DansGame;
    }

    public class PogChampAverageTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new ContainsTokenChecker(), "PogChamp" ),
            new Token(new ContainsTokenChecker(), "Wow" ),
            new Token(new ContainsTokenChecker(), "POGGERS" ),
            new Token(new ContainsTokenChecker(), "MLG" ),
            new Token(new WordTokenChecker(), "POG" ),
            new Token(new ExactlyTokenChecker(), "POG" ),
        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.PogChamp;
    }

    public class LolAverageTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "LOL" ),
            new Token(new WordTokenChecker(), "LUL" ),
            new Token(new ExactlyTokenChecker(), "LUL" ),
            new Token(new ExactlyTokenChecker(), "LOL" ),
            new Token(new ContainsTokenChecker(), "HAHA" ),
            new Token(new ContainsTokenChecker(), "LMFAO" ),
            new Token(new ContainsTokenChecker(), "OMEGALUL" ),

        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.LUL;
    }

    public class KreygasmTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "Kreygasm" ),
            new Token(new ContainsTokenChecker(), "gasm" ),
        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.Kreygasm;
    }

    public class MonkaSTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "monkaS" ),
            new Token(new ContainsTokenChecker(), "gasm" ),
        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.MonkaS;
    }
}
