using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIConstants
	{

		public const float ConnectionWidth = 1f;

		public const string ItemContainerPrefab = "ItemContainer";
		public const string ItemContainerDefaultSpriteName = "ItemContainer";
		public const string PanelItemContainerDefaultSpriteName = "ItemContainer_Panel";

		public static Color ItemContainerEnabledColor = new Color(1f, 1f, 1f, 1f);
		public static Color ItemContainerDisabledColor = new Color(1f, 1f, 1f, 0.33f);

		public const string ActivePlayerSortingLayer = "ActivePlayer";
	}
}