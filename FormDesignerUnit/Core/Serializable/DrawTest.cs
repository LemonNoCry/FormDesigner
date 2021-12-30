using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivytalk.DataWindow.Core.OperationControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormDesignerUnit.Core.Serializable
{
    [TestClass()]
    public class DrawTest
    {
        [TestMethod]
        public void Method01()
        {
            foreach (var value in Enum.GetValues(typeof(DragType)))
            {
                Console.WriteLine(value);
            }
        }
    }
}