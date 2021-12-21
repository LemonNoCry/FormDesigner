using System;
using System.Drawing;
using FormDesinger.Core.Serializable.CustomConverter;

namespace FormDesinger.Core.Serializable
{
    [Serializable]
    public struct CustomizeSize : ICustomConverter
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public static implicit operator Size(CustomizeSize x)
        {
            return x.MapsterCopyTo<Size>();
        }

        public static implicit operator CustomizeSize(Size c)
        {
            return c.MapsterCopyTo<CustomizeSize>();
        }

        public object Convert(object source)
        {
            if (source is Size point)
            {
                return (CustomizeSize) point;
            }

            return (CustomizeSize) new Size();
        }

        public object ReverseConvert(object target, Type tarType)
        {
            if (tarType == typeof(Size))
            {
                return (Size) this;
            }

            return default;
        }
    }
}