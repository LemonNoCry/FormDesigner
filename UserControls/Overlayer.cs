using FormDesinger.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FormDesinger
{
    /// <summary>
    /// 可设计层
    /// </summary>
    public partial class Overlayer : UserControl
    {
        public Overlayer(HostFrame host)
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            //StartListen();

            recter = new Recter(this);

            hostFrame = host; //默认被操作的是控件容器
            SelectHost();

            InitializeComponent();
        }

        /// <summary>
        /// 被遮罩的控件容器，通过Overlayer操作该容器（以及其中的子控件）
        /// </summary>
        public HostFrame hostFrame;

        /// <summary>
        /// 被操作控件（容器）周围的方框
        /// <para>选中效果</para>
        /// </summary>
        public Recter recter;

        /// <summary>
        /// 鼠标选中区域周围的方框
        /// <para>鼠标多选时方框</para>
        /// </summary>
        public SelectRectangle selectRectangle = null;

        public readonly OperationControlHistory OperationControlHistory = new OperationControlHistory();

        Point _firstSelectPoint = new Point(); //鼠标移动前的第一个位置
        bool _selectMouseDown = false;         //鼠标是否按下并选择区域

        /// <summary>
        /// 选中一个控件拖动时的初始位置
        /// </summary>
        Point _firstPoint = new Point();

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        bool _mouseDown = false;

        /// <summary>
        /// 鼠标操作类型
        /// </summary>
        DragType _dragType = DragType.None;

        /// <summary>
        /// 当前设计器中的所有控件
        /// </summary>
        public new ControlCollection Controls => hostFrame.Controls;

        private PropertyGrid _propertyGrid;

        public PropertyGrid PropertyGrid
        {
            get => _propertyGrid;
            set
            {
                _propertyGrid = value;
                _propertyGrid.PropertyValueChanged += _propertyGrid_PropertyValueChanged;
            }
        }

        public void FlushSelectProperty()
        {
            if (_propertyGrid == null) return;
            _propertyGrid.SelectedObjects = Collections.GetCollections(this);
        }


        private void _propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //recter.ModifyPropertyRecter(e, OperationControlHistory);
            RefreshMoveControls();
        }

        private void SelectHost()
        {
            Rectangle r = hostFrame.Bounds;
            r = hostFrame.Parent.RectangleToScreen(r);
            r = this.RectangleToClient(r);
            recter.SetSelect(r, hostFrame);

            FlushSelectProperty();
        }

        private void PushOperationHistory(OperationControlType operation)
        {
            var record = new OperationControlRecord(this, operation,
                recter.GetSelectRects().Select(s => s.Control).ToList());
            OperationControlHistory.Push(record);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams para = base.CreateParams;
                //para.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 透明支持
                para.ExStyle |= 0x02000000;
                return para;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) //不画背景
        {
            base.OnPaintBackground(e);
        }

        private void DrawBackHome(Graphics g)
        {
            using (Bitmap hotBit = new Bitmap(this.Width, this.Height))
            {
                hostFrame.DrawToBitmap(hotBit, new Rectangle(0, 0, hostFrame.Width, hostFrame.Height));


                g.DrawImage(hotBit, 10, 10);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.High;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            base.OnPaint(e);
            DrawBackHome(e.Graphics);

            recter?.Draw(e.Graphics);          //绘制被操作控件周围的方框
            selectRectangle?.Draw(e.Graphics); //绘制被操作控件周围的方框
        }

        #region 控件通用操作

        public Control FindControl(string name)
        {
            return FindControl(name, hostFrame);
        }

        public Control FindControl(string name, Control con)
        {
            if (con.Name == name)
            {
                return con;
            }

            foreach (Control c in con.Controls)
            {
                if (c.Name == name)
                {
                    return c;
                }

                if (c.HasChildren)
                {
                    var fc = FindControl(name, c);
                    if (fc != null)
                    {
                        return fc;
                    }
                }
            }

            return null;
        }

        #endregion

        #region 按键钩子

        private KeyEventHandler _keyEventHandler = null; //按键钩子
        private readonly KeyboardHook _kHook = new KeyboardHook();

        public void StartListen()
        {
            _keyEventHandler = hook_KeyDown;
            _kHook.KeyDownEvent += _keyEventHandler; //钩住键按下
            _kHook.Start();                          //安装键盘钩子
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //  这里写具体实现
            if (ModifierKeys == Keys.Control && e.KeyCode == Keys.Z)
            {
                Console.WriteLine("cz");
            }

            Console.WriteLine(e.KeyCode);
        }

        public void StopListen()
        {
            if (_keyEventHandler != null)
            {
                _kHook.KeyDownEvent -= _keyEventHandler; //取消按键事件
                _keyEventHandler = null;
                _kHook.Stop(); //关闭键盘钩子
            }
        }

        #endregion

        #region 代理所有用户操作

        private Rectangle HostToOverlayerRectangle(Control con)
        {
            Rectangle r = con.Bounds;
            r = (con.Parent ?? hostFrame).RectangleToScreen(r);
            r = this.RectangleToClient(r);
            return r;
        }

        private Rectangle OverlayerToHostRectangle(Control con)
        {
            Rectangle r = con.Bounds;
            r = this.RectangleToClient(r);
            r = con.Parent.RectangleToScreen(r);
            return r;
        }

        private void RefreshControlMoveDragType(MouseEventArgs e)
        {
            DragType dt = recter.GetMouseDragType(e.Location);
            switch (dt)
            {
                case DragType.Top:
                {
                    Cursor = Cursors.SizeNS;
                    break;
                }
                case DragType.RightTop:
                {
                    Cursor = Cursors.SizeNESW;
                    break;
                }
                case DragType.RightBottom:
                {
                    Cursor = Cursors.SizeNWSE;
                    break;
                }
                case DragType.Right:
                {
                    Cursor = Cursors.SizeWE;
                    break;
                }
                case DragType.LeftTop:
                {
                    Cursor = Cursors.SizeNWSE;
                    break;
                }
                case DragType.LeftBottom:
                {
                    Cursor = Cursors.SizeNESW;
                    break;
                }
                case DragType.Left:
                {
                    Cursor = Cursors.SizeWE;
                    break;
                }
                case DragType.Center:
                {
                    Cursor = Cursors.SizeAll;
                    break;
                }
                case DragType.Bottom:
                {
                    Cursor = Cursors.SizeNS;
                    break;
                }
                default:
                {
                    Cursor = Cursors.Default;
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                //鼠标移过形状改变
                RefreshControlMoveDragType(e);
            }
            else
            {
                if (_dragType == DragType.Center)
                {
                    Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                    recter.MoveRecter(delta);
                }
                else if (_dragType != DragType.None)
                {
                    //拖动
                    Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                    recter.ModifySizeRecter(_dragType, delta);
                }

                #region 拖动

                //switch (_dragType) //改变方框位置大小
                //{
                //    case DragType.Top:
                //    {
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            if (!sel.MoveHistory.HasValue)
                //            {
                //                sel.MoveHistory = sel.Rectangle;
                //            }

                //            sel.Rectangle = new Rectangle(sel.MoveHistory.Value.X, sel.MoveHistory.Value.Y + delta.Y, sel.MoveHistory.Value.Width, sel.MoveHistory.Value.Height + delta.Y * (-1));
                //        }

                //        //_firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.RightTop:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X, sel.Rectangle.Y + delta.Y, sel.Rectangle.Width + delta.X, sel.Rectangle.Height + delta.Y * (-1));
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.RightBottom:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X, sel.Rectangle.Y, sel.Rectangle.Width + delta.X, sel.Rectangle.Height + delta.Y);
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.Right:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X, sel.Rectangle.Y, sel.Rectangle.Width + delta.X, sel.Rectangle.Height);
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.LeftTop:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X + delta.X, sel.Rectangle.Y + delta.Y, sel.Rectangle.Width + delta.X * (-1), sel.Rectangle.Height + delta.Y * (-1));
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.LeftBottom:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X + delta.X, sel.Rectangle.Y, sel.Rectangle.Width + delta.X * (-1), sel.Rectangle.Height + delta.Y);
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.Left:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X + delta.X, sel.Rectangle.Y, sel.Rectangle.Width + delta.X * (-1), sel.Rectangle.Height);
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.Center:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        recter.MoveRecter(delta);

                //        //foreach (var sel in recter.GetSelectRects())
                //        //{
                //        //    sel.Rectangle = new Rectangle(sel.Rectangle.X + delta.X, sel.Rectangle.Y + delta.Y, sel.Rectangle.Width, sel.Rectangle.Height);
                //        //}

                //        //_firstPoint = e.Location;
                //        break;
                //    }
                //    case DragType.Bottom:
                //    {
                //        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                //        foreach (var sel in recter.GetSelectRects())
                //        {
                //            sel.Rectangle = new Rectangle(sel.Rectangle.X, sel.Rectangle.Y, sel.Rectangle.Width, sel.Rectangle.Height + delta.Y);
                //        }

                //        _firstPoint = e.Location;
                //        break;
                //    }
                //    default:
                //    {
                //        break;
                //    }
                //}

                #endregion
            }

            if (_mouseDown)
            {
                Invalidate2(true);
            }

            //左键选中区域
            if (recter.IsSelectFrom && _selectMouseDown)
            {
                selectRectangle = new SelectRectangle();
                Rectangle r = new Rectangle();
                r.X = _firstSelectPoint.X;
                r.Y = _firstSelectPoint.Y;
                Point mouse = this.PointToClient(MousePosition);

                if (mouse.X - _firstSelectPoint.X < 0)
                    r.X = mouse.X;
                if (mouse.Y - _firstSelectPoint.Y < 0)
                    r.Y = mouse.Y;
                r.Width = Math.Abs(mouse.X - _firstSelectPoint.X);
                r.Height = Math.Abs(mouse.Y - _firstSelectPoint.Y);
                selectRectangle.Rect = r;
                Invalidate2(false);
            }

            base.OnMouseMove(e);
        }

        public List<SelectRecter> GetClickControls(MouseEventArgs e)
        {
            return GetClickControls(Controls, e);
        }

        public List<SelectRecter> GetClickControls(ControlCollection controls, MouseEventArgs e, int spaceIndex = 1)
        {
            var sr = new List<SelectRecter>();
            foreach (Control c in controls) //遍历控件容器 看是否选中其中某一控件
            {
                Rectangle r = c.Bounds;
                r = c.Parent.RectangleToScreen(r);
                r = this.RectangleToClient(r);
                Rectangle rr = r;
                rr.Inflate(10, 10);

                if (rr.Contains(e.Location))
                {
                    sr.Add(new SelectRecter(c, r) {SpaceIndex = spaceIndex});
                }

                if (c.HasChildren)
                {
                    var clickControls = GetClickControls(c.Controls, e, spaceIndex + 1);
                    sr.AddRange(clickControls);
                }
            }

            return sr;
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //左键
            {
                var cons = GetClickControls(e);
                if (cons != null && cons.Any())
                {
                    //选中最上面某一控件
                    if (new[] {Keys.Shift, Keys.Control}.Contains(ModifierKeys))
                    {
                        var sel = cons.OrderByDescending(s => s.SpaceIndex)
                            .ThenByDescending(s => s.ZIndex)
                            .First();

                        Rectangle r = HostToOverlayerRectangle(sel.Control);
                        //Rectangle r = sel.Control.Bounds;
                        //r = sel.Control.Parent.RectangleToScreen(r);
                        //r = this.RectangleToClient(r);

                        recter.AddSelect(r, sel.Control);

                        FlushSelectProperty();
                        Invalidate2(false);
                    }
                    else if (!recter.IsMultipleSelect)
                    {
                        var sel = cons.OrderByDescending(s => s.SpaceIndex)
                            .ThenByDescending(s => s.ZIndex)
                            .First();

                        Rectangle r = HostToOverlayerRectangle(sel.Control);
                        //Rectangle r = sel.Control.Bounds;
                        //r = sel.Control.Parent.RectangleToScreen(r);
                        //r = this.RectangleToClient(r);
                        recter.SetSelect(r, sel.Control);

                        FlushSelectProperty();
                        Invalidate2(false);
                    }

                    _firstSelectPoint = new Point();
                    _selectMouseDown = false;
                    selectRectangle = null;
                }
                else //没有控件被选中，判断是否选中控件容器
                {
                    Rectangle r = hostFrame.Bounds;
                    r = Parent.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    if (r.Contains(e.Location))
                    {
                        SelectHost();

                        //只有在设计器内部才可以选中区域
                        _firstSelectPoint = e.Location;
                        _selectMouseDown = true;
                        selectRectangle = null;

                        Invalidate2(false);
                    }
                }

                DragType dt = recter.GetMouseDragType(e.Location); //判断是否可以进行鼠标操作
                if (dt != DragType.None)
                {
                    _mouseDown = true;
                    _firstPoint = e.Location;
                    _dragType = dt;
                }
            }

            base.OnMouseDown(e);
        }


        private void MouseUpSelectControls(MouseEventArgs e)
        {
            //选中控件
            if (selectRectangle != null)
            {
                recter.ClearSelect();
                Rectangle select_r = new Rectangle
                {
                    X = _firstSelectPoint.X,
                    Y = _firstSelectPoint.Y
                };
                Point mouse = this.PointToClient(MousePosition);
                if (mouse.X - _firstSelectPoint.X < 0)
                    select_r.X = mouse.X;
                if (mouse.Y - _firstSelectPoint.Y < 0)
                    select_r.Y = mouse.Y;
                select_r.Width = Math.Abs(mouse.X - _firstSelectPoint.X);
                select_r.Height = Math.Abs(mouse.Y - _firstSelectPoint.Y);
                if ((select_r.Width > 5 || select_r.Height > 5) && _firstSelectPoint != new Point())
                {
                    foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                    {
                        Rectangle r = c.Bounds;
                        r = hostFrame.RectangleToScreen(r);
                        r = this.RectangleToClient(r);

                        if (select_r.IntersectsWith(r)) //判断控件是否有部分包含在选中区域内
                        {
                            recter.AddSelect(r, c);
                            FlushSelectProperty();
                        }
                    }
                }

                if (!recter.IsSelect)
                {
                    SelectHost();
                }
            }
            else if (_dragType == DragType.None && _firstPoint.IsEmpty ||
                     _dragType == DragType.Center && _firstPoint == e.Location)
            {
                if (new[] {Keys.Shift, Keys.Control}.Contains(ModifierKeys))
                {
                    return;
                }

                var cons = GetClickControls(e);
                if (cons != null && cons.Any())
                {
                    //选中最上面某一控件
                    var sel = cons.OrderByDescending(s => s.SpaceIndex)
                        .ThenByDescending(s => s.ZIndex)
                        .First();
                    if (recter.IsSelect && !recter.IsMultipleSelect
                                        && recter.GetSelectRects()[0].Control == sel.Control)
                    {
                        return;
                    }

                    Rectangle r = HostToOverlayerRectangle(sel.Control);
                    recter.SetSelect(r, sel.Control);

                    FlushSelectProperty();
                    Invalidate2(false);

                    _firstSelectPoint = new Point();
                    _selectMouseDown = false;
                    selectRectangle = null;
                }
                else //没有控件被选中，判断是否选中控件容器
                {
                    Rectangle r = HostToOverlayerRectangle(hostFrame);
                    if (r.Contains(e.Location))
                    {
                        SelectHost();

                        //只有在设计器内部才可以选中区域
                        _firstSelectPoint = e.Location;
                        _selectMouseDown = true;
                        selectRectangle = null;

                        Invalidate2(false);
                    }
                }
            }
            else if (recter.IsMoving)
            {
                var cons = GetClickControls(e);
                if (cons != null && cons.Any())
                {
                    //选中最上面的容器控件
                    var sel = cons.Where(s => s.Control.IsContainerControl())
                        .OrderByDescending(s => s.SpaceIndex)
                        .ThenByDescending(s => s.ZIndex)
                        .FirstOrDefault();
                    if (sel == null)
                    {
                        //到Form控件
                        foreach (var s in recter.GetSelectControlsRects())
                        {
                            if (s.Control.Parent != hostFrame)
                            {
                                s.Parent = s.Control.Parent;
                                Controls.Add(s.Control);
                            }
                        }

                        return;
                    }

                    foreach (var sc in recter.GetSelectControlsRects())
                    {
                        if (sc.Control != sel.Control && sc.Control.Parent != sel.Control)
                        {
                            sc.Parent = sc.Control.Parent;
                            sel.Control.Controls.Add(sc.Control);
                        }
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //左键弹起
            {
                MouseUpSelectControls(e);

                recter.ModifyRecterEnd(_dragType, OperationControlHistory);

                _firstSelectPoint = new Point();
                _selectMouseDown = false;
                selectRectangle = null;
                _firstPoint = new Point();
                _mouseDown = false;
                _dragType = DragType.None;
                Invalidate2(true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                recter.GetSelectControlsRects()
                    .ForEach(s =>
                    {
                        Rectangle r = HostToOverlayerRectangle(s.Control);
                        s.Control.ContextMenuStrip.Show(s.Control,
                            e.X - r.Left, e.Y - r.Top);
                    });
            }

            base.OnMouseUp(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }

        /// <summary>
        /// 移动控件
        /// <para>供上下左右箭头键调用</para>
        /// </summary>
        private void RefreshMoveControls()
        {
            if (recter.IsSelectFrom) return;

            foreach (var sel in recter.GetSelectRects())
            {
                Rectangle r = hostFrame.RectangleToScreen(sel.Control.Bounds);
                r = this.RectangleToClient(r);
                sel.Rectangle = r;
            }

            FlushSelectProperty();
            Invalidate2(false);
        }

        public int GetControlId(Control con, Type findType)
        {
            int id = 0;
            foreach (Control c in con.Controls)
            {
                if (c.GetType().FullName == findType.FullName)
                {
                    var ids = Convert.ToInt32(Regex.Replace(c.Name, @"[^0-9]+", ""));
                    if (ids > id)
                    {
                        id = ids;
                    }
                }

                if (c.HasChildren)
                {
                    var ids = GetControlId(c, findType);
                    if (ids > id)
                    {
                        id = ids;
                    }
                }
            }

            return id;
        }

        public void SetControlName(Control con)
        {
            if (string.IsNullOrWhiteSpace(con.Name))
            {
                Type type = con.GetType();
                con.Name = type.Name + (GetControlId(hostFrame, type) + 1);
            }
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            try
            {
                string[] strs = (string[]) drgevent.Data.GetData(typeof(string[])); //获取拖拽数据
                Control ctrl = ControlHelper.CreateControl(strs[1], strs[0]);       //实例化控件
                SetControlName(ctrl);
                ctrl.Location = hostFrame.PointToClient(new Point(drgevent.X, drgevent.Y)); //屏幕坐标转换成控件容器坐标
                if (!new Rectangle(hostFrame.Location, hostFrame.Size).Contains(new Rectangle(ctrl.Location, ctrl.Size)))
                {
                    MessageBox.Show("你不能在设计器外面创建控件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!(ctrl is DateTimePicker))
                {
                    ctrl.Text = strs[1];
                }

                hostFrame.Controls.Add(ctrl);
                ctrl.ContextMenuStrip = cms;
                ctrl.BringToFront();

                //将控件容器坐标转换为Overlayer坐标
                Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
                r = this.RectangleToClient(r);
                recter.SetSelect(r, ctrl);
                FlushSelectProperty();
                Invalidate2(false);
            }
            catch
            {
                // ignored
            }

            base.OnDragDrop(drgevent);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyValue == 46) //删除
            {
                if (recter.IsSelect && !recter.IsSelectFrom)
                {
                    recter.DeleteRecter(OperationControlHistory);
                    SelectHost();
                    Invalidate2(false);
                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// 响应上下左右箭头
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (!recter.IsSelect) return;
            if (e.Control && e.KeyCode == Keys.Z)
            {
                OperationControlHistory.Record();
            }

            switch (e.KeyValue)
            {
                case 37:
                    PushOperationHistory(OperationControlType.Move);

                    recter.GetSelectRects()
                        .ForEach(s => s.Control.Location = new Point(s.Control.Location.X - 1, s.Control.Location.Y));

                    RefreshMoveControls();
                    break;
                case 38:
                    PushOperationHistory(OperationControlType.Move);

                    recter.GetSelectRects()
                        .ForEach(s => s.Control.Location = new Point(s.Control.Location.X, s.Control.Location.Y - 1));

                    RefreshMoveControls();
                    break;
                case 39:
                    PushOperationHistory(OperationControlType.Move);

                    recter.GetSelectRects()
                        .ForEach(s => s.Control.Location = new Point(s.Control.Location.X + 1, s.Control.Location.Y));

                    RefreshMoveControls();
                    break;
                case 40:
                    PushOperationHistory(OperationControlType.Move);

                    recter.GetSelectRects()
                        .ForEach(s => s.Control.Location = new Point(s.Control.Location.X, s.Control.Location.Y + 1));

                    RefreshMoveControls();
                    break;
            }

            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region 提供用户访问

        /// <summary>
        /// 绘制控件
        /// </summary>
        public void DrawControl(Control ctrl)
        {
            hostFrame.Controls.Add(ctrl);
            ctrl.BringToFront();

            //将控件容器坐标转换为Overlayer坐标
            Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
            r = this.RectangleToClient(r);

            recter.SetSelect(r, ctrl);
            FlushSelectProperty();
            Invalidate2(false);
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        public void DrawControls(List<Control> cons)
        {
            hostFrame.Controls.AddRange(cons.ToArray());

            recter.ClearSelect();
            foreach (var con in cons)
            {
                //将控件容器坐标转换为Overlayer坐标
                Rectangle r = hostFrame.RectangleToScreen(con.Bounds);
                r = this.RectangleToClient(r);

                recter.AddSelect(r, con);
            }

            FlushSelectProperty();
            Invalidate2(false);
        }

        /// <summary>
        /// 初始化设计界面
        /// </summary>
        /// <param name="formWidth">设计器宽</param>
        /// <param name="formHeight">设计器高</param>
        /// <param name="formText">设计器标题</param>
        public void Init(int formWidth, int formHeight, string formText)
        {
            hostFrame.Controls.Clear();
            hostFrame.Width = formWidth;
            hostFrame.Height = formHeight;
            hostFrame.Text = formText;
            recter = new Recter(this);
            selectRectangle = null;
            _firstSelectPoint = new Point();
            _selectMouseDown = false;

            SelectHost();
            Invalidate2(false);
        }

        /// <summary>
        /// 获取或设置设计窗体大小
        /// </summary>
        public Point DesingerFormSize
        {
            set
            {
                hostFrame.Width = value.X;
                hostFrame.Height = value.Y;
            }
            get { return new Point(hostFrame.Width, hostFrame.Height); }
        }

        /// <summary>
        /// 获取或设置设计器窗体名
        /// </summary>
        public string DesingerFormText
        {
            set { hostFrame.Text = value; }
            get { return hostFrame.Text; }
        }

        public HostFrame DesingerForm
        {
            get { return hostFrame; }
            set { hostFrame = value; }
        }

        #endregion

        #region 控件对齐

        private bool ControlAlginCheck()
        {
            if (hostFrame.Controls.Count == 0)
            {
                MessageBox.Show("当前设计器没有任何控件,\n请先添加控件再进行此操作.",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (hostFrame.Controls.Count < 2) //不足两个,排序无意义
            {
                MessageBox.Show("当前设计器控件不足两个,\n对齐操作无意义.",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 微调
        /// <para>对X轴与Y轴控件差距在5个像素点的控件对齐</para>
        /// <para></para>
        /// </summary>
        public void ControlAlginFineTune()
        {
            if (!ControlAlginCheck()) return;
            List<Point> needSourt = new List<Point>(); //需要调整的控件,当Count为0时，全部调整
            List<Control> psy = AlginFineTuneSort(false, ref needSourt);
            int miny = -1;
            foreach (Control c in psy)
            {
                if (needSourt.Count > 0)
                {
                    bool chioe = false;
                    foreach (Point p in needSourt)
                    {
                        if (p == c.Location)
                        {
                            chioe = true;
                            break;
                        }
                    }

                    if (!chioe)
                    {
                        return;
                    }
                }

                if (miny == -1)
                {
                    miny = c.Location.Y;
                    continue;
                }

                if (Math.Abs(c.Location.Y - miny) <= 5 && c.Location.Y - miny != 0)
                {
                    c.Location = new Point(c.Location.X, miny);
                }
                else if (Math.Abs(c.Location.Y - miny) > 5) //换行
                {
                    miny = c.Location.Y;
                }
            }

            List<Control> psx = AlginFineTuneSort(true, ref needSourt);
            int minx = -1;
            foreach (Control c in psx)
            {
                if (needSourt.Count > 0)
                {
                    bool chioe = false;
                    foreach (Point p in needSourt)
                    {
                        if (p == c.Location)
                        {
                            chioe = true;
                            break;
                        }
                    }

                    if (!chioe)
                    {
                        return;
                    }
                }

                if (minx == -1)
                {
                    minx = c.Location.X;
                    continue;
                }

                if (Math.Abs(c.Location.X - minx) <= 3 && c.Location.X - minx != 0)
                {
                    c.Location = new Point(minx, c.Location.Y);
                }
                else if (Math.Abs(c.Location.X - minx) > 3) //换列
                {
                    minx = c.Location.X;
                }
            }

            Invalidate2(true);
        }

        /// <summary>
        /// 微调排序
        /// </summary>
        private List<Control> AlginFineTuneSort(bool isX, ref List<Point> needsort)
        {
            if (hostFrame.Controls.Count < 2) //不足两个,排序无意义
                return null;
            var cs = hostFrame.Controls;
            if (recter.IsSelect)
            {
                foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                {
                    foreach (var sel in recter.GetSelectRects())
                    {
                        Rectangle r = c.Bounds;
                        r = hostFrame.RectangleToScreen(r);
                        r = this.RectangleToClient(r);
                        //Rectangle rr = r;
                        //rr.Inflate(10, 10);
                        if (sel.Rectangle.IntersectsWith(r)) //判断控件是否有部分包含在选中区域内
                        {
                            needsort.Add(c.Location);
                        }
                    }
                }
            }

            List<Control> p = new List<Control>();
            foreach (Control c in cs)
            {
                p.Add(c);
            }

            p.Sort(delegate(Control s1, Control s2)
            {
                if (isX)
                {
                    if (s1.Location.X == s2.Location.X)
                        return s1.Location.Y.CompareTo(s2.Location.Y);
                    else
                        return s1.Location.X.CompareTo(s2.Location.X);
                }
                else
                {
                    if (s1.Location.Y == s2.Location.Y)
                        return s1.Location.X.CompareTo(s2.Location.X);
                    else
                        return s1.Location.Y.CompareTo(s2.Location.Y);
                }
            });
            return p;
        }

        /// <summary>
        /// 对齐
        /// <para>可根据需求调整</para>
        /// </summary>
        public void ControlAlgin(AlginType alginType)
        {
            if (!ControlAlginCheck()) return;
            Point minPoint = new Point(-1, -1);
            Size minSize = new Size(0, 0);
            if (!recter.IsSelect)
            {
                return;
            }

            #region 取用于对齐的Control

            foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
            {
                foreach (var sel in recter.GetSelectRects())
                {
                    Rectangle r = c.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    //Rectangle rr = r;
                    //rr.Inflate(10, 10);
                    if (sel.Rectangle == r) //判断控件是否有部分包含在选中区域内
                    {
                        switch (alginType)
                        {
                            case AlginType.Left: //以最上面的为准
                                if (minPoint.X == -1)
                                    minPoint = c.Location;
                                if (c.Location.Y < minPoint.Y)
                                    minPoint = c.Location;
                                break;
                            case AlginType.Top: //以最左边的为准
                                if (minPoint.Y == -1)
                                    minPoint = c.Location;
                                if (c.Location.X < minPoint.X)
                                    minPoint = c.Location;
                                break;
                            case AlginType.Right:
                                if (minPoint.X == -1)
                                    minPoint = c.Location;
                                if (c.Location.Y < minPoint.Y)
                                    minPoint = c.Location;
                                break;
                            case AlginType.Bottom:
                                break;
                        }
                    }
                }
            }

            #endregion

            #region 设置值

            for (int i = 0; i < hostFrame.Controls.Count; i++) //遍历控件容器 看是否选中其中某一控件
            {
                Control c = hostFrame.Controls[i];
                for (int j = 0; j < recter.GetSelectRects().Count; j++)
                {
                    var sel = recter.GetSelectRects()[j];
                    Rectangle r = c.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    //Rectangle rr = r;
                    //rr.Inflate(10, 10);
                    if (sel.Rectangle == r) //判断控件是否有部分包含在选中区域内
                    {
                        switch (alginType)
                        {
                            case AlginType.Left:
                                c.Location = new Point(minPoint.X, c.Location.Y);
                                break;
                            case AlginType.Top:
                                c.Location = new Point(c.Location.X, minPoint.Y);
                                break;
                        }

                        c.Invalidate();

                        Rectangle newRec = c.Bounds;
                        newRec = hostFrame.RectangleToScreen(newRec);
                        newRec = this.RectangleToClient(newRec);
                        sel.Rectangle = newRec;
                    }
                }
            }

            #endregion

            Invalidate2(false);
        }

        #endregion

        #region 右键菜单

        private void tsmiControlPotTop_Click(object sender, EventArgs e)
        {
            recter.GetSelectControlsRects()
                .ForEach(s => s.Control.SendToBack());
            Invalidate2(false);
        }

        private void tsmiControlPotBottom_Click(object sender, EventArgs e)
        {
            recter.GetSelectControlsRects()
                .ForEach(s => s.Control.BringToFront());
            Invalidate2(false);
        }

        #endregion

        /// <summary>
        /// 重绘
        /// </summary>
        public void Invalidate2(bool mouseUp)
        {
            Invalidate();

            if (Parent != null) //更新父控件
            {
                Rectangle rc = new Rectangle(this.Location, this.Size);
                Parent.Invalidate(rc, true);
            }

            if (mouseUp) //鼠标弹起 更新底层控件
            {
                FlushSelectProperty();
                if (recter.IsSelectFrom)
                {
                    Rectangle r = recter.GetSelectRects()[0].Rectangle;
                    r = this.RectangleToScreen(r);
                    r = Parent.RectangleToClient(r);
                    hostFrame.SetBounds(r.Left, r.Top, r.Width, r.Height);
                }
                else
                {
                    foreach (var sel in recter.GetSelectRects())
                    {
                        //移动控件逻辑
                        Rectangle r = sel.Rectangle;
                        r = this.RectangleToScreen(r);
                        r = sel.Control.Parent.RectangleToClient(r);
                        sel.Control.SetBounds(r.Left, r.Top, r.Width, r.Height);

                        r = sel.Control.Bounds;
                        r = sel.Control.Parent.RectangleToScreen(r);
                        r = this.RectangleToClient(r);
                        sel.Rectangle = r;
                    }
                }

                //if (!recter.IsSelectFrom) //更新底层控件的位置、大小
                //{
                //    foreach (var sel in recter.GetSelectRects())
                //    {
                //        Rectangle r = sel.Rectangle;
                //        r = this.RectangleToScreen(r);
                //        r = hostFrame.RectangleToClient(r);
                //        sel.Control.SetBounds(r.Left, r.Top, r.Width, r.Height);

                //        r = sel.Control.Bounds;
                //        r = hostFrame.RectangleToScreen(r);
                //        r = this.RectangleToClient(r);
                //        sel.Rectangle = r;
                //    }
                //}
                //else //更新控件容器大小
                //{
                //    foreach (var sel in recter.GetSelectRects())
                //    {
                //        Rectangle r = sel.Rectangle;
                //        r = this.RectangleToScreen(r);
                //        r = Parent.RectangleToClient(r);
                //        sel.Control.SetBounds(r.Left, r.Top, r.Width, r.Height);

                //        r = sel.Control.Bounds;
                //        r = hostFrame.RectangleToScreen(r);
                //        r = this.RectangleToClient(r);
                //        sel.Rectangle = r;
                //    }
                //}
            }
        }

        ~Overlayer()
        {
            StopListen();
            OperationControlHistory.Clear();
            recter.ClearSelect();
            recter = null;
            hostFrame = null;
        }
    }
}