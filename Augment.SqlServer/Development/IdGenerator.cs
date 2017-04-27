using System.Security.Cryptography;

namespace Augment.SqlServer.Development
{
    static class IdGenerator
    {
        #region Members

        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        private const string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static object _lock = new object();

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Random(int size = 6)
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
