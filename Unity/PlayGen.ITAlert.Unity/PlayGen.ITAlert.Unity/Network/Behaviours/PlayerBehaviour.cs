using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	// ReSharper disable once CheckNamespace
	public class PlayerBehaviour : ActorBehaviour
	{
		/// <summary>
		/// number of positions to store
		/// </summary>
		private const int PositionHistory = 15;

		private const int InventoryPositionHistory = 5;

		public bool IsTalking = false;

		public int? InventoryItem => null;
		// EntityState.InventoryItem; } }

		private Color _playerColor;
		public Color PlayerColor => _playerColor;

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
			base.OnInitialize();
			//GetComponent<TrailRenderer>().sortingOrder = 10;
		}


		public void SetColor(string colour)
		{
			if (colour.IndexOf("#", StringComparison.Ordinal) == -1)
			{
				colour = $"#{colour}";
			}
			if (ColorUtility.TryParseHtmlString(colour, out _playerColor))
			{
				GetComponent<SpriteRenderer>().color = _playerColor;
				GetComponent<TrailRenderer>().startColor = _playerColor;
				GetComponent<TrailRenderer>().endColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.25f);
				GetComponent<TrailRenderer>().sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
				GetComponent<TrailRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
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

		protected override void OnStateUpdated()
		{
			UpdatePosition();

			////TODO: if inventory has changed
			//_historicPositions.Enqueue(transform.position);
			//if (_historicPositions.Count == PositionHistory)
			//{
			//	_historicPositions.Dequeue();
			//	ManageInventory(_historicPositions.Count < InventoryPositionHistory ? _historicPositions.Last() : _historicPositions.ToArray()[InventoryPositionHistory - 1]);
			//}
			//DrawTrail();
		}

		#endregion

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