using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FormDesinger.Core.Serializable;

namespace FormDesinger.Core
{
    /// <summary>
    /// 控件操作历史记录
    /// </summary>
    public class OperationControlRecord
    {
        public OperationControlRecord()
        {
            
        }
        public OperationControlRecord(OperationControlType operationControlType, List<Control> controls)
        {
            this.OperationControlType = operationControlType;
            this.Control = controls;
            this.OperationControls = new List<ControlSerializable>();
            this.OperationControls = controls.MapsterCopyTo(OperationControls);
        }

        public OperationControlType OperationControlType { get; set; }
        public List<Control> Control { get; set; }
        public List<ControlSerializable> OperationControls { get; set; }

        public void Record()
        {
            switch (OperationControlType)
            {
                case OperationControlType.Move:
                    RecordMove();
                    break;
                case OperationControlType.ModifySize:
                    break;
                case OperationControlType.Add:
                    break;
                case OperationControlType.Delete:
                    break;
                case OperationControlType.ModifyProperty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RecordMove()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];

                control.Location = history.Location;
            }
        }
    }

    public enum OperationControlType
    {
        Move,
        ModifySize,
        Add,
        Delete,
        ModifyProperty
    }
}