using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GraphenCalc.Classes
{
    public class Graph
    {
        private Matrix ma;        
        private int edges;
        private int diameter;
        private int radius;
        List<int> articulations = new List<int>();
        List<Tuple<int, int>> bridges = new List<Tuple<int, int>>();
        private List<int> center = new List<int>();
        private List<List<int>> components = new List<List<int>>();
       

        //-------------------------------------------------------------------Constructors----------------------------------------------------------------------------------------------------------------------

        public Graph(string file)
        {
            ma = new Matrix(file);
            CalcAll();
        }

        public Graph(int node, int chance)
        {
            ma = new Matrix(node, chance);
            CalcAll();
        }

        //-------------------------------------------------------------------GraphMethods----------------------------------------------------------------------------------------------------------------------
                
        public int CountEdges()
        {
            int number = 0;

            if(ma.GetAdjacency() != null)
            {
                for (int i = 0; i < ma.GetAdjacency().GetLength(0); i++)
                {
                    for (int j = 0; j < ma.GetAdjacency().GetLength(1); j++)
                    {
                        number += ma.GetAdjacency()[i, j];
                    }
                }
                number = number / 2;
                return number;
            }
            else
            {
                return 0;
            }
        }

        public List<int> Neighbors(int node, Matrix x)
        {
            List<int> neighbors = new List<int>();

            for (int i = 0; i < x.GetAdjacency().GetLength(0); i++)
            {
                if(x.GetAdjacency()[node, i] == 1)
                {
                    neighbors.Add(i);
                }
            }

            return neighbors;
        }        

        public int Eccentricity(int node)
        {
            int eccent = 0;

            for (int i = 0; i < ma.NodeCount; i++)
            {                
                if(ma.GetDistance()[i, node] > eccent)
                {
                    eccent = ma.GetDistance()[i, node];
                }
                if(i != node && ma.GetDistance()[i, node] == 0)
                {
                    eccent = -1;
                    break;
                }
            }
            return eccent;
        }

        public int Diameter()
        {
            int dia = 0;

            for (int i = 0; i < ma.NodeCount; i++)
            {
                if (Eccentricity(i) > dia)
                {
                    dia = Eccentricity(i);
                }
                if (Eccentricity(i) == -1)
                {
                    dia = -1;
                    break;
                }
            }
            return dia;
        }

        public int Radius()
        {
            int rad = ma.NodeCount;

            for (int i = 0; i < ma.NodeCount; i++)
            {
                if (Eccentricity(i) < rad)
                {
                    rad = Eccentricity(i);
                }
                if (Eccentricity(i) == -1)
                {
                    rad = -1;
                    break;
                }
            }
            return rad;
        }

        public List<int> Center()
        {
            List<int> centerlist = new List<int>();

            for (int i = 0; i < ma.NodeCount; i++)
            {
                if(Eccentricity(i) == radius)
                {
                    centerlist.Add(i);
                }                
            }
            return centerlist;
        }
        
        public int EdgesPerNode(int i)
        {
            int edges = 0;

            for (int k = 0; k < ma.GetAdjacency().GetLength(0); k++)
            {
                edges += ma.GetAdjacency()[i, k];
            }

            return edges;
        }

        public List<Tuple<int, int>> Bridges(Matrix x)
        {
            List<Tuple<int, int>> bridgeList = new List<Tuple<int, int>>();

            for (int i = 0; i < x.GetAdjacency().GetLength(0); i++)
            {
                for (int j = 0; j < x.GetAdjacency().GetLength(0); j++)
                {
                    if(x.GetAdjacency()[i, j] == 1)
                    {
                        Matrix b = Matrix.Copy(x);
                        b.RemoveEdge(i, j);

                        if (Components(x).Count < (Components(b).Count))
                        {    
                            if(!bridgeList.Contains(new Tuple<int, int>(i, j)) && !bridgeList.Contains(new Tuple<int, int>(j, i)))
                            {
                                bridgeList.Add(new Tuple<int, int>(i, j));
                            }   
                        }
                    }                    
                }
            }
            return bridgeList;
        }

        public List<int> Articulations(Matrix x)
        {
            List<int> articulationList = new List<int>();

            for (int i = 0; i < x.GetAdjacency().GetLength(0); i++)
            {
                Matrix b = Matrix.Copy(x);
                b.RemoveEdgesOfNode(i);

                if (Components(x).Count < (Components(b).Count - 1))
                {
                    articulationList.Add(i);
                }
            }
            return articulationList;
        }

        public List<List<int>> Components(Matrix x)
        {
            List<List<int>> component = new List<List<int>>();
            List<int> nodelist = new List<int>(ma.GetNodes());

            while (nodelist.Count > 0)
            {
                List<int> comp = new List<int>(DFS(nodelist[0], x));
                int count = comp.Count;
                component.Add(comp);

                for(int i = 0; i < count; i++)
                {
                    nodelist.Remove(comp[i]);
                }
            }

            return component;
        }

        public List<int> DFS(int start, Matrix x)
        {
            List<int> visited = new List<int>();

            Stack<int> stack = new Stack<int>();
            stack.Push(start);

            if(!ma.GetNodes().Contains(start))
            {
                return visited;
            }

            while (stack.Count > 0)
            {
                int node = stack.Pop();

                if (visited.Contains(node))
                {
                    continue;
                }                    

                visited.Add(node);

                foreach (int neighbor in Neighbors(node, x))
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }                        
                }                   
            }

            visited.Sort();

            return visited;
        }

        public Matrix GetMatrix()
        {
            return ma;
        }

        public void CalcAll()
        {
            edges = CountEdges();
            diameter = Diameter();
            radius = Radius();
            center = Center();
            components = Components(ma);
            articulations = Articulations(ma);
            bridges = Bridges(ma);
        }

        //-------------------------------------------------------------------ToString/Print--------------------------------------------------------------------------------------------------------------------        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (ma.GetAdjacency() != null)
            {          
                sb.AppendLine(ma.ToString());
                sb.AppendLine("Nodecount: " + ma.GetNodes().Count);
                sb.AppendLine("Edgecount: " + edges);
                sb.AppendLine("Diameter: " + diameter);
                sb.AppendLine("Radius: " + radius);
                sb.AppendLine("Center(Node): " + ToStringList(center) + "\n");
                sb.AppendLine("Components: " + "\n" + ToStringList(components));
                sb.AppendLine("Articulations: " + "\n" + ToStringList(articulations) + "\n");
                sb.AppendLine("Bridges: " + "\n" + ToStringTuple(bridges));  
                sb.AppendLine();
            }

            return sb.ToString();
        }       

        public string ToStringList(List<int> list)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i] + " ");
            }

            return sb.ToString();
        }

        public string ToStringList(List<List<int>> list)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(i + 1 + ": " + ToStringList(list[i]) + "\n");
            }

            return sb.ToString();
        }
            
        public string ToStringTuple(List<Tuple<int, int>> list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Tuple<int, int> bridge in bridges)
            {
                sb.Append(bridge.Item1 + " " + bridge.Item2 + "\n");
            }

            return sb.ToString();
        }
    }
}
