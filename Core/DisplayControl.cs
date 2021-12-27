using System.Windows.Forms;

namespace FormDesinger.Core
{
    public class DisplayControl
    {
        public DisplayControl(Control control)
        {
            Control = control;
        }

        public Control Control { get; set; }

        public string Name => Control.Name;
        public string ShowTitle => Control.Name + " " + Control.Text + " " + Control.GetType().Name;
    }
}