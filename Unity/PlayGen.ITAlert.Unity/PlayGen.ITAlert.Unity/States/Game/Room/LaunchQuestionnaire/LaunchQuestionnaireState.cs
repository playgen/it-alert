using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Serialization;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Game.Room.Initializing;
using PlayGen.Photon.Unity;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.States.Game.Room.LaunchQuestionnaire
{
	class LaunchQuestionnaireState : InputTickState
	{
		public const string StateName = "LaunchQuestionnaire";


		public override string Name => StateName;

		public LaunchQuestionnaireState(LaunchQuestionnaireStateInput input)
			: base(input)
		{
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
			LogProxy.Info("LaunchQuestionnaireState OnEnter");

			// The following string contains the key for the google form is used for the cognitive load questionnaire
			var formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

			// Google form ID
			var googleFormsURL = "https://docs.google.com/forms/d/e/"
			                     + formsKey
			                     + "/viewform?entry.1596836094="
			                     + SUGARManager.CurrentUser.Name;
			// Open the default browser and show the form
			UnityEngine.Application.OpenURL(googleFormsURL);
			UnityEngine.Application.Quit();
		}

		protected override void OnExit()
		{
			LogProxy.Info("InitializingState OnExit");
		}
	}
}