using System;

namespace Ivytalk.DataWindow.CustomConverter
{
    public interface ICustomConverter
    {
        object Convert(object source);

        object ReverseConvert(object source, Type tarType);
    }
}