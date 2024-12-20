using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartVertex { get; } // top left corner of the block
        public abstract int Id { get; }
        private int direction = 0; // N = 0, E = 1, S = 2, W = 3
        private Position vertex; // top left corner of the block

        public Block()
        {
            vertex = new Position(StartVertex.Row, StartVertex.Column);
        }

        public void RotateCW()
        {
            direction = (direction + 1) % 4;
        }

        public void RotateCCW()
        {
            direction = (direction + 3) % 4;
        }

        public void Move(int row, int column)
        {
            vertex.Row += row;
            vertex.Column += column;
        }

        public void Reset()
        {
            direction = 0;
            vertex.Row = StartVertex.Row;
            vertex.Column = StartVertex.Column;
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (Position tile in Tiles[direction])
            {
                yield return new Position(vertex.Row + tile.Row, vertex.Column + tile.Column);
            }
        }
    }

    public class OBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 0), new Position(0, 1), new Position(1, 0), new Position(1, 1) }
        };
        public override int Id => 1;
        protected override Position StartVertex => new Position(0, 4);
        protected override Position[][] Tiles => tiles;
    }

    public class TBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 1), new Position(1, 0), new Position(1, 1), new Position(1, 2) },
            new Position[] { new Position(0, 1), new Position(1, 1), new Position(1, 2), new Position(2, 1) },
            new Position[] { new Position(1, 0), new Position(1, 1), new Position(1, 2), new Position(2, 1) },
            new Position[] { new Position(0, 1), new Position(1, 0), new Position(1, 1), new Position(2, 1) }
        };
        public override int Id => 2;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class LBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 2), new Position(1, 0), new Position(1, 1), new Position(1, 2) },
            new Position[] { new Position(0, 1), new Position(1, 1), new Position(2, 1), new Position(2, 2) },
            new Position[] { new Position(1, 0), new Position(1, 1), new Position(1, 2), new Position(2, 0) },
            new Position[] { new Position(0, 0), new Position(0, 1), new Position(1, 1), new Position(2, 1) }
        };
        public override int Id => 3;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class JBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 0), new Position(1, 0), new Position(1, 1), new Position(1, 2) },
            new Position[] { new Position(0, 1), new Position(0, 2), new Position(1, 1), new Position(2, 1) },
            new Position[] { new Position(1, 0), new Position(1, 1), new Position(1, 2), new Position(2, 2) },
            new Position[] { new Position(0, 1), new Position(1, 1), new Position(2, 0), new Position(2, 1) }
        };
        public override int Id => 4;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class ZBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 0), new Position(0, 1), new Position(1, 1), new Position(1, 2) },
            new Position[] { new Position(0, 2), new Position(1, 1), new Position(1, 2), new Position(2, 1) },
            new Position[] { new Position(1, 0), new Position(1, 1), new Position(2, 1), new Position(2, 2) },
            new Position[] { new Position(0, 1), new Position(1, 0), new Position(1, 1), new Position(2, 0) }
        };
        public override int Id => 5;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class SBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(0, 1), new Position(0, 2), new Position(1, 0), new Position(1, 1) },
            new Position[] { new Position(0, 1), new Position(1, 1), new Position(1, 2), new Position(2, 2) },
            new Position[] { new Position(1, 1), new Position(1, 2), new Position(2, 0), new Position(2, 1) },
            new Position[] { new Position(0, 0), new Position(1, 0), new Position(1, 1), new Position(2, 1) }
        };
        public override int Id => 6;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class IBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new Position(1, 0), new Position(1, 1), new Position(1, 2), new Position(1, 3) },
            new Position[] { new Position(0, 2), new Position(1, 2), new Position(2, 2), new Position(3, 2) },
            new Position[] { new Position(2, 0), new Position(2, 1), new Position(2, 2), new Position(2, 3) },
            new Position[] { new Position(0, 1), new Position(1, 1), new Position(2, 1), new Position(3, 1) }
        };
        public override int Id => 7;
        protected override Position StartVertex => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new OBlock(),
            new TBlock(),
            new LBlock(),
            new JBlock(),
            new ZBlock(),
            new SBlock(),
            new IBlock()
        };

        private readonly Random random = new Random();

        //public Block NextBlock { get; private set; }

        public Block[] Queue { get; private set; }

        public BlockQueue()
        {
            Queue = new Block[3];
            Queue[0] = RandomBlock();
            Queue[1] = RandomBlock();
            if(Queue[0].Id == Queue[1].Id)
            {
                do
                {
                    Queue[1] = RandomBlock();
                }
                while (Queue[0].Id == Queue[1].Id);
            }
            Queue[2] = RandomBlock();
            if (Queue[1].Id == Queue[2].Id)
            {
                do
                {
                    Queue[2] = RandomBlock();
                }
                while (Queue[2].Id == Queue[1].Id);
            }
        }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetNextBlock()
        {
            Block block = Queue[0];
            Queue[0] = Queue[1];
            Queue[1] = Queue[2];
            do
            {
                Queue[2] = RandomBlock();
            }
            while (Queue[1].Id == Queue[2].Id);
            return block;
        }
    }
}
