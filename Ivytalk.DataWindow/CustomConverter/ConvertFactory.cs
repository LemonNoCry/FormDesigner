using System;

namespace Ivytalk.DataWindow.CustomConverter
{
    public static class ConvertFactory
    {
        public static object ConvertSingleItem(this object value, Type newType)
        {
            if (value is ICustomConverter converter)
            {
                return converter.ReverseConvert(value, newType);
            }
            else
            {
                //  Add custom conversion for non IConvertible types
                converter = CustomConvertersFactory.GetConverter(newType);
                return converter.Convert(value);
            }
        }
    }
}