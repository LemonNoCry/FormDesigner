using System;

namespace FormDesinger.Core.Serializable.CustomConverter
{
    public interface ICustomConverter
    {
        object Convert(object source);

        object ReverseConvert(object source, Type tarType);
    }
}