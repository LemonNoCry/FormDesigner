﻿using System;
using Ivytalk.DataWindow.DesignLayer;
using Ivytalk.DataWindow.Serializable;
using System.Collections.Generic;
using System.Windows.Forms;
using Ivytalk.DataWindow.Utility;
using Mapster;

namespace Ivytalk.DataWindow.Core.OperationControl.History
{
    /// <summary>
    /// 控件操作历史记录
    /// </summary>
    public class OperationControlRecord
    {
        public OperationControlRecord()
        {
        }

        public OperationControlRecord(Overlayer overlayer, OperationControlType operationControlType, List<Control> controls)
        {
            this.Overlayer = overlayer;
            this.OperationControlType = operationControlType;
            this.Control = controls;
            this.OperationControls = new List<ControlSerializable>();
            this.OperationControls = controls.MapsterCopyTo(OperationControls);
        }

        public OperationControlRecord(Overlayer overlayer, OperationControlType operationControlType, List<Control> controls, List<ControlSerializable> controlSerializables)
        {
            this.Overlayer = overlayer;
            this.OperationControlType = operationControlType;
            this.Control = controls;
            this.OperationControls = controlSerializables;
        }

        public OperationControlType OperationControlType { get; set; }
        public Overlayer Overlayer { get; set; }
        public List<Control> Control { get; set; }
        public List<ControlSerializable> OperationControls { get; set; }
        public Control Parent { get; set; }

        public void Record()
        {
            switch (OperationControlType)
            {
                case OperationControlType.Move:
                    RecordMove();
                    break;
                case OperationControlType.MoveParent:
                    RecordMoveParent();
                    break;
                case OperationControlType.ModifySize:
                    RecordModifySize();
                    break;
                case OperationControlType.Add:
                    RecordAdd();
                    break;
                case OperationControlType.Delete:
                    RecordDelete();
                    break;
                case OperationControlType.ModifyProperty:
                    RecordModifyProperty();
                    break;
                case OperationControlType.MoveShow:
                    RecordMoveShow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Overlayer.Recter.RefreshRecterRectangle();
            Overlayer.FlushSelectProperty();
            Overlayer.Invalidate2(false);
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

        private void RecordMoveShow()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];

                control.Visible = false;

                if (history.ParentSerializable != null)
                {
                    var parent = Overlayer.FindControl(history.ParentSerializable.Name);
                    if (parent != control.Parent)
                    {
                        parent.Controls.Add(control);
                    }
                }
                else
                {
                    control.Parent.Controls.Remove(control);
                }

                control.Location = history.Location;
            }
        }

        private void RecordMoveParent()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];

                var parent = Overlayer.FindControl(history.ParentSerializable.Name);
                parent.Controls.Add(control);
                control.Location = history.Location;
            }
        }

        private void RecordModifySize()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];
                var r = history.ClientRectangle;
                control.SetBounds(r.Left, r.Top, r.Width, r.Height);
            }
        }

        private void RecordAdd()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                control.Parent.Controls.Remove(control);
                control.Dispose();
            }
        }

        private void RecordDelete()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];

                var parent = Overlayer.FindControl(history.ParentSerializable.Name);
                if (parent != control.Parent)
                {
                    parent.Controls.Add(control);
                }
            }
        }

        private void RecordModifyProperty()
        {
            for (int i = 0; i < Control.Count; i++)
            {
                var control = Control[i];
                var history = OperationControls[i];

                history.ControlSerializableToControl(control);
            }
        }
    }
}