using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Unity.Network;
using UnityEngine;

namespace Assets.Debugging.Scripts
{
	public class NetworkDebugMenu : MonoBehaviour
	{
		public bool AutoTickOn { get; set; }

		public bool EnableSerializer { get; set; }

		private float _elapsedInterval;

		private float _lastTickTime;

		private float _autoTickInterval = 1f/20;

		private bool _initialized;

		private string _tpsText;

		private bool _isVisible = false;

		void Start()
		{
			EnableSerializer = true;
			AutoTickOn = true;
		}

		private void OnGUI()
		{
			if (!_isVisible) return;

			if (GUILayout.Button("Debug Initialize"))
			{
				if (!_initialized)
				{
					Director.DebugInitialize();
					_initialized = true;
				}
			}
			EnableSerializer = GUILayout.Toggle(EnableSerializer, "Enable Serializer");
			AutoTickOn = GUILayout.Toggle(AutoTickOn, "Auto Tick ON");
			if (GUILayout.Button("TICK"))
			{
				DebugTick();
			}
			if (GUILayout.Button("Spawn Virus"))
			{
				DebugCommands.SpawnVirus();
			}
			if (GUILayout.Button("Damage Subsystems"))
			{
				//foreach (var subsystem in Director.Simulation.Subsystems)
				//{
				//	// TODO fix from simulation refactor
				//	// subsystem.ModulateHealth((SimulationConstants.MaxHealth + SimulationConstants.MaxShield) / 2 * -1);
				//}
			}
			GUILayout.Label(_tpsText);
			//Director.Rules.VirusesDieWithSubsystem = GUILayout.Toggle(Director.Rules.VirusesDieWithSubsystem, "Viruses Die");
			//Director.Rules.VirusesAlwaysVisible = GUILayout.Toggle(Director.Rules.VirusesAlwaysVisible, "Viruses Visible");
			//Director.Rules.VirusesSpread = GUILayout.Toggle(Director.Rules.VirusesSpread, "Viruses Spread");
			//Director.Rules.RepairItemsConsumable = GUILayout.Toggle(Director.Rules.RepairItemsConsumable, "Repair Items Consumed");
			InputHandler.DebugLog = GUILayout.Toggle(InputHandler.DebugLog, "Log InputHandler");
			PlayerCommands.DebugLog = GUILayout.Toggle(PlayerCommands.DebugLog, "Log PlayerCommands");
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				_isVisible = !_isVisible;
			}

			if (_isVisible && AutoTickOn)
			{
				DebugAutoTick();
			}
		}

		void FixedUpdate()
		{
		}

		private void OnAutoTickToggled(bool isOn)
		{
			if (isOn == false)
			{
				_elapsedInterval = 0;
			}
		}

		public void DebugAutoTick()
		{
			_elapsedInterval += Time.deltaTime;
			if (_elapsedInterval > _autoTickInterval)
			{
				DebugTick();
			}
		}

		public void DebugTick()
		{
			_elapsedInterval = 0;
			Director.Tick(EnableSerializer);
			_tpsText = (1 / (Time.time - _lastTickTime)).ToString("N1") + "TPS";
			_lastTickTime = Time.time;
		}

		private void SpawnVirus()
		{
			DebugCommands.SpawnVirus();
		}
	}

}
