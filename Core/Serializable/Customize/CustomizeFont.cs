using System;
using System.Drawing;
using FormDesinger.Core.Serializable.CustomConverter;

namespace FormDesinger.Core.Serializable
{
    [Serializable]
    public struct CustomizeFont : ICustomConverter
    {
        public string Name { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }
        public GraphicsUnit Unit { get; set; }

        public static implicit operator Font(CustomizeFont x)
        {
            return x.MapsterCopyTo<Font>();
        }

        public static implicit operator CustomizeFont(Font c)
        {
            return c.MapsterCopyTo<CustomizeFont>();
        }

        public object Convert(object source)
        {
            if (source is Font point)
            {
                return (CustomizeFont) point;
            }

            return (CustomizeFont) new Font(FontFamily.GenericMonospace, 9);
        }

        public object ReverseConvert(object target, Type tarType)
        {
            if (tarType == typeof(Font))
            {
                return (Font) this;
            }

            return default;
        }
    }
}