using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{

	public class ConnectionBehaviour : NodeBehaviour
	{
		[SerializeField]
		private Color[] _weightColors = new Color[]
		{
		};

		private Vector2 _headPos;
		private Vector2 _tailPos;

		public Vector Position;

		[SerializeField]
		private Image _spriteRenderer;

		#region Components

		// required
		private MovementCost _movementCost;
		private int _length;

		//optional

		#endregion

		private float _angle;

		#region Initialization

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
			var headPos = head.GameObject.transform.localPosition;
			var tailPos = tail.GameObject.transform.localPosition;

			var length = Vector2.Distance(headPos, tailPos);
			var connectionZ = ((GameObject)Resources.Load("Connection")).transform.position.z;
			var midpoint = (headPos + tailPos) * 0.5f;
			transform.position = new Vector3(midpoint.x, midpoint.y, connectionZ);

			//get the angle between the locations
			Vector2 v2 = tailPos - headPos;
			_angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

			var connectionSquareSize = ((GameObject)Resources.Load(nameof(Subsystem))).transform.FindChild("ConnectionSquare").GetComponent<RectTransform>().rect.width * tail.GameObject.transform.localScale.x;
			_headPos = ScaleEndPoint(headPos, connectionSquareSize / 2);
			_tailPos = ScaleEndPoint(tailPos, connectionSquareSize / 2);

			//scale and position the connection accordingly
			var relativeWeight = UIConstants.ConnectionWidth; //(SimulationConstants.ConnectionMaxWeight + 1 - EntityState.RelativeWeight) * UIConstants.ConnectionWidth;

			var rectTransform = GetComponent<RectTransform>();

			rectTransform.sizeDelta = new Vector2(length - connectionSquareSize, 8 * relativeWeight);
			transform.eulerAngles = new Vector3(0, 0, _angle);
		}

		private Vector2 ScaleEndPoint(Vector2 point, float scaleDelta)
		{
			if (point.x > transform.localPosition.x)
			{
				point.x -= scaleDelta;
			}
			else if (point.x < transform.localPosition.x)
			{
				point.x += scaleDelta;
			}
			if (point.y > transform.localPosition.y)
			{
				point.y -= scaleDelta;
			}
			else if (point.y < transform.localPosition.y)
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