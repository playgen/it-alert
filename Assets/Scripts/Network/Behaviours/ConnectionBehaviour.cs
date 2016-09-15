using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Layout;

#pragma warning disable 649

// ReSharper disable CheckNamespace
public class ConnectionBehaviour : EntityBehaviour<PlayGen.ITAlert.Simulation.Contracts.ConnectionState>
{
	private Vector2 _headPos;
	private Vector2 _tailPos;

	public Vector Position;

	//private float _pointsPerSecond = 24;

	private readonly HashSet<int> _currentVisitors = new HashSet<int>();

	private float _angle;

	#region Initialization

	public void Start()
	{
		
	}

	public void Awake()
	{
		
	}

	/// <summary>
	/// Called by the base class after the state has been set during initialization
	/// </summary>
	protected override void OnInitialize()
	{
		DrawConnection(EntityState.Head, EntityState.Tail);
	}

	private void DrawConnection(int headId, int tailId)
	{
		var head = Director.GetEntity(headId).EntityBehaviour as SubsystemBehaviour;
		var headPos = head.ConnectionSquare.transform.position;
		var tail = Director.GetEntity(tailId).EntityBehaviour as SubsystemBehaviour;
		var tailPos = tail.ConnectionSquare.transform.position;

		//get distance between start and end points
		var distance = Vector2.Distance(headPos, tailPos);

		//move connection to centre point between locations
		transform.position = ((headPos + tailPos) * 0.5f);

		//get the angle between the locations
		Vector2 v2 = tailPos - headPos;
		_angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

		//scale and position the connection accordingly
		var relativeWeight = (SimulationConstants.ConnectionMaxWeight + 1 - EntityState.RelativeWeight) * UIConstants.ConnectionWidth;
		transform.localScale = new Vector2(relativeWeight, distance - (tail.ConnectionSquareRadius * 2));
		transform.eulerAngles = new Vector3(0, 0, _angle + 90);

		//adjust node images to correct scale
		_headPos = ScaleEndPoint(headPos, tail.ConnectionSquareRadius);
		_tailPos = ScaleEndPoint(tailPos, tail.ConnectionSquareRadius);

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

	protected override void OnUpdatedState()
	{
		MoveVisitors();
	}

	#region Visitor movement

	private void MoveVisitors()
	{
		foreach (var visitor in EntityState.VisitorPositions)
		{
			UpdateVisitorMovement(Director.GetEntity(visitor.Key), visitor.Value);
		}
		_currentVisitors.RemoveWhere(v => EntityState.VisitorPositions.ContainsKey(v) == false);
	}

	private void UpdateVisitorMovement(UIEntity visitor, int pathPoint)
	{
		var position = GetPositionFromPathPoint(pathPoint);
		visitor.GameObject.transform.position = position;
		if (_currentVisitors.Contains(visitor.Id) == false)
		{
			_currentVisitors.Add(visitor.Id);
			visitor.GameObject.transform.eulerAngles = new Vector3(0, 0, _angle + 180);
		}
	}

	private Vector3 GetPositionFromPathPoint(int pathPoint)
	{
		return Vector3.Lerp(_headPos, _tailPos, (float)pathPoint / EntityState.Weight);
	}


	#endregion

	#endregion

}
