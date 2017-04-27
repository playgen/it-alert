using System;
using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Scoring.Team
{
	public interface ITeamScoringExtension : ISystemExtension
	{
		event Action<int> Score;
	}
}
