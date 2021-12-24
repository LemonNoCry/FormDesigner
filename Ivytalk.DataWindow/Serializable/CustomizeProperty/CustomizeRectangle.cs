using Ivytalk.DataWindow.CustomConverter;
using System;
using System.Drawing;

namespace Ivytalk.DataWindow.Serializable.CustomizeProperty
{
    [Serializable]
    public struct CustomizeRectangle : ICustomConverter
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Left => this.X;

        public int Top => this.Y;

        public int Right => this.X + this.Width;

        public int Bottom => this.Y + this.Height;


        public static implicit operator Rectangle(CustomizeRectangle x)
        {
            return new Rectangle(x.X, x.Y, x.Width, x.Height);
        }

        public static implicit operator CustomizeRectangle(Rectangle c)
        {
            return new CustomizeRectangle() { X = c.X, Y = c.Y, Width = c.Width, Height = c.Height };
        }


        public object Convert(object source)
        {
            if (source is Rectangle point)
            {
                return (CustomizeRectangle)point;
            }

            return (CustomizeRectangle)new Rectangle();
        }

        public object ReverseConvert(object target, Type tarType)
        {
            if (tarType == typeof(Rectangle))
            {
                return (Rectangle)this;
            }

            return default;
        }
    }
}