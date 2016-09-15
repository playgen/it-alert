using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Contracts;
using UnityEngine;


// ReSharper disable once CheckNamespace
public class PlayerBehaviour : EntityBehaviour<PlayerState>
{
	/// <summary>
	/// number of positions to store
	/// </summary>
	private const int InventoryPositionHistory = 5;

	/// <summary>
	/// store the players most recent positions to enable the current inventory item to follow
	/// </summary>
	private readonly Queue<Vector2> _inventoryPositions = new Queue<Vector2>(InventoryPositionHistory);

	public bool IsTalking = false;

	public int? InventoryItem { get { return EntityState.InventoryItem; } }

	private Color _playerColor;
	public Color PlayerColor { get { return _playerColor;;} }

	[SerializeField]
	private GameObject _decorator;

	#region Initialization

	public void Start()
	{
		
	}

	public void Awake()
	{
		
	}

	protected override void OnInitialize()
	{
		SetColor();
		//GetComponent<TrailRenderer>().sortingOrder = 10;
	}


	private void SetColor()
	{
		if (ColorUtility.TryParseHtmlString(EntityState.Colour, out _playerColor))
		{
			GetComponent<SpriteRenderer>().color = _playerColor;

		}
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
		//TODO: if inventory has changed
		ManageInventory();
	}

	#endregion

	private void ManageInventory()
	{
		if (EntityState.InventoryItem.HasValue)
		{
			var item = Director.GetEntity(EntityState.InventoryItem.Value).GameObject;

			_inventoryPositions.Enqueue(transform.position);
			if (_inventoryPositions.Count == InventoryPositionHistory)
			{
				item.transform.position = _inventoryPositions.Dequeue();
				item.transform.localScale = Vector3.one * UIConstants.ItemPlayerScale;
			}
		}
		else
		{
			_inventoryPositions.Clear();
		}

	}

	public void EnableDecorator()
	{
		//_decorator.SetActive(true);

	}
}
