using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemDragBehaviour : MonoBehaviour
	{
		private Vector2 _defaultPosition;
		private Camera _camera;
		private RectTransform _rectTransform;

		private bool _beingClicked { get; set; }
		private bool _beingDragged { get; set; }
		private Vector2 _dragPosition { get; set; }
		private GameObject _dragContainer { get; set; }
		private ItemBehaviour _item { get; set; }

		private void Start()
		{
			_item = GetComponent<ItemBehaviour>();
			if (_item)
			{
				_item.StateUpdated += PositionReset;
			}
		}

		private void OnEnable()
		{
			if (_item)
			{
				_item.StateUpdated += PositionReset;
			}
		}

		private void OnDisable()
		{
			if (_item)
			{
				_item.StateUpdated -= PositionReset;
			}
		}

		public void StartPosition(Vector2 pos, Transform parent)
		{
			_defaultPosition = pos;
			_camera = parent.GetComponent<Canvas>().worldCamera;
			_rectTransform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			if (_beingClicked)
			{
				if (_item.CanActivate == false)
				{
					ClickReset();
					PositionReset();
					return;
				}
				if (_beingClicked && _dragContainer.GetComponent<ItemContainerBehaviour>().CanRelease)
				{
					var z = transform.position.z;
					_rectTransform.anchoredPosition = ((Vector2) _camera.ScreenToWorldPoint(Input.mousePosition) /
														(transform.lossyScale.x / transform.localScale.x)) - _dragPosition;
					transform.position = new Vector3(transform.position.x, transform.position.y, z);
					var raycastResults = new List<RaycastResult>();
					EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, raycastResults);
					var hits = raycastResults.Select(h => h.gameObject).ToList();
					if (!hits.Contains(_dragContainer))
					{
						_beingDragged = true;
					}
				}
			}
		}

		public bool OnClickDown(RaycastResult container)
		{
			LogProxy.Info("Item OnClick");
			if (_item.CanActivate == false)
			{
				return false;
			}
			_beingClicked = true;
			_beingDragged = false;
			_dragPosition = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) / (transform.lossyScale.x / transform.localScale.x) - _defaultPosition;
			_dragContainer = container.gameObject;
			transform.SetAsLastSibling();
			return true;
		}

		public bool OnClickUp(out int containerIndex, out bool dragged)
		{
			var containerIndexzNullable = _dragContainer?.GetComponent<ItemContainerBehaviour>()?.ContainerIndex;
			dragged = _beingDragged;
			containerIndex = containerIndexzNullable ?? -1;
			return containerIndexzNullable.HasValue;
		}

		public void ClickReset()
		{
			_beingClicked = false;
			_beingDragged = false;
			_dragContainer = null;
			_dragPosition = Vector2.zero;
		}

		public void PositionReset()
		{
			GetComponent<RectTransform>().anchoredPosition = _defaultPosition;
		}
	}
}
