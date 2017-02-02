using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
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
				//Debug.Log($"Actor {Entity.Id} curentLocation: {currentLocation.Value}, visitorPosition: {visitorPosition.Position}");

				var nodeBehaviour = currentLocationEntity.EntityBehaviour as NodeBehaviour;
				if (nodeBehaviour == null)
				{
					Debug.LogError($"EntityBehaviour for entity {Entity.Id} is not NodeBehaviour");
				}
				var position = nodeBehaviour.GetVisitorPosition(visitorPosition.Position);
				transform.position = position;
			}
			else
			{
				Debug.LogError($"Failed to load actor component(s) for UpdatePosition on entity {Entity.Id}");
			}
		}

		protected override void OnInitialize()
		{
			UpdatePosition();
		}

	}
}
