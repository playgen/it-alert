using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	// ReSharper disable once CheckNamespace
	public class PlayerBehaviour : EntityBehaviour
	{
		/// <summary>
		/// number of positions to store
		/// </summary>
		private const int PositionHistory = 15;

		private const int InventoryPositionHistory = 5;


		/// <summary>
		/// store the players most recent positions to enable the current inventory item to follow
		/// </summary>
		private readonly Queue<Vector2> _historicPositions = new Queue<Vector2>(PositionHistory);

		public bool IsTalking = false;

		public int? InventoryItem
		{
		get { return null; }
		} // EntityState.InventoryItem; } }

			private Color _playerColor;
		public Color PlayerColor { get { return _playerColor;;} }

		[SerializeField]
		private GameObject _decorator;

		private readonly GameObject[] _trail = new GameObject[PositionHistory];

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
			//if (ColorUtility.TryParseHtmlString(EntityState.Colour, out _playerColor))
			//{
			//	GetComponent<SpriteRenderer>().color = _playerColor;

			//}
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
			_historicPositions.Enqueue(transform.position);
			if (_historicPositions.Count == PositionHistory)
			{
				_historicPositions.Dequeue();
				ManageInventory(_historicPositions.Count < InventoryPositionHistory ? _historicPositions.Last() : _historicPositions.ToArray()[InventoryPositionHistory - 1]);
			}
			DrawTrail();
		}

		#endregion

		private void DrawTrail()
		{
			for (var i = 0; i < _historicPositions.Count; i++)
			{
				if (_trail[i] != null)
				{
					_trail[i].transform.position = _historicPositions.Reverse().ToArray()[i];
				}
			}
		}

		private void ManageInventory(Vector2 itemPosition)
		{
			//if (EntityState.InventoryItem.HasValue)
			//{
			//	var item = Director.GetEntity(EntityState.InventoryItem.Value);
			//	if ((item.EntityBehaviour as ItemBehaviour).IsOnSubsystem == false)
			//	{
			//		item.GameObject.transform.position = itemPosition;
			//		//item.GameObject.transform.localScale = Vector3.one*UIConstants.ItemPlayerScale;
			//	}
			//}
		}

		public void EnableDecorator()
		{
			//_decorator.SetActive(true);

		}

		public void SetActive()
		{
			GetComponent<Renderer>().sortingOrder++;
			//transform.localScale = new Vector3(2.5f, 2.5f, 0f);

			for (var i = 0; i < PositionHistory - 1; i++)
			{
				_trail[i] = new GameObject();
				var alpha = (float) (PositionHistory - (i + 1)) / PositionHistory;

				_trail[i].transform.localScale = new Vector2(1f + alpha, 1f + alpha);
				var spriteRenderer = _trail[i].AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = Resources.Load<Sprite>("PlaceholderCircle");

				var trailColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, alpha + 0.4f);
				spriteRenderer.color = trailColor;

			}
		}
	}
}