using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Block_o  : Block
    {
        #region POLA / WŁAŚCIWOŚCI - IMPLEMENTACJA(bo są abstrakcyje w klasie bazowej)

        private readonly Position[][] tiles = new Position[][]
        {
            // wypełnianie wartościami komórek w bounding boxie,
            // te komórki bedą miały wartosci takie jak ID danej klasy
            new Position[]{ new Position(0,0), new Position(0,1), new Position(1,0), new Position(1,1)}
        };

        public override int Id => 4;
        protected override Position StartOffset => new Position(0, 4);
        protected override Position[][] Tiles => tiles;

        #endregion
    }
}
