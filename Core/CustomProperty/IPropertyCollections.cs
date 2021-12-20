using System.Windows.Forms;

namespace FormDesinger
{
    /// <summary>
    /// 链式获取自定义获取控件属性,用于设计器显示 <br/>
    /// 只需关注单个控件的私有属性. 如Control已经有Text Fom里就不需要在写Text了 <br/>
    /// 例如Form完整的调用链: <br/>
    /// ControlSerializable --> FormSerializable
    /// </summary>
    public interface IPropertyCollections<in T>
    {
        CustomPropertyCollection GetCollections(T control);
    }
}