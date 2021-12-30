using System.Windows.Forms;

namespace FormDesinger.Core
{
    public class SetControlDefault
    {
        public static void SetDefault(Control control)
        {
            if (control is Label lbl)
            {
                lbl.Text = "标签";
            }
            else if (control is Button btn)
            {
                btn.Text = "按钮";
            }
        }
    }
}