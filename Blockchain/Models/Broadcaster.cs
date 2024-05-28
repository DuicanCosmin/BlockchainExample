using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Blockchain.Models
{
    internal class Broadcaster
    {
        public void BroadcastTransaction(Transaction transaction,Network network)
        {   
            if (transaction.ReceiverSignature == null)
            {
                //Console.WriteLine("Sent to recevier for signing");
            }
            else 
            {
                Console.WriteLine($"Broadcasting transaction: From {network.FullActors.Where(x => x.Actor.ActorID == transaction.Sender).FirstOrDefault().Actor.Name} to {network.FullActors.Where(x => x.Actor.ActorID == transaction.Receiver).FirstOrDefault().Actor.Name} : {transaction.Amount} ");
                //Console.WriteLine("Sent to network for recording");
            }

            //Console.WriteLine($"Broadcasting transaction: From {network.FullActors.Where(x=>x.Actor.ActorID==transaction.Sender).FirstOrDefault().Actor.Name} with value {transaction.Amount} to {network.FullActors.Where(x => x.Actor.ActorID == transaction.Receiver).FirstOrDefault().Actor.Name} ");

            //Parallel.ForEach(network.ActiveActors, actor =>
            //{
            //    actor.Actor.TransactionListner(transaction);
            //});

            foreach (var actor in network.FullActors)
            {
                actor.Actor.TransactionListner(transaction);
            }
        }


        public void OpenTransactions(Network network)
        {
            Parallel.ForEach(network.FullActors, actor =>
            {
                actor.Actor.OpenTransactions();
            });
        }

        public void CloseTransactions(Network network)
        {
            foreach (var actor in network.FullActors)
            {
                actor.Actor.CloseTransactionsListener();
            }

            //Parallel.ForEach(network.FullActors, actor =>
            //{
            //    actor.Actor.CloseTransactionsListener();
            //});
        }

        public void BroadcastStake(Network network,Stake stake)
        {
            foreach (var actor in network.FullActors)
            {
                actor.Actor.StakesListener(stake);
            }

            Console.WriteLine($"Broadcasting stake:Name {stake.ActorName} with value {stake.StakeValue}. Signed: {stake.Signature.ToString()}" );


            //Parallel.ForEach(network.ActiveActors, actor =>
            //{
            //    actor.Actor.ReceiveStake(stake);
            //});
        }

        public void NotifyAmount(string Name,double Amount)
        {
            Console.WriteLine($"{Name} new balance: {Amount}");
        }

        public void BroadcastBlock (Network network,Block block,bool Validity,string Type)
        {
            Console.WriteLine($"Block with ID {block.BlockGuid} boroadcasted with code {Type}.");

            foreach (var actor in network.FullActors)
            {
                actor.Actor.BlockListener(block, Validity,Type);
            }
        }

        //public void BroadcastBlacklist(Network network,Guid ActorID)
        //{
        //    Parallel.ForEach(network.ActiveActors, actor =>
        //    {
        //        actor.Actor.BlacklistListener(ActorID);
        //    });
        //}


        public void BroadcastNewRound(Network network)
        {
            foreach (var actor in network.FullActors)
            {
                actor.Actor.NewRoundListen();
            }

            //Parallel.ForEach(network.FullActors, actor =>
            //{
            //    actor.Actor.NewRoundListen();
            //});
        }
    }
}
