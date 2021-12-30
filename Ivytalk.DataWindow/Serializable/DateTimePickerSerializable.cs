using System;
using Ivytalk.DataWindow.CustomPropertys;
using System.Windows.Forms;

namespace Ivytalk.DataWindow.Serializable
{
    [Serializable]
    public class DateTimePickerSerializable : ControlSerializable, IPropertyCollections<DateTimePicker>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as DateTimePicker);
        }

        public CustomPropertyCollection GetCollections(DateTimePicker control)
        {
            var cpc = base.GetCollections(control);

            if (control.Format != DateTimePickerFormat.Custom)
            {
                //必须定义为Custom，CustomFormat才有效
                control.Format = DateTimePickerFormat.Custom;
                control.CustomFormat = "";
            }

            cpc.Add(new CustomProperty("格式化", "CustomFormat", "行为", "CustomFormat 用于格式化在控件中显示的日期/或时间的自定义字符串。", control));

            return cpc;
        }
    }
}