using System.Windows.Forms;
using Ivytalk.DataWindow.Serializable;
using Mapster;

namespace Ivytalk.DataWindow.Core
{
    public static class CollectionExpend
    {
        /// <summary>
        /// 控件转换 ControlSerializable
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ControlSerializable ControlConvertSerializable(this Control control)
        {
            return Collections.ControlConvertSerializable(control);
        }

        public static ControlSerializable ControlConvertAllSerializable(this Control control)
        {
            return DataWindowAnalysis.GetSerializationControls(control);
        }

        public static void ControlSerializableToControl(this ControlSerializable cs, Control control)
        {
            dynamic dyn = cs;
            TypeAdapter.Adapt(dyn, control);
        }
    }
}