using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Luffarschack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine( "    ___       ___       ___       ___       ___       ___            ___       ___    ||    ___       ___       ___   ");
            Console.WriteLine(@"   /\  \     /\  \     /\  \     /\  \     /\  \     /\__\          /\  \     /\__\   ||   /\  \     /\  \     /\  \ ");
            Console.WriteLine(@"  /::\  \   /::\  \   /::\  \    \:\  \   /::\  \   /:/  /         /::\  \   /:/__/_  ||  /::\  \   /::\  \   /::\  \ ");
            Console.WriteLine(@" /::\:\__\ /:/\:\__\ /::\:\__\   /::\__\ /::\:\__\ /:/__/         /:/\:\__\ /::\/\__\ || /::\:\__\ /\:\:\__\ /\:\:\__\");
            Console.WriteLine(@" \/\::/  / \:\/:/  / \;:::/  /  /:/\/__/ \/\::/  / \:\  \         \:\ \/__/ \/\::/  / || \:\:\/  / \:\:\/__/ \:\:\/__/");
            Console.WriteLine(@"    \/__/   \::/  /   |:\/__/   \/__/      /:/  /   \:\__\         \:\__\     /:/  /  ||  \:\/  /   \::/  /   \::/  / ");
            Console.WriteLine(@"             \/__/     \|__|               \/__/     \/__/          \/__/     \/__/   ||   \/__/     \/__/     \/__/  ");
            Console.SetCursorPosition(47, 20);
            Console.WriteLine("Tryck Mellanslag För Att Forstätta");
            ConsoleKeyInfo keyPressed;
            while(true)
            {
                keyPressed = Console.ReadKey();
                String key = keyPressed.Key.ToString();
                if(key == "Spacebar") { Console.Clear(); break; }
            }
            Console.WriteLine("Vad heter du?");
            string name = Console.ReadLine();
            while (true)
            {
                GameController.Start(name);
            }
        }
    }

    public static class GameController
    {
        public static void Start(string name)
        {
            List<Player> playerList = new List<Player>();
            playerList.Add(new HumanPlayer(name));
            playerList.Add(new HumanPlayer("Player 2"));
            playerList.Add(new RandomPlayer("RandomPlayer"));
            playerList.Add(new AveragePlayer("AveragePlayer"));
            Console.Clear();
            Console.WriteLine("Vilka spelare är med?");
            for (int i = 0; i < playerList.Count; i++)
            {
                Console.WriteLine((i + 1) + " " + playerList[i].Name);
            }
            int p1 = Convert.ToInt32(Console.ReadLine()) - 1;
            int p2 = Convert.ToInt32(Console.ReadLine()) - 1;
            int[] p = new int[] { p1, p2 };
            Console.WriteLine("Hur många i rad?(4 - 10)");
            int iRad = 0;
            while(iRad < 4 || iRad > 10)
            {
                iRad = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Storleken av brädan?(" + iRad + " < x <= 50)");
            int xy = 0;
            while (xy < iRad + 1 || xy > 50)
            {
                xy = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Hur många rundor skall spelas?");
            int nbrOfGames = Convert.ToInt32(Console.ReadLine());
            int nbrOfSim = nbrOfGames;
            if (p1 > 1 && p2 > 1)
            {
                Console.WriteLine("hur många rundor skall visas?");
                nbrOfSim = Convert.ToInt32(Console.ReadLine());
            }
            int[] wins = new int[] { 0, 0 };
            Player playerOne = playerList[p1];
            Player playerTwo = playerList[p2];
            Console.Clear();
            for (int i = 0; i < nbrOfGames; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    string s = "";
                    if (playerList[p[j]] == playerList[p1]) { s = "X"; }
                    else { s = "O"; }
                    Console.SetCursorPosition(xy * 2 + 5, 1 + j * 2);
                    Console.WriteLine(s + " " + playerList[p[j]].Name + " " + wins[j]);
                }
                bool simulate = true;
                if (i < nbrOfSim) { simulate = false; }
                Game game = new Game(playerList[p1], playerList[p2], xy, iRad, simulate);
                Player winner = game.PlayGame();
                if (winner == playerOne) { wins[0]++; }
                else if (winner == playerTwo) { wins[1]++; }
                int secondP = p1;
                p1 = p2;
                p2 = secondP;
            }
            Console.ReadKey();
        }
    }

    public class Game
    {
        private Player[] CurrentPlayers = new Player[2];

        public static Piece[,] Board { get; private set; }

        public static int[] CursorPos;

        public int IRad { get; private set; }

        private bool Simulate;

        public List<int[]> Moves { get; private set; }

        static ConsoleKeyInfo KeyPressed;

        public static Random rand = new Random();

        public Game(Player playerOne, Player playerTwo, int xy, int iRad, bool simulate)
        {
            CurrentPlayers[0] = playerOne;
            CurrentPlayers[1] = playerTwo;

            Board = new Piece[xy, xy];

            CursorPos = new int[] { 0, 0 };

            IRad = iRad;

            Simulate = simulate;

            Moves = new List<int[]>();
        }

        public Player PlayGame()
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    Board[x, y] = new Piece(null, false);
                }
            }
            if (!Simulate) { ShowTable(); }
            for (int i = 0; i < Board.Length / 6; i++)
            {
                Board[rand.Next(0, Board.GetLength(0)), rand.Next(0, Board.GetLength(1))] = new Piece(null, true);
            }
            int a = 0;
            while (true)
            {
                int[] move = new int[2];
                bool input = false;
                while (!input)
                {
                    bool enter = false;
                    while (!enter)
                    {
                        enter = CurrentPlayers[a % 2].MoveCursor(Moves);
                        CursorOk();
                        move = CursorPos;
                        if (!Simulate) { ShowTable(); }
                    }
                    input = MoveOk(move, CurrentPlayers[a % 2]);
                }
                PutPiece(move, CurrentPlayers[a % 2]);
                if (!Simulate) { ShowTable(); }
                if (Board[move[0], move[1]].Bomb) { if (!Simulate) { Console.Write("Bomb finns där (Tryck Mellanslag)"); PressedSpace(); ClearLine(); } Board[move[0], move[1]] = new Piece(null, false); }
                if (CheckWin(move)) { if (!Simulate) { Console.WriteLine(CurrentPlayers[a % 2].Name + " vann"); Console.WriteLine("Tryck Mellanslag För Att Forstätta"); PressedSpace(); Console.Clear(); } return CurrentPlayers[a % 2]; }
                Othello(move);
                if (CheckDraw()) { if (!Simulate) { Console.WriteLine("Oavgjort"); Console.WriteLine("Tryck Mellanslag För Att Forstätta"); PressedSpace(); Console.Clear(); } return null; }
                if (!Simulate && !(CurrentPlayers[0] is HumanPlayer || CurrentPlayers[1] is HumanPlayer))
                {
                    Console.ReadKey();
                }
                a++;
            }
        }

        private void ClearLine()
        {
            Console.SetCursorPosition(0, Board.GetLength(1) + 2);
            Console.WriteLine("                            ");
        }

        private void Othello(int[] move)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))].Owner != null && Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))].Owner != Board[move[0], move[1]].Owner)
                    {
                        CheckOthelloMove(move, x, y);
                    }
                }
            }
            return;
        }

        private void CheckOthelloMove(int[] move, int x, int y)
        {
            if (Board[Modulo(move[0] + x * 2, Board.GetLength(0)), Modulo(move[1] + y * 2, Board.GetLength(1))].Owner == Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))].Owner)
            {
                if (Board[Modulo(move[0] + x * 3, Board.GetLength(0)), Modulo(move[1] + y * 3, Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner)
                {
                    Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))] = new Piece(null, false);
                    Board[Modulo(move[0] + x * 2, Board.GetLength(0)), Modulo(move[1] + y * 2, Board.GetLength(1))] = new Piece(null, false);
                }
            }
        }

        private void PressedSpace()
        {
            while (true) 
            {
                KeyPressed = Console.ReadKey();
                string key = KeyPressed.Key.ToString();
                if (key == "Spacebar") { return; }
            }
        }

        private void CursorOk()
        {
            if(CursorPos[0] > Board.GetLength(0) - 1) { CursorPos[0] = 0; }
            if(CursorPos[0] < 0) { CursorPos[0] = Board.GetLength(0) - 1; }
            if(CursorPos[1] > Board.GetLength(1) - 1) { CursorPos[1] = 0; }
            if(CursorPos[1] < 0) { CursorPos[1] = Board.GetLength(1) - 1; }
        }

        private bool CheckDraw()
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    if (Board[x, y].Owner == null) { return false; }
                }
            }
            return true;
        }

        private void ShowTable()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = Board.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < Board.GetLength(0); x++)
                {
                    if (x == CursorPos[0] + 1 && y == CursorPos[1]) { Console.Write("<"); }
                    else if (x != CursorPos[0] || y != CursorPos[1]) { Console.Write("|"); }
                    else { Console.Write(">"); }
                    if (Board[x, y].Owner == null) { Console.Write(" "); }
                    else if (Board[x, y].Owner == CurrentPlayers[0]) { Console.Write("X"); }
                    else if (Board[x, y].Owner == CurrentPlayers[1]) { Console.Write("O"); }
                }
                if (Board.GetLength(0) == CursorPos[0] + 1 && y == CursorPos[1]) { Console.WriteLine("<"); }
                else { Console.WriteLine("|"); }
            }
            Console.SetCursorPosition(0, Board.GetLength(1) + 2);
        }

        private bool CheckWin(int[] move)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))] != Board[move[0], move[1]])
                    {
                        if (Board[Modulo(move[0] + x, Board.GetLength(0)), Modulo(move[1] + y, Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner)
                        {
                            if (CheckDir(move, x, y) == true) { return true; }
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckDir(int[] move, int x, int y)
        {
            int inARow = 0;
            for (int i = -(IRad - 1); i < IRad; i++)
            {
                if (Board[Modulo(move[0] + x * i, Board.GetLength(0)), Modulo(move[1] + y * i, Board.GetLength(1))].Owner == null) { inARow = 0; }
                else if (Board[Modulo(move[0] + x * i, Board.GetLength(0)), Modulo(move[1] + y * i, Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner) { inARow++; }
                else { inARow = 0; }
                if (inARow == IRad) { return true; }
            }
            return false;
        }

        public int Modulo(int dividend, int divisor)
        {
            return (dividend % divisor + divisor) % divisor;
        }

        private void PutPiece(int[] move, Player owner)
        {
            Moves.Add(move);
            Board[move[0], move[1]] = new Piece(owner, Board[move[0], move[1]].Bomb);
        }

        private bool MoveOk(int[] move, Player owner)
        {
            for (int i = 0; i < 2; i++)
            {
                if (move[i] > Board.GetLength(i) - 1 || move[i] < 0) { return false; }
            }
            if (Board[move[0], move[1]].Owner != null)
            {
                return false;
            }
            return true;
        }
    }

    public class Piece
    {
        public Player Owner { get; set; }

        public bool Bomb { get; set; }

        public Piece(Player owner, bool bomb = false)
        {
            Owner = owner;
            Bomb = bomb;
        }
    }

    public abstract class Player
    {
        public string Name { get; protected set; }
        public abstract bool MoveCursor(List<int[]> moves);
    }

    class HumanPlayer : Player
    {
        public HumanPlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor(List<int[]> moves)
        {
            ConsoleKeyInfo keyPressed;
            keyPressed = Console.ReadKey();
            string key = keyPressed.Key.ToString();
            if (key == "UpArrow") { Game.CursorPos[1]++; }
            else if (key == "DownArrow") { Game.CursorPos[1]--; }
            else if (key == "RightArrow") { Game.CursorPos[0]++; }
            else if (key == "LeftArrow") { Game.CursorPos[0]--; }
            else if (key == "Enter") { return true; }
            return false;
        }
    }

    class RandomPlayer : Player
    {
        public RandomPlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor(List<int[]> moves)
        {
            Game.CursorPos[0] = Game.rand.Next(0, Game.Board.GetLength(0));
            Game.CursorPos[1] = Game.rand.Next(0, Game.Board.GetLength(1));
            return true;
        }
    }

    class AveragePlayer : Player
    {
        public AveragePlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor(List<int[]> moves)
        {
            if (moves.Count > 1)
            {
                Game.CursorPos[0] = Game.rand.Next(moves[1][0] - 1, moves[1][0] + 1);
                Game.CursorPos[1] = Game.rand.Next(moves[1][1] - 1, moves[1][1] + 1);
            }
            else
            {
                Game.CursorPos[0] = Game.rand.Next(0, Game.Board.GetLength(0));
                Game.CursorPos[1] = Game.rand.Next(0, Game.Board.GetLength(1));
            }
            return true;
        }
    }
}