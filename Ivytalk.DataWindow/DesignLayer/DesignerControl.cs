using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ivytalk.DataWindow.Core.MessageFilter;
using Ivytalk.DataWindow.Core.OperationControl;
using Ivytalk.DataWindow.Events.EventArg;
using Ivytalk.DataWindow.Utility;

namespace Ivytalk.DataWindow.DesignLayer
{
    /// <summary>
    /// 设计面板
    /// </summary>
    public partial class DesignerControl : UserControl
    {
        /// <summary>
        /// 所有控件的容器
        /// </summary>
        public BaseDataWindow BaseDataWindow;

        public Overlayer Overlayer; //对父容器可见

        public DesignerControl()
        {
            InitializeComponent();

            BaseDataWindow = new BaseDataWindow();
            BaseDataWindow.DesignStatus = true;
            BaseDataWindow.TopLevel = false; //当做子控件添加到设计面板
            BaseDataWindow.Location = new Point(10, 10);
            BaseDataWindow.Text = "设计界面";
            Controls.Add(BaseDataWindow);
            BaseDataWindow.Show();

            Overlayer = new Overlayer(BaseDataWindow);
            Overlayer.Location = new Point(0, 0);
            Overlayer.Dock = DockStyle.Fill;
            Controls.Add(Overlayer);
            Overlayer.Show();
            Overlayer.BringToFront(); //将其设置Z轴最上，接受所有的用户操作，底层的其他控件无法接受用户输入。各位可以注释这条试一下，底层的其他控件（窗体）就能接受鼠标点击

            Application.AddMessageFilter(new MessageFilter(BaseDataWindow, this)); // 过滤控件容器中所有子控件的WM_PAINT消息 减少重绘操作
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public DesignerControl(BaseDataWindow baseDataWindow)
        {
            InitializeComponent();

            BaseDataWindow = baseDataWindow;
            BaseDataWindow.DesignStatus = true;
            BaseDataWindow.TopLevel = false; //当做子控件添加到设计面板
            BaseDataWindow.Location = new Point(10, 10);
            BaseDataWindow.Text = baseDataWindow.Text;
            Controls.Add(BaseDataWindow);
            BaseDataWindow.Show();

            Overlayer = new Overlayer(BaseDataWindow);
            Overlayer.Location = new Point(0, 0);
            Overlayer.Dock = DockStyle.Fill;
            Controls.Add(Overlayer);
            Overlayer.Show();
            Overlayer.BringToFront(); //将其设置Z轴最上，接受所有的用户操作，底层的其他控件无法接受用户输入。各位可以注释这条试一下，底层的其他控件（窗体）就能接受鼠标点击

            Application.AddMessageFilter(new MessageFilter(BaseDataWindow, this)); // 过滤控件容器中所有子控件的WM_PAINT消息 减少重绘操作
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        #region 自定义事件

        public event Recter.SelectControlChangedHandler SelectControlChanged
        {
            add => Overlayer.Recter.SelectControlChanged += value;
            remove => Overlayer.Recter.SelectControlChanged -= value;
        }


        public event BaseDataWindowControlChangedHandle BaseDataWindowControlChanged
        {
            add => Overlayer.BaseDataWindowControlChanged += value;
            remove => Overlayer.BaseDataWindowControlChanged -= value;
        }

        #endregion
    }
}