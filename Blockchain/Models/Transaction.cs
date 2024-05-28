using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class Transaction
    {
        [JsonProperty]
        internal Guid TransactionID { get; set; }

        [JsonProperty]
        internal Guid Sender { get; set; }

        [JsonProperty]
        internal Guid Receiver { get; set; }

        [JsonProperty]
        internal double Amount { get; set; }

        [JsonProperty]
        internal byte[] SenderSignature { get; set; }

        [JsonProperty]
        internal byte[] ReceiverSignature { get; set; }

        [JsonProperty]
        internal DateTime TimeStamp { get; set; }  

        public Transaction()
            {
                TransactionID = Guid.NewGuid();
            }

        public string SerializeTransaction(int SignatureLevel)
        {
            var temp = new Transaction();
            
            temp.TransactionID = TransactionID;
            temp.Sender = Sender;
            temp.Receiver = Receiver;
            temp.Amount = Amount;
            temp.TimeStamp = TimeStamp;
            

            switch (SignatureLevel)
            {
                case 0:
                    break;
                case 1:
                    temp.SenderSignature = SenderSignature;
                    break;
                case 2:
                    temp.ReceiverSignature = ReceiverSignature;
                    break;
                default:
                    break;
            }


            string json = JsonConvert.SerializeObject(temp);
            return json;

        }

        public Transaction Clone()
        {
            return new Transaction
            {
                TransactionID = TransactionID,
                Sender = Sender,
                Receiver = Receiver,
                Amount = Amount,
                SenderSignature = SenderSignature,
                ReceiverSignature = ReceiverSignature,
                TimeStamp = TimeStamp
            };
        }



    }
}
