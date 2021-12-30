using System.Drawing;
using System.Windows.Forms;

namespace Ivytalk.DataWindow.Core.OperationControl
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
        public Control Parent { get; set; }
        public int ZIndex { get; set; }

        /// <summary>
        /// 控件顺序 更复杂的Control.ZIndex
        /// </summary>
        public int SpaceIndex { get; set; }

        public bool IsMoveParent { get; set; }

        public void ClearHistory()
        {
            MoveHistory = null;
            Parent = null;
        }
    }
}