using System.Drawing;

namespace FormDesinger.Core.Serializable
{
    public struct CustomizeFont
    {
        public string Name { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }
        public GraphicsUnit Unit { get; set; }
    }
}