using System;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components.Property
{
	public abstract class RangedNumericProperty<TValue> : Property<TValue>
		where TValue : struct
	{
		public TValue MaxValue { get; private set; }
		public TValue MinValue { get; private set; }

		protected RangedNumericProperty(IEntity entity, 
			TValue value,
			TValue minValue, 
			TValue maxValue)
			: base(entity, value)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}
	}

	public abstract class RangedIntegerProperty : RangedNumericProperty<int>
	{
		protected RangedIntegerProperty(IEntity entity, 
			int value, 
			int minValue, 
			int maxValue) 
			: base(entity, value, minValue, maxValue)
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
		protected RangedDoubleProperty(IEntity entity,
			double value,
			double minValue, 
			double maxValue) 
			: base(entity, value, minValue, maxValue)
		{
		}

		public override void SetValue(double value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}
	}
}
