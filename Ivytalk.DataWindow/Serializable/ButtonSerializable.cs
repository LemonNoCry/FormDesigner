using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
{
    public class ButtonSerializable: ControlSerializable, IPropertyCollections<Button>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Button);
        }

        public CustomPropertyCollection GetCollections(Button control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}