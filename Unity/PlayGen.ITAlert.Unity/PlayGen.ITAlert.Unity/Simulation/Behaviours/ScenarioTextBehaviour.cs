using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ScenarioTextBehaviour : EntityBehaviour
	{
		private ITAlert.Simulation.Components.Tutorial.Text _textComponent;

		private Text _text;

		#region Initialization

		public void Awake()
		{
			_text = GetComponentInChildren<Text>();
			gameObject.transform.SetParent(Director.Canvas.transform, false);

		}

		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _textComponent))
			{
				_text.text = _textComponent.Value;
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
		}

		protected override void OnStateUpdated()
		{
		}

		#endregion
	}
}
