using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;

#pragma warning disable 649

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	// ReSharper disable CheckNamespace
	public class ConnectionBehaviour : NodeBehaviour
	{
		[SerializeField]
		private Color[] _weightColors = new Color[]
		{
		};

		private Vector2 _headPos;
		private Vector2 _tailPos;

		public Vector Position;

		private SpriteRenderer _spriteRenderer;

		#region Components

		// required
		private MovementCost _movementCost;
		private int _length;

		//optional

		#endregion

		private float _angle;

		#region Initialization

		public void Start()
		{
			gameObject.transform.SetParent(Director.Graph.transform, false);

		}

		public void Awake()
		{
		}

		/// <summary>
		/// Called by the base class after the state has been set during initialization
		/// </summary>
		protected override void OnInitialize()
		{
			GraphNode graphNode;
			if (Entity.TryGetComponent(out graphNode)
				&& Entity.TryGetComponent(out _movementCost))
			{
				DrawConnection(graphNode.EntrancePositions.Single().Key, graphNode.ExitPositions.Single().Key);
				_length = graphNode.ExitPositions.Single().Value;
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		private void DrawConnection(int headId, int tailId)
		{
			UIEntity head;
			UIEntity tail;
			if (Director.TryGetEntity(headId, out head) == false)
			{
				throw new SimulationIntegrationException($"Could not find entity id {headId} as head on connection id {Entity.Id}");
			}
			if (Director.TryGetEntity(tailId, out tail) == false)
			{
				throw new SimulationIntegrationException($"Could not find entity id {tailId} as tail on connection id {Entity.Id}");
			}
			var headBehaviour = (SubsystemBehaviour) head.EntityBehaviour;
			var headPos = headBehaviour.ConnectionSquare.transform.position;
			var tailBehaviour = (SubsystemBehaviour) tail.EntityBehaviour;
			var tailPos = tailBehaviour.ConnectionSquare.transform.position;

			//get distance between start and end points
			var distance = Vector2.Distance(headPos, tailPos);

			//move connection to centre point between locations
			transform.position = ((headPos + tailPos) * 0.5f);

			//get the angle between the locations
			Vector2 v2 = tailPos - headPos;
			_angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

			//scale and position the connection accordingly
			var relativeWeight = UIConstants.ConnectionWidth; //(SimulationConstants.ConnectionMaxWeight + 1 - EntityState.RelativeWeight) * UIConstants.ConnectionWidth;
			transform.localScale = new Vector2(relativeWeight, distance - (tailBehaviour.ConnectionSquareRadius * 2));
			transform.eulerAngles = new Vector3(0, 0, _angle + 90);

			//GetComponent<SpriteRenderer>().color = _weightColors[EntityState.RelativeWeight - 1];
			//transform.Find("Start Node").GetComponent<SpriteRenderer>().color = _weightColors[EntityState.RelativeWeight - 1];
			//transform.Find("End Node").GetComponent<SpriteRenderer>().color = _weightColors[EntityState.RelativeWeight - 1];

			//adjust node images to correct scale
			_headPos = ScaleEndPoint(headPos, tailBehaviour.ConnectionSquareRadius);
			_tailPos = ScaleEndPoint(tailPos, tailBehaviour.ConnectionSquareRadius);

			transform.Find("Start Node").position = _headPos;
			transform.Find("End Node").position = _tailPos;
		}

		private Vector2 ScaleEndPoint(Vector2 point, float scaleDelta)
		{
			if (point.x > transform.position.x)
			{
				point.x -= scaleDelta;
			}
			else if (point.x < transform.position.x)
			{
				point.x += scaleDelta;
			}
			if (point.y > transform.position.y)
			{
				point.y -= scaleDelta;
			}
			else if (point.y < transform.position.y)
			{
				point.y += scaleDelta;

			}
			return point;
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
		}

		#region Visitor movement

		protected override Vector3 GetPositionFromPathPoint(int pathPoint)
		{
			return Vector3.Lerp(_headPos, _tailPos, (float) pathPoint/_length);
		}

		#endregion

		#endregion

	}
}