using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class HoverController : MonoBehaviour
	{
		private Vector2 _currentHovered;
		private bool _mobileReadyToHide;
		private string _currentText;

		private RectTransform _hoverHint;
		private static HoverController _instance;

		public void Awake()
		{
			_instance = this;
			_hoverHint = transform.GetComponentInChildren<Image>(true).RectTransform();
			_hoverHint.gameObject.SetActive(false);
		}

		/// <summary>
		/// Triggered by PointerEnter/PointerClick on some UI objects. Stores position relative to pivot for the hovered object
		/// </summary>
		public static void SetHoverObject(Transform trans)
		{
			var adjust = (Vector2.one * 0.5f) - trans.RectTransform().pivot;
			_instance._currentHovered = (Vector2)trans.position + new Vector2(trans.RectTransform().rect.width * adjust.x, trans.RectTransform().rect.height * adjust.y);
		}

		/// <summary>
		/// Triggered by PointerEnter on some UI objects. Sets the text on this object and trigger the HoverCheck method in 1 second
		/// </summary>
		public static bool DisplayHover(string text)
		{
			if (!string.IsNullOrEmpty(text) && !Input.GetMouseButton(0))
			{
				_instance._currentText = text;
				_instance.Invoke(nameof(HoverCheck), 1f);
			}
			return !string.IsNullOrEmpty(text);
		}

		/// <summary>
		/// Triggered by OnClick on some UI objects. Sets the text on this object and triggers the HoverCheck method with no delay
		/// </summary>
		public static bool DisplayHoverNoDelay(string text)
		{
			if (!string.IsNullOrEmpty(text) && !Input.GetMouseButton(0))
			{
				_instance._currentText = text;
				_instance.HoverCheck();
			}
			return !string.IsNullOrEmpty(text);
		}

		/// <summary>
		/// If object is still being hovered over, display hover-over pop-up and text and position accordingly
		/// </summary>
		private void HoverCheck()
		{
			if (_hoverHint.gameObject.activeInHierarchy || Input.GetMouseButton(0))
			{
				return;
			}
			if (_currentHovered != Vector2.zero)
			{
				_mobileReadyToHide = false;
				var canvas = GetComponentInParent<Canvas>();
				var canvasSize = canvas.transform.RectTransform().rect.size;
				var canvasCam = canvas.worldCamera;
				_hoverHint.gameObject.SetActive(true);
				_hoverHint.GetComponentInChildren<Text>(true).text = Localization.Get(_currentText);
				_hoverHint.anchoredPosition = (Vector2)canvasCam.ScreenToWorldPoint(Input.mousePosition) / (transform.lossyScale.x / transform.localScale.x);
				//reposition accordingly if pop-up would display partially off screen
				if (_currentHovered.x < _hoverHint.transform.position.x)
				{
					_hoverHint.anchoredPosition += new Vector2(_hoverHint.rect.width * 0.5f, 0);
					if (_hoverHint.anchoredPosition.x + _hoverHint.rect.width * 0.5f > canvasSize.x * 0.5f)
					{
						_hoverHint.anchoredPosition -= new Vector2(_hoverHint.rect.width, 0);
					}
				}
				else
				{
					_hoverHint.anchoredPosition -= new Vector2(_hoverHint.rect.width * 0.5f, 0);
					if (_hoverHint.anchoredPosition.x - _hoverHint.rect.width * 0.5f < -canvasSize.x * 0.5f)
					{
						_hoverHint.anchoredPosition += new Vector2(_hoverHint.rect.width, 0);
					}
				}
				if (_currentHovered.y < _hoverHint.transform.position.y)
				{
					_hoverHint.anchoredPosition += new Vector2(0, _hoverHint.rect.height * 0.5f);
					if (_hoverHint.anchoredPosition.y + _hoverHint.rect.height * 0.5f > canvasSize.y * 0.5f)
					{
						_hoverHint.anchoredPosition -= new Vector2(0, -_hoverHint.rect.height);
					}
				}
				else
				{
					_hoverHint.anchoredPosition -= new Vector2(0, _hoverHint.rect.height * 0.5f);
					if (_hoverHint.anchoredPosition.y - _hoverHint.rect.height * 0.5f < -canvasSize.y * 0.5f)
					{
						_hoverHint.anchoredPosition -= new Vector2(0, -_hoverHint.rect.height);
					}
				}
			}
		}

		/// <summary>
		/// Triggered by PointerExit on some objects. Hides the hover object and resets the expected position
		/// </summary>
		public static void HideHover()
		{
			if (Application.isMobilePlatform && !_instance._mobileReadyToHide)
			{
				_instance._mobileReadyToHide = true;
				return;
			}
			_instance._hoverHint.gameObject.SetActive(false);
			_instance._currentHovered = Vector2.zero;
		}

		private void Update()
		{
			if (Application.isMobilePlatform && _mobileReadyToHide)
			{
				if (Input.GetMouseButton(0))
				{
					HideHover();
				}
			}
		}
	}
}