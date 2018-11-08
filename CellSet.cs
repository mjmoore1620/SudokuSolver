using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class CellSet : IEnumerable<Cell>, ICollection<Cell>
    {
        public HashSet<Cell> HashSet { get; set; }
        public bool solved;

        /// <summary>
        /// Constructor that uses an existing IEnumerable collection of cells.
        /// </summary>
        /// <param name="hashSet"></param>
        public CellSet(IEnumerable<Cell> hashSet) 
        {
            HashSet = new HashSet<Cell>(hashSet);
            solved = IsSolved();
        }

        /// <summary>
        /// Determins if each cell only has one value. 
        /// </summary>
        /// <returns>Returns true if the CellSet is solved.</returns>
        public bool IsSolved()
        {
            bool isSolved = true;
            foreach (var cell in HashSet)
            {
                if (!cell.IsSolved())
                {
                    isSolved = false;
                    break;
                }
            }
            return isSolved;
        }

        /// <summary>
        /// Determines if the CellSet is solved correctly.
        /// </summary>
        /// <returns></returns>
        public bool IsCorrect()
        {
            List<char> singleValues = new List<char>();
            
            foreach (var cell in HashSet)
            {
                char value = cell.ValueList[0];

                if (cell.ValueList.Count == 1)
                {
                    if (singleValues.Contains(value))
                    {
                        return false;
                    }
                    singleValues.Add(value);
                }
                
            }
            return true;
        }

        /// <summary>
        /// Returns an array of integers representing the quantity of each value represented in the 
        ///     HashSet of cells from this CellSet
        /// This maybe a good candidate for an internal class of Grid. 
        /// </summary>
        /// <param name="grid">The current working puzzle grid.</param>
        /// <returns>int array of value counts</returns>
        public int[] ValueCount(Grid grid)
        {
            int[] valueCount = new int[grid.PuzzleSize];
            List<Cell> setList = HashSet.ToList();
                
            //count instances of each value in this CellSet
            for (int i = 0; i < grid.AlphabetArr.Length; i++)
            {
                for (int j = 0; j < setList.Count; j++)
                {
                    if (setList[j].ValueList.Contains(grid.AlphabetArr[i]))
                    {
                        valueCount[i]++;
                    }
                }
            }
            return valueCount;
        }

        //Number of cells in the set. 
        public int Count => ((ICollection<Cell>)HashSet).Count;

        
        public bool IsReadOnly => ((ICollection<Cell>)HashSet).IsReadOnly;

        #region interface auto-implementations
        public IEnumerator<Cell> GetEnumerator()
        {
            return ((IEnumerable<Cell>)HashSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Cell>)HashSet).GetEnumerator();
        }

        public void Add(Cell item)
        {
            ((ICollection<Cell>)HashSet).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Cell>)HashSet).Clear();
        }

        public bool Contains(Cell item)
        {
            return ((ICollection<Cell>)HashSet).Contains(item);
        }

        public void CopyTo(Cell[] array, int arrayIndex)
        {
            ((ICollection<Cell>)HashSet).CopyTo(array, arrayIndex);
        }

        public bool Remove(Cell item)
        {
            return ((ICollection<Cell>)HashSet).Remove(item);
        }
        #endregion
    }//end CellSet class
}
