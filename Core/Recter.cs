using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace FormDesinger
{
    /// <summary>
    /// 选中控件时，周围出现的方框
    /// </summary>
    class Recter
    {
        private readonly List<SelectRecter> _selectRecters = new List<SelectRecter>();

        ///是否为窗体周围的方框（如果是，则位置不可改变，且只有从下、右、右下三个方向改变大小）
        public bool IsForm;

        public bool IsSelect => _selectRecters.Any();

        #region 多选

        /// <summary>
        /// 所有选中的控件
        /// </summary>
        public List<Rectangle> GetRects()
        {
            return _selectRecters.Select(s => s.Rectangle).ToList();
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
            IsForm = _selectRecters.Count > 1;
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

        #endregion

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
            Rectangle _rect = this._selectRecters[0].Rectangle;
            _rect.Inflate(new Size(3, 3));
            if (new Rectangle(_rect.Left - 2, _rect.Top - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.LeftTop;
            }

            if (new Rectangle(_rect.Left + 2, _rect.Top - 2, _rect.Width - 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.Top;
            }

            if (new Rectangle(_rect.Left - 2, _rect.Top + 2, 4, _rect.Height - 4).Contains(p)
                && !IsForm)
            {
                return DragType.Left;
            }

            if (new Rectangle(_rect.Left - 2, _rect.Top + _rect.Height - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                return DragType.LeftBottom;
            }

            if (new Rectangle(_rect.Left + 2, _rect.Top + _rect.Height - 2, _rect.Width - 4, 4).Contains(p))
            {
                return DragType.Bottom;
            }

            if (new Rectangle(_rect.Left + _rect.Width - 2, _rect.Top + _rect.Height - 2, 4, 4).Contains(p))
            {
                return DragType.RightBottom;
            }

            if (new Rectangle(_rect.Left + _rect.Width - 2, _rect.Top + 2, 4, _rect.Height - 4).Contains(p))
            {
                return DragType.Right;
            }

            if (new Rectangle(_rect.Left + _rect.Width - 2, _rect.Top - 2, 4, 4).Contains(p) && !IsForm)
            {
                return DragType.RightTop;
            }

            if (new Rectangle(_rect.Left + 2, _rect.Top + 2, _rect.Width - 4, _rect.Height - 4).Contains(p) && !IsForm)
            {
                return DragType.Center;
            }

            return DragType.None;
        }
    }

    class SelectRecter
    {
        public SelectRecter(Control control, Rectangle rectangle)
        {
            Control = control;
            Rectangle = rectangle;
        }

        public Rectangle Rectangle { get; set; }
        public Control Control { get; set; }
    }

    /// <summary>
    /// 鼠标操作类型
    /// </summary>
    enum DragType
    {
        None,
        Left,
        Top,
        Right,
        Bottom,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Center
    }
}