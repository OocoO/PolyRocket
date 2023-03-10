using System;

namespace Carotaa.Code
{
    public class ShareVariable<TData> : ShareEvent<TData>, IShareVariable<TData>
    {
        protected TData _value;

        public TData Value
        {
            get => _value;

            set
            {
                if (IsValueDifferent(value)) Raise(value);

                _value = value;
            }
        }

        protected virtual bool IsValueDifferent(TData other)
        {
            return !Equals(_value, other);
        }
    }

    public class BoolVariable : ShareVariable<bool>
    {
    }

    public class IntVariable : ShareVariable<int>
    {
    }

    public class FloatVariable : ShareVariable<float>
    {
        protected override bool IsValueDifferent(float other)
        {
            return Math.Abs(_value - other) > 0.0001f;
        }
    }

    public class DoubleVariable : ShareVariable<double>
    {
        protected override bool IsValueDifferent(double other)
        {
            return Math.Abs(_value - other) > 0.0000001;
        }
    }
}