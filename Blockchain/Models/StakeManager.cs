using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class StakeManager
    {   
  

        public int RoundsWithoutWin { get; set; }=0;

        Random random = new Random();

        internal bool IsVerifier { get; set;}

        public StakeManager()
        {
            RoundsWithoutWin = 0;
            IsVerifier = false;
        }

        internal void Win()
        {
            RoundsWithoutWin = 0;
            IsVerifier = true;
        }

        internal void Lose()
        {
            RoundsWithoutWin++;
            IsVerifier = false;
        }

        internal double GenerateStake(double Amount)
        {   
            double BaseStake = Math.Round(random.NextDouble()* Amount);

            double UpdatedStake=BaseStake*(10^RoundsWithoutWin);

            return UpdatedStake;
        }   

        internal Guid DecideWinner(List<Stake> stakes)
        {
            Guid? winnerID =stakes.FirstOrDefault(x=>x.StakeValue == stakes.Max(x => x.StakeValue)).ActorID;
            return winnerID.Value;
        }
    }

}
