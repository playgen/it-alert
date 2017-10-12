using System.Collections.Generic;

using UnityEngine;
using RAGE.Analytics;

using UnityEngine.EventSystems;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class TrackerUtility : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var raycastResults = new List<RaycastResult>();
				//gets all UI objects below the cursor
				EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, raycastResults);
				foreach (var hit in raycastResults)
				{
					RayHit(hit.gameObject.transform);
				}
			}
		}

		private void RayHit(Transform hit)
		{
			var hitParent = hit.transform.parent;
			var path = hit.transform.name;
			while (hitParent != null)
			{
				path = hitParent.name + "/" + path;
				hitParent = hitParent.transform.parent;
			}
			Tracker.T.trackedGameObject.Interacted(path, GameObjectTracker.TrackedGameObject.GameObject);
		}
	}
}
