using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public abstract class ActorBehaviour : EntityBehaviour
	{
		protected CurrentLocation CurrentLocation { get; private set; }
		public UIEntity CurrentLocationEntity { get; private set; }
		
		protected VisitorPosition VisitorPosition { get; private set; }

		protected void UpdatePosition()
		{
			UIEntity currentLocationEntity;
			if (CurrentLocation.Value.HasValue && Director.TryGetEntity(CurrentLocation.Value.Value, out currentLocationEntity))
			{
				CurrentLocationEntity = currentLocationEntity;

				var nodeBehaviour = CurrentLocationEntity.EntityBehaviour as NodeBehaviour;
				if (nodeBehaviour == null)
				{
					Debug.LogError($"EntityBehaviour for entity {Entity.Id} is not NodeBehaviour");
				}
				var position = nodeBehaviour.GetVisitorPosition(VisitorPosition.Position);
				transform.position = new Vector3(position.x, position.y, transform.position.z);
			}
			else
			{
				Debug.LogError($"Failed to load actor component(s) for UpdatePosition on entity {Entity.Id}");
			}
		}

		protected override void OnInitialize()
		{
			CurrentLocation currentLocation;
			VisitorPosition visitorPosition;
			if (Entity.TryGetComponent(out currentLocation)
				&& Entity.TryGetComponent(out visitorPosition))
			{
				CurrentLocation = currentLocation;
				VisitorPosition = visitorPosition;
			}
			else
			{
				throw new SimulationIntegrationException($"Failed to load actor component(s) for UpdatePosition on entity {Entity.Id}");
			}

			UpdatePosition();
		}

	}
}
