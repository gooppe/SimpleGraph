using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SimpleGraphRepresentation.Sources
{
    public class Vertex
    {
        public readonly Grid Graphic;
        public readonly Ellipse Background;
        public readonly Label Text;
        public Point Position
        {
            get { return new Point(Graphic.Margin.Left, Graphic.Margin.Top); }
            set { SetPosition(value); }
        }
        public Point Velocity = new Point(0, 0);
        public Point NetForce = new Point();
        public bool isDragged = false;
        public Vertex(Point Location, string Content = "1", double Diameter = 24)
        {
            Background = new Ellipse()
            {
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF005E")),
                Height = Diameter,
                Width = Diameter
            };
            Text = new Label()
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                Content = Content,
                Padding = new Thickness(),
                Foreground = (SolidColorBrush)(Brushes.White),
            };
            Graphic = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(Location.X, Location.Y, 0, 0),
                Width = Diameter,
                Height = Diameter
            };
            Graphic.Children.Add(Background);
            Graphic.Children.Add(Text);
        }

        public void SetPosition(Point Position)
        {
            if (Position == null)
                throw new ArgumentNullException("Position");
            Graphic.Margin = new Thickness(Position.X, Position.Y, 0, 0);
        }
    }
}
