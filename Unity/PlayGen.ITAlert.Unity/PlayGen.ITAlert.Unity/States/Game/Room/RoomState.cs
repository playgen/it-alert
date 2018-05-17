﻿using System;
using System.Collections.Generic;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
using PlayGen.ITAlert.Unity.States.Game.SimulationEventSummary;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Messages.Error;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Unity.States.Game.Room
{
	public class RoomState : InputTickState
	{
		public const string StateName = "RoomState";
		public override string Name => StateName;
		
		private readonly RoomStateControllerFactory _controllerFactory;
		private readonly ITAlertPhotonClient _photonClient;

		private TickStateController _stateController;
		private VoiceController _voiceController;

		private readonly Director _director;
	    private readonly SimulationSummary _simulationSummary;

	    public RoomState(RoomStateInput roomStateInput, ITAlertPhotonClient photonClient,
		    SimulationSummary simulationSummary) 
			: base(roomStateInput)
		{
			_director = roomStateInput.Director;
			_director.Exception += GameExceptionHandler.OnException;
			_photonClient = photonClient;
			_controllerFactory = new RoomStateControllerFactory(_director, photonClient, _simulationSummary);
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_controllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			LogProxy.Info("RoomState: OnEnter");


			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.Error, ProcessErrorMessage);

			_voiceController = new VoiceController(_photonClient, _director);

			_stateController = _controllerFactory.Create();
			_stateController.Initialize();

			if (!GameExceptionHandler.HasException)
			{
				_stateController.EnterState(LobbyState.StateName);
			}
		}

		private void ProcessErrorMessage(Message message)
		{
			if (message is ErrorMessage errorMessage)
		    {
		        LogProxy.Error($"Received server error message: {errorMessage.Message}");
		        throw new Exception(errorMessage.Message);
		    }
		}

		protected override void OnExit()
		{
			LogProxy.Info("RoomState: OnExit");


			_director.StopWorker();
			_director.ResetDirector();

			_photonClient.CurrentRoom?.Leave();

			// CurrentRoom and messenger would have been destroyed by this point so no need to unsubscribe
			_stateController.Terminate();

            PlayGen.Unity.Utilities.Loading.Loading.Stop();
		}

		protected override void OnTick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_stateController.ActiveStateName != null)
			{
				_stateController.Tick(deltaTime);
			}
		}
	}
}