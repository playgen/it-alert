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

		public override bool CanCapture(int? itemId = null)
		{
			ComponentEntityTuple<Capture> captureEntity;
			return itemId.HasValue && _captureMatcherGroup.TryGetMatchingEntity(itemId.Value, out captureEntity);
		}
	}
}
