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
            GameController.Start();
        }
    }

    public static class GameController
    {
        public static void Start()
        {
            List<Player> playerList = new List<Player>();
            Console.WriteLine("Vad heter du?");
            playerList.Add(new HumanPlayer(Console.ReadLine()));
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
            Console.WriteLine("Hur många i rad(4-8)");
            int iRad = 0;
            while (iRad < 4 || iRad > 8)
            {
                iRad = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Storleken av brädan?(>" + iRad + ")");
            int xy = 0;
            while (xy < iRad + 1)
            {
                xy = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Hur många rundor skall spelas?");
            int nbrOfGames = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("hur många rundor skall visas?");
            int nbrOfSim = Convert.ToInt32(Console.ReadLine());
            int[] wins = new int[] { 0, 0 };
            Player playerOne = playerList[p1];
            Player playerTwo = playerList[p2];
            Console.Clear();
            for (int i = 0; i < nbrOfGames; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.SetCursorPosition(xy * 4, 2 + j * 2);
                    Console.WriteLine(playerList[p[j]].Name + " " + wins[j]);
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

        public Piece[,] Board { get; private set; }

        public static int[] CursorPos;

        public int IRad { get; private set; }

        private bool Simulate;

        public int[] LastMove { get; private set; }

        static ConsoleKeyInfo KeyPressed;

        Random rand = new Random();

        public Game(Player playerOne, Player playerTwo, int xy, int iRad, bool simulate)
        {
            CurrentPlayers[0] = playerOne;
            CurrentPlayers[1] = playerTwo;

            Board = new Piece[xy, xy];

            CursorPos = new int[] { 0, 0 };

            IRad = iRad;

            Simulate = simulate;

            LastMove = new int[] { 0, 0 };
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
            if (Simulate == false) { ShowTable(); }
            for (int i = 0; i < Board.Length / 4; i++)
            {
                Debug.WriteLine(Board.Length / 4);
                Board[rand.Next(0, Board.GetLength(0)), rand.Next(0, Board.GetLength(1))] = new Piece(null, true);
            }
            int a = 0;
            while (true)
            {
                int[] move = new int[2];
                bool input = false;
                while (input == false)
                {
                    bool enter = false;
                    while (enter == false)
                    {
                        enter = CurrentPlayers[a % 2].MoveCursor();
                        CursorOk();
                        move = CursorPos;
                        if (Simulate == false) { ShowTable(); }
                    }
                    input = MoveOk(move, CurrentPlayers[a % 2]);
                }
                PutPiece(move, CurrentPlayers[a % 2]);
                Console.SetCursorPosition(0, Board.GetLength(1) + 10);
                if (Simulate == false) { Console.WriteLine("Jag Lägger min pjäs på x =" + (move[0] + 1) + " och y = " + (move[1] + 1)); }
                if (Simulate == false) { ShowTable(); }
                if (Board[move[0], move[1]].Bomb == true) { Console.WriteLine("Bomb finns där"); Board[move[0], move[1]] = new Piece(null, false); }
                if (CheckWin(move) == true) { if (Simulate == false) { Console.WriteLine(CurrentPlayers[a % 2].Name + " vann"); PressedSpace(); } return CurrentPlayers[a % 2]; }
                if (CheckDraw() == true) { if (Simulate == false) { Console.WriteLine("Draw"); PressedSpace(); } return null; }
                a++;
            }
        }

        private void Checkbomb(int[] move)
        {
            if (Board[move[0], move[1]].Bomb == true)
            {
                Board[move[0], move[1]] = new Piece(null, false);
            }
            throw new NotImplementedException();
        }

        private void PressedSpace()
        {
            Console.WriteLine("Press Space to Continue"); 
            while (true) 
            {
                KeyPressed = Console.ReadKey();
                string key = KeyPressed.Key.ToString();
                if (key == "Spacebar") { return; }
            }
        }

        private void CursorOk()
        {
            if(CursorPos[0] > Board.GetLength(0) - 1) { CursorPos[0] = Board.GetLength(0) - 1; }
            if(CursorPos[0] < 0) { CursorPos[0] = 0; }
            if(CursorPos[1] > Board.GetLength(1) - 1) { CursorPos[1] = Board.GetLength(1) - 1; }
            if(CursorPos[1] < 0) { CursorPos[1] = 0; }
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
            if (CheckVertical(move) == true) { return true; }
            if (CheckHorizontal(move) == true) { return true; }
            if (CheckDiagonal(move) == true) { return true; }
            return false;
        }

        private bool CheckDiagonal(int[] move)
        {
            int reverse = 1;
            for (int i = 0; i < 2; i++)
            {
                if (DiagonalUD(move, reverse, reverse) == true) { return true; }
                if (DiagonalDU(move, reverse, -reverse) == true) { return true; }
                reverse = -1;
            }
            return false;
        }

        private bool DiagonalDU(int[] move, int reverseX, int reverseY)
        {
            int inARow = 0;
            for (int xy = -(IRad - 1); xy < IRad - 1; xy++)
            {
                if (Board[Mod((move[0] + reverseX * xy), Board.GetLength(0)), Mod((move[1] + reverseY * xy), Board.GetLength(1))].Owner == null) { inARow = 0; }
                else if (Board[Mod((move[0] + reverseX * xy), Board.GetLength(0)), Mod((move[1] + reverseY * xy), Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner) { inARow++; }
                else { inARow = 0; }
                if (inARow == IRad) { return true; }
            }
            return false;
        } //diagonal(Down and Up) "/"

        private bool DiagonalUD(int[] move, int reverseX, int reverseY)
        {
            int inARow = 0;
            for (int xy = -(IRad - 1); xy < IRad - 1; xy++)
            {
                if (Board[Mod((move[0] + reverseX * xy), Board.GetLength(0)), Mod((move[1] + reverseY * xy), Board.GetLength(1))].Owner == null) { inARow = 0; }
                else if (Board[Mod((move[0] + reverseX * xy), Board.GetLength(0)), Mod((move[1] + reverseY * xy), Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner) { inARow++; }
                else { inARow = 0; }
                if (inARow == IRad) { return true; }
            }
            return false;
        } //Diagonal(Up and Down) "\"

        private bool CheckHorizontal(int[] move)
        {
            int reverse = 1;
            for (int i = 0; i < 2; i++)
            {
                int inARow = 0;
                for (int x = -(IRad - 1); x < IRad - 1; x++)
                {
                    if (Board[Mod((move[0] + reverse * x), Board.GetLength(0)), move[1]].Owner == null) { inARow = 0; }
                    else if (Board[Mod((move[0] + reverse * x), Board.GetLength(0)), move[1]].Owner == Board[move[0], move[1]].Owner) { inARow++; }
                    else { inARow = 0; }
                    if (inARow == IRad) { return true; }
                }
                reverse = -1;
            }
            return false;
        }

        private bool CheckVertical(int[] move)
        {
            int reverse = 1;
            for (int i = 0; i < 2; i++)
            {
                int inARow = 0;
                for (int y = -(IRad - 1); y < IRad - 1; y++)
                {
                    if (Board[move[0], Mod((move[1] + reverse * y), Board.GetLength(1))].Owner == null) { inARow = 0; }
                    else if (Board[move[0], Mod((move[1] + reverse * y), Board.GetLength(1))].Owner == Board[move[0], move[1]].Owner) { inARow++; }
                    else { inARow = 0; }
                    if (inARow == IRad) { return true; }
                }
                reverse = -1;
            }
            return false;
        }

        public int Mod(int dividend, int divisor)
        {
            return (dividend % divisor + divisor) % divisor;
        }

        private void PutPiece(int[] move, Player owner)
        {
            LastMove[0] = move[0];
            LastMove[1] = move[1];
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
        public abstract bool MoveCursor();
    }

    class HumanPlayer : Player
    {
        public HumanPlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor()
        {
            //int number = Convert.ToInt32(Console.ReadLine());
            //return number;
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
        Random rand = new Random();
        public RandomPlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor()
        {
            while (true)
            {
                int key = rand.Next(1, 6);
                if (key == 1) { Game.CursorPos[1]++; }
                else if (key == 2) { Game.CursorPos[1]--; }
                else if (key == 3) { Game.CursorPos[0]++; }
                else if (key == 4) { Game.CursorPos[0]--; }
                else { return true; }
            }
        }
    }

    class AveragePlayer : Player
    {
        Random rand = new Random();
        public AveragePlayer(string name)
        {
            Name = name;
        }

        public override bool MoveCursor()
        {
            //(rand.Next(lastMove, lastMove + 3) % BoardSize + BoardSize) % BoardSize;
            //return rand.Next(1, BoardSize + 2);
            return false;
        }
    }
}