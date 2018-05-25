using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Scoring
{
    public class Score : IComponent
    {
		public int ResourceManagement { get; set; }

		public int Systematicity { get; set; }

		public int PublicScore { get; set; }

	    private const int ActionValue = 100;

	    public void ActionCompleted(int multiplier)
	    {
		    PublicScore += ActionValue * multiplier;
	    }
	}
}
