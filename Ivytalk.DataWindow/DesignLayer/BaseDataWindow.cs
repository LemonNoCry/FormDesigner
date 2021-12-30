using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ivytalk.DataWindow.DesignLayer
{
    /// <summary>
    /// 用于放置控件的容器
    /// </summary>
    public partial class BaseDataWindow : Form
    {
        public bool DesignStatus { get; set; }

        /// <summary>
        /// 控件必须
        /// </summary>
        public readonly List<Control> MustEditControls = new List<Control>();

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
            return MustEditControls.Contains(con);
        }

        public bool IsInherentControl(Control con)
        {
            return InherentControls.Contains(con);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EachDataWindowControls(this, c => { InherentControls.Add(c); });
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