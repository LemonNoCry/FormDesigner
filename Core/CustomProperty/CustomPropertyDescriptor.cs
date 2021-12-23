using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using FormDesinger.Core;
using FormDesinger.Core.Serializable;

namespace FormDesinger
{
    /// <summary>
    /// 属性描述
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private readonly CustomProperty _customProperty = null;
        private readonly Overlayer _overlayer = null;

        public CustomPropertyDescriptor(Overlayer overlayer, CustomProperty customProperty, Attribute[] attrs)
            : base(customProperty.Name, attrs)
        {
            _overlayer = overlayer;
            _customProperty = customProperty;
        }

        public override bool CanResetValue(object component)
        {
            return _customProperty.DefaultValue != null;
        }

        public override Type ComponentType
        {
            get { return _customProperty.GetType(); }
        }

        [RefreshProperties(RefreshProperties.All)]
        public override object GetValue(object component)
        {
            return _customProperty.Value;
        }

        public override bool IsReadOnly
        {
            get { return _customProperty.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _customProperty.ValueType; }
        }

        public override void ResetValue(object component)
        {
            _customProperty.ResetValue();
        }

        private static OperationControlRecord ocr;

        public void HistoryChangeValue()
        {
            if (_overlayer == null)
            {
                return;
            }

            if (ocr == null)
            {
                ocr = new OperationControlRecord
                {
                    Overlayer = _overlayer,
                    OperationControlType = OperationControlType.ModifyProperty,
                    Control = new List<Control>(),
                    OperationControls = new List<ControlSerializable>()
                };
            }

            var sel = _overlayer.recter.GetSelectRects();
            if (sel.Count > 1)
            {
                var current = sel.First(s => s.Control == (Control) CustomProperty.ObjectSource);
                ocr.Control.Add(current.Control);
                ocr.OperationControls.Add(Collections.ControlConvertSerializable(current.Control));
                if (ocr.Control.Count == sel.Count)
                {
                    _overlayer.OperationControlHistory.Push(ocr);
                    ocr = null;
                }
            }
            else
            {
                ocr.Control.Add(sel[0].Control);
                ocr.OperationControls.Add(Collections.ControlConvertSerializable(sel[0].Control));

                _overlayer.OperationControlHistory.Push(ocr);
                ocr = null;
            }
        }

        public override void SetValue(object component, object value)
        {
            HistoryChangeValue();
            _customProperty.Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        //
        public override string Description
        {
            get { return _customProperty.Description; }
        }

        public override string Category
        {
            get { return _customProperty.Category; }
        }

        [RefreshProperties(RefreshProperties.All)]
        public override string DisplayName
        {
            get { return _customProperty.Name; }
        }

        public override bool IsBrowsable
        {
            get { return _customProperty.IsBrowsable; }
        }

        public CustomProperty CustomProperty
        {
            get { return _customProperty; }
        }
    }
}