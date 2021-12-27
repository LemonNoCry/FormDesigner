namespace Ivytalk.DataWindow.DesignLayer
{
    partial class Overlayer
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiControlPotTop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiControlPotBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectControlStart = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSelectControlEnd = new System.Windows.Forms.ToolStripSeparator();
            this.cms.SuspendLayout();
            this.SuspendLayout();
            // 
            // cms
            // 
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiControlPotTop,
            this.tsmiControlPotBottom,
            this.tsmiSelectControlStart,
            this.tsmiSelectControlEnd});
            this.cms.Name = "cms";
            this.cms.Size = new System.Drawing.Size(181, 82);
            // 
            // tsmiControlPotTop
            // 
            this.tsmiControlPotTop.Name = "tsmiControlPotTop";
            this.tsmiControlPotTop.Size = new System.Drawing.Size(180, 22);
            this.tsmiControlPotTop.Text = "置于顶层";
            this.tsmiControlPotTop.Click += new System.EventHandler(this.tsmiControlPotTop_Click);
            // 
            // tsmiControlPotBottom
            // 
            this.tsmiControlPotBottom.Name = "tsmiControlPotBottom";
            this.tsmiControlPotBottom.Size = new System.Drawing.Size(180, 22);
            this.tsmiControlPotBottom.Text = "置于底层";
            this.tsmiControlPotBottom.Click += new System.EventHandler(this.tsmiControlPotBottom_Click);
            // 
            // tsmiSelectControlStart
            // 
            this.tsmiSelectControlStart.Name = "tsmiSelectControlStart";
            this.tsmiSelectControlStart.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiSelectControlEnd
            // 
            this.tsmiSelectControlEnd.Name = "tsmiSelectControlEnd";
            this.tsmiSelectControlEnd.Size = new System.Drawing.Size(177, 6);
            // 
            // Overlayer
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "Overlayer";
            this.cms.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip cms;
        private System.Windows.Forms.ToolStripMenuItem tsmiControlPotTop;
        private System.Windows.Forms.ToolStripMenuItem tsmiControlPotBottom;
        private System.Windows.Forms.ToolStripSeparator tsmiSelectControlStart;
        private System.Windows.Forms.ToolStripSeparator tsmiSelectControlEnd;
    }
}
