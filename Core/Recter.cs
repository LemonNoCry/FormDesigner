using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FormDesinger.Core;

namespace FormDesinger
{
    /// <summary>
    /// 选中控件时，周围出现的方框
    /// </summary>
    public class Recter
    {
        private readonly List<SelectRecter> _selectRecters = new List<SelectRecter>();

        ///是否为窗体周围的方框（如果是，则位置不可改变，且只有从下、右、右下三个方向改变大小）
        private bool IsForm;

        public bool IsSelect => _selectRecters.Any();
        public bool IsMultipleSelect => _selectRecters.Count > 1;
        public bool IsSelectFrom => _selectRecters.Any() && IsForm;
        public int Count => _selectRecters.Count;


        /// <summary>
        /// 所有选中的控件
        /// </summary>
        public List<SelectRecter> GetSelectRects()
        {
            return _selectRecters;
        }

        public List<SelectRecter> GetSelectControlsRects()
        {
            if (IsForm) return new List<SelectRecter>();
            return _selectRecters;
        }

        public void SetSelect(Rectangle r, Control c)
        {
            ClearSelect();
            _selectRecters.Add(new SelectRecter(c, r));
            IsForm = c is Form;
        }

        public void AddSelect(Rectangle r, Control c)
        {
            _selectRecters.Add(new SelectRecter(c, r));
            IsForm = _selectRecters.Count == 0 && c is Form;
        }


        public void RemoveSelect(Rectangle r)
        {
            var sel = _selectRecters.FirstOrDefault(s => s.Rectangle.Contains(r));
            if (sel != null)
                _selectRecters.Remove(sel);
        }

        public void RemoveSelect(Control c)
        {
            var sel = _selectRecters.FirstOrDefault(s => s.Control == c);
            if (sel != null)
                _selectRecters.Remove(sel);
        }

        public void RemoveSelect(List<Rectangle> rs)
        {
            foreach (Rectangle r in rs)
            {
                RemoveSelect(r);
            }
        }

        public bool Contains(Rectangle r)
        {
            return _selectRecters.Any(s => s.Rectangle.Contains(r));
        }

        public void ClearSelect()
        {
            _selectRecters.Clear();
            IsForm = false;
        }


        /// <summary>
        /// 绘制方框
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            foreach (var sel in _selectRecters)
            {
                DrawReciter(g, sel.Rectangle);
            }
        }

        private void DrawReciter(Graphics g, Rectangle rect)
        {
            using (Pen p = new Pen(Brushes.Black, 1))
            {
                p.DashStyle = DashStyle.Dot;
                rect.Inflate(new Size(+1, +1));
                g.DrawRectangle(p, rect); //方框

                p.DashStyle = DashStyle.Solid;

                //8个方块
                if (!IsForm)
                {
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                }

                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));

                if (!IsForm)
                {
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                }

                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));
            }
        }

        /// <summary>
        /// 判断鼠标操作类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public DragType GetMouseDragType(Point p)
        {
            return _selectRecters.Select(sel => GetMouseDragType(sel.Rectangle, p))
                .FirstOrDefault(dragType => dragType != DragType.None);
        }

        public DragType GetMouseDragType(Rectangle rect, Point p)
        {
            rect.Inflate(new Size(3, 3));
            if (new Rectangle(rect.Left - 2, rect.Top - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.LeftTop;
            }

            if (new Rectangle(rect.Left + 2, rect.Top - 2, rect.Width - 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.Top;
            }

            if (new Rectangle(rect.Left - 2, rect.Top + 2, 4, rect.Height - 4).Contains(p)
                && !IsForm)
            {
                return DragType.Left;
            }

            if (new Rectangle(rect.Left - 2, rect.Top + rect.Height - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.LeftBottom;
            }

            if (new Rectangle(rect.Left + 2, rect.Top + rect.Height - 2, rect.Width - 4, 4).Contains(p))
            {
                return DragType.Bottom;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top + rect.Height - 2, 4, 4).Contains(p))
            {
                return DragType.RightBottom;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top + 2, 4, rect.Height - 4).Contains(p))
            {
                return DragType.Right;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top - 2, 4, 4).Contains(p) && !IsForm)
            {
                return DragType.RightTop;
            }

            if (new Rectangle(rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4).Contains(p) && !IsForm)
            {
                return DragType.Center;
            }

            return DragType.None;
        }

        /// <summary>
        /// 移动控件
        /// </summary>
        /// <param name="movePoint"></param>
        public void MoveRecter(Point movePoint)
        {
            foreach (var sel in _selectRecters)
            {
                if (!sel.MoveHistory.HasValue)
                {
                    sel.MoveHistory = sel.Rectangle;
                }

                sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X + movePoint.X, sel.MoveHistory.Value.Y + movePoint.Y, sel.MoveHistory.Value.Width, sel.MoveHistory.Value.Height);
            }
        }

        /// <summary>
        /// 移动控件
        /// </summary>
        /// <param name="movePoint"></param>
        public void MoveRecterEnd()
        {
            _selectRecters.ForEach(s => s.MoveHistory = null);
        }
    }
}