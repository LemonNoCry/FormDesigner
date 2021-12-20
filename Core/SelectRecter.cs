using System.Drawing;
using System.Windows.Forms;

namespace FormDesinger.Core
{
    public class SelectRecter
    {
        public SelectRecter(Control control, Rectangle rectangle)
        {
            Control = control;
            Rectangle = rectangle;
            ZIndex = control.Parent.Controls.GetChildIndex(control);
        }

        public Rectangle Rectangle { get; set; }
        public Rectangle? MoveHistory { get; set; }
        public Control Control { get; set; }
        public int ZIndex { get; set; }

    }
}