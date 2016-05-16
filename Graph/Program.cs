using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraph
{
    class Program
    {
        public class Test
        {
            public string data;
            public Test(string data)
            {
                this.data = data;
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Format of graph file: It have to be matrix of zeros and ones separeted by gap\r\nFor example:\r\n1 0 0 1\r\n0 1 0 0\r\n0 0 0 1\r\n0 1 1 0\r\n\r\n");
            while (true)
            {
                Console.Write("Please, enter name of the graph file: ");
                string name = Console.ReadLine();
                ImportGraph(name);
            }
        }

        static void ImportGraph(string fileName)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileName);
            }
            catch
            {
                Console.WriteLine("Error. File not founded.");
                return;
            }

            List<string> lines = new List<string>();
            string line;
            string errorParMsg = "Bad file format. It should be logical adjacent matrix.";

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            bool[,] mat = new bool[lines.Count, lines.Count];

            int i = 0;
            foreach (string ln in lines)
            {
                string[] items = ln.Split(' ');
                if (items.Length != lines.Count)
                {
                    Console.WriteLine(errorParMsg);
                    reader.Close();
                    return;
                }
                int j = 0;
                foreach (string item in items)
                {
                    try
                    {
                        mat[i, j] = Convert.ToBoolean(int.Parse(item));
                    }
                    catch
                    {
                        Console.WriteLine(errorParMsg);
                        reader.Close();
                        return;
                    }
                    j++;
                }
                i++;
            }

            Graph<int> graph = new Graph<int>();
            for (int x = 0; x < mat.GetLength(0); x++)
            {
                graph.AddNode(x);
            }
            graph.SetEdges(mat);
            Separate(graph);
        }

        static void Separate(Graph<int> graph)
        {
            Stopwatch swatch = new Stopwatch();
            swatch.Start();
            Graph<Graph<int>> part = graph.MalgrangePartion();
            swatch.Stop();
            Console.Write("\r\nElapsed time: " + swatch.Elapsed + "\r\n\n");
            PrintPartion(part);
            Console.WriteLine();
        }

        static void PrintClosure(int id, List<int> el)
        {
            string msg = id + ") ";
            foreach(int t in el)
            {
                msg += t + ", ";
            }

            Console.WriteLine(msg.Trim(' ', ',') + ".");
        }

        static void PrintMatrix(bool[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    Console.Write(Convert.ToInt16(matrix[i, j]) + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void PrintPartion(Graph<Graph<int>> subGraph)
        {
            Console.WriteLine("Elements of each subgraph:");
            int id = 0;
            foreach (Graph<int> graph in subGraph.Nodes)
            {
                id++;
                PrintClosure(id, graph.Nodes);
            }

            Console.WriteLine();
            Console.WriteLine("Condensations:");

            foreach (Link<Graph<int>> edges in subGraph.Edges)
            {
                Console.WriteLine("{0} -> {1}", subGraph.Nodes.IndexOf(edges.Child) + 1, subGraph.Nodes.IndexOf(edges.Parent) + 1);
            }
        }
    }
}
