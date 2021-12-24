using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ivytalk.DataWindow.Core.OperationControl
{
    /// <summary>
    /// 鼠标左键按下拖动时,出现的选中区域方框
    /// </summary>
    public class SelectRectangle
    {
        public Rectangle Rect { get; set; } = new Rectangle();

        /// <summary>
        /// 绘制方框
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            Rectangle rect = Rect;
            using (Pen p = new Pen(Brushes.Black, 1))
            {
                p.DashStyle = DashStyle.Dot;

                g.DrawRectangle(p, rect); //方框

                g.FillRectangle(new SolidBrush(Color.FromArgb(100, 151, 209, 255)), new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1));

                g.DrawRectangle(p, rect);
            }
        }
    }
}