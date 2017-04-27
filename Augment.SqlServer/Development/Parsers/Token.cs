using System;

namespace Augment.SqlServer.Development.Parsers
{
    /// <summary>
    /// Class that represents a token.
    /// </summary>
    public class Token : IEquatable<Token>
    {
        #region Constructors

        public Token(TokenTypes type, Scanner sc)
        {
            Type = type;

            Line = sc.Line;
            Column = sc.Column;
            Start = sc.Position;
        }

        public Token(TokenTypes type, Scanner sc, string text)
            : this(type, sc)
        {

            Text = text;
        }

        #endregion

        #region Methods

        public bool Equals(Token other)
        {
            return Type == other.Type && Text == other.Text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public TokenTypes Type { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return _text; }
            internal set
            {
                OriginalText = value;

                _text = value;

                switch (Type)
                {
                    case TokenTypes.Comment:
                    case TokenTypes.StringLiteral:
                        break;

                    default:
                        _text = _text.ToUpper();
                        break;
                }

                Stop = Start + _text.Length - 1;
            }
        }
        private string _text;

        /// <summary>
        /// 
        /// </summary>
        public string OriginalText { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Stop { get; private set; }

        #endregion
    }

    ///// <summary>
    ///// Extension to Queue class.
    ///// </summary>
    //public static class QueueExtensions
    //{
    //    /// <summary>
    //    /// Verify if the queue contains a specific item.
    //    /// </summary>
    //    /// <param name="tokens">The queue to be verified</param>
    //    /// <param name="type">The type of the token to be compared</param>
    //    /// <param name="value">The value of the token to be compared</param>
    //    /// <returns></returns>
    //    public static bool Contains(this Queue<Token> tokens, TokenType type, string value)
    //    {
    //        var result = false;

    //        var tokensArray = tokens.ToArray();

    //        for(int i = 0; i < tokensArray.Length; i++)
    //        {
    //            if(tokensArray[i].Type == type && tokensArray[i].Value == value)
    //            {
    //                result = true;
    //                break;
    //            }
    //        }

    //        return result;
    //    }
    //}
}
