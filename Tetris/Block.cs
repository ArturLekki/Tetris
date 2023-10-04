using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Block
    {
        #region POLA / WŁAŚCIWOŚCI / INDEKSERY

        protected abstract Position[][] Tiles { get; } // tiles=płytki czyli komórki
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; } // id typu bloku

        private int rotationState; // faza bloku czyli ile stopni
        private Position offset; // przesunięcie

        #endregion

        #region KONSTRUKTORY

        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        #endregion

        #region METODY - LOGIKA DLA KAŻDEGO Z BLOKÓW

        public IEnumerable<Position> TilePositions()
        {
            foreach(Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }


        public void RotateCW() // CW=Clock wise - wg wskazowek zegara
        {
            rotationState = (rotationState +1) % Tiles.Length;
        }

        public void RotateCCW() // CCW=Counter Clock wise - przeciwnie do wskazowek zegara
        {
            if(rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }


        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }

        public void Reset()
        {
            rotationState=0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }

        #endregion
    }
}
