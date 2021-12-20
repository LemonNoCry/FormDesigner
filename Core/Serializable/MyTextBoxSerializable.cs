using System.Windows.Forms;
using FormDesinger.UserControls;

namespace FormDesinger.Core.Serializable
{
    public class MyTextBoxSerializable : ControlSerializable, IPropertyCollections<MyTextBox>
    {
        public bool Multiline { get; set; }

        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as MyTextBox);
        }

        public CustomPropertyCollection GetCollections(MyTextBox control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("多行显示", "Multiline", "行为", "Multiline 控件的文本是否能跨越多行。", control));

            return cpc;
        }
    }
}