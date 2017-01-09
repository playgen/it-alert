using System;
using System.Linq;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

// ReSharper disable once CheckNamespace
public class SubsystemBehaviour : EntityBehaviour<SubsystemState>
{
	//01
	//23
	private static readonly Vector2[] CornerItemOffsets = new[]
	{
		new Vector2(-0.34f, 0.18f),
		new Vector2(0.34f, 0.18f),
		new Vector2(-0.34f, -0.18f),
		new Vector2(0.34f, -0.18f),
	};

	#region game elements

	private Text _nameText;
	private GameObject _health;
	private GameObject _shield;
	private SpriteRenderer _iconRenderer;
	private SpriteRenderer _filled;

	public BoxCollider2D DropCollider
	{
		get; private set;
	}

	// everything is centered around this
	private GameObject _connectionSquare;

	public GameObject ConnectionSquare
	{
		get { return _connectionSquare; }
	}

	private BoxCollider2D _connectionSquareCollider;

	public BoxCollider2D ConnectionSquareCollider
	{
		get { return _connectionSquareCollider; }
	}

	public float ConnectionSquareRadius
	{
		get { return _connectionSquare.transform.localScale.x * (_connectionSquareCollider.size.x / 2); }
	}


	#endregion

	private Vector2[] _itemPositions;

	#region public state 

	public int LogicalId { get { return EntityState.LogicalId; } }

	public bool HasActiveItem { get { return EntityState.HasActiveItem; } }

	#endregion

	#region movement constants

	private static float _connectionScaleCoefficient;
	private static readonly int PathPointsInSubsystem = SimulationConstants.Positions;
	private const int SquareSideCount = 4;
	private static readonly float PointsPerSide = (float)PathPointsInSubsystem / SquareSideCount;

	#endregion

	#region Initialization

	public void Start()
	{

	}

	public void Awake()
	{
		_iconRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
		_filled = transform.Find("Filled").GetComponent<SpriteRenderer>();

		//var material = Resources.Load<Material>("DefaultMaterial");
		//_iconRenderer.transparentMaterial = material;
		//_filled.transparentMaterial = material;

		_nameText = transform.Find("Canvas/Name").GetComponent<Text>();

		_health = transform.Find("Canvas/Health/Amount").gameObject;
		_shield = transform.Find("Canvas/Shield/Amount").gameObject;

		_connectionSquare = transform.Find("Connections").gameObject;
		_connectionSquareCollider = _connectionSquare.GetComponent<BoxCollider2D>();
		_connectionScaleCoefficient = 1/_connectionSquare.transform.localScale.x;

		DropCollider = GetComponent<BoxCollider2D>();
	}

	protected override void OnInitialize()
	{
		SetPosition();
		SetName();

		OnUpdatedState();
	}

	private void SetPosition()
	{
		transform.position = new Vector3(UIConstants.NetworkOffset.x + (UIConstants.SubsystemSpacing.x * EntityState.X), UIConstants.NetworkOffset.y + (UIConstants.SubsystemSpacing.y * EntityState.Y));
		_itemPositions = CornerItemOffsets.Select(c => c + (Vector2)_connectionSquare.transform.position).ToArray();
	}

	#endregion

	#region Unity Update

	protected override void OnFixedUpdate()
	{
	}

	protected override void OnUpdate()
	{
		//	MoveVisitors(_pathPositionsPerSecond * Time.deltaTime);
	}

	#endregion

	#region State Update

	protected override void OnUpdatedState()
	{
		SetHealth();
		SetShield();

		MoveVisitors();

		SetSystemProperties();
		SetItems();

		//if (EntityState.VisitorPositions.Count > 0)
		//{
		//	FadeInUpdate();
		//}
		//else
		//{
		//	FadeOutUpdate();
		//}
	}

	#region static properties

	private void SetName()
	{
		_nameText.text = EntityState.Name;
	}

	private void SetHealth()
	{
		_health.transform.localScale = new Vector3(EntityState.Health, 1);
	}

	private void SetShield()
	{
		_shield.transform.localScale = new Vector3(EntityState.Shield, 1);
	}

	private void SetSystemProperties()
	{
		if (EntityState.Disabled)
		{
			//SetName("SYSTEM OFFLINE");
			_iconRenderer.color = new Color(0,0,0,0.5f);
		}
		else if (EntityState.Infection.HasValue)
		{
			Color infectedColor;
			ColorUtility.TryParseHtmlString("#E32730", out infectedColor);
			_iconRenderer.color = infectedColor - new Color(0, 0, 0, 1 - _iconRenderer.color.a);

			var virus = Director.GetEntity(EntityState.Infection.Value);
			//TODO: get center of square properly
			virus.GameObject.transform.position = transform.position;
		}
		else
		{
			_iconRenderer.color = Color.white;
			//_nameText.text = Name.ToUpper();
		}
	}

	public void SetItems()
	{
		for (var i = 0; i < EntityState.ItemPositions.Length; i++)
		{
			if (EntityState.ItemPositions[i].HasValue)
			{
				var item = Director.GetEntity(EntityState.ItemPositions[i].Value);
				var itemBehaviour = item.GameObject.GetComponent<ItemBehaviour>();
				if (itemBehaviour.Dragging == false)
				{
					if (itemBehaviour.IsActive)
					{
						item.GameObject.transform.position = _connectionSquare.transform.position;
					}
					else
					{
						item.GameObject.transform.position = _itemPositions[i];
					}
				}
			}
		}
	}

	#endregion

	#region Visitor Movement

	private void MoveVisitors()
	{
		foreach (var visitor in EntityState.VisitorPositions)
		{
			UpdateVisitorMovement(Director.GetEntity(visitor.Key).GameObject, visitor.Value);
		}
	}

	private void UpdateVisitorMovement(GameObject visitor, int pathPoint)
	{
		var position = GetPositionFromPathPoint(pathPoint);
		visitor.transform.position = position;
	}

	private Vector3 GetPositionFromPathPoint(int pathPoint)
	{
		var localPosition = GetPositionOnSquare(pathPoint);
		localPosition.y *= -1;                  // Flip Y so displays correctly relative to the view
		localPosition *= _connectionSquare.transform.localScale.x;
		localPosition += _connectionSquare.transform.position;    // Move relative to this subsystem

		return localPosition;
	}

	private Vector3 GetPositionOnSquare(int pathPoint)
	{
		var squarePermimiterSideScale = 1f;
		var offsetPositionAlong = GetOffsetPositionAlongSquare(pathPoint, squarePermimiterSideScale);

		// Position on square perimeter
		// Top Left = (-1, -1), Bottom Right = (1, 1)
		var sideLength = _connectionScaleCoefficient * _connectionSquareCollider.bounds.size.x;
		var halfSide = sideLength/2;
		//var step = sideLength / PointsPerSide * 2;
		var localPositionAlong = (offsetPositionAlong % squarePermimiterSideScale) * sideLength;

		// TOP
		if (offsetPositionAlong < squarePermimiterSideScale)
		{
			return new Vector3((localPositionAlong) - halfSide, -halfSide, 0);
		}
		// RIGHT
		else if (offsetPositionAlong < 2 * squarePermimiterSideScale)
		{
			return new Vector3(halfSide, (localPositionAlong) - halfSide, 0);
		}
		// BOTTOM
		else if (offsetPositionAlong < 3 * squarePermimiterSideScale)
		{
			return new Vector3(halfSide - (localPositionAlong), halfSide, 0);
		}
		// LEFT
		else if (offsetPositionAlong < 4 * squarePermimiterSideScale)
		{
			return new Vector3(-halfSide, halfSide - (localPositionAlong), 0);
		}
		else
		{
			throw new Exception("This should never be hit.");
		}
	}

	private float GetOffsetPositionAlongSquare(int pathPoint, float scale)
	{
		
		pathPoint = pathPoint % PathPointsInSubsystem;   // Wrap around

		// Point along the perimeter as if it were a straight line.
		var halfScale = scale / 2;
		
		var progressAlongSide = pathPoint / PointsPerSide;

		var offsetPositionAlong = (progressAlongSide + halfScale) % (SquareSideCount * scale);  // Offsets the position so that entry points are half way along each square side.

		return offsetPositionAlong;
	}

	#endregion

	#endregion

	public void FadeInUpdate()
	{
		if (_filled.color.a < 1)
		{
			_filled.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			_iconRenderer.color -= new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
		}
	}

	public void FadeOutUpdate()
	{
		if (_filled.color.a > 0)
		{
			_filled.color -= new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			_iconRenderer.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
		}
	}

}

