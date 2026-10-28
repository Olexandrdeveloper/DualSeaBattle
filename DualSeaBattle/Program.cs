
using DualSeaBattle.Model;
using DualSeaBattle.Network;
using DualSeaBattle.UIpaint;
using System.Text;
using System.Text.Json;

namespace DualSeaBattle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var state = new GameState();   
            var network = new ClientUdp("127.0.0.1", 5000);
            var paint = new ConsolePaint();
            var command = new Command();

            Console.WriteLine("[CLIENT] Отправляю пакет JOIN серверу...");
            var joinMsg = new Message
            {
                Type = MessageType.Join

            };
            network.SendMessage(joinMsg);
            Console.WriteLine("[CLIENT] JOIN отправлен. Ожидаю ответ...");
            //Thread.Sleep(200);

            Console.WriteLine("============================");
            Console.WriteLine("======== МОРСКОЙ БОЙ =======");
            Console.WriteLine("============================");

            paint.DrawBoard(state.EnemyBoard);           
            paint.ShowStatus(state.IsMyTurn, state.GameActive);

            bool needRepaint = false;          

            //while (state.GameActive)
            while (true)
            {
                if (!state.GameActive)  break;

                while (true)
                {
                    var msg = network.TryReceiveMessage();

                    if (msg == null)
                        break;

                    Console.WriteLine($"[CLIENT] Получено сообщение: {msg.Type}");

                    switch (msg.Type)
                    {
                        case MessageType.ShotResult:
                            if (msg.ShotResult is ShotResultLoad shotRes)
                            {
                                Console.WriteLine($"[CLIENT] Результат выстрела: {shotRes.Status}");

                                state.ProcessShotResult(shotRes);
                                needRepaint = true;
                            }
                            break;

                        case MessageType.GameStatus:                           

                            if (msg.GameStatus != null)
                            {                                
                                state.SetTurnStatus(msg.GameStatus.IsMyTurn);
                                needRepaint = true;                                  

                                if (!string.IsNullOrWhiteSpace(msg.GameStatus.Message))
                                {
                                    paint.ShowMessage(msg.GameStatus.Message, ConsoleColor.Green);
                                }
                            }
                            break;

                        case MessageType.EndGame:
                            state.GameActive = false;
                            paint.ShowMessage("<<< КОНЕЦ ИГРЫ >>>", ConsoleColor.Red);
                            needRepaint = true;
                            break;
                    }
                }

                if (needRepaint)
                {
                    //Console.Clear();
                    paint.DrawBoard(state.EnemyBoard);
                    paint.ShowStatus(state.IsMyTurn, state.GameActive);
                    needRepaint = false;
                }                

                if (state.IsMyTurn)
                {
                    Console.Write("\nВАШ ХОД (напр, B3): ");                   
                    string input = Console.ReadLine() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }

                    if (command.IsCommand(input))
                    {
                        Console.WriteLine($"Получена команда: '{input}'");

                        string enteredCommand = input.Trim();
                        command.Execute(enteredCommand, state);

                        if (enteredCommand.Equals("/restart", StringComparison.OrdinalIgnoreCase))
                        {
                            state.Reset();

                            network.SendMessage(new Message
                            {
                                Type = MessageType.Join
                            });
                            Console.WriteLine("[CLIENT] Повторный JOIN отправлен серверу");
                        }

                        needRepaint = true;
                        continue;
                    }

                    if (command.TryParseShot(input, out int r, out int c))
                    {
                        var shotMsg = new Message
                        {
                            Type = MessageType.Shot,
                            Shot = new ShotLoad
                            {
                                Row = r,
                                Col = c
                            }
                        };
                        network.SendMessage(shotMsg);
                        Console.WriteLine($"Выстрел по {(char)('A' + c)}{r + 1}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        paint.ShowMessage("Неправильный формат, надо: B3", ConsoleColor.DarkGray);
                        Thread.Sleep(800);
                        needRepaint = true;
                    }
                }
                else
                {
                    Thread.Sleep(300);
                }                
            }         
            Console.WriteLine("\n === КЛИЕНТ ЗАВЕРШЕН ===");
            network.Dispose();
        }
    }
}

