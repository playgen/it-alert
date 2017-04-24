using Engine.Components;

namespace PlayGen.ITAlert.Scoring.Components
{
    public class Score : IComponent
    {
		public int ResourceManagement { get; set; }

		public int Systematicity { get; set; }
	}
}
