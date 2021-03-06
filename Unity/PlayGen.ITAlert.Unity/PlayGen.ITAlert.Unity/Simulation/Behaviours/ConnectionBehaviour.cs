﻿using System;
using System.Linq;
using Engine.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.Unity.Utilities.Extensions;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{

	public class ConnectionBehaviour : NodeBehaviour
	{
		public Vector Position;

		[SerializeField]
		private RectTransform _rectTransform;

		[SerializeField]
		private AnimateConnectionUV _animateConnectionUv;

		#region Components

		// required
		private MovementCost _movementCost;
		private int _length;
		private GraphNode _graphNode;

		private UIEntity _head;
		private UIEntity _tail;

		#endregion

		[SerializeField]
		private float _angle;

		#region Initialization

		/// <inheritdoc />
		/// <summary>
		/// Called by the base class after the state has been set during initialization
		/// </summary>
		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _graphNode)
				&& Entity.TryGetComponent(out _movementCost))
			{
				GetSubsystems();
				DrawConnection();
				_length = _graphNode.ExitPositions.Single().Value;
				gameObject.name = $"{Name}_{_head.Id}:{_tail.Id}";
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		private void GetSubsystems()
		{
			var headId = _graphNode.EntrancePositions.Single().Key;
			var tailId = _graphNode.ExitPositions.Single().Key;
			if (Director.TryGetEntity(headId, out _head) == false)
			{
				throw new SimulationIntegrationException($"Could not find entity id {headId} as head on connection id {Entity.Id}");
			}
			if (Director.TryGetEntity(tailId, out _tail) == false)
			{
				throw new SimulationIntegrationException($"Could not find entity id {tailId} as tail on connection id {Entity.Id}");
			}
			
		}

		public override void UpdateScale(Vector3 scale)
		{
			DrawConnection();
		}

		private void DrawConnection()
		{
			var headPos = _head.GameObject.RectTransform().anchoredPosition;
			var tailPos = _tail.GameObject.RectTransform().anchoredPosition;

			var length = Math.Abs(Vector2.Distance(headPos, tailPos));
			var connectionZ = ((GameObject)Resources.Load("Connection")).transform.position.z;
			var midpoint = (headPos + tailPos) * 0.5f;
			var rectTransform = gameObject.RectTransform();

			//get the angle between the locations
			var v2 = tailPos - headPos;
			_angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

			transform.eulerAngles = new Vector3(0, 0, _angle);

			var connectionSquareSize = ((GameObject)Resources.Load(nameof(Subsystem))).transform.FindRect("ConnectionSquare").rect.width * _tail.GameObject.transform.localScale.x;
			ScaleEndPoint(headPos, connectionSquareSize / 2);
			ScaleEndPoint(tailPos, connectionSquareSize / 2);

			//LogProxy.Info($"Connection {gameObject.name}, angle: {_angle}, head: {_headPos.x},{_headPos.y}, tail {_tailPos.x},{_tailPos.y}");
			//scale and position the connection accordingly
			var relativeWeight = UIConstants.ConnectionWidth; //(SimulationConstants.ConnectionMaxWeight + 1 - EntityState.RelativeWeight) * UIConstants.ConnectionWidth;

			const float offset = 5f;

			Vector2 edgeDirectionOffset;

			switch ((int)_angle)
			{
				case 0:
					edgeDirectionOffset = new Vector2(0, -1 * offset);
					break;
				case 90:
					edgeDirectionOffset = new Vector2(offset, 0);
					break;
				case 180:
				case -180:
					edgeDirectionOffset = new Vector2(0, offset);
					break;
				case -90:
					edgeDirectionOffset = new Vector2(-1 * offset, 0);
					break;
				default:
					edgeDirectionOffset = new Vector2(0, 0);
					break;
			}

			rectTransform.anchoredPosition = new Vector3(midpoint.x + edgeDirectionOffset.x, midpoint.y + edgeDirectionOffset.y, connectionZ);
			rectTransform.sizeDelta = new Vector2(length - connectionSquareSize, 8 * relativeWeight);

		}

		private void ScaleEndPoint(Vector2 point, float scaleDelta)
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
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
			UpdateConnectionCost();
		}

		private void UpdateConnectionCost()
		{
			var movementCostScaled = (1 / (float)_movementCost.Value);
			//_connectionImage.color = _movementCostGradient.Evaluate(movementCostScaled);
			if (_animateConnectionUv != null)
			{
				_animateConnectionUv.AnimationRate = movementCostScaled * _animateConnectionUv.DefaultAnimationRate;
			}
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
		}

		#region Visitor movement

		protected override VisitorVectors GetPositionFromPathPoint(int pathPoint)
		{
			var halfWidth = _rectTransform.rect.width / 2;
			var position = Vector2.Lerp(new Vector2(-1 * halfWidth, 0), new Vector2(halfWidth, 0), (float) pathPoint/_length);
			return new VisitorVectors
						{
				Position = position,
				Rotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z + 180) % 360)
			};
		}



		#endregion

		#endregion

	}
}