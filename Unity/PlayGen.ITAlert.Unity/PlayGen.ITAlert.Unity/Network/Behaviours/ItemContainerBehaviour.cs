using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Components;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	public class ItemContainerBehaviour : MonoBehaviour
	{
		public const string Prefab = "ItemContainer";

		public const string DefaultSprite = "ItemContainer";

		#region game elements

		private SpriteRenderer _containerImage;

		#endregion

		private ItemContainer _itemContainer;

		public ItemContainerBehaviour(ItemContainer itemContainer)
		{
			_itemContainer = itemContainer;
		}

		#region unity initialization

		public void Awake()
		{
			_containerImage = gameObject.GetComponent<SpriteRenderer>();
		}

		#endregion

		public void Initialize(ItemContainer itemContainer)
		{
			_itemContainer = itemContainer;

			var itemContainerTypeName = itemContainer.GetType().Name.ToLowerInvariant();

			var sprite = Resources.Load<Sprite>(itemContainerTypeName) ?? Resources.Load<Sprite>(DefaultSprite);

			_containerImage.sprite = sprite;
		}
	}
}
