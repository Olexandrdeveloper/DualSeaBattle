using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DualSeaBattle.Model
{
    public class GameState
    {
        private const int Board = 10;        
        public bool IsMyTurn { get; set; } = false;
        public bool GameActive { get; set; } = true;
        
        public CellState[,] EnemyBoard { get; private set; } = new CellState[Board, Board];
        
        public List<Ship> MyShips { get; private set; } = new List<Ship>();

        public GameState()
        {
            
            for (int r = 0; r < Board; r++)
            {
                for (int c = 0; c < Board; c++)
                {
                    EnemyBoard[r, c] = CellState.Empty;
                }
            }
        }

        public bool ControlPlaceShip(int r, int c, int len, char dir)
        {           
            if (dir == 'H')
            {
                if (c < 0 || c + len > Board || r < 0 || r >= Board)
                    return false;
            }
            else 
            {
                if (r < 0 || r + len > Board || c < 0 || c >= Board)
                    return false;
            }
            
            int startRow = Math.Max(0, r - 1);
            int endRow = Math.Min(Board - 1, dir == 'V' ? r + len : r);

            int startCol = Math.Max(0, c - 1);
            int endCol = Math.Min(Board - 1, dir == 'H' ? c + len : c);

            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {                    
                    if (CellOccupiedMyShip(i, j))
                        return false;
                }
            }

            return true;
        }

        public void PlaceShip(int r, int c, int len, char dir)
        {
            var newShip = new Ship
            {
                R = r,
                C = c,
                Len = len,
                Dir = dir
            };
            MyShips.Add(newShip);
            
            for (int i = 0; i < len; i++)
            {
                int cr;
                if (dir == 'V')
                {
                    cr = r + i;
                }
                else {cr = r; }

                int cc;
                if (dir == 'H')
                {
                    cc = r + i;
                }
                else { cc = r; }                
                EnemyBoard[cr, cc] = CellState.Ship;
            }
        }
                
        public PlacementLoad GetPlacementPayload()
        {
            return new PlacementLoad 
            {
                Ships = MyShips 
            };
        }

        public void ProcessShotResult(ShotResultLoad payload)
        {
            if (!payload.Row.HasValue || !payload.Col.HasValue)
                return;

            int r = payload.Row.Value;
            int c = payload.Col.Value;

            if (r < 0 || r >= Board || c < 0 || c >= Board)
                return;

            switch (payload.Status)
            {
                case "hit":
                    EnemyBoard[r, c] = CellState.Hit;
                    break;
                case "miss":
                    EnemyBoard[r, c] = CellState.Miss;
                    break;
                case "sunk":
                    EnemyBoard[r, c] = CellState.Hit; 
                    break;
                case "win":
                case "lose":
                    GameActive = false;
                    break;
            }
        }

        public void SetTurnStatus(bool isMyTurn)
        {
            IsMyTurn = isMyTurn;
        }

        public void Reset()
        {
            MyShips.Clear();
            GameActive = true;
            IsMyTurn = false;

            for (int r = 0; r < Board; r++)
                for (int c = 0; c < Board; c++)
                    EnemyBoard[r, c] = CellState.Empty;
        }
                
        private bool CellOccupiedMyShip(int r, int c)
        {            
            foreach (var ship in MyShips)
            {
                int startRow = ship.Dir == 'V' ? ship.R : ship.R;
                int endRow = ship.Dir == 'V' ? ship.R + ship.Len - 1 : ship.R;

                int startC = ship.Dir == 'H' ? ship.C : ship.C;
                int endC = ship.Dir == 'H' ? ship.C + ship.Len - 1 : ship.C;

                if (r >= startRow && r <= endRow && c >= startC && c <= endC)
                    return true;
            }
            return false;
        }
    }
}

