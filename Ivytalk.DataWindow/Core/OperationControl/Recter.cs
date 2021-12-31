using System;
using Ivytalk.DataWindow.Core.OperationControl.History;
using Ivytalk.DataWindow.DesignLayer;
using Ivytalk.DataWindow.Serializable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;
using Ivytalk.DataWindow.Utility;

namespace Ivytalk.DataWindow.Core.OperationControl
{
    /// <summary>
    /// 选中控件时，周围出现的方框
    /// </summary>
    public class Recter
    {
        public Recter(Overlayer overlayer)
        {
            this.Overlayer = overlayer;
        }

        public Overlayer Overlayer;
        private readonly List<SelectRecter> _selectRecters = new List<SelectRecter>();

        public delegate void SelectControlChangedHandler(object sender, Recter selectRecter);

        public event SelectControlChangedHandler SelectControlChanged;

        ///是否为窗体周围的方框（如果是，则位置不可改变，且只有从下、右、右下三个方向改变大小）
        private bool IsForm;

        public bool IsSelect => _selectRecters.Any();
        public bool IsMultipleSelect => _selectRecters.Count > 1;
        public bool IsSelectFrom => _selectRecters.Any() && IsForm;
        public int Count => _selectRecters.Count;

        public bool IsMoving => _selectRecters
            .Any(s => s.MoveHistory.HasValue && s.MoveHistory != s.Rectangle);

        /// <summary>
        /// 所有选中的控件
        /// </summary>
        public List<SelectRecter> GetSelectRects()
        {
            return _selectRecters;
        }

        public List<Control> GetSelectControls()
        {
            return _selectRecters.Select(s => s.Control).ToList();
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
            SelectControlChanged?.Invoke(Overlayer, this);
        }

        public void AddSelect(Rectangle r, Control c)
        {
            _selectRecters.Add(new SelectRecter(c, r));
            IsForm = _selectRecters.Count == 0 && c is Form;
            SelectControlChanged?.Invoke(Overlayer, this);
        }


        public void RemoveSelect(Rectangle r)
        {
            var sel = _selectRecters.FirstOrDefault(s => s.Rectangle.Contains(r));
            if (sel == null) return;

            _selectRecters.Remove(sel);
            SelectControlChanged?.Invoke(Overlayer, this);
        }

        public void RemoveSelect(Control c)
        {
            var sel = _selectRecters.FirstOrDefault(s => s.Control == c);
            if (sel == null) return;

            _selectRecters.Remove(sel);
            SelectControlChanged?.Invoke(Overlayer, this);
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
            SelectControlChanged?.Invoke(Overlayer, this);
        }


        /// <summary>
        /// 绘制方框
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            foreach (var sel in _selectRecters)
            {
                DrawReciter(g, sel);
            }
        }

        private void DrawReciter(Graphics g, SelectRecter sel)
        {
            using (Pen p = new Pen(Brushes.Black, 1))
            {
                p.DashStyle = DashStyle.Dot;
                var rect = sel.Rectangle;

                rect.Inflate(new Size(+1, +1));
                g.DrawRectangle(p, rect); //方框

                p.DashStyle = DashStyle.Solid;

                //8个方块
                foreach (var value in Enum.GetValues(typeof(DragType)))
                {
                    DrawOperRecter((DragType) value, g, sel, p);
                }
                //if (!IsForm)
                //{
                //    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                //    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                //    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                //    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                //    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                //}

                //g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                //g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                //g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));

                //if (!IsForm)
                //{
                //    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                //    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                //    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                //    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                //    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                //}

                //g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                //g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                //g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));
            }
        }

        #region 操作框

        private void DrawOperRecter(DragType type, Graphics g, SelectRecter sel, Pen p)
        {
            var rect = sel.Rectangle;
            if (!CheckOperDragType(type, sel))
            {
                return;
            }

            switch (type)
            {
                case DragType.Left:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height / 2 - 3, 6, 6));
                    break;
                case DragType.LeftTop:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top - 6, 6, 6));
                    break;
                case DragType.LeftBottom:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left - 6, rect.Top + rect.Height, 6, 6));
                    break;
                case DragType.Top:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 6, 6, 6));
                    break;
                case DragType.Right:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 6, 6));
                    break;
                case DragType.RightTop:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top - 6, 6, 6));
                    break;
                case DragType.RightBottom:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 6, 6));
                    break;
                case DragType.Bottom:
                    g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                    g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 6, 6));
                    break;
            }
        }

        /// <summary>
        /// 判断鼠标操作类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public DragType GetMouseDragType(Point p)
        {
            return _selectRecters.Select(sel => GetMouseDragType(sel, p))
                .FirstOrDefault(dragType => dragType != DragType.None);
        }

        public DragType GetMouseDragType(SelectRecter sel, Point p)
        {
            var rect = sel.Rectangle;
            rect.Inflate(new Size(3, 3));
            if (new Rectangle(rect.Left - 2, rect.Top - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                if (!CheckOperDragType(DragType.LeftTop, sel))
                {
                    return DragType.None;
                }

                return DragType.LeftTop;
            }

            if (new Rectangle(rect.Left + 2, rect.Top - 2, rect.Width - 4, 4).Contains(p)
                && !IsForm)
            {
                if (!CheckOperDragType(DragType.Top, sel))
                {
                    return DragType.None;
                }

                return DragType.Top;
            }

            if (new Rectangle(rect.Left - 2, rect.Top + 2, 4, rect.Height - 4).Contains(p)
                && !IsForm)
            {
                if (!CheckOperDragType(DragType.Left, sel))
                {
                    return DragType.None;
                }

                return DragType.Left;
            }

            if (new Rectangle(rect.Left - 2, rect.Top + rect.Height - 2, 4, 4).Contains(p)
                && !IsForm)
            {
                if (!CheckOperDragType(DragType.LeftBottom, sel))
                {
                    return DragType.None;
                }

                return DragType.LeftBottom;
            }

            if (new Rectangle(rect.Left + 2, rect.Top + rect.Height - 2, rect.Width - 4, 4).Contains(p))
            {
                if (!CheckOperDragType(DragType.Bottom, sel))
                {
                    return DragType.None;
                }

                return DragType.Bottom;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top + rect.Height - 2, 4, 4).Contains(p))
            {
                if (!CheckOperDragType(DragType.RightBottom, sel))
                {
                    return DragType.None;
                }

                return DragType.RightBottom;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top + 2, 4, rect.Height - 4).Contains(p))
            {
                if (!CheckOperDragType(DragType.Right, sel))
                {
                    return DragType.None;
                }

                return DragType.Right;
            }

            if (new Rectangle(rect.Left + rect.Width - 2, rect.Top - 2, 4, 4).Contains(p) && !IsForm)
            {
                if (!CheckOperDragType(DragType.RightTop, sel))
                {
                    return DragType.None;
                }

                return DragType.RightTop;
            }

            if (new Rectangle(rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4).Contains(p) && !IsForm)
            {
                if (!CheckOperDragType(DragType.Center, sel) || sel.Control.Dock != DockStyle.None)
                {
                    return DragType.None;
                }

                return DragType.Center;
            }

            return DragType.None;
        }

        public bool CheckOperDragType(DragType dragType, SelectRecter sel)
        {
            if (sel.Control.Dock == DockStyle.Fill)
            {
                return false;
            }

            switch (dragType)
            {
                case DragType.Left:
                    if (IsForm)
                    {
                        return false;
                    }

                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Bottom}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.LeftTop:
                    if (IsForm)
                    {
                        return false;
                    }

                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.LeftBottom:
                    if (IsForm)
                    {
                        return false;
                    }

                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.Top:
                    if (IsForm)
                    {
                        return false;
                    }

                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.Right:
                    if (new[] {DockStyle.Top, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }


                    break;
                case DragType.RightTop:
                    if (IsForm)
                    {
                        return false;
                    }

                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.RightBottom:
                    if (new[] {DockStyle.Left, DockStyle.Top, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
                case DragType.Bottom:
                    if (new[] {DockStyle.Left, DockStyle.Bottom, DockStyle.Right}.Contains(sel.Control.Dock))
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }

        #endregion


        public void RefreshRecterRectangle()
        {
            for (var index = _selectRecters.Count - 1; index >= 0; index--)
            {
                var selectRecter = _selectRecters[index];
                if (selectRecter.Control.Parent == null)
                {
                    _selectRecters.Remove(selectRecter);
                    continue;
                }

                var r = Overlayer.HostToOverlayerRectangle(selectRecter.Control);
                selectRecter.Rectangle = r;
            }
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
        /// 改变控件大小
        /// </summary>
        /// <param name="movePoint"></param>
        public void ModifySizeRecter(DragType dragType, Point movePoint)
        {
            foreach (var sel in _selectRecters)
            {
                if (!sel.MoveHistory.HasValue)
                {
                    sel.MoveHistory = sel.Rectangle;
                }

                switch (dragType)
                {
                    case DragType.Left:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X + movePoint.X, sel.MoveHistory.Value.Y, sel.MoveHistory.Value.Width + movePoint.X * (-1), sel.MoveHistory.Value.Height);
                        break;
                    case DragType.Top:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y + movePoint.Y, sel.MoveHistory.Value.Width, sel.MoveHistory.Value.Height + movePoint.Y * (-1));
                        break;
                    case DragType.Right:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y, sel.MoveHistory.Value.Width + movePoint.X, sel.MoveHistory.Value.Height);
                        break;
                    case DragType.Bottom:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y, sel.MoveHistory.Value.Width, sel.MoveHistory.Value.Height + movePoint.Y);
                        break;
                    case DragType.LeftTop:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X + movePoint.X, sel.MoveHistory.Value.Y + movePoint.Y, sel.MoveHistory.Value.Width + movePoint.X * (-1), sel.MoveHistory.Value.Height + movePoint.Y * (-1));
                        break;
                    case DragType.RightTop:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y + movePoint.Y, sel.MoveHistory.Value.Width + movePoint.X, sel.MoveHistory.Value.Height + movePoint.Y * (-1));
                        break;
                    case DragType.LeftBottom:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X + movePoint.X, sel.MoveHistory.Value.Y, sel.MoveHistory.Value.Width + movePoint.X * (-1), sel.MoveHistory.Value.Height + movePoint.Y);
                        break;
                    case DragType.RightBottom:
                        sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y, sel.MoveHistory.Value.Width + movePoint.X, sel.MoveHistory.Value.Height + movePoint.Y);
                        break;
                }
            }
        }

        /// <summary>
        /// 结束控件操动
        /// </summary>
        /// <param name="dragType"></param>
        /// <param name="history"></param>
        public void ModifyRecterEnd(DragType dragType, OperationControlHistory history)
        {
            switch (dragType)
            {
                case DragType.None:
                    break;
                case DragType.Center:
                    MoveRecterEnd(history);
                    break;
                default:
                    ModifySizeRecterEnd(history);
                    break;
            }
        }

        public void MoveRecterEnd(OperationControlHistory history)
        {
            var cons = new List<Control>();
            var controlSerializables = new List<ControlSerializable>();
            var ot = OperationControlType.Move;
            foreach (var s in _selectRecters)
            {
                if (s.MoveHistory.HasValue)
                {
                    cons.Add(s.Control);

                    var cs = s.Control.MapsterCopyTo<ControlSerializable>();

                    var r = s.MoveHistory.Value;
                    r = Overlayer.RectangleToScreen(r);
                    if (s.Parent != null)
                    {
                        r = s.Parent.RectangleToClient(r);
                    }
                    else
                    {
                        r = s.Control.Parent.RectangleToClient(r);
                    }

                    var point = new Point(r.X, r.Y);
                    if (point == cs.Location)
                    {
                        continue;
                    }

                    cs.Location = point;
                    if (s.Parent != null)
                    {
                        cs.ParentSerializable = s.Parent.ControlConvertSerializable();
                        ot = OperationControlType.MoveParent;
                    }

                    controlSerializables.Add(cs);
                }

                s.ClearHistory();
            }

            if (controlSerializables.Count > 0)
            {
                history.Push(new OperationControlRecord(Overlayer, ot, cons, controlSerializables));
            }
        }

        public void ModifySizeRecterEnd(OperationControlHistory history)
        {
            List<Control> cons = new List<Control>();
            List<ControlSerializable> controlSerializables = new List<ControlSerializable>();
            _selectRecters.ForEach(s =>
            {
                if (s.MoveHistory.HasValue)
                {
                    cons.Add(s.Control);

                    var cs = s.Control.MapsterCopyTo<ControlSerializable>();

                    var r = s.MoveHistory.Value;
                    r = Overlayer.RectangleToScreen(r);
                    r = s.Control.Parent.RectangleToClient(r);

                    if (r == cs.ClientRectangle)
                    {
                        return;
                    }

                    cs.ClientRectangle = r;
                    controlSerializables.Add(cs);
                }

                s.ClearHistory();
            });

            if (controlSerializables.Count > 0)
            {
                history.Push(new OperationControlRecord(Overlayer, OperationControlType.ModifySize, cons, controlSerializables));
            }
        }

        private PropertyDescriptor[] GetMergeProperty(PropertyValueChangedEventArgs e)
        {
            Type propertyType = e.ChangedItem.PropertyDescriptor.GetType();
            FieldInfo fieldInfo = propertyType.GetField(
                "descriptors",
                BindingFlags.NonPublic
                | BindingFlags.Instance
            );
            PropertyDescriptor[] descriptors =
                (PropertyDescriptor[]) (fieldInfo.GetValue(e.ChangedItem.PropertyDescriptor));
            return descriptors;
        }

        public void ModifyPropertyRecter(PropertyValueChangedEventArgs e, OperationControlHistory history)
        {
            CustomProperty cp;
            if (e.ChangedItem.PropertyDescriptor is CustomPropertyDescriptor cpd)
            {
                cp = cpd.CustomProperty;
            }
            else
            {
                var mps = GetMergeProperty(e);
                cpd = (CustomPropertyDescriptor) mps[0];
                cp = cpd.CustomProperty;
            }

            List<Control> cons = new List<Control>();
            List<ControlSerializable> controlSerializables = new List<ControlSerializable>();
            foreach (var sel in _selectRecters)
            {
                cons.Add(sel.Control);
                var cs = Collections.ControlConvertSerializable(sel.Control);
                var csType = cs.GetType();
                foreach (var name in cp.PropertyNames)
                {
                    var pro = csType.GetProperty(name);
                    if (pro != null)
                    {
                        pro.SetValue(cs, e.OldValue, null);
                    }
                }

                controlSerializables.Add(cs);
            }

            history.Push(new OperationControlRecord(Overlayer, OperationControlType.ModifyProperty, cons, controlSerializables));
        }

        public void DeleteRecter(OperationControlHistory history)
        {
            if (_selectRecters
                .Where(s => Overlayer.BaseDataWindow.IsProhibitEditControl(s.Control) || Overlayer.BaseDataWindow.IsMustControl(s.Control))
                .Any())
            {
                MessageBox.Show("控件必须存在，禁止删除", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<ControlSerializable> css = new List<ControlSerializable>();
            foreach (var sel in _selectRecters)
            {
                var cs = sel.Control.ControlConvertSerializable();
                cs.ParentSerializable = sel.Control.Parent.ControlConvertSerializable();
                css.Add(cs);
                sel.Control.Parent.Controls.Remove(sel.Control);
            }

            history.Push(new OperationControlRecord(Overlayer, OperationControlType.Delete, _selectRecters.Select(s => s.Control).ToList(), css));
        }
    }
}