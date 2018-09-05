using System.Security.Cryptography;
using System.Text;

namespace IntranetSystem.Models
{
    public class Encryption
    {
        /// <summary>
        /// 16位：ComputeHash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getMd5Method(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] myData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < myData.Length; i++)
            {
                sBuilder.Append(myData[i].ToString("x"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 32位加密：ComputeHash
        /// </summary>
        /// <param name="input"></param>
        /// <returns><</returns>
        public static string getMd5Method2(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] myData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < myData.Length; i++)
            {
                sBuilder.Append(myData[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 32位加密：直接使用HashPasswordForStoringInConfigFile
        /// </summary>
        /// <param name="input"></param>
        /// <returns><</returns>
        public static string getMd5Method3(string input)
        {
            string myReturn = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input, "MD5");
            return myReturn.ToString();
        }
    }
}