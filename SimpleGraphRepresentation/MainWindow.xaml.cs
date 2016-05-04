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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using SimpleGraph;
using SimpleGraphRepresentation.Sources;
using System.Reflection;

namespace SimpleGraphRepresentation
{
    public partial class MainWindow : Window
    {
        string[] colors = { "#EE1289", "#7A67EE", "#03A89E", "#006400", "#808000", "#8B7E66", "#8B4500", "#CD3333", "#71C671", "#575757" };
        Graph<int> _Graph = new Graph<int>();
        List<Vertex> Vertexes = new List<Vertex>();
        List<Edge> Edges = new List<Edge>();
        Random Randomizer = new Random();

        Timer timer;
        Timer drawEdge;

        bool AddEnabled = false;

        int fce = -1;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(new Action(delegate ()
                {
                    Proccess();
                }));
            }
            catch { }
        }

        public void CreateVertex(Point position)
        {
            int id = _Graph.Nodes.Count == 0 ? 0 : _Graph.Nodes.Last() + 1;
            _Graph.AddNode(id);
            Vertex vertex = new Vertex(position, _Graph.Nodes.Last().ToString());

            vertex.Graphic.Tag = vertex;
            vertex.Graphic.MouseLeftButtonDown += Graphic_MouseLeftButtonDown;
            vertex.Graphic.MouseLeftButtonUp += Graphic_MouseLeftButtonUp;
            Vertexes.Add(vertex);
            DrawSurface.Children.Add(vertex.Graphic);
        }

        public void DrawGraphics()
        {
            DrawSurface.Children.Clear();
            
            foreach (Vertex v in Vertexes)
            {
                DrawSurface.Children.Add(v.Graphic);
            }
            foreach (Edge edge in Edges)
            {
                edge.Process();
                DrawSurface.Children.Add(edge.Graphics);
                DrawSurface.Children.Add(edge.Arrow1);
                DrawSurface.Children.Add(edge.Arrow2);
            }
        }

        public void CreateEdge(Vertex a, Vertex b)
        {
            _Graph.AddEdge(new Link<int>(Convert.ToInt32(a.Text.Content), Convert.ToInt32(b.Text.Content)));
            if (Edges.Exists(e => e.v1 == a && e.v2 == b))
                return;
            Edge edge = new Edge(a, b);
            Edges.Add(edge);
        }

        private void Graphic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Grid).Tag as Vertex).isDragged = false;
            //(sender as Ellipse).Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF005E"));
        }

        private void Graphic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AddEnabled)
            {
                e.Handled = true;
                
                if (fce == -1)
                {
                    fce = Convert.ToInt32(((sender as Grid).Tag as Vertex).Text.Content);

                    Line ln = new Line();
                    ln.StrokeThickness = 0.5;
                    ln.Stroke = Brushes.Green;

                    DrawSurface.Children.Add(ln);

                    drawEdge = new Timer(1);
                    drawEdge.AutoReset = true;
                    drawEdge.Elapsed += DrawEdge_Elapsed;
                    drawEdge.Start();
                    return;
                }

                int sce = Convert.ToInt32(((sender as Grid).Tag as Vertex).Text.Content);

                DrawSurface.Children.RemoveAt(DrawSurface.Children.Count - 1);
                
                if (sce != fce)
                {
                    CreateEdge(Vertexes[sce], Vertexes[fce]);
                    DrawGraphics();
                }
                fce = -1;
            }
            else
            {
                ((sender as Grid).Tag as Vertex).isDragged = true;
            }
        }

        private void DrawEdge_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(new Action(delegate ()
                {
                    CanvasAddEdge();
                }));
            }
            catch { }
        }

        private void CanvasAddEdge()
        {
            if (fce == -1)
            {
                drawEdge.Stop();
                return;
            }

            Line ln = DrawSurface.Children[DrawSurface.Children.Count - 1] as Line;
            ln.X1 = Vertexes[fce].Position.X + 12;
            ln.Y1 = Vertexes[fce].Position.Y + 12;
            ln.X2 = Mouse.GetPosition(DrawSurface).X - 1;
            ln.Y2 = Mouse.GetPosition(DrawSurface).Y - 1;
        }

        public void Proccess()
        {
            bool[,] edges = _Graph.GetAdjacenyMatrix();
            for (int i = 0; i < Vertexes.Count; i++)
            {
                Vertex v = Vertexes[i];
                Vertex u;
                v.NetForce.X = v.NetForce.Y = 0;
                for (int j = 0; j < Vertexes.Count; j++)
                {
                    if (i == j)
                        continue;
                    u = Vertexes[j];
                    double distance = (v.Position.X - u.Position.X) * (v.Position.X - u.Position.X) + (v.Position.Y - u.Position.Y) * (v.Position.Y - u.Position.Y);
                    v.NetForce.X += 100 * (v.Position.X - u.Position.X) / distance;
                    v.NetForce.Y += 100 * (v.Position.Y - u.Position.Y) / distance;
                }
                for (int j = 0; j < Vertexes.Count; j++)
                {
                    if (!edges[i, j] && !edges[j,i])
                        continue;
                    u = Vertexes[j];
                    v.NetForce.X += 0.06 * (u.Position.X - v.Position.X);
                    v.NetForce.Y += 0.06 * (u.Position.Y - v.Position.Y);
                }
                v.Velocity.X = (v.Velocity.X + v.NetForce.X) * 0.15;
                v.Velocity.Y = (v.Velocity.Y + v.NetForce.Y) * 0.15;
            }

            foreach(Vertex v in Vertexes)
            {
                if (v.isDragged)
                {
                    Point p = Mouse.GetPosition(DrawSurface);
                    p.X -= 9;
                    p.Y -= 9;
                    v.Position = p;
                }
                else
                {
                    Point pos = new Point(v.Position.X + v.Velocity.X, v.Position.Y + v.Velocity.Y);
                    v.Position = pos;
                }
            }
            DrawGraphics();
        }

        private void SepButton_Click(object sender, RoutedEventArgs e)
        {
            Graph<Graph<int>> part = _Graph.MalgrangePartion();
            foreach(Graph<int> node in part.Nodes)
            {
                Brush result = Brushes.Transparent;
                result = (Brush)new BrushConverter().ConvertFrom(colors[Randomizer.Next(0, 9)]);

                foreach (int id in node.Nodes)
                {
                    Vertexes[id].Background.Fill = result;
                }
            }
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            timer = new Timer(1);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddEnabled)
            {
                AddButton.Foreground = Brushes.Black;
                SepButton.IsEnabled = true;
                SimButton.IsEnabled = true;
            }
            else
            {
                AddButton.Foreground = Brushes.DarkGreen;
                SepButton.IsEnabled = false;
                SimButton.IsEnabled = false;
            }
            AddEnabled = !AddEnabled;
        }

        private void DrawSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AddEnabled)
                return;
            if (fce != -1)
            {
                fce = -1;
                DrawSurface.Children.RemoveAt(DrawSurface.Children.Count - 1);
                return;
            }
            Point pos = new Point(Mouse.GetPosition(DrawSurface).X - 8, Mouse.GetPosition(DrawSurface).Y - 8);
            CreateVertex(pos);
        }
    }
}
