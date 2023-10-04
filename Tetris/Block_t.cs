using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Block_t : Block
    {
        #region POLA / WŁAŚCIWOŚCI - IMPLEMENTACJA(bo są abstrakcyje w klasie bazowej)

        private readonly Position[][] tiles = new Position[][]
        {
            // wypełnianie wartościami komórek w bounding boxie,
            // te komórki bedą miały wartosci takie jak ID danej klasy
            new Position[]{ new Position(0,1), new Position(1,0), new Position(1,1), new Position(1,2)},
            new Position[]{ new Position(0,1), new Position(1,1), new Position(1,2), new Position(2,1)},
            new Position[]{ new Position(1,0), new Position(1,1), new Position(1,2), new Position(2,1)},
            new Position[]{ new Position(0,1), new Position(1,0), new Position(1,1), new Position(2,1)}
        };

        public override int Id => 6;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;

        #endregion
    }
}
