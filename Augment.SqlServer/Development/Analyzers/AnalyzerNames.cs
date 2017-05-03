using System.Security.Cryptography;

namespace Augment.SqlServer.Development.Analyzers
{
    static class AnalyzerNames
    {
        #region Members

        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        private const string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static object _lock = new object();

        #endregion

        #region Methods

        /// <summary>
        /// Convention: This is used to create a temporary object name
        /// for comparisons
        /// </summary>
        /// <returns></returns>
        public static string CreateCompareName(string baseName)
        {
            return $"dbo.ZC{RandomName()}_{ baseName}";
        }

        /// <summary>
        /// Convention: This is used to create a temporary object name
        /// for object manipulation
        /// ** Notice the missing schema
        /// </summary>
        /// <returns></returns>
        public static string CreateForRename(string baseName)
        {
            return $"ZM{RandomName()}_{ baseName}";
        }

        #endregion

        #region Random Name Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string RandomName(int size = 6)
        {
            lock (_lock)
            {
                byte[] data = new byte[size];

                _crypto.GetNonZeroBytes(data);

                char[] chars = new char[size];

                for (int i = 0; i < size; i++)
                {
                    chars[i] = _alphabet[data[i] % (_alphabet.Length - 1)];
                }

                return new string(chars);
            }
        }

        #endregion
    }
}
