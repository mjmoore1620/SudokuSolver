/////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		SudokuSolver
//	File Name:		Grid
//	Description:	Contains all of the logic for solving the puzzle and the grid 
//                  containing the cells. 
//	Course:			CSCI 5300 - Software Design
//	Author:			Matthew Moore, zmjm1320@gmail.com
//	Created:	    10/31/2017
//	Copyright:		Matthew Moore, 2017
//
/////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    internal class Grid
    {
        #region attributes
        public Cell[,] GridArr { get; set; }
        public string Alphabet { get; set; }
        public int SubgridRows { get; set; }
        public int SubgridCols { get; set; }
        public List<List<Cell>> Subgrids { get; set; }
        public char[] AlphabetArr { get; set; }
        public int PuzzleSize { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a default 9x9 sudoku completely unsolved puzzle grid
        ///     with each cell populated by the default alphabet.
        /// </summary>
        public Grid()
        {
            Alphabet = "123456789";
            GridArr = new Cell[9, 9];
            SubgridRows = 3;
            SubgridCols = 3;

            //create all default cells
            for (int x = 0; x < GridArr.GetLength(0); x++)
                for (int y = 0; y < GridArr.GetLength(1); y++)
                    GridArr[x, y] = new Cell(x, y, Alphabet);
        }

        /// <summary>
        /// Creates a sudoku grid according to the puzzle file params
        /// </summary>
        /// <param name="puzzle"></param>
        public Grid(XmlPuzzle puzzle)
        {
            //TODO error catch for puzzle obj import on grid creation
            if (!(puzzle.Dimensions == null))
            {
                Alphabet = puzzle.Alphabet.Replace("\"", "");

                int MxN = puzzle.Dimensions.Columns * puzzle.Dimensions.Rows;
                GridArr = new Cell[MxN, MxN];
                SubgridRows = puzzle.Dimensions.Rows;
                SubgridCols = puzzle.Dimensions.Columns;
                
                AlphabetArr = Alphabet.ToArray();
            }
            else
            {
                Alphabet = "123456789";
                GridArr = new Cell[9, 9];
                SubgridRows = 3;
                SubgridCols = 3;
                AlphabetArr = Alphabet.ToArray();
            }
            PuzzleSize = SubgridRows * SubgridCols;


            //Write the alphabet to every cell of the grid.
            for (int row = 0; row < GridArr.GetLength(0); row++)
                for (int col = 0; col < GridArr.GetLength(1); col++)
                    GridArr[row, col] = new Cell(row, col, Alphabet);

            //TODO error catch for puzzle obj
            var initialValues = puzzle.InitialValues.Cells;

            //Overwrite default grid with initial values 
            for (int i = 0; i < initialValues.Count; i++)
                GridArr[initialValues[i].Row, initialValues[i].Col] = new Cell(initialValues[i]);

            //Create a list of subgrids 
            Subgrids = new List<List<Cell>>(GridArr.GetLength(0));
            for (int colIndex = 0; colIndex < SubgridRows; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < SubgridCols; rowIndex++)
                {
                    Subgrids.Add(GetSubgrid(rowIndex, colIndex));
                }
            }
        }
        #endregion constructors

        #region NCellSieve
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="cellGroup"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public int SieveNCellGroup(List<Cell> cellGroup, int n)
        {
            int removals = 0;
            //look at each cell in a group for cases where the cell(keycell) contains n values
            foreach (var keyCell in cellGroup)
            {
                //TODO: clean up this n=1 case. It smells.
                if (keyCell.Value.Length == n)
                {
                    int nCount = 1;
                    if (nCount == n && RemoveInferenceValues(keyCell, cellGroup) > 0)
                    {
                        removals++;
                    }
                    else
                    {
                        for (int i = 0; i < cellGroup.Count; i++)
                        {
                            //if current cell is not the keycell then see if their sets are equal
                            //*requires lists be in same order
                            if (cellGroup[i] != keyCell && cellGroup[i].ValueList.SequenceEqual(keyCell.ValueList))
                            {
                                nCount++;
                            }
                            if (nCount == n && RemoveInferenceValues(keyCell, cellGroup) > 0)
                            {
                                removals++;
                            }
                        } 
                    }
                }
            }

            return removals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        public int RemoveInferenceValues(Cell keyCell, List<Cell> cellGroup)
        {
            int removals = 0;
            //for each cell in the group
            for (int i = 0; i < cellGroup.Count; i++)
            {
                //if the key cell ValueList is not equal to current cell[i]
                if (!cellGroup[i].ValueList.SequenceEqual(keyCell.ValueList))
                {
                    //look for each character in the keycell to remove it from the current cell
                    for (int j = 0; j < keyCell.ValueList.Count; j++)
                    {
                        if (cellGroup[i].ValueList.Remove(keyCell.ValueList[j]))
                        {
                            removals++;
                            cellGroup[i].UpdateValue();
                        }
                    }
                }
            }
            return removals;
        }
        #endregion

        #region ValueCount
        /// <summary>
        /// Searches for a value only represented once in a group of cells in an unsolved cell. 
        /// This is an simplified application of ValueCountInference() that applies the n = 1 case.
        /// </summary>
        /// <param name="cellGroup">The the group of cells being considered.</param>
        /// <returns>The number of removals.</returns>
        public int SingleValueCount(CellSet cellGroup)
        {
            int removals = 0;
            int[] alphabetCount = cellGroup.ValueCount(this);
            //HashSet<char> matchSet;
            
            for (int i = 0; i < alphabetCount.Length; i++)
            {
                //for each value represented only once
                if (alphabetCount[i] == 1)
                {
                    foreach (var cell in cellGroup)
                    {
                        if (!cell.IsSolved() && cell.ValueSet.Contains(AlphabetArr[i]))
                        {
                            cell.OverWriteValueMembers(AlphabetArr[i].ToString());
                            removals++;
                        }
                    }
                }
            }
            return removals;
        }

        /// <summary>
        /// Looks at a group of cells and determines if any of the values are only represented once.
        /// I suspect this is incapable of reaching n > 1 cases, though the concept could be applied in certain cases. 
        /// </summary>
        /// <param name="cellGroup"></param>
        /// 
        /// <param name="n"></param>
        /// <returns></returns>
        public int ValueCountInference(List<Cell> cellGroup, int n)
        {
            int removals = 0;
            int[] alphabetCount = new int[PuzzleSize];
            HashSet<char> matchSet;

            CellSet cellSet = new CellSet(cellGroup);
            alphabetCount = cellSet.ValueCount(this);

            //for each v with v.count = n
            for (int i = 0; i < alphabetCount.Length; i++)
            {
                if (alphabetCount[i] == n)
                {
                    //add v to new matchlist, hashset
                    matchSet = new HashSet<char>()
                    {
                        AlphabetArr[i]
                    };

                    //if matchlist.count = n
                    //only runs for n = 1, maybe a hack
                    if (matchSet.Count == n)
                    {
                        //remove values that matchlist does not contain from cells with v in current group
                        for (int j = 0; j < cellGroup.Count; j++)
                        {
                            //this is the list way to check for subset: "!matchlist.Except(cellGroup[j].ValueList).Any()"
                                //This still returns true if matchlist is a proper subset of ValueList
                            if (matchSet.IsProperSubsetOf(cellGroup[j].ValueSet))
                            {
                                removals += cellGroup[j].OverWriteValueMembers(matchSet);
                            }
                        }
                    }
                }
            }

            return removals;
        }
#endregion


        /// <summary>
        /// Calls the methods capable of making solution progress and records the number of inferences resulting in removal.
        /// </summary>
        /// <returns></returns>
        public bool Solve()
        {
            int removals;

            //Increases the unison/cell match count requirement for inferences. 
            for (int n = 1; n < PuzzleSize; n++)
            {
                removals = 0;

                //make inferences for all rows, then cols, then subgrids
                for (int i = 0; i < PuzzleSize; i++)
                {
                    removals += SieveNCellGroup(GetRow(i), n);
                    removals += SieveNCellGroup(GetCol(i), n);
                    removals += SieveNCellGroup(Subgrids[i], n);

                    if (n == 1)
                    {
                        removals += SingleValueCount(new CellSet(GetRow(i)));
                        removals += SingleValueCount(new CellSet(GetCol(i)));
                        removals += SingleValueCount(new CellSet(Subgrids[i]));
                    }

                    ////ValueCountInference either doesn't work for n > 1 or I can't find a case to verify it works for n > 1
                    //if (n <= SubgridRows || n <= SubgridCols)
                    //{
                    //    //for methods requiring combinations in the same subRow/Col
                    //    removals += ValueCountInference(GetRow(i), n);
                    //    removals += ValueCountInference(GetCol(i), n);
                    //    removals += ValueCountInference(Subgrids[i], n);
                    //}
                }

                //if successful inferences made, reset inference requirement
                if (removals > 0)
                {
                    Console.WriteLine($"Removals made: {removals} with n = {n}");
                    n = 0;

                    if (IsSolved())
                    {
                        
                        return IsCorrect();
                    }
                }
            } 
            
            return IsCorrect();
        }
        
        /// <summary>
        /// Checks if each cell in the grid has only one value. Does not check for correctness. 
        /// </summary>
        /// <returns></returns>
        public bool IsSolved()
        {
            bool solved = true;

            for (int x = 0; x < GridArr.GetLength(0); x++)
            {
                for (int y = 0; y < GridArr.GetLength(1); y++)
                {
                    if (GridArr[x, y].Value.Length > 1)
                        solved = false;
                }
            }

            Console.WriteLine($"Puzzle solution reached: {solved}");
            Console.WriteLine(ToString());

            if (Program.solveLogFlag)
            {
                Program.solveLog.Write($"Puzzle solution reached: {solved}\n{ToString()}\n");
            }

            return solved;
        }

        /// <summary>
        /// Checks the entire grid for correctness. Slower than IsSolved().
        /// </summary>
        /// <returns></returns>
        public bool IsCorrect()
        {
            bool isCorrect = true;

            CellSet row, col, subgrid;

            for (int i = 0; i < PuzzleSize; i++)
            {
                row = new CellSet(GetRow(i));
                col = new CellSet(GetCol(i));
                if (!row.IsCorrect() || !col.IsCorrect())
                {
                    isCorrect = false;
                    break;
                }
            }

            for (int i = 0; i < Subgrids.Count; i++)
            {
                subgrid = new CellSet(Subgrids[i]);
                if (!subgrid.IsCorrect())
                {
                    isCorrect = false;
                    break;
                }
            }

            Console.WriteLine(($"Puzzle is free of single value conflicts: {isCorrect}"));
            Console.WriteLine(ToString());

            if (Program.solveLogFlag)
            {
                //Program.solveLog.Write($"Puzzle is correct: {isCorrect}" + "\n" + ToString());
                using (Program.solveLog)
                {
                    Program.solveLog.Write($"Puzzle is free of single value conflicts: {isCorrect}" + "\n" + ToString());
                }
            }
            if (Program.finalStateFlag)
            {
                using (Program.finalStateFile)
                {
                    Program.finalStateFile.Write($"Puzzle is free of single value conflicts: {isCorrect}" + "\n" + ToString());
                }
            }
            return isCorrect;
        }

        #region getGroups
        /// <summary>
        /// Returns a row of cells as an array. 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns>Row of cells as a cell array</returns>
        public List<Cell> GetRow(int rowIndex)
        {
            //Cell[] row = new Cell[GridArr.GetLength(1)];
            List<Cell> row = new List<Cell>();

            for (int i = 0; i < GridArr.GetLength(1); i++)
            {
                row.Add(GridArr[rowIndex, i]);
            }

            return row;
        }

        /// <summary>
        /// Returns a column of cells as an array.
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns>Column of cells as a cell array</returns>
        public List<Cell> GetCol(int colIndex)
        {
            //Cell[] col = new Cell[GridArr.GetLength(0)];
            List<Cell> col = new List<Cell>();

            for (int i = 0; i < GridArr.GetLength(0); i++)
            {
                col.Add(GridArr[i, colIndex]);
            }

            return col;
        }

        /// <summary>
        /// Returns a subgrid of cells as a list.
        /// </summary>
        /// <param name="subgridRowIndex"></param>
        /// <param name="subgridColIndex"></param>
        /// <returns>Subgrid of cells as a list</returns>
        public List<Cell> GetSubgrid(int subgridRowIndex, int subgridColIndex)
        {
            List<Cell> subgrid = new List<Cell>();

            for (int y = 0; y < SubgridRows; y++)
            {
                for (int x = 0; x < SubgridCols; x++)
                {
                    //cell = GridArr[x + (SubgridRows * subgridRowIndex), y + (SubgridCols * subgridColIndex)];
                    //Console.WriteLine($"{cell.Row}, {cell.Col}");
                    subgrid.Add(GridArr[x + (SubgridRows * subgridRowIndex), y + (3 * subgridColIndex)]);
                }
            }

            return subgrid;
        }
        #endregion

        /// <summary>
        /// Returns a string of the entire grid array.
        /// </summary>
        /// <returns>String containing the entire grid state.</returns>
        public override string ToString()
        {
            string grid = "";
            string line;
            int additionalPad;

            for (int x = 0; x < GridArr.GetLength(0); x++)
            {
                additionalPad = 0;
                line = "";

                for (int y = 0; y < GridArr.GetLength(1); y++)
                {
                    if (y != 0 && y % SubgridRows == 0)
                    {
                        line += "| ";
                        additionalPad += 2;
                    }
                    line += GridArr[x, y].Value;
                    line = line.PadRight((y + 1) * (Alphabet.Length + 1) + additionalPad);
                }
                grid += line + "\n";

                if ((x+ 1) != GridArr.GetLength(0) && (x + 1)  % SubgridRows == 0)
                {
                    string lineSeparator = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        lineSeparator += "-";
                    }
                    grid += lineSeparator + "\n";
                }
            }
            return grid;
        }

        #region Obsolete or never worked

        public int ValueCount2(List<Cell> cellGroup, int n)
        {
            int removals = 0;
            int[] alphabetCount = new int[9];
            List<char> matchList;
            HashSet<char> matchSet;
            //count instances of each value in a group

            CellSet cellSet = new CellSet(cellGroup);
            alphabetCount = cellSet.ValueCount(this);
            
            //for each v with v.count = n
            for (int i = 0; i < alphabetCount.Length; i++)
            {
                if (alphabetCount[i] == n)
                {
                    //add v to new matchlist, hashset
                    matchSet = new HashSet<char>()
                    {
                        AlphabetArr[i]
                    };

                    //HACK this will get stuck
                    
                    int shareCount = 0;
                    //for each other value (ov) sharing any cell with v 
                    for (int j = i + 1; j < AlphabetArr.Length; j++)
                    {
                        if (AlphabetArr[j] == n)
                        {
                            matchSet.Add(AlphabetArr[j]);
                        }
                        for (int k = 0; k < cellGroup.Count; k++)
                        {
                            if (true)
                            {
                                //TODO start here
                            }
                        }
                    }
                    //if ov.count = n 
                        //add ov to matchlist 
                        
                    
                    if (matchSet.Count == n)
                    {
                        //Remove other values in cells with values that are a superset of matchset,
                        //leaving only matchset values in cells with matchset values
                        for (int j = 0; j < cellGroup.Count; j++)
                        {
                            //this is the list way to check for subset: "!matchlist.Except(cellGroup[j].ValueList).Any()"
                                //This still returns true if matchlist is a proper subset of ValueList
                            if (matchSet.IsProperSubsetOf(cellGroup[j].ValueSet))
                            {
                                removals += cellGroup[j].OverWriteValueMembers(matchSet);
                                Console.WriteLine($"ValueCount removal at : {cellGroup[j].Row}, {cellGroup[j].Col}");
                            }
                        }
                        //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
                            //remove v's of matchlist from intersecting row/col
                    }
                    else
                    {
                        //this is unecessary with the above while statement
                        Console.WriteLine("subset sieve else not implemented");
                        //Console.ReadLine();
                        //for each other value (ov) sharing any cell with v 
                            //if ov.count = n 
                                //add ov to matchlist
                                    //if matchlist.count = n
                                        //remove values that matchlist does not contain from cells with v in in current group
                                        //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
                                            //remove v's of matchlist from intersecting row/col
                    }
                }
            }

            //count instances of each value in a group
            //for each v with v.count = n
                    //add v to new matchlist
                    //if matchlist.count = n
                        //remove values that matchlist does not contain from cells with v in in current group
                        //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
                            //remove v's of matchlist from intersecting row/col
                    //else
                        //for each other value (ov) sharing any cell with v 
                            //if ov.count = n 
                                //add ov to matchlist
                                    //if matchlist.count = n
                                        //remove values that matchlist does not contain from cells with v in in current group
                                        //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
                                            //remove v's of matchlist from intersecting row/col
            return removals;
        }

        public void ValueCount2()
        {

            //count instances of each value in a group

            //for n < 9
            //for each v with v.count = n
            //add v to new matchlist
            //if matchlist.count = n
            //remove values that matchlist does not contain from cells with v in in current group
            //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
            //remove v's of matchlist from intersecting row/col
            //else
            //for each other value (ov) sharing any cell with v 
            //if ov.count = n 
            //add ov to matchlist
            //if matchlist.count = n
            //remove values that matchlist does not contain from cells with v in in current group
            //if cells containing v are in the same row/col (only check for n count <= SubgridRows or SubgridCols)
            //remove v's of matchlist from intersecting row/col
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">The equal set size to make an inference.</param>
        public bool NCellRowSieve(int n)
        {
            bool success = false;

            //for each row
            for (int i = 0; i < GridArr.GetLength(0); i++)
            {
                //
                if (SieveNCellGroup(GetRow(i), n) > 0)
                {
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">The equal set size to make an inference.</param>
        public bool NCellColSieve(int n)
        {
            bool success = false;

            for (int i = 0; i < GridArr.GetLength(1); i++)
            {
                if (SieveNCellGroup(GetCol(i), n) > 0)
                {
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">The equal set size to make an inference.</param>
        public bool NCellSubgridSieve(int n)
        {
            bool success = false;

            for (int i = 0; i < Subgrids.Count; i++)
            {
                if (SieveNCellGroup(Subgrids[i], n) > 0)
                    success = true;
            }

            return success;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="n">The equal set size to make an inference.</param>
        //public void NCellSubgridInference(int n)
        //{
        //    List<Cell> matches;

        //    for (int subCol = 0; subCol < SubgridCols; subCol++)
        //    {
        //        for (int subRow = 0; subRow < SubgridRows; subRow++)
        //        {
        //            for (int y = 0 * SubgridCols; y < (subCol + 1) * SubgridCols; y++)
        //            {

        //                //consider a group: this is a row
        //                for (int i = 0 * SubgridRows; i < (subRow + 1) * SubgridRows; i++)
        //                {
        //                    matches = new List<Cell>();
        //                    //if row 0 a cell with length n
        //                    if (GridArr[i, y].Value.Length == n)
        //                    {
        //                        //add cell with length n 
        //                        matches.Add(GridArr[i, y]);
        //                        //look at other left over cells in group for n set matches
        //                        if (n > 1)
        //                        {
        //                            for (int j = i; j < SubgridRows; j++)
        //                            {
        //                                if (matches[0].Value.Equals(GridArr[y, j].Value))
        //                                {
        //                                    matches.Add(GridArr[y, j]);
        //                                }
        //                            }
        //                        }
        //                        //if matches found, remove value(s) from other cells in the group
        //                        if (matches.Count == n)
        //                        {
        //                            //for each item in the row
        //                            for (int k = subCol * SubgridCols; k < (subCol + 1) * SubgridCols; k++)
        //                            {
        //                                //for every matched value
        //                                for (int c = 0; c < matches[0].Value.Length; c++)
        //                                {
        //                                    //if a the current cell contains the value
        //                                    if (GridArr[k, y].Value.Contains(matches[0].Value.Substring(c, 1)))
        //                                    {
        //                                        bool replace = true;
        //                                        for (int l = 0; l < matches.Count; l++)
        //                                        {
        //                                            if (GridArr[k, y].Row == matches[l].Row && GridArr[k, y].Col == matches[l].Col)
        //                                            {
        //                                                replace = false;
        //                                            }
        //                                        }

        //                                        if (replace)
        //                                        {
        //                                            //remove the value
        //                                            GridArr[k, y].Value = GridArr[k, y].Value.Replace(matches[0].Value.Substring(c, 1), "");
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="n">The equal set size to make an inference.</param>
        //public bool NCellColumnInference(int n)
        //{
        //    bool success = false;
        //    List<Cell> matches;

        //    for (int y = 0; y < GridArr.GetLength(1); y++)
        //    {
        //        //consider a group: this is a row
        //        for (int i = 0; i < GridArr.GetLength(0); i++)
        //        {
        //            matches = new List<Cell>();
        //            //if row 0 a cell with length n
        //            if (GridArr[i, y].Value.Length == n)
        //            {
        //                //add cell with length n 
        //                matches.Add(GridArr[i, y]);
        //                //look at other left over cells in group for n set matches
        //                if (n > 1)
        //                {
        //                    for (int j = i; j < GridArr.GetLength(0); j++)
        //                    {
        //                        if (matches[0].Value.Equals(GridArr[y, j].Value))
        //                        {
        //                            matches.Add(GridArr[y, j]);
        //                        }
        //                    }
        //                }
        //                //if matches found, remove value(s) from other cells in the group
        //                if (matches.Count == n)
        //                {
        //                    //for each item in the row
        //                    for (int k = 0; k < GridArr.GetLength(0); k++)
        //                    {
        //                        //for every matched value
        //                        for (int c = 0; c < matches[0].Value.Length; c++)
        //                        {
        //                            //if a the current cell contains the value
        //                            if (GridArr[k, y].Value.Contains(matches[0].Value.Substring(c, 1)))
        //                            {
        //                                bool replace = true;
        //                                for (int l = 0; l < matches.Count; l++)
        //                                {
        //                                    if (GridArr[k, y].Row == matches[l].Row && GridArr[k, y].Col == matches[l].Col)
        //                                    {
        //                                        replace = false;
        //                                    }
        //                                }

        //                                if (replace)
        //                                {
        //                                    //remove the value
        //                                    GridArr[k, y].Value = GridArr[k, y].Value.Replace(matches[0].Value.Substring(c, 1), "");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return success;
        //}

        ///// <summary>
        ///// Looks for row inferences in entire grid.
        ///// </summary>
        ///// <param name="n">The equal set size to make an inference.</param>
        //public void NCellRowInference(int n)
        //{
        //    List<Cell> matches;
        //    //consider a group: this is a row
        //    for (int x = 0; x < GridArr.GetLength(0); x++)
        //    {
        //        //consider each item in a row
        //        for (int i = 0; i < GridArr.GetLength(1); i++)
        //        {
        //            matches = new List<Cell>();
        //            //if row 0 a cell with length n
        //            if (GridArr[x, i].Value.Length == n)
        //            {
        //                //add cell with length n 
        //                matches.Add(GridArr[x, i]);
        //                //look at other left over cells in group for n set matches
        //                if (n > 1)
        //                {
        //                    for (int j = i; j < GridArr.GetLength(1); j++)
        //                    {
        //                        if (matches[0].Value.Equals(GridArr[x, j].Value))
        //                        {
        //                            matches.Add(GridArr[x, j]);
        //                        }
        //                    }
        //                }
        //                //if matches found, remove value(s) from other cells in the group
        //                if (matches.Count == n)
        //                {
        //                    //for each item in the row
        //                    for (int k = 0; k < GridArr.GetLength(1); k++)
        //                    {
        //                        //for every matched value
        //                        for (int c = 0; c < matches[0].Value.Length; c++)
        //                        {
        //                            //if a the current cell contains the value
        //                            if (GridArr[x, k].Value.Contains(matches[0].Value.Substring(c, 1)))
        //                            {
        //                                bool replace = true;
        //                                for (int l = 0; l < matches.Count; l++)
        //                                {
        //                                    if (GridArr[x, k].Row == matches[l].Row && GridArr[x, k].Col == matches[l].Col)
        //                                    {
        //                                        replace = false;
        //                                    }
        //                                }

        //                                if (replace)
        //                                {
        //                                    //remove the value
        //                                    GridArr[x, k].Value = GridArr[x, k].Value.Replace(matches[0].Value.Substring(c, 1), "");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}