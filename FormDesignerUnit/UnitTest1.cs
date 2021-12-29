using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ivytalk.DataWindow.Core.OperationControl;

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
    }
}
