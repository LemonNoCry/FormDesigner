using System.Windows.Forms;

namespace Ivytalk.DataWindow.Utility
{
    public static class ControlUtilityExpand
    {
        public static bool IsContainerControl(this Control con)
        {
            switch (con)
            {
                case Panel _:
                case ScrollableControl _:
                    return true;
            }

            return false;
        }
    }
}