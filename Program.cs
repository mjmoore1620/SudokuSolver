/////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		SudokuSolver
//	File Name:		Program
//	Description:	Main driver of Sudoku solver. Specifications set by Phil Pfeiffer
//                  Framework: .Net Framework 4.6
//	Course:			CSCI 5300 - Software Design
//	Author:			Matthew Moore, zmjm1320@gmail.com
//	Created:	    10/30/2017
//	Copyright:		Matthew Moore, 2017
//
/////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SudokuSolver
{
    class Program
    {
        public static Grid grid;
        //Command-line option flags
        public static bool puzzleInputFlag, errorLogFlag, solveLogFlag, finalStateFlag = false;
        public static StreamWriter errorLog, solveLog, finalStateFile;
        public static string puzzleFile = "";


        static void Main(string[] args)
        {
            grid = new Grid();

            ParseCommandLine(args);
            

            if (!string.IsNullOrEmpty(puzzleFile))
            {
                grid = new Grid(DeserializeXmlPuzzle( OpenPuzzleFile(puzzleFile)));
            }
            else
            {
                grid = new Grid(DeserializeXmlPuzzle( OpenPuzzleFile(@"../../../xmlPuzzles/MxNpuzzleInternet2.xml")));
            }
            Console.WriteLine(grid.ToString());

            var watch = System.Diagnostics.Stopwatch.StartNew();

            grid.Solve();

            watch.Stop();
            Console.WriteLine("Milliseconds elapsed: " + watch.ElapsedMilliseconds);

            Console.Write("Program ran with these args: ");
            foreach (var arg in args)
            {
                Console.Write(arg + " ");
            }

            Console.ReadLine();
        }

        private static void InitializeLog()
        {

        }

        /// <summary>
        /// Alters runtime flag variables to execute with user's desired configuration. 
        /// </summary>
        /// <param name="args">The command-line args at execution.</param>
        private static void ParseCommandLine(string[] args)
        {
            bool puzzleFileProcessed = false;
            foreach(string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    //Look for flags
                    char[] argArr = arg.ToArray();
                    for (int i = 1; i < argArr.Length; i++)
                    {
                        switch (argArr[i])
                        {
                            case 'i':
                                puzzleInputFlag = true;
                                break;
                            case 'e':
                                errorLogFlag = true;
                                errorLog = new StreamWriter("errorLog.txt");
                                break;
                            case 'l':
                                solveLogFlag = true;
                                solveLog = new StreamWriter("solutionLog.txt");
                                break;
                            case 'o':
                                finalStateFlag = true;
                                finalStateFile = new StreamWriter("finalState.txt");
                                break;
                            default:
                                InvalidArgumentException();
                                break;
                        }
                    }
                }
                //if the arg isn't a command-line flag
                else
                {
                    if (puzzleInputFlag && !puzzleFileProcessed)
                    {
                        puzzleFile = arg;
                        puzzleFileProcessed = true;
                    }
                    else
                    {
                        InvalidArgumentException();
                    }
                }
            }
        }

        /// <summary>
        /// Throw exception in case of invalid command-line argument and exit with code '3'.
        /// HACK: This should probably be a subclass of ArgumentException class.
        /// </summary>
        private static void InvalidArgumentException()
        {
            Console.WriteLine("Invalid argument syntax. Example: " );
            Console.WriteLine("SudokuSolver.exe -i|-e|-l|-o|-ielo| ..//..//file//path//puzzle.xml");
            Console.WriteLine("Temporary break to keep console open. \nExit Code: 3");
            Environment.Exit(3);

        }

        /// <summary>
        /// Experimental: http://www.informit.com/articles/article.aspx?p=2438407&seqNum=11
        /// Unfinished.
        /// </summary>
        //private class CommandLineClass
        //{
        //    public string Action { get; set; }
        //    public string AfterAction { get; set; }

        //    public CommandLineClass(string[] arguments)
        //    {
        //        for (int argCounter = 0; argCounter < arguments.Length; argCounter++)
        //        {
        //            switch (argCounter)
        //            {
        //                case 0:
        //                    Action = arguments[0].ToLower();
        //                    break;

        //                default:
        //                    AfterAction += arguments[argCounter];
        //                    break;                            
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Lists the initial values of the puzzle file.
        /// </summary>
        /// <param name="xmlPuzzleFile">Test files: @"../../puzzle1.xml", @"../../puzzle2.xml", @"../../MxNpuzzle1.xml", </param>
        private static void TestPrintXmlInitialValues(string xmlPuzzleFile)
        {
            TextReader puzzleStream = OpenPuzzleFile(xmlPuzzleFile);
            XmlPuzzle puzzle = DeserializeXmlPuzzle(puzzleStream);
            Console.WriteLine("\nInitial values: ");
            for (int i = 0; i < puzzle.InitialValues.Cells.Count; i++)
            {
                Console.WriteLine("Row: " + puzzle.InitialValues.Cells[i].Row.ToString() +
                                " Col: " + puzzle.InitialValues.Cells[i].Col.ToString() +
                                " Value: " + puzzle.InitialValues.Cells[i].Value);
            }
        }

        /// <summary>
        /// TODO
        /// Attempts to open an xml puzzle file. 
        /// Where does error handling happen?
        /// </summary>
        /// <param name="xmlPuzzleFile">Test files: @"../../puzzle1.xml", @"../../puzzle2.xml", @"../../MxNpuzzle1.xml",</param>
        /// <returns>Returns the reader if opening the file is successful. (Next step is deserialization)</returns>
        private static TextReader OpenPuzzleFile(string xmlPuzzleFile)
        {
            TextReader reader = null;

            try
            {
                reader = new StreamReader(xmlPuzzleFile);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Exit code: 3");
                if (errorLogFlag)
                {
                    using (errorLog)
                    {
                        errorLog.Write($"{e.Message}\nExit code: 3"); 
                    }
                }
                Console.ReadLine();
                Environment.Exit(3);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (errorLogFlag)
                {
                    using (errorLog)
                    {
                        errorLog.Write(e.Message); 
                    }
                }
                Console.ReadLine();
                throw e;
            }
            
            return reader;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static XmlPuzzle DeserializeXmlPuzzle(TextReader reader)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(XmlPuzzle));
            XmlPuzzle xmlData = null;

            try
            {
                var puzzle = deserializer.Deserialize(reader);
                xmlData = (XmlPuzzle)puzzle;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Exit code: 3");
                if (errorLogFlag)
                {
                    using (errorLog)
                    {
                        errorLog.Write($"{e.Message}\nExit code: 3"); 
                    }
                }
                Console.ReadLine();
                Environment.Exit(3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (errorLogFlag)
                {
                    using (errorLog)
                    {
                        errorLog.Write(e.Message); 
                    }
                }
                Console.ReadLine();
                throw e;
            }
            finally
            {
                reader.Close();
            }

            return xmlData;
        }
    }
}
