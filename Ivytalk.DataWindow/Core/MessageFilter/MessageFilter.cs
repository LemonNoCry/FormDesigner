using Ivytalk.DataWindow.DesignLayer;
using System.Windows.Forms;

namespace Ivytalk.DataWindow.Core.MessageFilter
{
    /// <summary>
    /// 消息过滤器
    /// </summary>
    class MessageFilter : IMessageFilter
    {
        BaseDataWindow _thehost;
        DesignerControl _theDesignerBoard;
        public MessageFilter(BaseDataWindow baseDataWindow, DesignerControl designer)
        {
            _thehost = baseDataWindow;
            _theDesignerBoard = designer;
        }
        #region IMessageFilter 成员
        public bool PreFilterMessage(ref Message m) //过滤所有控件的WM_PAINT消息
        {
            Control ctrl = (Control)Control.FromHandle(m.HWnd);
            if (_thehost != null && _theDesignerBoard != null && _thehost.Controls.Contains(ctrl) && m.Msg == 0x000F) // 0x000F == WM_PAINT
            {
                _theDesignerBoard.Refresh();
                return true;
            }
            return false;
        }
        #endregion
    }
}
