using System;
using masschat.Enums;

namespace masschat.Models
{
    public class AverageTokensChannelInfo
    {
        public AverageTokensMessageHandlerName HandlerName { get; set; }

        public string Channel { get; set; }

        public DateTime StartDate { get; set; }

        public int MessageCount { get; set; }

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

        public AverageTokensChannelInfo()
        {

        }

    }
}
