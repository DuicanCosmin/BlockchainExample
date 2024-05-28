using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class Stake
    {
        internal Guid StakeID { get; set; }

        internal Guid ActorID { get; set; }

        internal string ActorName { get; set; } 

        internal double StakeValue { get; set; }

        [JsonProperty]
        internal byte[] Signature { get; set; }

        public Stake()
        {
            StakeID = Guid.NewGuid();
        }

    }
}
