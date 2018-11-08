/////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		SudokuSolver
//	File Name:		InitialValues.cs
//	Description:	TODO
//	Course:			CSCI 5300 - Software Design
//	Author:			Matthew Moore, zmjm1320@gmail.com
//	Created:	    10/30/2017
//	Copyright:		Matthew Moore, 2017
//
/////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SudokuSolver
{
    public class InitialValues
    {
        [XmlElement("cell")]
        public List<Cell> Cells { get; set; }
    }
}