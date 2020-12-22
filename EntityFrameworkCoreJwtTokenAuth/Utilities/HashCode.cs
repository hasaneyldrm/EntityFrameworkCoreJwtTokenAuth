using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HashidsNet;

namespace EntityFrameworkCoreJwtTokenAuth.Utilities
{
    public class HashCode
    {
        public static string GeneratePublicId(int id)
        {

            var hashids = new Hashids(salt: "yourSeed", minHashLength: 8,
                alphabet: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            var returnId = hashids.Encode(id);

            return returnId;
        }

        public static string RandomString(int length)
        {
            Random r = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(value: Enumerable.Repeat(element: chars, count: length)
                .Select(selector: s => s[index: r.Next(maxValue: s.Length)]).ToArray());
        }


        public static string Encrypt(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(s: password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: "yourSeed"));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider()
                { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(inputBuffer: data, inputOffset: 0, inputCount: data.Length);
                    return Convert.ToBase64String(inArray: results, offset: 0, length: results.Length);
                }
            }
        }


        public static string Decrypt(string decryptValue)
        {
            byte[] data = Convert.FromBase64String(s: decryptValue);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: "yourSeed"));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(inputBuffer: data, inputOffset: 0, inputCount: data.Length);
                    return Encoding.UTF8.GetString(bytes: results);
                }
            }
        }
    }
}
