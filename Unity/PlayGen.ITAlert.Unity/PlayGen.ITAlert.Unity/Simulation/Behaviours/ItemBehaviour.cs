using System.Collections.Generic;
using System.Linq;

using Engine.Systems.Activation.Components;

using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.UI.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Extensions;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemBehaviour : EntityBehaviour
	{
		#region editor fields

		[SerializeField]
		private Image _activationTimerImage;

		[SerializeField]
		private Image _foregroundSprite;

		[SerializeField]
		private Text _foregroundText;

		[SerializeField]
		private Image _midgroundSprite;

		[SerializeField]
		private Image _backgroundSprite;

		[SerializeField]
		private GameObject _selectionOptions;

		[SerializeField]
		private GameObject _leftButton;

		[SerializeField]
		private GameObject _middleButton;

		[SerializeField]
		private GameObject _rightButton;

		[SerializeField]
		private Text _descriptionText;

		#endregion

		#region components


		// required
		private CurrentLocation _currentLocation;
		private Owner _owner;
		private IItemType _itemType;
		private Activation _activation;

		// optional components
		private TimedActivation _timedActivation;

		private Antivirus _antivirus;
		private ConsumableActivation _consumableActivation;
		private Capture _capture;

		private bool _moveState;
		private Vector2? _selectPos;

		public CurrentLocation CurrentLocation => _currentLocation;

		#endregion

		#region Initialization

		protected override void OnInitialize()
		{
			_antivirus = null;
			_capture = null;
			_timedActivation = null;
			_consumableActivation = null;

			if (Entity.TryGetComponent(out _itemType)
				&& Entity.TryGetComponent(out _currentLocation)
				&& Entity.TryGetComponent(out _owner)
				&& Entity.TryGetComponent(out _activation))
			{
				_selectionOptions.SetActive(false);
				var spriteName = _itemType.GetType().Name.ToLowerInvariant();
				_descriptionText.GetComponent<TextLocalization>().Key = spriteName.ToUpperInvariant() + "_DESCRIPTION";
				LogProxy.Info($"Creating item type: {spriteName}");
				gameObject.name = $"{Name}_{spriteName}";
				_foregroundSprite.sprite = Resources.Load<Sprite>(spriteName);
				_foregroundText.text = string.Empty;

				Entity.TryGetComponent(out _timedActivation);
				Entity.TryGetComponent(out _antivirus);
				Entity.TryGetComponent(out _capture);
				Entity.TryGetComponent(out _consumableActivation);

				UpdateColour();

				_midgroundSprite.enabled = false;
				_midgroundSprite.type = Image.Type.Simple;
				
				//TODO: extract item specific logic to subclasses
				InitializeAntivirus();
				InitializeCapture();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
			base.OnStateUpdated();
			UpdateColour();

			//TODO: extract item specific logic to subclasses
			UpdateAntivirus();
			UpdateCapture();

			UpdateActivationTimer();
			UpdateInventory();
		}

		private void UpdateInventory()
		{
			if ((_owner?.Value.HasValue ?? false)
				&& _owner.Value.Value != (Director.Player?.Id ?? -1)
				&& _currentLocation.Value.HasValue == false)
			{
				if (GetComponentInParent<SubsystemBehaviour>() && gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
			else
			{
				if (GetComponentInParent<SubsystemBehaviour>() && !gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
			}
		}

		private void UpdateForegroundColour()
		{
		}

		private void UpdateMidgroundColour()
		{
		}

		private void UpdateColour()
		{
			if ((_owner?.Value.HasValue ?? false)
				&& _activation.ActivationState == ActivationState.Active)
			{
				if (Director.TryGetEntity(_owner.Value.Value, out var owner))
				{
					var playerColour = ((PlayerBehaviour)owner.EntityBehaviour).PlayerColor;
					_activationTimerImage.color = playerColour;
					_backgroundSprite.color = playerColour;
					_foregroundSprite.color = playerColour;
				}
			}
			else
			{
				_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);
				_backgroundSprite.color = new Color(1f, 1f, 1f, 1f);
				_foregroundSprite.color = new Color(1f, 1f, 1f, 1f);
			}
		}

		private void InitializeAntivirus()
		{
			if (_antivirus != null)
			{
				_midgroundSprite.sprite = Resources.Load<Sprite>("antivirus-full");
				if (_consumableActivation != null)
				{
					_midgroundSprite.type = Image.Type.Filled;
					_midgroundSprite.fillMethod = Image.FillMethod.Vertical;
				}
				_midgroundSprite.enabled = true;
				UpdateAntivirus();
			}
		}

		private void UpdateAntivirus()
		{
			if (_antivirus != null)
			{
				_midgroundSprite.color = _antivirus.TargetGenome.GetColourForGenome();
				if (_consumableActivation != null)
				{
					_midgroundSprite.fillAmount = (float)_consumableActivation.ActivationsRemaining / _consumableActivation.TotalActivations;
					_foregroundText.text = _consumableActivation.ActivationsRemaining > 0 ? _consumableActivation.ActivationsRemaining.ToString() : string.Empty;
				}
			}
		}

		private void InitializeCapture()
		{
			if (_capture != null)
			{
				_midgroundSprite.sprite = Resources.Load<Sprite>("virus");
				_midgroundSprite.enabled = true;
				UpdateCapture();
			}
		}

		private void UpdateCapture()
		{
			if (_capture != null)
			{
				if (_capture.CapturedGenome == 0)
				{
					_midgroundSprite.enabled = false;
				}
				else
				{
					_midgroundSprite.enabled = true;
					_midgroundSprite.color = _capture.CapturedGenome.GetColourForGenome();
				}
			}
		}

		private void UpdateActivationTimer()
		{
			if (_timedActivation != null)
			{
				if (_activation.ActivationState == ActivationState.Active)
				{
					_activationTimerImage.fillAmount = 1f - _timedActivation.GetActivationProportion();
				}
				else
				{
					_activationTimerImage.fillAmount = 0f;
				}

			}
		}

		#endregion

		#region player interaction

		public bool CanActivate => _activation.ActivationState == ActivationState.NotActive;
		#endregion

		protected override void OnUpdate()
		{
			if (_selectionOptions != null && (_selectionOptions.activeSelf || _moveState) && !IsInvoking("OptionsDelay") && !IsInvoking(nameof(ResetOptions)) && !IsInvoking("EnableOptions"))
			{
				if (Input.GetMouseButtonUp(0) || _activation.ActivationState == ActivationState.Active || transform.position != _selectPos || Director.Player.CurrentLocationEntity.EntityBehaviour.Entity.CreatedFromArchetype == nameof(ConnectionNode))
				{
					DisableOptions();
				}
			}
		}

		private void OnEnable()
		{
			LogProxy.Info("Enabled");
		}

		private void OnDisable()
		{
			if (_selectionOptions.activeSelf || _moveState)
			{
				DisableOptions();
			}
		}

		public void OnPointerClick(ItemContainerBehaviour container, Director director)
		{
			if (director.Player.CurrentLocationEntity.EntityBehaviour.Entity.CreatedFromArchetype != nameof(ConnectionNode) && !_selectionOptions.activeSelf && CanActivate && !IsInvoking("OptionsDelay") && !IsInvoking(nameof(ResetOptions)) && !IsInvoking("EnableOptions"))
			{
				_leftButton.transform.localScale = Vector3.one;
				_rightButton.transform.localScale = Vector3.one;
				_middleButton.transform.localScale = Vector3.one;
				_descriptionText.transform.parent.localScale = Vector3.one;
				GetComponent<Canvas>().sortingOrder = 100;
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				Invoke("EnableOptions", 0.34f);
				_selectionOptions.SetActive(true);
				if (container.CanRelease && CanActivate && CurrentLocation.Value != null)
				{
					var validMoveExists = true;
					var currentLocation = director.Player.CurrentLocationEntity;
					var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
					if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
					{
						var containers = subsystemBehaviour.GetComponentsInChildren<ItemContainerBehaviour>().Where(c => c != container).ToList();
						validMoveExists = containers.Any(c => c.CanContain(Id) && c.IsEnabled && (!c.TryGetItem(out var cItem) || container.CanContain(cItem.Id)));
					}
					GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory);
					var canTake = !inventory || container.CanContain(inventory.Id);
					_leftButton.SetActive(Director.CommandSystem.TryGetHandler(typeof(ActivateItemCommand), out var activateHandler) && activateHandler.Enabled);
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_leftButton.GetComponent<Button>().onClick.AddListener(Use);
					_leftButton.GetComponentInChildren<Text>().text = Localization.Get("USE_BUTTON");
					_leftButton.GetComponentInChildren<TextLocalization>().Key = "USE_BUTTON";
					_rightButton.SetActive(canTake && Director.CommandSystem.TryGetHandler(typeof(PickupItemCommand), out var pickUpHandler) && pickUpHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapInventoryItemCommand), out pickUpHandler) && pickUpHandler.Enabled);
					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.GetComponent<Button>().onClick.AddListener(() => Take(container));
					_rightButton.GetComponentInChildren<Text>().text = Localization.Get("TAKE_BUTTON");
					_rightButton.GetComponentInChildren<TextLocalization>().Key = "TAKE_BUTTON";
					_middleButton.SetActive(validMoveExists && Director.CommandSystem.TryGetHandler(typeof(MoveItemCommand), out var moveHandler) && moveHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapSubsystemItemCommand), out moveHandler) && moveHandler.Enabled);
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.AddListener(() => Move(container, director));
					_middleButton.GetComponentInChildren<Text>().text = Localization.Get("MOVE_BUTTON");
					_middleButton.GetComponentInChildren<TextLocalization>().Key = "MOVE_BUTTON";
					var bestFitSize = _selectionOptions.GetComponentsInChildren<Button>().BestFit(true, new List<string> { Localization.Get("USE_BUTTON"), Localization.Get("MOVE_BUTTON"), Localization.Get("TAKE_BUTTON") });
					_descriptionText.fontSize = (int)(bestFitSize * 0.8f);
				}
				else if (!container.CanRelease && CanActivate && CurrentLocation.Value != null)
				{
					_leftButton.SetActive(false);
					_rightButton.SetActive(false);
					_middleButton.SetActive(Director.CommandSystem.TryGetHandler(typeof(ActivateItemCommand), out var activateHandler) && activateHandler.Enabled);
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.AddListener(Use);
					_middleButton.GetComponentInChildren<Text>().text = Localization.Get("USE_BUTTON");
					_middleButton.GetComponentInChildren<TextLocalization>().Key = "USE_BUTTON";
					var bestFitSize = _selectionOptions.GetComponentsInChildren<Button>().BestFit(true, new List<string> { Localization.Get("USE_BUTTON"), Localization.Get("MOVE_BUTTON"), Localization.Get("TAKE_BUTTON") });
					_descriptionText.fontSize = (int)(bestFitSize * 0.8f);
				}
				else if (CurrentLocation.Value == null)
				{
					_leftButton.SetActive(Director.CommandSystem.TryGetHandler(typeof(DropAndActivateItemCommand), out var dropAndActivateHandler) && dropAndActivateHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapInventoryItemAndActivateCommand), out dropAndActivateHandler) && dropAndActivateHandler.Enabled);
					_rightButton.SetActive(Director.CommandSystem.TryGetHandler(typeof(DropItemCommand), out var dropHandler) && dropHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapInventoryItemCommand), out dropHandler) && dropHandler.Enabled);
					_middleButton.SetActive(false);
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_leftButton.RectTransform().anchorMin = new Vector2(-1.5f, 1f);
					_leftButton.RectTransform().anchorMax = new Vector2(0, 1.7f);
					_leftButton.RectTransform().anchoredPosition = Vector2.zero;
					_leftButton.RectTransform().sizeDelta = Vector2.zero;
					_leftButton.GetComponent<Button>().onClick.AddListener(() => MoveAndUse(container, director));
					_leftButton.GetComponentInChildren<Text>().text = Localization.Get("PLACE_AND_USE_BUTTON");
					_leftButton.GetComponentInChildren<TextLocalization>().Key = "PLACE_AND_USE_BUTTON";

					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.RectTransform().anchorMin = new Vector2(1f, 1f);
					_rightButton.RectTransform().anchorMax = new Vector2(2.5f, 1.7f);
					_rightButton.RectTransform().anchoredPosition = Vector2.zero;
					_rightButton.RectTransform().sizeDelta = Vector2.zero;
					_rightButton.GetComponent<Button>().onClick.AddListener(() => Move(container, director));
					_rightButton.GetComponentInChildren<Text>().text = Localization.Get("PLACE_BUTTON");
					_rightButton.GetComponentInChildren<TextLocalization>().Key = "PLACE_BUTTON";
					_selectionOptions.GetComponentsInChildren<Button>().BestFit(true, new List<string> { Localization.Get("PLACE_BUTTON"), Localization.Get("PLACE_AND_USE_BUTTON") });
				}
				else
				{
					_leftButton.SetActive(false);
					_rightButton.SetActive(false);
					_middleButton.SetActive(false);
				}
				var optionAnim = _selectionOptions.GetComponent<Animation>();
				var clipName = CurrentLocation.Value != null ? optionAnim.clip.name : optionAnim.clip.name + "Inventory";
				optionAnim[clipName].time = 0;
				optionAnim[clipName].speed = 1;
				optionAnim.Play(clipName);
				_selectPos = transform.position;
			}
		}

		private void DisableOptions()
		{
			_moveState = false;
			if (_selectionOptions != null && _selectionOptions.activeInHierarchy)
			{
				var optionAnim = _selectionOptions.GetComponent<Animation>();
				var clipName = CurrentLocation.Value != null ? optionAnim.clip.name : optionAnim.clip.name + "Inventory";
				optionAnim[clipName].time = optionAnim[clipName].length;
				optionAnim[clipName].speed = -1;
				optionAnim.Play(clipName);
				Invoke(nameof(ResetOptions), 0.34f);
			}
			else
			{
				ResetOptions();
			}
			foreach (var con in transform.root.GetComponentsInChildren<ItemContainerBehaviour>(true))
			{
				con.RemoveHighlight();
			}
			_selectPos = null;
			Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
		}

		private void ResetOptions()
		{
			GetComponent<Canvas>().sortingOrder = 0;
			_leftButton.transform.localScale = Vector3.one;
			_rightButton.transform.localScale = Vector3.one;
			_middleButton.transform.localScale = Vector3.one;
			_descriptionText.transform.parent.localScale = Vector3.one;
			_selectionOptions.SetActive(false);
		}

		private void Use()
		{
			if (!IsInvoking(nameof(DisableOptions)) && !IsInvoking("EnableOptions"))
			{
				PlayerCommands.ActivateItem(Id);
			}
		}

		private void Take(ItemContainerBehaviour container)
		{
			if (!IsInvoking(nameof(DisableOptions)) && !IsInvoking("EnableOptions"))
			{
				if (GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory))
				{
					PlayerCommands.SwapInventoryItem(Id, container.ContainerIndex, inventory.Id);
				}
				else
				{
					PlayerCommands.PickupItem(Id);
				}
			}
		}

		private void Move(ItemContainerBehaviour container, Director director)
		{
			if (!IsInvoking(nameof(DisableOptions)) && !IsInvoking("EnableOptions"))
			{
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				_moveState = true;
				ResetOptions();
				var currentLocation = director.Player.CurrentLocationEntity;
				var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
				if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
				{
					var containers = subsystemBehaviour.GetComponentsInChildren<ItemContainerBehaviour>().Where(c => c != container).ToList();
					foreach (var systemContainer in containers)
					{
						systemContainer.Highlight(this, container);
					}
				}
			}
		}

		private void MoveAndUse(ItemContainerBehaviour container, Director director)
		{
			if (!IsInvoking(nameof(DisableOptions)) && !IsInvoking("EnableOptions"))
			{
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				_moveState = true;
				ResetOptions();
				var currentLocation = director.Player.CurrentLocationEntity;
				var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
				if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
				{
					foreach (var systemContainer in subsystemBehaviour.GetComponentsInChildren<ItemContainerBehaviour>())
					{
						if (systemContainer != container)
						{
							systemContainer.Highlight(this, null);
						}
					}
				}
			}
		}
	}
}