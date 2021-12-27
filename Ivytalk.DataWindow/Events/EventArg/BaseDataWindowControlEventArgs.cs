using System.Collections.Generic;
using System.Windows.Forms;
using Ivytalk.DataWindow.DesignLayer;

namespace Ivytalk.DataWindow.Events.EventArg
{
    public delegate void BaseDataWindowControlChangedHandle(BaseDataWindow sender, BaseDataWindowControlEventArgs e);

    public class BaseDataWindowControlEventArgs
    {
        public List<Control> AllControls { get; set; }
        public Control AddControl { get; set; }
        public Control RemoveControl { get; set; }
        public bool IsChanged => AddControl != null || RemoveControl != null;
        public bool IsAdd => AddControl != null;
        public bool IsRemove => RemoveControl != null;
    }
}