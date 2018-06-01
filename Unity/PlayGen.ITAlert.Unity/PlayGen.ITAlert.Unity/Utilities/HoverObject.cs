using PlayGen.ITAlert.Unity.Controllers;

using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class HoverObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private string _hoverText;

		/// <summary>
		/// Set text and pop-up for this object
		/// </summary>
		public void SetHoverText(string text)
		{
			_hoverText = text;
		}

		/// <summary>
		/// Triggered by Unity UI. If enabled set text to be displayed if user continues to hover
		/// </summary>
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (Application.isMobilePlatform)
			{
				if (HoverController.DisplayHoverNoDelay(_hoverText))
				{
					HoverController.SetHoverObject(transform);
				}
			}
			else
			{
				if (HoverController.DisplayHover(_hoverText))
				{
					HoverController.SetHoverObject(transform);
				}
			}
		}

		/// <summary>
		/// Triggered by Unity UI. If enabled hide hover pop-up
		/// </summary>
		public void OnPointerExit(PointerEventData eventData)
		{
			HoverController.HideHover();
		}
	}
}