// todo replace all usages with command driven state workflow
using System.Collections.Generic;
using GameWork.Core.Commands;
using GameWork.Core.Commands.Interfaces;

namespace GameWork.Legacy.Core.Interfacing
{
	public abstract class TickableStateInterface
	{
		private readonly CommandQueue _commandQueue = new CommandQueue();

		public bool HasCommands
		{
			get { return _commandQueue.HasCommands; }
		}

		public ICommand TakeFirstCommand()
		{
			return _commandQueue.TakeFirstCommand();
		}

		public IList<ICommand> TakeAllCommands()
		{
			return _commandQueue.TakeAllCommands();
		}

		public abstract void Enter();

		public abstract void Exit();

		public virtual void Tick(float deltaTime)
		{
		}

		public virtual void Initialize()
		{
		}

		public virtual void Terminate()
		{
		}

		protected void EnqueueCommand(ICommand command)
		{
			_commandQueue.AddCommand(command);
		}

		protected void EnqueueCommands(IEnumerable<ICommand> commands)
		{
			_commandQueue.AddCommands(commands);
		}
	}
}
