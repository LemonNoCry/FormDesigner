using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.Control;

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
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true);         //双缓冲


            hostFrame = host; //默认被操作的是控件容器
            SelectHost();

            InitializeComponent();
        }

        /// <summary>
        /// 被遮罩的控件容器，通过Overlayer操作该容器（以及其中的子控件）
        /// </summary>
        HostFrame hostFrame;

        /// <summary>
        /// 被操作控件（容器）周围的方框
        /// <para>选中效果</para>
        /// </summary>
        Recter recter = new Recter();

        /// <summary>
        /// 鼠标选中区域周围的方框
        /// <para>鼠标多选时方框</para>
        /// </summary>
        SelectRectangle selectRectangle = null;

        Point _firstSelectPoint = new Point(); //鼠标移动前的第一个位置
        bool _selectMouseDown = false;         //鼠标是否按下并选择区域

        /// <summary>
        /// 当前活动控件
        /// </summary>
        // Control _currentCtrl = null;

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

        private void _propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            RefreshMoveControls();
        }

        private void SelectHost()
        {
            Rectangle r = hostFrame.Bounds;
            r = hostFrame.Parent.RectangleToScreen(r);
            r = this.RectangleToClient(r);
            recter.SetSelect(r, hostFrame);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams para = base.CreateParams;

                //para.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 透明支持
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
            base.OnPaint(e);
            DrawBackHome(e.Graphics);

            recter?.Draw(e.Graphics);          //绘制被操作控件周围的方框
            selectRectangle?.Draw(e.Graphics); //绘制被操作控件周围的方框
        }

        #region 代理所有用户操作

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
                #region 拖动

                switch (_dragType) //改变方框位置大小
                {
                    case DragType.Top:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X, recter.Rect.Y + delta.Y, recter.Rect.Width, recter.Rect.Height + delta.Y * (-1));
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.RightTop:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X, recter.Rect.Y + delta.Y, recter.Rect.Width + delta.X, recter.Rect.Height + delta.Y * (-1));
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.RightBottom:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X, recter.Rect.Y, recter.Rect.Width + delta.X, recter.Rect.Height + delta.Y);
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.Right:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X, recter.Rect.Y, recter.Rect.Width + delta.X, recter.Rect.Height);
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.LeftTop:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X + delta.X, recter.Rect.Y + delta.Y, recter.Rect.Width + delta.X * (-1), recter.Rect.Height + delta.Y * (-1));
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.LeftBottom:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X + delta.X, recter.Rect.Y, recter.Rect.Width + delta.X * (-1), recter.Rect.Height + delta.Y);
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.Left:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X + delta.X, recter.Rect.Y, recter.Rect.Width + delta.X * (-1), recter.Rect.Height);
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.Center:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X + delta.X, recter.Rect.Y + delta.Y, recter.Rect.Width, recter.Rect.Height);
                        _firstPoint = e.Location;
                        break;
                    }
                    case DragType.Bottom:
                    {
                        Point delta = new Point(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
                        recter.Rect = new Rectangle(recter.Rect.X, recter.Rect.Y, recter.Rect.Width, recter.Rect.Height + delta.Y);
                        _firstPoint = e.Location;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }

                #endregion
            }

            if (_mouseDown)
            {
                Invalidate2(true);
            }

            //左键选中区域
            if (recter.IsSelect && recter.IsForm && _selectMouseDown)
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //左键
            {
                bool flag = false;
                foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                {
                    Rectangle r = c.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    Rectangle rr = r;
                    rr.Inflate(10, 10);
                    if (rr.Contains(e.Location))
                    {
                        recter.Rect = r;
                        _currentCtrl = c;
                        _propertyGrid.SelectedObject = Collections.GetCollections(c); //选中控件
                        recter.IsForm = false;
                        flag = true;
                        Invalidate2(false);
                        break;
                    }
                }

                if (!flag) //没有控件被选中，判断是否选中控件容器
                {
                    Rectangle r = hostFrame.Bounds;
                    r = Parent.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    if (r.Contains(e.Location))
                    {
                        recter.Rect = r;
                        recter.IsForm = true;
                        _currentCtrl = null;
                        _propertyGrid.SelectedObject = Collections.GetCollections(hostFrame); //选中控件容器

                        //只有在设计器内部才可以选中区域
                        _firstSelectPoint = e.Location;
                        _selectMouseDown = true;
                        selectRectangle = null;

                        Invalidate2(false);
                    }
                }
                else
                {
                    _firstSelectPoint = new Point();
                    _selectMouseDown = false;
                    selectRectangle = null;
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //左键弹起
            {
                #region 选中区域

                recter.ClearRect();
                Rectangle select_r = new Rectangle();
                select_r.X = _firstSelectPoint.X;
                select_r.Y = _firstSelectPoint.Y;
                Point mouse = this.PointToClient(MousePosition);
                if (mouse.X - _firstSelectPoint.X < 0)
                    select_r.X = mouse.X;
                if (mouse.Y - _firstSelectPoint.Y < 0)
                    select_r.Y = mouse.Y;
                select_r.Width = Math.Abs(mouse.X - _firstSelectPoint.X);
                select_r.Height = Math.Abs(mouse.Y - _firstSelectPoint.Y);
                if ((select_r.Width > 5 || select_r.Height > 5) && _firstSelectPoint != new Point())
                {
                    bool select = false;
                    foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                    {
                        Rectangle r = c.Bounds;
                        r = hostFrame.RectangleToScreen(r);
                        r = this.RectangleToClient(r);

                        if (select_r.IntersectsWith(r)) //判断控件是否有部分包含在选中区域内
                        {
                            if (!select) //处理第一个
                            {
                                _currentCtrl = c;
                                _propertyGrid.SelectedObject = Collections.GetCollections(c); //选中控件
                                recter.IsForm = false;
                                recter.Rect = r;
                            }

                            recter.AddRect(r);
                        }
                    }
                }

                #endregion

                _firstSelectPoint = new Point();
                _selectMouseDown = false;
                selectRectangle = null;
                _firstPoint = new Point();
                _mouseDown = false;
                _dragType = DragType.None;
                Invalidate2(true);
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
            if (_currentCtrl == null) return;
            Rectangle r = hostFrame.RectangleToScreen(_currentCtrl.Bounds);
            r = this.RectangleToClient(r);
            recter.ClearRect();
            recter.Rect = r;
            recter.IsForm = false;
            _propertyGrid.SelectedObject = Collections.GetCollections(_currentCtrl); //选中控件
            Invalidate2(false);
            _currentCtrl.Focus();
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
                ctrl.BringToFront();

                _currentCtrl = ctrl;
                _propertyGrid.SelectedObject = Collections.GetCollections(ctrl); //选中控件

                //将控件容器坐标转换为Overlayer坐标
                Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
                r = this.RectangleToClient(r);

                recter.Rect = r;
                recter.IsForm = false;

                Invalidate2(false);
            }
            catch
            {
            }

            base.OnDragDrop(drgevent);
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyValue == 46) //删除
            {
                if (_currentCtrl != null)
                {
                    if (recter.GetRects().Count == 0) //只有一个
                    {
                        hostFrame.Controls.Remove(_currentCtrl);
                        _currentCtrl = null;
                        _propertyGrid.SelectedObject = Collections.GetCollections(hostFrame); //选中控件容器
                        Rectangle r = hostFrame.Bounds;
                        r = hostFrame.Parent.RectangleToScreen(r);
                        r = this.RectangleToClient(r);
                        recter.Rect = r; //绘制的矩形转移到Form上
                        recter.IsForm = true;
                        Invalidate2(false);
                    }
                    else //删除多个
                    {
                        int delnum = 0;
                        Agian:
                        List<Rectangle> listdel = new List<Rectangle>();
                        foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                        {
                            foreach (Rectangle select_r in recter.GetRects())
                            {
                                Rectangle r = c.Bounds;
                                r = hostFrame.RectangleToScreen(r);
                                r = this.RectangleToClient(r);
                                //Rectangle rr = r;
                                //rr.Inflate(10, 10);
                                if (select_r.IntersectsWith(r)) //判断控件是否有部分包含在选中区域内
                                {
                                    hostFrame.Controls.Remove(c);
                                    _currentCtrl = null;
                                    listdel.Add(r);
                                }
                            }
                        }

                        recter.RemoveRect(listdel);
                        if (delnum < 10 && recter.GetRects().Count != 0)
                        {
                            delnum++;
                            goto Agian;
                        }

                        _propertyGrid.SelectedObject = Collections.GetCollections(hostFrame); //选中控件容器
                        Rectangle form_r = hostFrame.Bounds;
                        form_r = hostFrame.Parent.RectangleToScreen(form_r);
                        form_r = this.RectangleToClient(form_r);
                        recter.Rect = form_r; //绘制的矩形转移到Form上
                        recter.IsForm = true;
                        Invalidate2(false);
                    }
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
            if (_currentCtrl == null) return;
            if (e.KeyValue == 37)
            {
                _currentCtrl.Location = new Point(_currentCtrl.Location.X - 1, _currentCtrl.Location.Y);
                RefreshMoveControls();
            }

            if (e.KeyValue == 38)
            {
                _currentCtrl.Location = new Point(_currentCtrl.Location.X, _currentCtrl.Location.Y - 1);
                RefreshMoveControls();
            }

            if (e.KeyValue == 39)
            {
                _currentCtrl.Location = new Point(_currentCtrl.Location.X + 1, _currentCtrl.Location.Y);
                RefreshMoveControls();
            }

            if (e.KeyValue == 40)
            {
                _currentCtrl.Location = new Point(_currentCtrl.Location.X, _currentCtrl.Location.Y + 1);
                RefreshMoveControls();
            }

            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region 提供用户访问

        /// <summary>
        /// 当前设计器中的所有控件
        /// </summary>
        public new ControlCollection Controls
        {
            get { return hostFrame.Controls; }
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        public void DrawControl(string Name, string Text, Point p)
        {
            Point _p = new Point(p.X + 24, p.Y + 119);
            //窗体顶部偏移量24,119
            string[] strs = {"", Name};
            Control ctrl = ControlHelper.CreateControl(strs[1], strs[0]); //实例化控件

            ctrl.Location = hostFrame.PointToClient(_p); //屏幕坐标转换成控件容器坐标
            if (!(ctrl is DateTimePicker))
            {
                ctrl.Text = Text;
            }

            hostFrame.Controls.Add(ctrl);
            ctrl.BringToFront();

            _currentCtrl = ctrl;
            _propertyGrid.SelectedObject = Collections.GetCollections(ctrl); //选中控件

            //将控件容器坐标转换为Overlayer坐标
            Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
            r = this.RectangleToClient(r);

            recter.Rect = r;
            recter.IsForm = false;
            Invalidate2(false);
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        public void DrawControl(Control ctrl)
        {
            hostFrame.Controls.Add(ctrl);
            ctrl.BringToFront();

            _currentCtrl = ctrl;
            if (Parent.Parent is FromDesigner frm)
            {
                frm.propertyGrid1.SelectedObject = Collections.GetCollections(ctrl); //选中控件
            }


            //将控件容器坐标转换为Overlayer坐标
            Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
            r = this.RectangleToClient(r);

            recter.Rect = r;
            recter.IsForm = false;
            Invalidate2(false);
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        public void DrawControls(List<Control> ctrls)
        {
            Control ctrl = null;
            foreach (Control control in ctrls)
            {
                ctrl = control;
                hostFrame.Controls.Add(ctrl);
            }

            ctrl.BringToFront();
            _currentCtrl = ctrl;
            if (Parent.Parent is FromDesigner frm)
            {
                frm.propertyGrid1.SelectedObject = Collections.GetCollections(ctrl); //选中控件
            }


            //将控件容器坐标转换为Overlayer坐标
            Rectangle r = hostFrame.RectangleToScreen(ctrl.Bounds);
            r = this.RectangleToClient(r);

            recter.Rect = r;
            recter.IsForm = false;
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
            recter = new Recter();
            selectRectangle = null;
            _firstSelectPoint = new Point();
            _selectMouseDown = false;
            Rectangle form_r = hostFrame.Bounds;
            form_r = hostFrame.Parent.RectangleToScreen(form_r);
            form_r = this.RectangleToClient(form_r);
            recter.Rect = form_r; //绘制的矩形转移到Form上
            recter.IsForm = true;
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
            List<Point> needsort = new List<Point>(); //需要调整的控件,当Count为0时，全部调整
            List<Control> psy = AlginFineTuneSort(false, ref needsort);
            int miny = -1;
            foreach (Control c in psy)
            {
                if (needsort.Count > 0)
                {
                    bool chioe = false;
                    foreach (Point p in needsort)
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

            List<Control> psx = AlginFineTuneSort(true, ref needsort);
            int minx = -1;
            foreach (Control c in psx)
            {
                if (needsort.Count > 0)
                {
                    bool chioe = false;
                    foreach (Point p in needsort)
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
            if (recter.GetRects().Count != 0)
            {
                foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
                {
                    foreach (Rectangle select_r in recter.GetRects())
                    {
                        Rectangle r = c.Bounds;
                        r = hostFrame.RectangleToScreen(r);
                        r = this.RectangleToClient(r);
                        //Rectangle rr = r;
                        //rr.Inflate(10, 10);
                        if (select_r.IntersectsWith(r)) //判断控件是否有部分包含在选中区域内
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
            Point minpoint = new Point(-1, -1);
            Size minzine = new Size(0, 0);
            if (recter.GetRects().Count < 1)
            {
                return;
            }

            #region 取用于对齐的Control

            foreach (Control c in hostFrame.Controls) //遍历控件容器 看是否选中其中某一控件
            {
                foreach (Rectangle select_r in recter.GetRects())
                {
                    Rectangle r = c.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    //Rectangle rr = r;
                    //rr.Inflate(10, 10);
                    if (select_r == r) //判断控件是否有部分包含在选中区域内
                    {
                        switch (alginType)
                        {
                            case AlginType.Left: //以最上面的为准
                                if (minpoint.X == -1)
                                    minpoint = c.Location;
                                if (c.Location.Y < minpoint.Y)
                                    minpoint = c.Location;
                                break;
                            case AlginType.Top: //以最左边的为准
                                if (minpoint.Y == -1)
                                    minpoint = c.Location;
                                if (c.Location.X < minpoint.X)
                                    minpoint = c.Location;
                                break;
                            case AlginType.Right:
                                if (minpoint.X == -1)
                                    minpoint = c.Location;
                                if (c.Location.Y < minpoint.Y)
                                    minpoint = c.Location;
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
                for (int j = 0; j < recter.GetRects().Count; j++)
                {
                    Rectangle select_r = recter.GetRects()[j];
                    Rectangle r = c.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    //Rectangle rr = r;
                    //rr.Inflate(10, 10);
                    if (select_r == r) //判断控件是否有部分包含在选中区域内
                    {
                        switch (alginType)
                        {
                            case AlginType.Left:
                                c.Location = new Point(minpoint.X, c.Location.Y);
                                break;
                            case AlginType.Top:
                                c.Location = new Point(c.Location.X, minpoint.Y);
                                break;
                        }

                        c.Invalidate();
                        recter.RemoveRect(select_r);
                        Rectangle new_r = c.Bounds;
                        new_r = hostFrame.RectangleToScreen(new_r);
                        new_r = this.RectangleToClient(new_r);
                        recter.AddRect(new_r);
                    }
                }
            }

            #endregion

            Invalidate2(false);
        }

        #endregion

        /// <summary>
        /// 重绘
        /// </summary>
        private void Invalidate2(bool mouseUp)
        {
            Invalidate();
            if (Parent != null) //更新父控件
            {
                Rectangle rc = new Rectangle(this.Location, this.Size);
                Parent.Invalidate(rc, true);
            }

            if (mouseUp) //鼠标弹起 更新底层控件
            {
                if (_currentCtrl != null) //更新底层控件的位置、大小
                {
                    Rectangle r = recter.Rect;
                    r = this.RectangleToScreen(r);
                    r = hostFrame.RectangleToClient(r);
                    _currentCtrl.SetBounds(r.Left, r.Top, r.Width, r.Height);

                    r = _currentCtrl.Bounds;
                    r = hostFrame.RectangleToScreen(r);
                    r = this.RectangleToClient(r);
                    recter.Rect = r;
                }
                else //更新控件容器大小
                {
                    Rectangle r = recter.Rect;
                    r = this.RectangleToScreen(r);
                    r = Parent.RectangleToClient(r);
                    hostFrame.SetBounds(r.Left, r.Top, r.Width, r.Height);
                }
            }
        }
    }
}