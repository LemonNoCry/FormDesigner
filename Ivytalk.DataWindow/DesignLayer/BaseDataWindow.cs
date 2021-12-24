using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ivytalk.DataWindow.DesignLayer
{
    /// <summary>
    /// 用于放置控件的容器
    /// </summary>
    public partial class BaseDataWindow : Form
    {
        public bool DesignStatus { get; set; }

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