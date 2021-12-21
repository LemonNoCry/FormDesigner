using System;
using System.Windows.Forms;
using FormDesinger.Core.Serializable.CustomConverter;

namespace FormDesinger.Core.Serializable
{
    [Serializable]
    public struct CustomizePadding : ICustomConverter
    {
        public int Top { get; set; }

        public int Bottom { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }


        public static implicit operator Padding(CustomizePadding x)
        {
            return x.MapsterCopyTo<Padding>();
        }

        public static implicit operator CustomizePadding(Padding c)
        {
            return c.MapsterCopyTo<CustomizePadding>();
        }

        public object Convert(object source)
        {
            if (source is Padding point)
            {
                return (CustomizePadding) point;
            }

            return (CustomizePadding) new Padding();
        }

        public object ReverseConvert(object target, Type tarType)
        {
            if (tarType == typeof(Padding))
            {
                return (Padding) this;
            }

            return default;
        }
    }
}