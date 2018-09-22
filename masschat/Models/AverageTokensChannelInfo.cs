using System;
using System.Collections.Generic;
using System.Linq;
using masschat.Enums;

namespace masschat.Models
{
    public class AverageTokensChannelInfo
    {
        public AverageTokensMessageHandlerName HandlerName { get; set; }

        public string Channel { get; set; }

        public DateTime StartDate { get; set; }

        private int messageCount;
        public int MessageCount
        {
            get => messageCount;
            set
            {

                MessageDates.Add(DateTime.UtcNow);
                this.messageCount = value;
            }
        }

        public double AveragePerMinuteAllTime
        {
            get
            {
                var timeSpan = DateTime.UtcNow - StartDate;

                if (timeSpan.TotalMinutes >= 1)
                {
                    return Math.Round(MessageCount / timeSpan.TotalMinutes, 2);
                }

                return 0;
                

            }
        }

        public double CountLast(int seconds)
        {

            var end = DateTime.UtcNow;
            var start = DateTime.UtcNow.AddSeconds(-seconds);

            return MessageDates.Count(md => md > start && md < end);
            
        }


        public IList<DateTime> MessageDates { get; set; }

        public AverageTokensChannelInfo()
        {
            MessageDates = new List<DateTime>();
        }

    }
}
