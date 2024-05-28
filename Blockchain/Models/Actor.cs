using Blockchain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    internal class Actor
    {
        public Guid ActorID { get; set; }

        public string Name { get; set; }

        double Balance { get; set; } 

        double tempBalance { get; set; } 


        private Ledger Ledger { get; set; }

        private Block CurrentPersonalBlock { get; set; }


        private Ledger TempLedger { get; set; }

        private StakeManager StakeManager { get; set; }

        private Network Network { get; set; }
        

    
        internal Broadcaster Broadcaster { get; set; }

        public IEncryptModule EncryptionModule { get; set; }

        bool CanTransact { get; set; }

        List<Stake> CurrentStakes { get; set; }

        public Actor(string name, IEncryptModule encryptModule)
        {
            ActorID = Guid.NewGuid();
            Name = name;
            EncryptionModule = encryptModule;
            Network = new Network(this);

            Ledger = new Ledger();
            TempLedger = new Ledger();

            CurrentPersonalBlock = new Block();

            Broadcaster = new Broadcaster();

            StakeManager= new StakeManager();

            Random random = new Random();

            Balance = Math.Round(random.NextDouble() * 100000,0);

            CurrentStakes = new List<Stake>();

            tempBalance =Balance;

            
        }

        public RSAParameters GetPublicKey()
        {
            return EncryptionModule.PublicKey;
        }


        //Create a transaction
        public void CreateTransaction(Guid receiver, double amount)
        {

            if (CanTransact == false)
            {
                return;
            }

            Transaction transaction = new Transaction();
            transaction.Sender = ActorID;
            transaction.Receiver = receiver;

            if (amount > tempBalance)
            {
                return;
            }
            else
            {
                tempBalance -= amount;
            }

            transaction.Amount = amount;

            transaction.TimeStamp = DateTime.Now;

            transaction.SenderSignature = EncryptionModule.Sign(transaction.SerializeTransaction(0));

            BroadcastTransaction(transaction);
        }

        public void Start()
        {
            BroadcastNewRound();
        }



        //Broadcast the transaction to the network
        void BroadcastTransaction(Transaction transaction)
        {   
            Broadcaster.BroadcastTransaction(transaction, Network);
        }


        //Receive a transaction
        public void TransactionListner(Transaction IncomingTransaction)
        {

            if (Program.Counter % 5 ==0 && Name!="Alice" && IsVerifier())
            {
                var x = 2;
            }

            if (CanTransact == false)
            {   

                return;
            }

            Transaction transaction = IncomingTransaction.Clone();

            if (transaction.SenderSignature !=null && transaction.ReceiverSignature!=null)
            {
                CurrentPersonalBlock.Transactions.Add(transaction);
                
                if (CurrentPersonalBlock.Transactions.Count==CurrentPersonalBlock.BlockLimit )
                {
                    CloseTransactionsListener();

                }
                
            }
            else if (transaction.Receiver == ActorID)
            {
                var sender = Network.ActiveActors.FirstOrDefault(x => x.Actor.ActorID == transaction.Sender);

                if (sender != null)
                {
                    Transaction LatestTransaction = transaction.Clone(); // remove circular reference

                    string Message = LatestTransaction.SerializeTransaction(0);

                    if (EncryptionModule.Verify(Message, LatestTransaction.SenderSignature, sender.Actor.GetPublicKey()))
                    {
                        string SignedMessage = LatestTransaction.SerializeTransaction( 1);
                        LatestTransaction.ReceiverSignature = EncryptionModule.Sign(SignedMessage);
                        //CurrentPersonalBlock.Transactions.Add(LatestTransaction);
                        BroadcastTransaction(LatestTransaction);
                    }
                }
            }
        }

        void VerifyBlock(Block block)
        {
            if (block.Transactions.Count > 0)
            {
                foreach (var transaction in block.Transactions)
                {
                    if (!VerifyTransaction(transaction)) 
                    {   
                        block.valid = false;
                        return;
                    };
                }
                block.valid= true;
            }

            
        }

        void SignBlock(Block block)
        {   
            block.Hash = block.CreateHash();
            block.SignerGuid = ActorID;
            block.Signature = EncryptionModule.Sign(block.SerializeBlock(0,true));
        }

        bool VerifyTransaction(Transaction transaction)
        {   
            var ReceiverPublicKey =Network.FullActors.FirstOrDefault(x => x.Actor.ActorID == transaction.Receiver).Actor.GetPublicKey();

            bool verifyReceiver = EncryptionModule.Verify(transaction.SerializeTransaction(1), transaction.ReceiverSignature, ReceiverPublicKey);

            if (verifyReceiver)
            {
                var SenderPublicKey = Network.FullActors.FirstOrDefault(x => x.Actor.ActorID == transaction.Sender).Actor.GetPublicKey();

                bool verifySender = EncryptionModule.Verify(transaction.SerializeTransaction(2), transaction.SenderSignature, SenderPublicKey);

                if (verifySender)
                {
                    return true;
                }
            }

            return false;
        }

        void BroadcastBlock(Block block, bool Validity,string Type)
        {
            Broadcaster.BroadcastBlock(Network,block, Validity,Type);
        }

        public void BlockListener(Block block,bool Validity,string Type)
        {   
            Block MyBlock= block.Clone();

            if (Type=="N")
            {
                TempLedger.Blocks.Add(MyBlock);
            }
            else if (Type == "R")
            {
                VerifyBlock(MyBlock);
                if (!VerifyBlockSignature(MyBlock))
                {
                    StrikeActor(MyBlock.SignerGuid);
                }
            }
            CurrentPersonalBlock = new Block();
            Settle();
        }


        void Settle()
        {
            if (TempLedger.Blocks.Count() > 3)
            {
                Block block = TempLedger.Blocks.First(); 

                if (block.valid)
                {
                    foreach (var transaction in block.Transactions)
                    {
                        if (transaction.Receiver == ActorID)
                        {
                            Balance += transaction.Amount;
                        }
                        if (transaction.Sender == ActorID)
                        {
                            Balance -= transaction.Amount;
                        }
                    }

                    Ledger.Blocks.Add(block);
                    TempLedger.Blocks.Remove(block);
                }
                tempBalance = Balance;
                Broadcaster.NotifyAmount(Name, Balance);
            }


             
            
        }


        void VerifyPreviousBlocks()
        {
            foreach (var block in TempLedger.Blocks)
            {
              VerifyPreviousBlock(block);
            }
        }   

        void VerifyPreviousBlock(Block block)
        {   
            if(block.valid)
            {
               if(!VerifyBlockIntegrity(block))
               {    
                    block.valid = false;
                    if (IsVerifier())
                    {
                        BroadcastBlock(block, false, "R");
                    }
                    return;
               }
               if (!VerifyBlockTransactions(block))
               {    
                    block.valid = false;

                    if (!VerifyBlockSignature(block))
                    {
                        StrikeActor(block.SignerGuid);
                    }


                    if (IsVerifier())
                    {
                        BroadcastBlock(block, false, "R");
                    }
                    return;
                }
               

            }


        }

        bool VerifyBlockSignature(Block block)
        {
            string Message = block.SerializeBlock(0, true);

            if (EncryptionModule.Verify(Message, block.Signature, Network.FullActors.FirstOrDefault(x => x.Actor.ActorID == block.SignerGuid).Actor.GetPublicKey()))
            {
                return true;
            }

            return false;
        }

        void StrikeActor(Guid ActorGuid)
        {
            InternalActor actor = Network.ActiveActors.Where(x => x.Actor.ActorID == ActorGuid).FirstOrDefault();

            if (actor!=null)
            {
                actor.StrikeCount++;
                if (actor.StrikeCount > 3)
                {
                    Network.RemoveActor(actor.Actor);
                }
            }
        }

        bool VerifyBlockTransactions(Block block)
        {
            foreach (var transaction in block.Transactions)
            {
                if (!VerifyTransaction(transaction))
                {
                    return false;
                }
            }

            var IDList = block.Transactions.Select(x => x.TransactionID).ToList().Distinct();

            if ( IDList.Count()!=block.Transactions.Count())
            {
                return false;
            }

            return true;
        }

        bool VerifyBlockIntegrity(Block block)
        {
            block.CreateHash();
            if (block.Hash == block.CreateHash())
            {
                return true;
            }

            return false;
        }


        public void ReceiveActor(Actor actor)
        {   
            Network.AddActor(actor);
        }

        //public void BroadcastBlacklist(Guid ActorID)
        //{
        //    Broadcaster.BroadcastBlacklist(Network, ActorID);
        //}

        //public void BlacklistListener(Guid ActorID)
        //{
        //    //Add the actor to the blacklist
        //}

        void RemoveActor(Actor actor)
        {
            Network.RemoveActor(actor);
        }

        public void BroadcastCloseTransactions()
        {
            Broadcaster.CloseTransactions(Network);
        }

        public void CloseTransactionsListener()
        {   
            // if verifier check the transactions, then broadcast.
            CanTransact = false;
            if (StakeManager.IsVerifier == true)
            {

                VerifyPreviousBlocks();

                VerifyBlock(CurrentPersonalBlock);

                SignBlock(CurrentPersonalBlock);
                
                BroadcastBlock(CurrentPersonalBlock, CurrentPersonalBlock.valid,"N");

                BroadcastNewRound();
            }
            
           
        }




        public void OpenTransactions()
        {



        }

        internal Stake DecideOwnStake() 
        {
            double StakeValue=StakeManager.GenerateStake(Balance);

            Stake stake = new Stake();
            stake.StakeValue =StakeValue;
            stake.ActorID = ActorID;
            stake.ActorName = Name;
            stake.Signature = EncryptionModule.Sign(ActorID.ToString());

            return stake;
        }
        
        
        
        internal void BroadcastStake(Stake stake)
        {
            Broadcaster.BroadcastStake(Network, stake);
        }


         void BroadcastNewRound()
        {   

            Broadcaster.BroadcastNewRound(Network);
            
        }


        public void NewRoundListen()
        {
            Stake MyStake = DecideOwnStake();
            CurrentStakes.Add(MyStake);
            BroadcastStake(MyStake);
        }


        public void StakesListener(Stake IncomingStake)
        {

            var a = Name;

            if (!CurrentStakes.Any(x=>x.StakeID==IncomingStake.StakeID))
            {
                CurrentStakes.Add(IncomingStake);
            }
            if (CurrentStakes.Count == Network.FullActors.Count)
            {
                DecideWinner();

            }
        }

        internal void DecideWinner()
        {
            if (StakeManager.DecideWinner(CurrentStakes) == ActorID)
            {   
                Console.WriteLine("Winner is " + Name    );
                if (Name!="Alice")
                {
                    var x = 2;
                }

                StakeManager.Win();
                //Broadcaster.OpenTransactions(Network);
            }
            else
            {
                StakeManager.Lose();
            }
            CurrentStakes = new List<Stake>();
            CanTransact = true;
        }

        public bool IsVerifier()
        {
            return StakeManager.IsVerifier;
        }
    }
}

