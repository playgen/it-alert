using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Components;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemContainerBehaviour : MonoBehaviour
	{
		public const string Prefab = "ItemContainer";

		public const string DefaultSprite = "ItemContainer";

		#region game elements

		private SpriteRenderer _containerImage;

		#endregion

		public bool ClickEnable { get; set; }

		private ItemContainer _itemContainer;

		#region unity initialization

		public void Awake()
		{
			
		}

		#endregion

		public void Initialize(ItemContainer itemContainer)
		{
			_itemContainer = itemContainer;
			_containerImage = gameObject.GetComponent<SpriteRenderer>();
			_itemContainer = itemContainer;

			var itemContainerTypeName = itemContainer.GetType().Name.ToLowerInvariant();

			var sprite = Resources.Load<Sprite>(itemContainerTypeName) ?? Resources.Load<Sprite>(DefaultSprite);

			_containerImage.sprite = sprite;
		}

		#region player interaction

		public bool Capturing { get; set; }

		private bool CanActivate => true;

		public void OnClick()
		{
			Debug.Log("ItemContainer OnClick");

			if (ClickEnable && CanActivate)
			{
				Capturing = !Capturing;
			}
		}

		private bool _pulseDown;

		public void HandlePulse()
		{
			if (Capturing)
			{
				if (_pulseDown)
				{
					_containerImage.color -= new Color(0, 0, 0, 0.05f);
				}
				else
				{
					_containerImage.color += new Color(0, 0, 0, 0.05f);
				}
				if (_containerImage.color.a <= 0)
				{
					_pulseDown = false;
				}
				else if (_containerImage.color.a >= 1)
				{
					_pulseDown = true;
				}
			}
			else
			{
				_pulseDown = false;
				_containerImage.color = new Color(_containerImage.color.r, _containerImage.color.g, _containerImage.color.b, 1);
			}
		}

		#endregion
	}
}
