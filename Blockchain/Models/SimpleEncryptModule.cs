using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blockchain.Interfaces;

namespace Blockchain.Models
{
    public class SimpleEncryptModule : IEncryptModule
    {
        public RSAParameters PublicKey { get; set; }
        private RSAParameters _privateKey { get; set; }

        public SimpleEncryptModule()
        {
            using (var rsa = RSA.Create())
            {
                PublicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);
            }
        }

        public byte[] Encrypt(string plainText)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(PublicKey);
                var data = Encoding.UTF8.GetBytes(plainText);
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
        }

        public string Decrypt(byte[] cipherText)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(_privateKey);
                var decryptedData = rsa.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        public byte[] Sign(string plainText)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(_privateKey);
                var data = Encoding.UTF8.GetBytes(plainText);
                return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public bool Verify(string plainText, byte[] signature,RSAParameters SignerPublicKey)
        {
            using (var rsa = RSA.Create())
            {   
                rsa.ImportParameters(SignerPublicKey);
                var data = Encoding.UTF8.GetBytes(plainText);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
