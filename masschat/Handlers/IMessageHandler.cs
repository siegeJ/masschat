﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using masschat.Models;
using TwitchLib.Client.Models;

namespace masschat.Handlers
{
    public interface IMessageHandler
    {
        IList<Token> Tokens { get; }
        bool Handle(ChatMessage message);
    }

}
