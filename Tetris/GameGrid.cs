﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameGrid
    {
        #region POLA / WŁAŚCIWOŚCI / INDEKSERY

        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        
        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        #endregion

        #region KONSTRUKTORY

        public GameGrid(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            grid = new int[rows, columns];
        }

        #endregion

        #region METODY


        public bool IsInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }


        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }


        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r,c] == 0)
                {
                    return false;
                }
            }

            return true;
        }


        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }


        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r, c] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;

            for (int r = Rows - 1; r >= 0 ; r--)
            {
                if(IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if(cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }

        #endregion
    }
}
