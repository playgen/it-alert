using System;
using Engine.Entities;

namespace Engine.Components.Property
{
	public abstract class RangedNumericProperty<TValue> : Property<TValue>
		where TValue : struct
	{
		public TValue MaxValue { get; private set; }
		public TValue MinValue { get; private set; }

		protected RangedNumericProperty(TValue value,
			TValue minValue, 
			TValue maxValue)
			: base(value)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}
	}

	public abstract class RangedIntegerProperty : RangedNumericProperty<int>
	{
		protected RangedIntegerProperty(int value, 
			int minValue, 
			int maxValue) 
			: base(value, minValue, maxValue)
		{
		}

		public override void SetValue(int value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}
	}

	public abstract class RangedDoubleProperty : RangedNumericProperty<double>
	{
		protected RangedDoubleProperty(double value,
			double minValue, 
			double maxValue) 
			: base(value, minValue, maxValue)
		{
		}

		public override void SetValue(double value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}
	}
}
