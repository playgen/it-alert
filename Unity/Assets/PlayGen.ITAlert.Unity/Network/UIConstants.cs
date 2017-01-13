using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network
{
	// ReSharper disable once InconsistentNaming
	// ReSharper disable once CheckNamespace
	public class UIConstants
	{
		/// <summary>
		/// Graph spacing
		/// </summary>
		public static Vector2 SubsystemSpacing = new Vector2(1.5f, -1.5f);

		/// <summary>
		/// Graph centering
		/// </summary>
		public static Vector2 NetworkOffset = new Vector2(0, -0.5f);


		public const float ItemLocationScale = 0.75f;
		public const float ItemPlayerScale = 0.5f;
		public const float ItemActiveScale = 1.0f;

		//TODO: these can probably be removed
		public static Vector2 LocationColliderOffset = new Vector2(0, 0);
		public static Vector2 LocationColliderSize = new Vector2(7.5f, 5f);
		public static Vector2 LocationUIOffset = new Vector2(0f, -0.8f);

		public const float ConnectionWidth = 0.02f;

		public const int PlayerTrailRendererMaterials = 5;
	}
}