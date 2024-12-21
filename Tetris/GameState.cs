using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;
        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();
            }
        }

        public Grid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Block Held { get; private set; }
        public bool HeldThisTurn { get; private set; }
        public bool IsPaused { get; set; }

        public GameState()
        {
            GameGrid = new Grid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetNextBlock();
            GameOver = false;
            HeldThisTurn = false;
        }

        private bool BlocksFits()
        {
            foreach (Position tile in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(tile.Row, tile.Column))
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateBlockCW()
        {
            if (CurrentBlock.Id == 1)
            {
                return;
            }
            CurrentBlock.RotateCW();
            if (!BlocksFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW()
        {
            if (CurrentBlock.Id == 1)
            {
                return;
            }
            CurrentBlock.RotateCCW();
            if (!BlocksFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlocksFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlocksFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return GameGrid.IsDanger();
        }

        private void PlaceBlock()
        {
            foreach (Position tile in CurrentBlock.TilePositions())
            {
                GameGrid[tile.Row, tile.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.CheckRowComplete();
            
            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetNextBlock();
                HeldThisTurn = false;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlocksFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position tile)
        {
            int distance = 0;

            while(GameGrid.IsEmpty(tile.Row + distance + 1, tile.Column))
            {
                distance++;
            }
            return distance;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position tile in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(tile));
            }

            return drop;
        }

        public void DropBlock()
        {
            int drop = BlockDropDistance();
            CurrentBlock.Move(drop, 0);
            PlaceBlock();
        }

        public void HoldBlock()
        {
            if(HeldThisTurn)
            {
                return;
            }

            if (Held == null)
            {
                Held = CurrentBlock;
                CurrentBlock = BlockQueue.GetNextBlock();
            }
            else
            {
                Block temp = Held;
                Held = CurrentBlock;
                CurrentBlock = temp;
            }
            HeldThisTurn = true;
        }
    }
}
