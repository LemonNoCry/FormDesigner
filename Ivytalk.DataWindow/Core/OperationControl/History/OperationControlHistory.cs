using System.Collections.Generic;

namespace Ivytalk.DataWindow.Core.OperationControl.History
{
    public class OperationControlHistory : Stack<OperationControlRecord>
    {
        /// <summary>
        /// 存储最大操作历史记录
        /// </summary>
        private const int Limit = 500;

        public new void Push(OperationControlRecord item)
        {
            base.Push(item);
            while (Count > Limit)
            {
                Pop();
            }
        }

        public new OperationControlRecord Pop()
        {
            if (Count == 0)
            {
                return null;
            }

            return base.Pop();
        }

        public void Record()
        {
            var history = Pop();
            history?.Record();
        }
    }
}