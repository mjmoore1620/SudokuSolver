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
    [XmlRoot("puzzle")]
    public class XmlPuzzle
    {
        [XmlElement("subgrid-dimensions")]
        public SubgridDimensions Dimensions { get; set; }

        [XmlElement("alphabet")]
        public string Alphabet { get; set; }

        [XmlElement("initial-values")]
        public InitialValues InitialValues { get; set; }
    }
}