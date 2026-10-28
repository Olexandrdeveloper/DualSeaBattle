using System;
using DualSeaBattle.Model;

namespace DualSeaBattle.UIpaint
{
    public class ConsolePaint
    {
        public void DrawBoard(CellState[,] board)
        {
            Console.Write("    ");
            for (int c = 0; c < 10; c++)
            {
                char letter = (char)('A' + c);
                Console.Write($"{letter} ");
            }
            Console.WriteLine();

            for (int r = 0; r < 10; r++)
            {
                Console.Write($"{r + 1,2} ");

                for (int c = 0; c < 10; c++)
                {
                    var cell = board[r, c];

                    if (cell == CellState.Empty)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("~ ");
                        Console.ResetColor();
                    }
                    else if (cell == CellState.Ship)
                    {
                        Console.Write("# ");
                        
                    }
                    else if (cell == CellState.Hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X ");
                        Console.ResetColor();
                    }
                    else if (cell == CellState.Miss)
                    { 
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("O ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("? ");
                    }
                }
                Console.WriteLine();
            }
        }
        public void ShowStatus(bool isMyTurn, bool gameActive)
        {
            Console.WriteLine("-------------------------------------");
            if(!gameActive)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" === ИГРА ОКОНЧЕНА ===");
                Console.ResetColor();

                Console.WriteLine("-------------------------------------");
                return;
            }

            if (isMyTurn)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(">>>> ВАШ ХОД (вводим координаты (напр, B3) или /restart");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("ХОД ПРОТИВНИКА...");
                Console.ResetColor();
            }
            Console.WriteLine("-------------------------------------");
        }
        public void ShowMessage(string msg, ConsoleColor color = ConsoleColor.White)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = oldColor;
        }
    }
}
