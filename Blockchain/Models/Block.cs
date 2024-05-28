using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Blockchain.Models
{
    internal class Block
    {
        public int Index { get; set; }

        public Guid BlockGuid { get; set; }

        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; } = "";

        public List<Transaction> Transactions { get; set; }=new List<Transaction>();

        public bool valid = false;

        public int BlockLimit=5;

        

        public Guid SignerGuid { get; set; }

        internal byte[] Signature { get; set; }

        public Block()
        {
            BlockGuid = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }

        public string SerializeBlock(int SignatureLevel,bool IncludeHash)
        {
            var temp = new Block();

            temp.Index = Index;
            temp.BlockGuid = BlockGuid;
            temp.TimeStamp = TimeStamp;
            temp.PreviousHash = PreviousHash;
            if(IncludeHash)
            {
                temp.Hash = Hash;
            }
            else
            {
                temp.Hash = "";
            }
            temp.Transactions = Transactions;
            temp.valid = valid;
            temp.BlockLimit = BlockLimit;


            switch (SignatureLevel)
            {
                case 0:
                    break;
                case 1:
                    temp.Signature = Signature;
                    break;
                default:
                    break;
            }


            string json = JsonConvert.SerializeObject(temp);
            return json;
        }

         public string CreateHash()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(SerializeBlock(0,false)));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public Block Clone()
        {
            return new Block
            {
                Index = Index,
                BlockGuid = BlockGuid,
                TimeStamp = TimeStamp,
                PreviousHash = PreviousHash,
                Hash = Hash,
                Transactions = Transactions,
                valid = valid,
                BlockLimit = BlockLimit,
                SignerGuid = SignerGuid,
                Signature = Signature
            };
        }
    }

    

}
