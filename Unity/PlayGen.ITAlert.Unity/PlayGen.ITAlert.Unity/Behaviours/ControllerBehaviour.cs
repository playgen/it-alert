using System.Collections;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Controllers;
using UnityEngine;
using PlayGen.ITAlert.Unity.GameStates;
using PlayGen.ITAlert.Unity.GameStates.Loading;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.ITAlert.Unity.Photon.Messaging;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.SUGAR.Client;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class ControllerBehaviour : MonoBehaviour
	{
		private const string GamePlugin = "RoomControllerPlugin";
		private TickStateController<TickState> _stateController;
		private string _gameVersion = "1";
		private Client _photonClient;
		private SUGARClient _sugarClient;

		private PlayGen.Photon.Unity.Client.ClientState _lastState;

		private void Awake()
		{
			DontDestroyOnLoad(transform.gameObject);

			_photonClient = CreateClient();
			_sugarClient = new SUGARClient("http://api.sugarengine.org/");
			PlayerCommands.PhotonClient = _photonClient;

			CreatePopupController();

			var stateControllerFactory = new StateControllerFactory(_photonClient);
			_stateController = stateControllerFactory.Create();
		}

		private void Start()
		{
			_stateController.Initialize(LoadingState.StateName);
		}

		private void Update()
		{
			_stateController.Tick(Time.deltaTime);
		}

		private IEnumerator ClientLoop(Client photonClient)
		{
			while (true)
			{
				if (photonClient.ClientState != _lastState)
				{
					Debug.Log("Photon State Changed to: " + photonClient.ClientState);
					_lastState = photonClient.ClientState;
				}

				switch (photonClient.ClientState)
				{
					case PlayGen.Photon.Unity.Client.ClientState.Disconnected:
						photonClient.Connect();
						yield return new WaitForSeconds(0.1f);
						break;

					case PlayGen.Photon.Unity.Client.ClientState.Connecting:
						yield return new WaitForSeconds(0.5f);
						break;

					case PlayGen.Photon.Unity.Client.ClientState.Connected:
						yield return new WaitForSeconds(1.0f);
						break;
				}

				yield return new WaitForSeconds(1.0f);
			}
		}

		private Client CreateClient()
		{
			var clientBase = gameObject.AddComponent<PhotonClientWrapper>();
			clientBase.Initialize(_gameVersion, GamePlugin);

			var client = new Client(clientBase, new ITAlertMessageSerializationHandler());

			StartCoroutine(ClientLoop(client));

			return client;
		}

		private void CreatePopupController()
		{
			var popupController = new PopupController();
			PopupUtility.LogErrorEvent += popupController.ShowErrorPopup;
			PopupUtility.StartLoadingEvent += popupController.ShowLoadingPopup;
			PopupUtility.EndLoadingEvent += popupController.HideLoadingPopup;
		}
	}
}