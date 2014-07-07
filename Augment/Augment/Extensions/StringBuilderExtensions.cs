using System.Text;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// StringBuilder extensions
    /// </summary>
    public static class StringBuilderExtensions
    {
        #region Methods

        /// <summary>
        /// Append If condition true
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="condition"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string trueValue, string falseValue = null)
        {
            Ensure.That(sb).IsNotNull();

            if (condition)
            {
                return sb.Append(trueValue);
            }
            
            if (!condition && falseValue.IsNotEmpty())
            {
                return sb.Append(falseValue);
            }

            return sb;
        }

        #endregion
    }
}
