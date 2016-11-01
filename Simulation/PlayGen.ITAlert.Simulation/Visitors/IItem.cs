using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public interface IItem : IVisitor
	{
		ItemState GenerateState();

		bool IsConsumable { get; }

		IActor Owner { get; }

		bool HasOwner { get; }

		void SetOwnership(IActor actor);

		bool IsOwnedBy(IActor actor);

		bool CanBeActivatedBy(IActor actor);

		ItemType ItemType { get; }

		bool CanBeDropped();

		bool Drop();

		bool IsOnSystem { get; }
	}
}
