﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using masschat.Enums;
using masschat.Models;
using TwitchLib.Models.Client;

namespace masschat.Handlers
{


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

        public virtual bool Handle(ChatMessage message)
        {
            foreach (var token in Tokens)
            {
                if (token.Check(message.Message))
                {

                    if (Channels.ContainsKey(message.Channel))
                    {
                        Channels[message.Channel].MessageCount++;
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
                    return true;
                }
            }

            return false;
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

    public class LolAverageTokensMessageHandler : AverageTokensMessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "LOL" ),
            new Token(new WordTokenChecker(), "LUL" ),
            new Token(new WordTokenChecker(), "OMEGALUL" ),

        };

        public override AverageTokensMessageHandlerName MessageHandlerName => AverageTokensMessageHandlerName.LUL;
    }
}