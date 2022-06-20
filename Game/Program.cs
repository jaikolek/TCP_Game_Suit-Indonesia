using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Multiplayer_Game
{
    class Program
    {
        //  exit or not?
        public static bool exit = false;

        //  variable to choose join or create game
        static Mode mode;
        static int choose;
        
        //  enum for player choose
        enum Mode
        { Server, Client, Error, }

        enum playerMove
        { Gajah, Orang, Semut, Exit, noInput, }

        struct Player
        {
            public playerMove move;
        }


        //  game
        static string game()
        {
            Player player;
            int choose;
            string input = "0";

            //  prevent error in program
            player.move = playerMove.noInput;

            Console.WriteLine();
            Console.WriteLine("Enter your move");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Gajah");
            Console.WriteLine("2. Orang");
            Console.WriteLine("3. Semut");
            Console.WriteLine("Input: "); choose = Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 0:
                    player.move = playerMove.Exit;
                    input = Convert.ToString(player.move);
                    break;

                case 1:
                    player.move = playerMove.Gajah;
                    input = Convert.ToString(player.move);
                    break;

                case 2:
                    player.move = playerMove.Orang;
                    input = Convert.ToString(player.move);
                    break;

                case 3:
                    player.move = playerMove.Semut;
                    input = Convert.ToString(player.move);
                    break;

                default:
                    Console.WriteLine("Error::Please check your input and restart the program");
                    player.move = playerMove.noInput;
                    input = Convert.ToString(playerMove.noInput);
                    break;
            }

            return input;
        }


        //  create server
        static void becameServer()
        {
            int port = 1212;
            string ipAddress = "127.0.0.1";
            Socket serverListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            serverListener.Bind(ep);
            serverListener.Listen(100);
            Console.WriteLine("Waiting Opponent...");
            Socket clientSocket = default(Socket);
            int num = 0;
            Program p = new Program();

            while (!exit)
            {
                clientSocket = serverListener.Accept();
                Console.WriteLine("Opponent connected...");
                Console.WriteLine();

                Thread userThread = new Thread(new ThreadStart(() => Program.User(clientSocket)));
                userThread.Start();
            }    
        }

        public static void User(Socket client)
        {
            while (!exit)
            {
                //  get data
                byte[] message = new byte[1024];
                int size = client.Receive(message);

                //  turn
                string opponent = System.Text.Encoding.ASCII.GetString(message, 0, size);

                //  check disconnected
                if (opponent == Convert.ToString(playerMove.Exit))
                {
                    Console.WriteLine();
                    Console.WriteLine("Opponent Disconnected...");
                    Console.WriteLine();
                    exit = true;
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("Your turn...");
                Console.WriteLine();

                string yourAnswer = null;
                yourAnswer = game();

                //  check
                check(opponent, yourAnswer);

                //  send
                client.Send(System.Text.Encoding.ASCII.GetBytes(yourAnswer), 0, yourAnswer.Length, SocketFlags.None);
            }
        }


        //  join server
        static void becameClient()
        {
            int port = 1212;
            string ipAddress = "127.0.0.1";
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            clientSocket.Connect(ep);
            Console.WriteLine("Connected to Opponent...");

            Program p = new Program();

            while (!exit)
            {
                //  turn
                Console.WriteLine();
                Console.WriteLine("Your turn...");
                Console.WriteLine();
                string yourAsnwer = null;
                yourAsnwer = game();

                //  send
                clientSocket.Send(System.Text.Encoding.ASCII.GetBytes(yourAsnwer), 0, yourAsnwer.Length, SocketFlags.None);

                //  get data
                byte[] message = new byte[1024];
                int size = clientSocket.Receive(message);
                string opponent = null;
                opponent = System.Text.Encoding.ASCII.GetString(message, 0, size);

                //  check disconnected
                if (opponent == Convert.ToString(playerMove.Exit))
                {
                    Console.WriteLine();
                    Console.WriteLine("Opponent Disconnected...");
                    Console.WriteLine();
                    exit = true;
                    break;
                }

                // check
                check(opponent, yourAsnwer);   
            }
        }

        
        //  checking game
        public static void check(string opponentAnswer, string yourAnswer)
        {
            if (opponentAnswer == Convert.ToString(playerMove.Gajah))
            {
                //  Gajah
                if (yourAnswer == Convert.ToString(playerMove.Orang))
                {
                    Console.WriteLine();
                    Console.WriteLine("You lose...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Semut))
                {
                    Console.WriteLine();
                    Console.WriteLine("You win...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Gajah))
                {
                    Console.WriteLine();
                    Console.WriteLine("Draw...");
                    Console.WriteLine();
                }
            }

            if (opponentAnswer == Convert.ToString(playerMove.Orang))
            {
                //  Orang
                if (yourAnswer == Convert.ToString(playerMove.Semut))
                {
                    Console.WriteLine();
                    Console.WriteLine("You lose...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Gajah))
                {
                    Console.WriteLine();
                    Console.WriteLine("You win...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Orang))
                {
                    Console.WriteLine();
                    Console.WriteLine("Draw...");
                    Console.WriteLine();
                }
            }

            if (opponentAnswer == Convert.ToString(playerMove.Semut))
            {
                //  Semut
                if (yourAnswer == Convert.ToString(playerMove.Gajah))
                {
                    Console.WriteLine();
                    Console.WriteLine("You lose...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Orang))
                {
                    Console.WriteLine();
                    Console.WriteLine("You win...");
                    Console.WriteLine();
                }
                else if (yourAnswer == Convert.ToString(playerMove.Semut))
                {
                    Console.WriteLine();
                    Console.WriteLine("Draw...");
                    Console.WriteLine();
                }
            }

            //  no input
            if (opponentAnswer == Convert.ToString(playerMove.noInput))
            {
                Console.WriteLine();
                Console.WriteLine("False opponent input...");
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my game (Suit Indonesia)");
            Console.WriteLine("Choose Game");
            Console.WriteLine("1. Create a server");
            Console.WriteLine("2. Joining a server");
            Console.Write("Input: "); choose = Convert.ToInt32(Console.ReadLine());
            
            switch(choose)
            {
                case 1:
                    mode = Mode.Server;
                    break;

                case 2:
                    mode = Mode.Client;
                    break;

                default:
                    mode = Mode.Error;
                    break;
            }

            switch(mode)
            {
                case Mode.Server:
                    Console.WriteLine();
                    Console.WriteLine("becoming " + mode);
                    if(!exit) becameServer();
                    break;

                case Mode.Client:
                    Console.WriteLine();
                    Console.WriteLine("becoming " + mode);
                    if(!exit) becameClient();
                    break;

                case Mode.Error:
                    Console.WriteLine();
                    Console.WriteLine("Error::Please check your input and restart the program");
                    break;

                default: break;
            }
            
            //  preventing program close before user can read all data
            Console.ReadLine();
        }
    }
}
