using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	public class TextBehaviour : MonoBehaviour //EntityBehaviour
	{
		private string _text;

		#region Initialization

		public void Start()
		{
			var textObject = GetComponentInChildren<Text>();
			if (textObject)
			{
				textObject.text = _text;
			}
		}

		#endregion
	}
}
