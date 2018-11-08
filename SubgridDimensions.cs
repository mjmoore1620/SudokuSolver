/////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		SudokuSolver
//	File Name:		Puzzle.cs
//	Description:	// TODO 
//	Course:			CSCI 5300 - Software Design
//	Author:			Matthew Moore, zmjm1320@gmail.com
//	Created:	    10/30/2017
//	Copyright:		Matthew Moore, 2017
//
/////////////////////////////////////////////////////////////////////////////////////////
using System.Xml.Serialization;

namespace SudokuSolver
{
    public class SubgridDimensions
    {
        [XmlElement("rows")]
        public int Rows { get; set; }
        [XmlElement("columns")]
        public int Columns { get; set; }
    }
}