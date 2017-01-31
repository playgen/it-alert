using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Interfaces;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Loading
{
	public class LoadingState : InputTickState, ICompletable
	{
		public const string StateName = nameof(LoadingState);

		public override string Name => StateName;

		private float _timer;
		private float _splashDelay = 2;

		public bool IsComplete { get; private set; }

		public LoadingState(LoadingStateInput input) : base(input)
		{
		}

		protected override void OnEnter()
		{
			IsComplete = false;
			_timer = 0;
		}

		protected override void OnTick(float deltaTime)
		{
			if (_timer >= _splashDelay)
			{
				IsComplete = true;
			}
			_timer += deltaTime;
		}
	}
}