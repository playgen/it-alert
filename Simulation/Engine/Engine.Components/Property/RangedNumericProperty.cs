using System;
using Engine.Core.Components;

namespace Engine.Components.Property
{
	public abstract class RangedNumericProperty<TValue> : Property<TValue>
		where TValue : struct
	{
		public TValue MaxValue { get; private set; }
		public TValue MinValue { get; private set; }

		protected RangedNumericProperty(IComponentContainer container, 
			string propertyName, 
			bool includeInState,
			TValue value,
			TValue minValue, 
			TValue maxValue)
			: base( container, propertyName, includeInState, value)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}
	}

	public abstract class RangedIntegerProperty : RangedNumericProperty<int>
	{
		protected RangedIntegerProperty(IComponentContainer container, 
			string propertyName, 
			bool includeInState,
			int value, 
			int minValue, 
			int maxValue) 
			: base(container, propertyName, includeInState, value, minValue, maxValue)
		{
		}

		public override void Set(int value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.Set(value);
		}
	}

	public abstract class RangedDoubleProperty : RangedNumericProperty<double>
	{
		protected RangedDoubleProperty(IComponentContainer container,
			string propertyName, 
			bool includeInState,
			double value,
			double minValue, 
			double maxValue) 
			: base(container, propertyName, includeInState, value, minValue, maxValue)
		{
		}

		public override void Set(double value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.Set(value);
		}
	}
}
