using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueue
    {
        #region POLA / WŁAŚCIWOŚCI

        private readonly Block[] blocks = new Block[]
        {
            new Block_i(),
            new Block_j(),
            new Block_l(),
            new Block_o(),
            new Block_s(),
            new Block_t(),
            new Block_z(),
        };

        private readonly Random random = new Random();

        public Block Nextblock { get; private set; }

        #endregion


        #region KONSTRUKTORY

        public BlockQueue()
        {
            Nextblock = RandomBlock();
        }

        #endregion


        #region METODY

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }


        public Block GetAndUpdate()
        {
            Block block = Nextblock;

            do
            {
                Nextblock = RandomBlock();
            }
            while(block.Id == Nextblock.Id);

            return block;
        }

        #endregion
    }
}
