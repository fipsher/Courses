using System.Security.Cryptography;
using System.Text;

namespace LNU.Courses.Repositories
{
    public class HashProvider : IHashProvider
    {
        private MD5 _md5 = MD5.Create();
        /// <summary>
        /// Get hash code of stringToHash param
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        public string Encrypt(string stringToHash)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(stringToHash);
            byte[] hash = md5.ComputeHash(inputBytes);


            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
