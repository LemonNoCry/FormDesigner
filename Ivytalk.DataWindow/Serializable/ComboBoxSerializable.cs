using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
{
    public class ComboBoxSerializable: ControlSerializable, IPropertyCollections<ComboBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as ComboBox);
        }

        public CustomPropertyCollection GetCollections(ComboBox control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}