using System.Collections;
using System.Windows.Forms;

namespace FormDesinger.Core
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