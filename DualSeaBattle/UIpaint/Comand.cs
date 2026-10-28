using DualSeaBattle.Model;
using System;
using System.Globalization;

namespace DualSeaBattle.UIpaint
{
    public class Command
    {
        public bool IsCommand(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Trim().StartsWith("/");
        }
        public bool Execute(string input, GameState state)
        {
            switch (input.ToLower().Trim())
            {
                case "/restart":
                    state.Reset();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n<<<<< ПЕРЕЗАПУСК ИГРЫ >>>>>");
                    Console.ResetColor();
                    return true;
                case "/help":
                    Console.WriteLine("КОМАНДЫ: /restart, /help. Ввод хода: (B3)");
                    return true;
                case "/exit":
                    state.GameActive = false;
                    Console.WriteLine("Выход из игры...");
                    return true;
                default:
                    Console.WriteLine($"НЕПОНЯТНАЯ КОМАНДА: {input}");
                    return true;
            }
        }
        public bool TryParseShot(string input, out int row, out int col)
        {
            row = -1;
            col = -1;
            
            if (string.IsNullOrWhiteSpace(input)) return false;
            input = input.Trim();

            if (input.Length < 2)
            {
                return false;
            }

            char letter = char.ToUpper(input[0]);
            string numberPart = input.Substring(1);

            if (letter < 'A' || letter > 'J')
            {
                return false;
            }
            if (!int.TryParse(numberPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out int rawRow))
            {
                return false;
            }                

            col = letter - 'A';
            row = rawRow - 1;
            return row >= 0 && row <= 9;
        }
    }
}
