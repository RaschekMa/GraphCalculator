using GraphCalculator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GraphenCalc
{
    public class File
    {
        public static int[,] ReadFile(string file)
        {            
            try 
            {
                string[] lines = System.IO.File.ReadAllLines(file);                

                int[,] matrix = new int[lines.Length, lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] rowData = lines[i].Split(';');

                    for (int j = 0; j < rowData.Length; j++)
                    {
                        matrix[i, j] = Int32.Parse(rowData[j]);
                    }
                }

                if (CheckFormat(matrix))
                {
                    return matrix;
                }
                else
                {
                    throw new FormatException("Format of matrix is not correct.");
                }
            }            
            catch (FormatException ex)
            {
                throw new FormatException(ex.Message);
            }
            
        }

        public static bool CheckFormat(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if(matrix[i, i] != 0)
                {
                    return false;
                }

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] != matrix[j, i])
                    {                        
                        return false;
                    }

                    if (matrix[i, j] != 0 && matrix[i, j] != 1)
                    {                        
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
