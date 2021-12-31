using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Windows.Forms;
using Ivytalk.DataWindow.Core.OperationControl;
using Ivytalk.DataWindow.Serializable;

namespace FormDesignerUnit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        { foreach (var value in Enum.GetValues(typeof(DragType)))
            {
                Console.WriteLine(value.GetType());
            }
        }

        [TestMethod]
        public void Method01()
        {
            Console.WriteLine(Assembly.Load("Ivytalk.DataWindow"));
            Console.WriteLine(typeof(ControlSerializable).FullName);

           var assemblies =AppDomain.CurrentDomain.GetAssemblies();
           foreach (var ass in assemblies)
           {
               Console.WriteLine(ass); 
           }
           
        }
    }
}
