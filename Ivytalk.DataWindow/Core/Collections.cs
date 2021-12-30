using Ivytalk.DataWindow.DesignLayer;
using Ivytalk.DataWindow.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ivytalk.DataWindow.CustomPropertys;
using Ivytalk.DataWindow.Utility;

namespace Ivytalk.DataWindow.Core
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

                    AllControlSerializable.Add(controlType.GetGenericArguments()[0].Name,
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

            if (serializable != null)
            {
                serializable.Type = type;
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
        public static ControlSerializable ControlConvertSerializable(Control control)
        {
            var type = GetBaseSerializable(control.GetType()).GetType();
            var cs = (ControlSerializable) Activator.CreateInstance(type);
            control.MapsterCopyTo(cs);
            cs.Type = control.GetType();
            return cs;
        }

        public static XmlSerializer XmlSerializer;

        public static XmlSerializer GetXmlSerializer()
        {
            if (XmlSerializer == null)
            {
                Type[] types = AllControlSerializable.Values.Select(s => s.GetType()).ToArray();
                XmlSerializer = new XmlSerializer(typeof(ControlSerializable), types);

                //XmlAttributeOverrides aor = new XmlAttributeOverrides();
                //XmlAttributes xas = new XmlAttributes();
                //foreach (var controlSerializable in AllControlSerializable)
                //{
                //    xas.XmlElements.Add(new XmlElementAttribute(controlSerializable.Value.GetType().Name,
                //        controlSerializable.Value.GetType()));
                //}

                //aor.Add(typeof(ControlSerializable), "ControlsSerializable", xas);
            }

            return XmlSerializer;
        }

        #region 获取自定义属性

        /// <summary>
        /// 显示指定的属性
        /// <para>自定义属性必须定义ValueType</para>
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="controlName">控件名</param>
        /// <returns>属性集合</returns>
        public static CustomPropertyCollection GetCollections(Control control, Overlayer overlayer)
        {
            var cpc = GetBaseSerializable(control.GetType()).GetCollections(control);
            cpc.Overlayer = overlayer;
            return cpc;
        }

        public static CustomPropertyCollection[] GetCollections(Overlayer overlayer)
        {
            return overlayer.Recter.GetSelectRects()
                .Select(s => GetCollections(s.Control, overlayer))
                .ToArray();
        }

        #endregion
    }
}