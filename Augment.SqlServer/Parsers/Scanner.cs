using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Augment.SqlServer.Parsers
{
    public sealed class Scanner
    {
        #region Members

        private char[] _chars;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate the character buffer
        /// </summary>
        /// <param name="text"></param>
        public Scanner(string text)
        {
            _chars = text.ToUpper().ToCharArray();

            Text = text;
            Length = text.Length;

            Position = 0;
            Line = 1;
            Column = 1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ahead"></param>
        /// <returns></returns>
        public char Peek(int ahead)
        {
            int index = Position + ahead - 1;

            if (index.IsBetween(0, Text.Length - 1))
            {
                return _chars[index];
            }

            return '\0';
        }

        /// <summary>
        /// 
        /// </summary>
        public void Match()
        {
            char x = Peek(1);

            Consume();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanMatch(char c)
        {
            char x = Peek(1);

            return x == c;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CannotMatch(char c)
        {
            return !CanMatch(c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void Match(char c)
        {
            Ensure.That(CanMatch(c), "Matches", x => x.WithMessage($"{Line}:{Column}: found '{Peek(1)}' expected '{c}'"))
                .IsTrue();

            Consume();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CanMatchSequence(string text)
        {
            string chars = new string(GetRange(Position, text.Length).ToArray());

            return chars == text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CannotMatchSequence(string text)
        {
            return !CanMatchSequence(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void MatchSequence(string text)
        {
            string chars = new string(GetRange(Position, text.Length).ToArray());

            Ensure.That(CanMatchSequence(text), "Match Sequence", x => x.WithMessage($"{Line}:{Column}: found '{chars}' expected '{text}'"))
                .IsTrue();

            for (int x = 0; x < chars.Length; x++)
            {
                Consume();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CanMatchAny(string text)
        {
            char x = Peek(1);

            return text.IndexOf(x) > -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CannotMatchAny(string text)
        {
            return !CanMatchAny(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void MatchAny(string text)
        {
            char x = Peek(1);

            string chars = text.ToCharArray().Select(y => y.ToString()).Join(",");

            Ensure.That(CanMatchAny(text), "Match Any", o => o.WithMessage($"{Line}:{Column}: found '{x}' expected one of '{chars}'"))
                .IsTrue();

            Consume();
        }

        /// <summary>
        /// Remove the first character, and pull the next one in.
        /// </summary>
        private void Consume()
        {
            if (Position < Length)
            {
                if (Text[Position] == '\n')
                {
                    Column = 0;
                    Line += 1;
                }

                Column += 1;
                Position += 1;
            }
        }

        private IEnumerable<char> GetRange(int position, int length)
        {
            for (int x = position; x < position + length; x++)
            {
                yield return Peek(x - position + 1);
            }
        }

        #endregion

        #region Properties

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
        public int Position { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EndOfText { get { return Position >= Length; } }

        /// <summary>
        /// 
        /// </summary>
        public bool HasText { get { return !EndOfText; } }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; private set; }

        #endregion
    }
}
