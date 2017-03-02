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

		[SerializeField]
		private RectTransform _rectTransform;

		public float ActorZ => _rectTransform.position.z;

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
				if (transform.parent != CurrentLocationEntity.GameObject)
				{
					var scale = transform;
					transform.SetParent(CurrentLocationEntity.GameObject.transform, true);
				}
				var visitorVectors = nodeBehaviour.GetVisitorPosition(VisitorPosition.Position);
				_rectTransform.anchoredPosition = new Vector3(visitorVectors.Position.x, visitorVectors.Position.y, transform.position.z);
				_rectTransform.rotation = new Quaternion(0, 0, visitorVectors.Rotation.z, 0);
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
