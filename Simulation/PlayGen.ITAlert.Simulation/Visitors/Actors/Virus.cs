using System;
using System.Linq;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	public class Virus : NpcActor<VirusState>, IInfection
	{
		/// <summary>
		/// number of units to move every tick
		/// </summary>
		private const int VirusSpeed = 1;

		/// <summary>
		/// Tick interval to apply damage
		/// </summary>
		private const int DamageInterval = 1;
		
		/// <summary>
		/// Damage to apply every interval
		/// </summary>
		private const int DamageIncrement = -1;

		/// <summary>
		/// Tick interval to infect adjacent systems
		/// </summary>
		private const int InfectInterval = 400;

		//private static readonly Random InfectionRNG = new Random();

		[SyncState(StateLevel.Minimal)]
		private int _enterSubsystemTick;

		[SyncState(StateLevel.Minimal)]
		public bool Visible { get; private set; }

		#region Constructors

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Virus()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		public Virus(ISimulation simulation) 
			: base(simulation, NpcActorType.Virus, VirusSpeed)
		{
		}

		#endregion

		public override VirusState GenerateState()
		{
			// what state does a virus need?
			return new VirusState(Id)
			{
				Active = CurrentNode is Subsystem && HasInfectIntent(),
				Visible = Visible || Simulation.Rules.VirusesAlwaysVisible,
			};
		}

		public override void OnEnterNode(INode current)
		{
			base.OnEnterNode(current);

			var subsystem = CurrentNode as Subsystem;
			if (subsystem != null)
			{
				_enterSubsystemTick = 0;
			}

		}

		protected override void OnTick()
		{
			Intent currentIntent;
			if (TryGetIntent(out currentIntent))
			{
				var moveIntent = currentIntent as MoveIntent;
				if (moveIntent != null && CurrentNode == moveIntent.Destination)
				{
					Intents.Pop();
				}

				var subsystem = CurrentNode as Subsystem;
				if (subsystem != null)
				{
					if (_enterSubsystemTick%DamageInterval == 0)
					{
						subsystem.ModulateHealth(DamageIncrement);
						if (Simulation.Rules.VirusesDieWithSubsystem && subsystem.IsDead)
						{
							Dispose();
						}
					}

					if (Simulation.Rules.VirusesSpread
						&& subsystem.Shield == 0
						&& _enterSubsystemTick > 0
						&& _enterSubsystemTick%InfectInterval == 0)
					{
						InfectNeighbours(subsystem);
					}

					_enterSubsystemTick++;
				}
			}
		}

		private bool HasInfectIntent()
		{
			return Intents.Any() && Intents.Peek() is InfectSystemIntent;
		}

		private void InfectNeighbours(Subsystem subsystem)
		{
			var potentialInfections = subsystem.GetAdjacentNodes()
				.ToDictionary(k => k, n =>
			{
				var adjacentSubsystem = n as Subsystem;
				var adjacentConnection = n as Connection;
				if (adjacentConnection != null)
				{
					adjacentSubsystem = adjacentConnection.Tail;
				}
				if (adjacentSubsystem == null)
				{
					throw new Exception("Unknown adjacent node type");
				}
				return adjacentSubsystem;
			})
			.Where(kvp => kvp.Value.IsInfected == false 
				&& kvp.Value.IsDead == false)
			.ToList();

			if (potentialInfections.Any())
			{
				var infectee = potentialInfections.OrderByDescending(i => i.Value.Shield).First();

				var moveIntent = new MoveIntent(infectee.Value);
				var infectIntent = new InfectSystemIntent();
				var virus = (Virus)Simulation.CreateNpc(NpcActorType.Virus);
				virus.Visible = Visible;
				virus.SetIntents(new Intent[] { infectIntent, moveIntent });
				infectee.Key.AddVisitor(virus, subsystem, 0);
			}
		}

		public void SetVisible(bool visible)
		{
			Visible = visible;
		}
	}
}
