using System;
using System.Collections.Generic;
using System.Text;

namespace GraphenCalc
{
    public class Matrix
    {
        private int[,] adjacency;
        private int[,] distance;
        private List<int> nodes = new List<int>();
        public int NodeCount => adjacency.GetLength(0);

        //-------------------------------------------------------------------Constructors----------------------------------------------------------------------------------------------------------------------

        public Matrix(string file)
        {
            if(File.ReadFile(file) != null)
            {
                adjacency = File.ReadFile(file);
            }
            distance = DistanceMatrix();
            nodes = CountNodes();
        }

        public Matrix(int node, int chance)
        {
            if(node >= 1)
            {
                adjacency = new int[node, node];
                RandomMatrix(adjacency, node, chance);
            }
            else
            {
                throw new Exception("Error: Node must be at least 1.");
            }
            distance = DistanceMatrix();
            nodes = CountNodes();
        }

        public Matrix(int a)
        {
            adjacency = new int[a, a];
            distance = DistanceMatrix();
            nodes = CountNodes();
        }

        public Matrix(int[,] values)
        {
            adjacency = new int[values.GetLength(0), values.GetLength(1)];

            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    adjacency[i, j] = values[i, j];
                }
            }
            distance = DistanceMatrix();
            nodes = CountNodes();
        }        

        //-------------------------------------------------------------------Operations------------------------------------------------------------------------------------------------------------------------
        
        public static Matrix Copy(Matrix matrix)
        {
            return new Matrix(matrix.GetAdjacency());
        }

        public static Matrix CopyRmvNode(Matrix matrix, int node)
        {
            Matrix a = Matrix.Copy(matrix);
            a.RemoveEdgesOfNode(node);
            return a;
        }

        public static Matrix CopyRmvEdge(Matrix matrix, int edge1, int edge2)
        {
            Matrix a = Matrix.Copy(matrix);
            a.RemoveEdge(edge1, edge2);
            return a;
        }

        public static Matrix Add(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(a.adjacency.Length);

            int dim = a.adjacency.Length;

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    result.adjacency[i, j] = a.adjacency[i, j] + b.adjacency[i, j];
                }
            }
            return result;
        }

        public static int[,] CopyMatrix(int[,] matrix)
        {
            int[,] result = new int[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = matrix[i, j];
                }
            }
            return result;
        }

        public void RandomMatrix(int[,] matrix, int node, int chance)
        {
            Random rnd = new Random();

            int count = 0;

            if(chance <= 100 && chance >= 0)
            {
                for (int i = 0; i < node; i++)
                {
                    for (int j = count; j < node; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, i] = 0;
                        }
                        else
                        {
                            if (rnd.Next(1, 101) > chance)
                            {
                                matrix[j, i] = 0;
                            }
                            else
                            {
                                matrix[j, i] = 1;
                            }
                            matrix[i, j] = matrix[j, i];
                        }
                    }
                    count++;
                }
            }
            else
            {
                Console.WriteLine("Error: Chance must be at least 0 and must not be bigger than 100.\n");
            }
            
        }
            
        public int[,] DistanceMatrix()
        {
            int[,] distance = CopyMatrix(adjacency);

            int[,] power = CopyMatrix(adjacency);

            for (int count = 1; count < NodeCount; count++)
            {
                int[,] temp = new int[NodeCount, NodeCount];

                for (int i = 0; i < NodeCount; i++)
                {
                    for (int j = 0; j < NodeCount; j++)
                    {
                        int sum = 0;

                        for (int k = 0; k < NodeCount; k++)
                        {
                            sum += power[i, k] * GetAdjacency()[k, j];
                        }

                        temp[i, j] = sum;

                        if (distance[i, j] == 0 && sum > 0 && i != j)
                        {
                            distance[i, j] = count + 1;
                        }
                    }
                }
                power = temp;
            }
            return distance;
        }

        //more legible but less performant
        public int[,] NewDistanceMatrix()
        {
            int[,] distance = CopyMatrix(adjacency);
            int[,] power = CopyMatrix(adjacency);

            for (int i = 2; i < NodeCount; i++)
            {
                power = PowerMatrix(i);

                distance = CalcDistanceMatrix(distance, power, i);
            }

            return distance;
        }

        public int[,] CalcDistanceMatrix(int[,] _matrix, int[,] _powermatrix, int power)
        {
            int[,] matrix = _matrix;

            for (int i = 0; i < NodeCount; i++)
            {
                for (int j = 0; j < NodeCount; j++)
                {
                    if (_matrix[i, j] == 0 && _powermatrix[i, j] > 0 && i != j)
                    {
                        matrix[i, j] = power;
                    }
                }
            }

            return matrix;
        }

        public int[,] PowerMatrix(int power)
        {
            int[,] powerm = CopyMatrix(GetAdjacency());

            for (int count = 1; count < power; count++)
            {
                int[,] temp = new int[NodeCount, NodeCount];

                for (int i = 0; i < NodeCount; i++)
                {
                    for (int j = 0; j < NodeCount; j++)
                    {
                        int sum = 0;

                        for (int k = 0; k < NodeCount; k++)
                        {
                            sum += powerm[i, k] * GetAdjacency()[k, j];
                        }
                        temp[i, j] = sum;
                    }
                }
                powerm = temp;
            }
            return powerm;
        }

        //-------------------------------------------------------------------Misc------------------------------------------------------------------------------------------------------------------------------

        public void AddEdge(int x, int y)
        {
            if (x != y && x < adjacency.Length && y < adjacency.Length)
            {
                SetValue(x, y, 1);
                SetValue(y, x, 1);
            }
        }

        public void RemoveEdge(int x, int y)
        {
            if(x < adjacency.Length && y < adjacency.Length)
            {
                SetValue(x, y, 0);
                SetValue(y, x, 0);
            }            
        }
        
        public void RemoveEdgesOfNode(int x)
        {
            if (x < adjacency.GetLength(0))
            {
                for (int i = 0; i < adjacency.GetLength(0); i++)
                {
                    adjacency[i, x] = 0;
                    adjacency[x, i] = 0;
                }
            }
        }

        public List<int> CountNodes()
        {
            List<int> node = new List<int>();

            for (int i = 0; i < GetAdjacency().GetLength(0); i++)
            {
                node.Add(i);
            }

            return node;
        }

        public List<int> GetNodes()
        {
            return nodes;
        }

        public int[,] GetAdjacency()
        {
            return adjacency;
        }

        public int[,] GetDistance()
        {
            return distance;
        }

        public void SetValue(int a, int b, int c)
        {
            if(a < adjacency.GetLength(0) && b < adjacency.GetLength(1))
            {
                this.adjacency[a, b] = c;
            }            
        }

        //-------------------------------------------------------------------ToString--------------------------------------------------------------------------------------------------------------------------
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Adjacencymatrix:\n");
            
            for (int i = 0; i < adjacency.GetLength(0); i++)
            {
                for (int j = 0; j < adjacency.GetLength(1); j++)
                {
                    sb.Append(adjacency[i, j] + " ");
                }
                sb.Append("\n");
            }
            sb.AppendLine();

            sb.AppendLine("Distancematrix:\n");

            for (int i = 0; i < adjacency.GetLength(0); i++)
            {
                for (int j = 0; j < adjacency.GetLength(1); j++)
                {
                    sb.Append(distance[i, j] + " ");
                }
                sb.Append("\n");
            }
            return sb.ToString();                          
        }
    }
}
