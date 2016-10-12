﻿using System;

namespace PlayGen.Engine.Components.Property
{
	public abstract class RangedNumericPropertyComponent<TValue> : PropertyComponentBase<TValue>
		where TValue : struct
	{
		public TValue MaxValue { get; private set; }
		public TValue MinValue { get; private set; }

		protected RangedNumericPropertyComponent(IComponentContainer container, 
			string propertyName, 
			TValue value,
			TValue minValue, 
			TValue maxValue)
			: base( container, propertyName, value)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}
	}

	public abstract class RangedIntegerPropertyComponent : RangedNumericPropertyComponent<int>
	{
		protected RangedIntegerPropertyComponent(IComponentContainer container, 
			string propertyName, 
			int value, 
			int minValue, 
			int maxValue) 
			: base(container, propertyName, value, minValue, maxValue)
		{
		}

		public override void Set(int value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.Set(value);
		}
	}

	public abstract class RangedDoublePropertyComponent : RangedNumericPropertyComponent<double>
	{
		protected RangedDoublePropertyComponent(IComponentContainer container,
			string propertyName, 
			double value,
			double minValue, 
			double maxValue) 
			: base(container, propertyName, value, minValue, maxValue)
		{
		}

		public override void Set(double value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.Set(value);
		}
	}
}
