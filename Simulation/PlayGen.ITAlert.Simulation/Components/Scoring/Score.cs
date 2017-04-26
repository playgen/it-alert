using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Scoring
{
    public class Score : IComponent
    {
		public int ResourceManagement { get; set; }

		public int Systematicity { get; set; }
	}
}
