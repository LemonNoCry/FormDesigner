using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FormDesinger.Core.Serializable.CustomConverter
{
    public class CustomConvertersFactory
    {
        static CustomConvertersFactory()
        {
            Init();
        }

        public static Dictionary<string, ICustomConverter> AllControlSerializable = new Dictionary<string, ICustomConverter>();

        public static void Init()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                var pcTypes = type.GetInterface(nameof(ICustomConverter));
                if (pcTypes != null)
                {
                    AllControlSerializable.Add(type.Name,
                        (ICustomConverter) Activator.CreateInstance(type));
                }
            }
        }

        public static ICustomConverter GetConverter(Type type)
        {
            return AllControlSerializable[type.Name];
        }
    }
}