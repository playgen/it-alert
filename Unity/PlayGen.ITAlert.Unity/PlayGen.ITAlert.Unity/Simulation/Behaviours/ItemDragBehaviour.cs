using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemDragBehaviour : MonoBehaviour
	{
		private Vector2 _defaultPosition;
		private Camera _camera;

		private bool _beingClicked { get; set; }
		private bool _beingDragged { get; set; }
		private Vector2 _dragPosition { get; set; }
		private RaycastHit2D _dragContainer { get; set; }
		private ItemBehaviour _item { get; set; }

		private void Start()
		{
			_item = GetComponent<ItemBehaviour>();
		}

		public void StartPosition(Vector2 pos, Transform parent)
		{
			_defaultPosition = pos;
			_camera = parent.GetComponent<Canvas>().worldCamera;
		}

		private void Update()
		{
			if (_beingClicked)
			{
				if (_item.CanActivate == false)
				{
					ClickReset();
					return;
				}
				var z = transform.position.z;
				GetComponent<RectTransform>().anchoredPosition = ((Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) / (transform.lossyScale.x / transform.localScale.x)) - _dragPosition;
				transform.position = new Vector3(transform.position.x, transform.position.y, z);
				if (!Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).Contains(_dragContainer))
				{
					_beingDragged = true;
				}
			}
		}

		public void OnClickDown(RaycastHit2D container)
		{
			LogProxy.Info("Item OnClick");
			if (_item.CanActivate == false)
			{
				return;
			}
			_beingClicked = true;
			_beingDragged = false;
			_dragPosition = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) / (transform.lossyScale.x / transform.localScale.x) - _defaultPosition;
			_dragContainer = container;
			transform.SetAsLastSibling();
		}

		public bool OnClickUp()
		{
			return _beingDragged;
		}

		public void ClickReset()
		{
			_beingClicked = false;
			_beingDragged = false;
			GetComponent<RectTransform>().anchoredPosition = _defaultPosition;
		}
	}
}
