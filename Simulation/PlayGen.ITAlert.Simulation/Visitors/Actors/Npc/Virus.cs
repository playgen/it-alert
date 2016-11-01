using System;
using Engine.Core.Components;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors.Npc
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



		[SyncState(StateLevel.Differential)]
		public bool Visible { get; private set; }

		#region Constructors

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Virus()
		{
			
		}

		public Virus(ISimulation simulation, IComponentContainer componentContainer) 
			: base(simulation, componentContainer, NpcActorType.Virus, VirusSpeed)
		{
		}

		#endregion

		public override VirusState GenerateState()
		{
			// what state does a virus need?
			return new VirusState(Id)
			{
				Active = CurrentNode is System && HasInfectIntent(),
				Visible = Visible || Simulation.Rules.VirusesAlwaysVisible,
			};
		}

		public override void OnEnterNode(INode current)
		{
			base.OnEnterNode(current);



		}

		protected override void OnTick()
		{
			base.OnTick();

			Intent currentIntent;
			if (TryGetIntent(out currentIntent))
			{
				var moveIntent = currentIntent as MoveIntent;
				if (moveIntent != null && CurrentNode == moveIntent.Destination)
				{
					Intents.Pop();
				}

				var subsystem = CurrentNode as System;
				if (subsystem != null)
				{
					//Container.ForEachComponentImplementing<IVirusComponent>(component => component.OnTickOnSystem(this, subsystem));

					//if (_enterSystemTick%DamageInterval == 0)
					//{
					//	subsystem.ModulateHealth(DamageIncrement);
					//	if (Simulation.Rules.VirusesDieWithSystem && subsystem.IsDead)
					//	{
					//		Dispose();
					//	}
					//}

					//if (Simulation.Rules.VirusesSpread
					//	&& subsystem.Shield == 0
					//	&& _enterSystemTick > 0
					//	&& _enterSystemTick%InfectInterval == 0)
					//{
					//	InfectNeighbours(subsystem);
					//}

					_enterSystemTick++;
				}
			}
		}

		private bool HasInfectIntent()
		{
			return Intents.Any() && Intents.Peek() is InfectSystemIntent;
		}

		private void InfectNeighbours(System subsystem)
		{
			//var potentialInfections = subsystem.GetAdjacentNodes()
			//	.ToDictionary(k => k, n =>
			//{
			//	var adjacentSystem = n as System;
			//	var adjacentConnection = n as Connection;
			//	if (adjacentConnection != null)
			//	{
			//		adjacentSystem = adjacentConnection.Tail;
			//	}
			//	if (adjacentSystem == null)
			//	{
			//		throw new Exception("Unknown adjacent node type");
			//	}
			//	return adjacentSystem;
			//})
			//.Where(kvp => kvp.Value.IsInfected == false 
			//	&& kvp.Value.IsDead == false)
			//.ToList();

			//if (potentialInfections.Any())
			//{
			//	var infectee = potentialInfections.OrderByDescending(i => i.Value.Shield).First();

			//	var moveIntent = new MoveIntent(infectee.Value);
			//	var infectIntent = new InfectSystemIntent();
			//	var virus = (Virus)Simulation.CreateNpc(NpcActorType.Virus);
			//	virus.Visible = Visible;
			//	virus.SetIntents(new Intent[] { infectIntent, moveIntent });
			//	infectee.Key.AddVisitor(virus, subsystem, 0);
			//}
		}

		public void SetVisible(bool visible)
		{
			Visible = visible;
		}
	}
}
