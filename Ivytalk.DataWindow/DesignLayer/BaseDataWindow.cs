using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ivytalk.DataWindow.Utility;

namespace Ivytalk.DataWindow.DesignLayer
{
    /// <summary>
    /// 用于放置控件的容器
    /// </summary>
    public partial class BaseDataWindow : Form
    {
        /// <summary>
        /// 是否设计模式
        /// </summary>
        public bool DesignStatus { get; set; }

        /// <summary>
        /// 控件必须
        /// </summary>
        public readonly List<Control> MustEditControls = new List<Control>();

        public readonly List<Control> ProhibitEditControls = new List<Control>();

        /// <summary>
        /// 窗体上固有的控件
        /// </summary>
        public readonly List<Control> InherentControls = new List<Control>();


        public void AddMustControls(params Control[] cons)
        {
            MustEditControls.AddRange(cons);
        }

        /// <summary>
        /// 判断控件是否必须
        /// 1.控件必须
        /// 2.控件.子控件存在必须
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public bool IsMustControl(Control con)
        {
            var flag = MustEditControls.Contains(con);
            if (flag)
            {
                return flag;
            }

            if (con.HasChildren)
            {
                foreach (Control c in con.Controls)
                {
                    flag = IsMustControl(c);
                    if (flag)
                    {
                        return flag;
                    }
                }
            }

            return flag;
        }

        public bool IsInherentControl(Control con)
        {
            return InherentControls.Contains(con);
        }

        public bool IsProhibitEditControl(Control con)
        {
            return ProhibitEditControls.Contains(con);
        }

        public Control GetInherentControl(string name)
        {
            return InherentControls.SingleOrDefault(s => s.Name.Equals(name));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EachDataWindowControls(this, c =>
            {
                InherentControls.Add(c);
                ProhibitedOperationControl(c);
            });
        }

        public void EachDataWindowControls(Control control, Action<Control> action)
        {
            foreach (Control con in control.Controls)
            {
                action?.Invoke(con);
                if (con.HasChildren)
                {
                    EachDataWindowControls(con, action);
                }
            }
        }

        public void ProhibitedOperationControl(Control con)
        {
            if (con is ToolStrip ts)
            {
                ProhibitEditControls.Add(ts);
            }
        }

        #region 事件

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignStatus)
            {
                if (!this.TopLevel)
                    ControlPaint.DrawGrid(e.Graphics, this.ClientRectangle, new Size(10, 10), Color.White); //绘制底层网格White
            }

            base.OnPaint(e);
        }

        #endregion
    }
}