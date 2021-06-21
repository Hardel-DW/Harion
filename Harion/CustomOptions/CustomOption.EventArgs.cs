using System;
using System.ComponentModel;

namespace Harion.CustomOptions {
    public class OptionOnValueChangedEventArgs : CancelEventArgs {
        public object Value { get; set; }
        public object OldValue { get; private set; }

        public OptionOnValueChangedEventArgs(object value, object oldValue) {
            Value = value;
            OldValue = oldValue;
        }
    }

    public class ToggleOptionOnValueChangedEventArgs : OptionOnValueChangedEventArgs {
        public new bool Value {
            get => (bool) base.Value;
            set => Value = value;
        }

        public new bool OldValue { get => (bool) base.OldValue; }

        public ToggleOptionOnValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }

    public class NumberOptionOnValueChangedEventArgs : OptionOnValueChangedEventArgs {
        public new float Value { 
            get => (float) base.Value;
            set => Value = value;
        }

        public new float OldValue { get => (float) base.OldValue; }

        public NumberOptionOnValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }

    public class StringOptionOnValueChangedEventArgs : OptionOnValueChangedEventArgs {
        public new int Value {
            get => (int) base.Value;
            set => Value = value;
        }

        public new int OldValue { get => (int) base.OldValue; }

        public StringOptionOnValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }

    public class OptionValueChangedEventArgs : EventArgs {
        public readonly object OldValue;
        public readonly object Value;

        public OptionValueChangedEventArgs(object value, object oldValue) {
            Value = value;
            OldValue = oldValue;
        }
    }

    public class ToggleOptionValueChangedEventArgs : OptionValueChangedEventArgs {
        public new bool OldValue { get => (bool) base.OldValue; }
        public new bool Value { get => (bool) base.Value; }

        public ToggleOptionValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }

    public class NumberOptionValueChangedEventArgs : OptionValueChangedEventArgs {
        public new float OldValue { get => (float) base.OldValue; }
        public new float Value { get => (float) base.Value; }

        public NumberOptionValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }

    public class StringOptionValueChangedEventArgs : OptionValueChangedEventArgs {
        public new int OldValue { get => (int) base.OldValue; }
        public new int Value { get => (int) base.Value; }

        public StringOptionValueChangedEventArgs(object value, object oldValue) : base(value, oldValue) { }
    }
}