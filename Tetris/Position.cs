using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Position
    {
        #region POLA / WŁAŚCIWOŚCI / INDEKSERY

        public int Row { get; set; }
        public int Column { get; set; }

        #endregion

        #region KONSTRUKTORY

        public Position(int row, int column)
        {
           this.Row = row;
           this.Column = column;
        }

        #endregion
    }
}
