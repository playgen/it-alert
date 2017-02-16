using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIConstants
	{
		/// <summary>
		/// Graph spacing
		/// </summary>
		public static Vector2 SubsystemSpacing = new Vector2(2.5f, -2.5f);

		/// <summary>
		/// Graph centering
		/// </summary>
		public static Vector2 NetworkOffset = new Vector2(0, -0.5f);
		public static Vector2 CurrentNetworkOffset = new Vector2(0, -0.5f);


		public static Vector3 DefaultItemScale = new Vector3(0.18f, 0.18f);
		public static Vector3 ItemPanelItemScale = new Vector3(0.25f, 0.25f);

		//TODO: these can probably be removed
		public static Vector2 LocationColliderOffset = new Vector2(0, 0);
		public static Vector2 LocationColliderSize = new Vector2(7.5f, 5f);
		public static Vector2 LocationUIOffset = new Vector2(0f, -0.8f);

		public const float ConnectionWidth = 0.05f;

		public const int PlayerTrailRendererMaterials = 5;

		public const string ItemContainerPrefab = "ItemContainer";
		public const string ItemContainerDefaultSpriteName = "ItemContainer";
		public const string PanelItemContainerDefaultSpriteName = "ItemContainer_Panel";

		public static Color ItemContainerEnabledColor = new Color(1f, 1f, 1f, 1f);
		public static Color ItemContainerDisabledColor = new Color(1f, 1f, 1f, 0.33f);
	}
}