using System.Security.Cryptography;

namespace Blockchain.Interfaces
{
    internal interface IEncryptModule
    {
        public RSAParameters PublicKey { get; set; }

        //string Decrypt(byte[] cipherText);
        //byte[] Encrypt(string plainText);
        byte[] Sign(string plainText);
        bool Verify(string plainText, byte[] signature, RSAParameters SignerPublicKey);
    }
}