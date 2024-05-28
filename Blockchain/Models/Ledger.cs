using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class Ledger
    {
        public List<Block> Blocks { get; set; }

        public Ledger()
        {
            Blocks = new();
        }
    }
}
