namespace Augment.SqlServer.Parsers
{
    public enum TokenTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Word,
        /// <summary>
        /// SQL Keyword
        /// </summary>
        Keyword,
        /// <summary>
        /// Not a letter or digit
        /// </summary>
        Symbol,
        /// <summary>
        /// Starts with '@'
        /// </summary>
        Variable,
        /// <summary>
        /// Words starting with ' or N' or n'
        /// </summary>
        StringLiteral,
        /// <summary>
        /// Integer, Float, Scientific Notation
        /// </summary>
        NumericLiteral,
        /// <summary>
        /// Starts with 0x
        /// </summary>
        BinaryLiteral,
        /// <summary>
        /// -- or /*
        /// </summary>
        Comment,
        /// <summary>
        /// Enclosed with brackets
        /// </summary>
        EnclosedWord
    }
}
