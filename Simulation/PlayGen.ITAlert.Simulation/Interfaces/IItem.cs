using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IItem : IVisitor, IActivatable
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

		bool IsOnSubsystem { get; }
	}
}
