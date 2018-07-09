using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Logging.Loggers;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public static class GameObjectUtilities
	{
		/// <summary>
		/// Breadth first search for game objects.
		/// 
		/// Root cannot be inactive.
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <returns></returns>
		public static Transform[] FindAll(string absolutePath)
		{
			var segments = absolutePath.Split('/');
			var level = 0;

			var childObject = GameObject.Find(segments[level]);

			if (childObject == null)
			{
				LogProxy.Warning("Couldn't find any object at path: " + absolutePath);
				return null;
			}

			var rootTransform = childObject.transform;

			var currentLevel = new List<Transform> {rootTransform};
			var nextLevel = new List<Transform>();

			var matches = FindMatches(++level, segments, currentLevel, nextLevel);

			return matches.ToArray();
		}

		public static GameObject[] FindAllGameObjects(string absolutePath)
		{
			var results = FindAll(absolutePath);
			return results.Select(t => t.gameObject).ToArray();
		}

		/// <summary>
		/// Breadth first search for game objects.
		/// 
		/// Root cannot be inactive.
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <returns></returns>
		public static Transform Find(string absolutePath)
		{
			var results = FindAll(absolutePath);

			if (results.Length != 1)
			{
				LogProxy.Warning(results.Length == 0 ? $"Couldn't find any objects matching the path: \"{absolutePath}\"" : $"Found {results.Length} objects matching the path: \"{absolutePath}\"");
				return null;
			}

			return results[0];
		}

		public static GameObject FindGameObject(string absolutePath)
		{
			var result = Find(absolutePath);
			return result.gameObject;
		}

		public static GameObject[] FindAllChildren(string absolutePath)
		{
			var result = Find(absolutePath);

			var childCount = result.childCount;

			if (childCount < 1)
			{
				LogProxy.Warning($"Couldn't find any children of the object matching the path: \"{absolutePath}\"");
				return null;
			}

			var children = new List<Transform>();

			for (var i = 0; i < childCount; i++)
			{
				children.Add(result.GetChild(i));
			}

			return children.Select(t => t.gameObject).ToArray();
		}

		private static List<Transform> FindMatches(int level, string[] pathSegments, List<Transform> currentLevel, List<Transform> nextLevel)
		{
			if (level < pathSegments.Length)
			{
				foreach (var transform in currentLevel)
				{
					for (var i = 0; i < transform.childCount; i++)
					{
						if (transform.GetChild(i).name == pathSegments[level])
						{
							nextLevel.Add(transform.GetChild(i));
						}
					}
				}

				currentLevel.Clear();
				level++;

				return FindMatches(level, pathSegments, nextLevel, currentLevel);
			}

			return currentLevel;
		}

	}
}