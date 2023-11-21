using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Crm.Sdk.Messages;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class CryptoExtension
    {
        private static TripleDESCryptoServiceProvider GetCryptoProvider(Executor executor, string secureKey = null)
        {
            if (secureKey == null)
            {
                var orgDetails = ((RetrieveCurrentOrganizationResponse)executor.ElevatedOrganizationService.Execute(request: new RetrieveCurrentOrganizationRequest())).Detail;
                secureKey = orgDetails.UniqueName;
            }
            var md5 = new MD5CryptoServiceProvider();
            var desKey = md5.ComputeHash(Encoding.UTF8.GetBytes(secureKey));
            return new TripleDESCryptoServiceProvider()
            {
                Key = desKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }

        public static string Encrypt(this Executor executor, string plainString, string key = null)
        {
            var data = Encoding.UTF8.GetBytes(plainString);
            var tripleDes = GetCryptoProvider(executor, key);
            var transform = tripleDes.CreateEncryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(resultsByteArray);
        }

        public static string Decrypt(this Executor executor, string encryptedString, string key = null)
        {
            var data = Convert.FromBase64String(encryptedString);
            var tripleDes = GetCryptoProvider(executor, key);
            var transform = tripleDes.CreateDecryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(resultsByteArray);
        }
    }
}
