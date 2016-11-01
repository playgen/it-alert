using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems.Extensions
{
	// ReSharper disable once InconsistentNaming
	public static class IItemArrayExtensions
	{
		public static bool TryGetItemIndex(this IItem[] items, IItem item, out int index)
		{
			for (index = 0; index < items.Length; index++)
			{
				if (items[index]?.Equals(item) ?? false)
				{
					return true;
				}
			}
			return false;
		}
	}
}
