using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class Network
    {
        public Actor Self { get; set; }

        public List<InternalActor> ActiveActors { get; set; } = new();

        public List<InternalActor> BlackList { get; set; } = new();

        public Network(Actor self)
        {
            Self = self;
        }


        public List<InternalActor> FullActors { get
            {
                List<InternalActor> fullActors = new();
                fullActors.AddRange(ActiveActors);
                fullActors.Add(new InternalActor(Self));
                return fullActors;
            }
        }

        internal void AddActor(Actor actor)
        {   
            if (actor!=Self)
            {
                ActiveActors.Add(new InternalActor(actor));
            }

        }

        internal void RemoveActor(Actor actor)
        {

            InternalActor internalActor = new InternalActor(actor);
            BlackList.Add(internalActor);
            ActiveActors.Remove(internalActor);
        }   

    }
}
