using System;
using System.Collections.Generic;
using System.Text;

namespace masschat.Models
{

    public class Token
    {
        private readonly ITokenChecker _checker;
        private readonly string _tokenString;

        public bool Check(string message)
        {
            return _checker.Check(message, _tokenString);
        }

        public Token(ITokenChecker checker, string tokenString)
        {
            _checker = checker;
            _tokenString = tokenString;
        }
    }

    public interface ITokenChecker
    {
        //Return true if token is found in message
        bool Check(string message, string token);
    }

    /// <summary>
    /// Message must contain the token as a word surrounded by spaces ie: ("blabla token blalba") or start with or end with the word
    /// </summary>
    public class WordTokenChecker : ITokenChecker
    {
        public WordTokenChecker(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }
        public bool CaseSensitive { get; }

        public bool Check(string message, string token)
        {
            return message.Contains(" " + token + " ") || message.StartsWith(token + " ") || message.EndsWith(" " + token);
        }

    }

    /// <summary>
    /// Message must contain the token ("blablatokenblalba")
    /// </summary>
    public class ContainsTokenChecker : ITokenChecker
    {
        public ContainsTokenChecker(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }
        public bool CaseSensitive { get; }

        public bool Check(string message, string token)
        {
            if (CaseSensitive)
            {
                message = message.ToLower();
                token = token.ToLower();
            }

            return message.Contains(token);
        }

    }

    /// <summary>
    /// Message must exacly match 
    /// </summary>
    public class ExactlyTokenChecker : ITokenChecker
    {
        public ExactlyTokenChecker(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }

        public bool CaseSensitive { get; }

        public bool Check(string message, string token)
        {
            if (CaseSensitive)
            {
                message = message.ToLower();
                token = token.ToLower();
            }

            return message.Equals(token);
        }
    }

    /// <summary>
    /// Message must start with the token
    /// </summary>
    public class StartsWith : ITokenChecker
    {
        public StartsWith(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }

        public bool CaseSensitive { get; }

        public bool Check(string message, string token)
        {
            if (CaseSensitive)
            {
                message = message.ToLower();
                token = token.ToLower();
            }

            return message.StartsWith(token);
        }
    }

    /// <summary>
    /// Message must end with the token
    /// </summary>
    public class EndsWith : ITokenChecker
    {
        public EndsWith(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }

        public bool CaseSensitive { get; }
        public bool Check(string message, string token)
        {
            if (CaseSensitive)
            {
                message = message.ToLower();
                token = token.ToLower();
            }
            return message.EndsWith(token);
        }
    }
}
