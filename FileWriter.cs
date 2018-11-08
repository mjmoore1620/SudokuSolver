using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SudokuSolver
{
    class FileWriter
    {
        string fileName;
        
        StreamWriter file;

        public FileWriter(string fileName)
        {
            this.fileName = fileName;
            file = new StreamWriter(fileName);
        }

        public void Write(string text)
        {
            using (file)
            {
                file.Write(text);
            }
        }

    }
}
