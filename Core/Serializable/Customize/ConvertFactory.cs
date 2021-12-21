using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FormDesinger.Core.Serializable.CustomConverter;

namespace FormDesinger.Core.Serializable
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