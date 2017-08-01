using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus
{
	public class AnalysisTargetItemContainer : TargetItemContainer
	{
		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		public AnalysisTargetItemContainer(ComponentMatcherGroup<Capture> captureMatcherGroup)
		{
			_captureMatcherGroup = captureMatcherGroup;
		}

		public override bool CanContain(int itemId)
		{
			return _captureMatcherGroup.TryGetMatchingEntity(itemId, out var captureEntity) && base.CanContain(itemId);
		}
	}
}
