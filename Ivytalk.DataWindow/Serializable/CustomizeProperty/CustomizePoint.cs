using Ivytalk.DataWindow.CustomConverter;
using System;
using System.Drawing;

namespace Ivytalk.DataWindow.Serializable.CustomizeProperty
{
    [Serializable]
    public struct CustomizePoint : ICustomConverter
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static implicit operator Point(CustomizePoint x)
        {
            return new Point(x.X, x.Y);
        }

        public static implicit operator CustomizePoint(Point c)
        {
            return new CustomizePoint() { X = c.X, Y = c.Y };
        }

        public object Convert(object source)
        {
            if (source is Point point)
            {
                return (CustomizePoint)point;
            }

            return (CustomizePoint)new Point();
        }

        public object ReverseConvert(object target, Type tarType)
        {
            if (tarType == typeof(Point))
            {
                return (Point)this;
            }

            return default;
        }
    }
}