using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleGraphRepresentation.Sources
{
    class Edge
    {
        public  Line Graphics;
        public  Line Arrow1;
        public  Line Arrow2;

        public readonly Vertex v1;
        public readonly Vertex v2;

        public Edge(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public void Process()
        {
            double distance = Math.Sqrt((v1.Position.X - v2.Position.X) * (v1.Position.X - v2.Position.X) + (v1.Position.Y - v2.Position.Y) * (v1.Position.Y - v2.Position.Y));
            double shiftX = v2.Graphic.Width / 2 * (v2.Position.X - v1.Position.X) / distance;
            double shiftY = v2.Graphic.Height / 2 * (v2.Position.Y - v1.Position.Y) / distance;

            Graphics = new Line()
            {
                X1 = v1.Graphic.Margin.Left + shiftX + v2.Graphic.Width / 2,
                Y1 = v1.Graphic.Margin.Top + shiftY + v2.Graphic.Height / 2,
                X2 = v2.Graphic.Margin.Left - shiftX + v2.Graphic.Width / 2,
                Y2 = v2.Graphic.Margin.Top - shiftY + v2.Graphic.Height / 2,
                StrokeThickness = 0.5,
                Stroke = Brushes.Gray
            };

            //double dx = Math.Sign(Graphics.X2 - Graphics.X1) * 12;
            //double dy = Math.Sign(Graphics.Y2 - Graphics.Y1) * 12;


            double dx = (Graphics.X2 - Graphics.X1) / distance * 16;
            double dy = (Graphics.Y2 - Graphics.Y1) / distance * 16;

            double cos = 0.9659;
            double sin = 0.2588;

            Arrow1 = new Line()
            {
                X1 = Graphics.X1,
                Y1 = Graphics.Y1,
                X2 = Graphics.X1 + dx * cos + dy * -sin,
                Y2 = Graphics.Y1 + dx * sin + dy * cos,
                StrokeThickness = 0.5,
                Stroke = Brushes.Gray,
            };

            Arrow2 = new Line()
            {
                X1 = Graphics.X1,
                Y1 = Graphics.Y1,
                X2 = Graphics.X1 + dx * cos + dy * sin,
                Y2 = Graphics.Y1 + -dx * sin + dy * cos,
                StrokeThickness = 0.5,
                Stroke = Brushes.Gray,
            };
        }
    }
}
