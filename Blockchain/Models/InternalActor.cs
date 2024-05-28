using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class InternalActor
    {
        public Actor Actor { get; set; }

        public InternalActor(Actor actor)
        {
            Actor = actor;
        }

        internal int StrikeCount { get; set; }  
    }
}
