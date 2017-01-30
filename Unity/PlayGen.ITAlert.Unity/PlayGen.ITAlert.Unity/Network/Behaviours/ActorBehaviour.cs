using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	public abstract class ActorBehaviour : EntityBehaviour
	{
		protected void UpdatePosition()
		{
			CurrentLocation currentLocation;
			VisitorPosition visitorPosition;
			UIEntity currentLocationEntity;
			if (Entity.TryGetComponent(out currentLocation)
				&& Entity.TryGetComponent(out visitorPosition)
				&& Director.TryGetEntity(currentLocation.Value, out currentLocationEntity))
			{
				var nodeBehaviour = currentLocationEntity.EntityBehaviour as NodeBehaviour;
				if (nodeBehaviour != null)
				{
					var position = nodeBehaviour.GetVisitorPosition(visitorPosition.Position);
					transform.position = position;

					return;
				}
			}
			Debug.LogError($"Failed to load actor component(s) for UpdatePosition on entity {Entity.Id}");
		}
	}
}
