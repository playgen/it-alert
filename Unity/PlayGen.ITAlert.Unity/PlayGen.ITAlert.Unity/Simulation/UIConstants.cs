using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIConstants
	{
		/// <summary>
		/// Graph spacing
		/// </summary>
		public static float SubsystemSpacingMultiplier = 2f;

		/// <summary>
		/// Graph centering
		/// </summary>
		public static Vector2 NetworkOffset = new Vector2(0, -0.5f);
		public static Vector2 CurrentNetworkOffset = new Vector2(0, -0.5f);


		public static Vector3 DefaultItemScale = new Vector3(0.15f, 0.15f, 0.15f);
		public static Vector3 ItemPanelItemScale = new Vector3(1.5f, 1.5f, 1f);

		public const float ConnectionWidth = 1f;

		public const int PlayerTrailRendererMaterials = 5;

		public const string ItemContainerPrefab = "ItemContainer";
		public const string ItemContainerDefaultSpriteName = "ItemContainer";
		public const string PanelItemContainerDefaultSpriteName = "ItemContainer_Panel";

		public static Color ItemContainerEnabledColor = new Color(1f, 1f, 1f, 1f);
		public static Color ItemContainerDisabledColor = new Color(1f, 1f, 1f, 0.33f);
	}
}