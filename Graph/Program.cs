using System;
using System.Collections.Generic;
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
            Test e1 = new Test("g1");
            Test e2 = new Test("g2");
            Test e3 = new Test("g3");
            Test e4 = new Test("g4");
            Test e5 = new Test("g5");
            Test e6 = new Test("g6");
            Test e7 = new Test("g7");
            Test e8 = new Test("g8");
            Test e9 = new Test("g9");
            Test e10 = new Test("g10");
            Test e11 = new Test("g11");

            List<Test> nodes = new List<Test>(new Test[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 });


             bool[,] edges = new bool[11, 11]
             {
                 { false, false, false, false, false, false, true, false, false, false, false },
                 { true, true, false, false, false, false, false, true, false, false, true },
                 { false, false, true, true, false, false, false, false, true, true, false },
                 { false, false, false, true, true, false, false, false, false, false, false },
                 { false, false, false, true, false, false, false, false, false, false, false },
                 { false, false, false, false, false, true, false, true, false, false, false },
                 { false, false, false, false, false, false, false, false, false, false, true },
                 { false, false, false, false, false, false, false, false, false, false, false },
                 { false, false, true, false, false, false, false, false, false, false, false },
                 { false, false, false, true, true, false, false, false, true, false, false },
                 { true, false, false, false, true, true, false, false, false, false, true }
             };
            
            Graph<Test> graph = new Graph<Test>(nodes, new List<Link<Test>>());
            graph.SetEdges(edges);
            PrintMatrix(graph.GetAdjacenyMatrix());
            PrintPartion(graph.MalgrangePartion());
            Console.ReadKey();
        }

        static void PrintClosure(List<Test> el)
        {
            foreach(Test t in el)
            {
                Console.Write(t.data + " ");
            }
            Console.WriteLine();
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

        static void PrintPartion(Graph<Graph<Test>> subGraph)
        {
            foreach (Graph<Test> graph in subGraph.Nodes)
            {
                PrintClosure(graph.Nodes);
            }

            Console.WriteLine();

            foreach (Link<Graph<Test>> edges in subGraph.Edges)
            {
                Console.WriteLine("{0} -> {1}", subGraph.Nodes.IndexOf(edges.Parent) + 1, subGraph.Nodes.IndexOf(edges.Child) + 1);
            }
        }
    }
}
