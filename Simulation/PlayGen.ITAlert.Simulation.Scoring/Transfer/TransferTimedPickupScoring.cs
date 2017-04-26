//using System;
//using Engine.Components;
//using Engine.Events;
//using Engine.Systems.Scoring;
//using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

//namespace PlayGen.ITAlert.Scoring.Transfer
//{
//	public class TransferTimedPickupScoring : IScoringExtension
//	{
//		private readonly EventSystem _eventSystem;

//		private IDisposable _transferACtivationSubscription;
//		private IDisposable _pickupItemSubscription;

//		public TransferTimedPickupScoring(EventSystem eventSystem)
//		{
//			_eventSystem = eventSystem;

//			_transferACtivationSubscription = _eventSystem.Subscribe<TransferActivationEvent>(OnTransferActivationEvent);
//			_pickupItemSubscription = _eventSystem.Subscribe<PickupItemEvent>(OnPickupItemEvent);
//		}

//		private void OnTransferActivationEvent(TransferActivationEvent transferActivationEvent)
//		{
			
//		}

//		private void OnPickupItemEvent(PickupItemEvent pickupItemEvent)
//		{

//		}

//		public override void Dispose()
//		{
//			base.Dispose();
//		}
//	}
//}
