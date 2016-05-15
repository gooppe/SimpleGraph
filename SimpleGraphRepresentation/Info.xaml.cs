using SimpleGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleGraphRepresentation
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info(Graph<Graph<int>> graph)
        {
            InitializeComponent();
            GenerateResult(graph);
        }

        private void GenerateResult(Graph<Graph<int>> graph)
        {
            int id = 0;
            foreach(Graph<int> subgraph in graph.Nodes)
            {
                id++;
                string nodes = id + ") ";
                foreach(int n in subgraph.Nodes)
                {
                    nodes += n + ", ";
                }

                _Vertices.Text += nodes.Trim(' ', ',') + ".\r\n";
            }

            foreach (Link<Graph<int>> edges in graph.Edges)
            {
                _Condensations.Text += string.Format("{0} -> {1}\r\n", graph.Nodes.IndexOf(edges.Child) + 1, graph.Nodes.IndexOf(edges.Parent) + 1);
            }
        }
    }
}
