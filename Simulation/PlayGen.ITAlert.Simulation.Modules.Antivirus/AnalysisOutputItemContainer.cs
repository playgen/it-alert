using Engine.Components;

using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus
{
	public class AnalysisOutputItemContainer : OutputItemContainer
	{
		private readonly ComponentMatcherGroup<Components.Antivirus> _outputMatcherGroup;

		public AnalysisOutputItemContainer(ComponentMatcherGroup<Components.Antivirus> outputMatcherGroup)
		{
			_outputMatcherGroup = outputMatcherGroup;
		}

		public override bool CanContain(int itemId)
		{
			return _outputMatcherGroup.TryGetMatchingEntity(itemId, out var outputEntity) && base.CanContain(itemId);
		}
	}
}
