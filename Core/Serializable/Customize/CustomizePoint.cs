using System.Drawing;
using Mapster;

namespace FormDesinger.Core.Serializable
{
    public struct CustomizePoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static implicit operator Point(CustomizePoint x)
        {
            return new Point(x.X, x.Y);
        }

        public static implicit operator CustomizePoint(Point c)
        {
            return new CustomizePoint() {X = c.X, Y = c.Y};
        }
    }
}