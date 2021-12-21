using System;
using System.Collections.Generic;

namespace FormDesinger.Core
{
    public class OperationControlHistory : Stack<OperationControlRecord>
    {
        /// <summary>
        /// 存储最大操作历史记录
        /// </summary>
        private int limit = 500;

        public new void Push(OperationControlRecord item)
        {
            base.Push(item);
            while (Count > 500)
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