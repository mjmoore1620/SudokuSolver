/////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		SudokuSolver
//	File Name:		Cell.cs
//	Description:	TODO description
//	Course:			CSCI 5300 - Software Design
//	Author:			Matthew Moore, zmjm1320@gmail.com
//	Created:	    10/30/2017
//	Copyright:		Matthew Moore, 2017
//
/////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SudokuSolver
{
    public class Cell
    {
        [XmlElement("row")]
        public int Row { get; set; }
        [XmlElement("col")]
        public int Col { get; set; }
        [XmlElement("value")]
        public string Value { get; set; }

        public List<char> ValueList { get; set; }
        public HashSet<char> ValueSet { get; set; }
        

        public Cell()
        {
            ValueList = new List<char>();
            ValueSet = new HashSet<char>();
        }

        

        public Cell(int row, int col, string value)
        {
            Row = row;
            Col = col;
            Value = value;
            ValueList = new List<char>(value);
            ValueSet = new HashSet<char>(value);
        }

        public Cell(Cell cell)
        {
            Row = cell.Row;
            Col = cell.Col;
            Value = cell.Value;
            ValueList = new List<char>(Value);
            ValueSet = new HashSet<char>(Value);
        }

        /// <summary>
        /// Compares two cells to see if they are the same cell
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool equals = false;
            Cell otherCell;

            try
            {
                otherCell = (Cell)obj;
            }
            catch (InvalidCastException)
            {
                Console.WriteLine("Error: obj not type Cell");
                throw;
            }

            if (Row == otherCell.Row && Col == otherCell.Col && Value == otherCell.Value)
                equals = true;

            return equals;
        }

        /// <summary>
        /// Updates the string Value member of this class to what the ValueList contains.
        /// </summary>
        internal void UpdateValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in ValueList)
                sb.Append(c.ToString());

            Value = sb.ToString();
            //HACK 
            ValueSet.Clear();
            ValueSet.UnionWith(ValueList);
        }

        /// <summary>
        /// Checks if a cell contains only value. 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsSolved()
        {
            bool solved = false;

            if (ValueSet.Count == 1)
                solved = true;

            return solved;
        }

        public int OverWriteValueMembers(string values)
        {
            int valueDifference = Value.Length - values.Length;

            Value = values;
            ValueList.Clear();
            ValueList.AddRange(values);
            ValueSet.Clear();
            ValueSet.UnionWith(values);

            return valueDifference;
        }

        public int OverWriteValueMembers(HashSet<char> values)
        {
            int valueDifference = ValueSet.Count - values.Count;

            StringBuilder sb = new StringBuilder();
            foreach (var c in values)
                sb.Append(c.ToString());

            Value = sb.ToString();
            ValueList.Clear();
            ValueList.AddRange(values);
            ValueSet.Clear();
            ValueSet.UnionWith(values);

            return valueDifference;
        }

        public int OverWriteValueMembers(List<char> values)
        {
            int valueDifference = ValueList.Count - values.Count;

            StringBuilder sb = new StringBuilder();
            foreach (var c in values)
                sb.Append(c.ToString());

            Value = sb.ToString();
            ValueList.Clear();
            ValueList.AddRange(values);
            ValueSet.Clear();
            ValueSet.UnionWith(values);

            return valueDifference;
        }
        
    }
}