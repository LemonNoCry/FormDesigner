using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using FormDesinger.Core.Serializable;

namespace FormDesinger
{
    /// <summary>
    /// 显示属性
    /// <para>只显示需要的属性</para>
    /// </summary>
    public class Collections
    {
        static Collections()
        {
            Init();
        }

        public static Dictionary<string, ControlSerializable> AllControlSerializable = new Dictionary<string, ControlSerializable>();

        public static void Init()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                var pcTypes = type.GetInterfaces()
                    .Where(s => s.IsAssignableFrom(typeof(IPropertyCollections<Control>)));

                var enumerable = pcTypes as Type[] ?? pcTypes.ToArray();
                if (enumerable.Any())
                {
                    var controlType = enumerable.Last();
                    AllControlSerializable.Add(controlType.GenericTypeArguments[0].Name,
                        (ControlSerializable) Activator.CreateInstance(type));
                }
            }
        }

        public static ControlSerializable GetBaseSerializable(Type type)
        {
            if (!AllControlSerializable.TryGetValue(type.Name, out var serializable))
            {
                serializable = GetBaseSerializable(type.BaseType);
            }

            return serializable;
        }

        /// <summary>
        /// 显示指定的属性
        /// <para>自定义属性必须定义ValueType</para>
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="controlName">控件名</param>
        /// <returns>属性集合</returns>
        public static CustomPropertyCollection GetCollections(Control control)
        {
            return GetBaseSerializable(control.GetType()).GetCollections(control);
        }
    }
}