using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DualSeaBattle.Model
{   
    public enum MessageType
    {
        Join, Placement, Shot, Command, ShotResult, GameStatus, EndGame
    }

    public class Message
    {
        public MessageType Type { get; set; }
        public object? Playload {  get; set; }
    }
    public class ShotLoad
    {
        public int Row {  get; set; }
        public int Col { get; set; }
    }
    public class PlacementLoad
    {
        public List<Ship> Ships { get; set; } = new List<Ship>();  
    }

    public class Ship
    {
        public int R { get; set; }
        public int C { get; set; }
        public int Len { get; set; }
        public char Dir { get; set; }
    }

    public class ShotResultLoad
    {
        public string? Status { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; } 
    }
    public class CommandLoad
    {
        public string? Cmd { get; set; }
    }
    public enum CellState
    {
        Empty, Ship, Hit, Miss       
    }
}
