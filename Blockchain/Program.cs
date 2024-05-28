using Blockchain.Models;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Blockchain
{
    internal class Program
    {
        public static bool Spark = false;

        public static int Counter = 1;

        static void Main(string[] args)
        {

            Actor Alice=new Actor("Alice", new SimpleEncryptModule());

            Actor Bob = new Actor("Bob", new SimpleEncryptModule());

            Actor Charlie = new Actor("Charlie", new SimpleEncryptModule());

            Actor David = new Actor("David", new SimpleEncryptModule());

            Actor Eve = new Actor("Eve", new SimpleEncryptModule());

            List<Actor> TempList = new List<Actor>();

            

            TempList.Add(Alice);
            TempList.Add(Bob);
            TempList.Add(Charlie);
            TempList.Add(David);
            TempList.Add(Eve);

            foreach (var actor in TempList)
            {
                foreach (var actor2 in TempList)
                {
                    if (actor != actor2)
                    {
                        actor.ReceiveActor(actor2);
                    }
                }
            }

               



            Alice.Start();
            Random random = new Random();


            for (int i = 1; i < 100; i++)
            {   
                Program.Counter= i;
                int SenderIndex = new Random().Next(0, TempList.Count());
                int ReceiverIndex = new Random().Next(0, TempList.Count());
                int Amount = new Random().Next(0, 100);
                TempList[SenderIndex].CreateTransaction(TempList[ReceiverIndex].ActorID, Amount);
                Thread.Sleep(50);

            }


           


            //Bob.CreateTransaction(Alice.ActorID, 20);


            //Bob.CreateTransaction(Charlie.ActorID, 30);

            //Eve.CreateTransaction(David.ActorID, 40);

            //David.CreateTransaction(Alice.ActorID, 50);


            //Bob.CreateTransaction(Eve.ActorID, 60);

            //Alice.CreateTransaction(Bob.ActorID, 70);

            //Alice.CreateTransaction(Bob.ActorID, 100);

            //Bob.CreateTransaction(Alice.ActorID, 200);


            //Bob.CreateTransaction(Charlie.ActorID, 300);

            //Eve.CreateTransaction(David.ActorID, 400);

            //David.CreateTransaction(Alice.ActorID, 500);


            //Bob.CreateTransaction(Eve.ActorID, 600);

            //Alice.CreateTransaction(Bob.ActorID, 700);


            //Bob.CreateTransaction(Charlie.ActorID, 300);

            //Eve.CreateTransaction(David.ActorID, 400);

            //David.CreateTransaction(Alice.ActorID, 500);


        }   


    }
}
