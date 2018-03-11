using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Models.Client;

namespace masschat.Models
{
    public interface IMessageHandler
    {
        bool Handle(ChatMessage message);
    }

    public abstract class MessageHandler : IMessageHandler
    {
        public abstract IList<Token> Tokens { get; }
        public IList<ChatMessage> MatchedMessages { get; set; }
        public int AverageMatchedTokensPerMinute { get; set; }

        protected MessageHandler()
        {
            MatchedMessages = new List<ChatMessage>();
            var minuteTimer = new Timer(10000);
            minuteTimer.Elapsed += async (sender, e) => await MinuteTimer();
            minuteTimer.Start();
        }
        private static Task MinuteTimer()
        {
            return new Task(() =>
            {

                //TODO do average calculatiosn


            });
        }

        public virtual bool Handle(ChatMessage message)
        {
            foreach (var token in Tokens)
            {
                if (token.Check(message.Message))
                {
                    MatchedMessages.Add(message);
                    return true;
                }
            }

            return false;
        }
    }

    public class KappaMessageHandler : MessageHandler
    {
        public override IList<Token> Tokens => new List<Token>()
        {
            new Token(new WordTokenChecker(), "Kappa" )
        };
    }
}
