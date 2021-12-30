using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
{
    public class CheckBoxSerializable: ControlSerializable, IPropertyCollections<CheckBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as CheckBox);
        }

        public CustomPropertyCollection GetCollections(CheckBox control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}