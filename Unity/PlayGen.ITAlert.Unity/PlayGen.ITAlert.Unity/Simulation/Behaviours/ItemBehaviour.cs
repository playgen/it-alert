using System.Collections.Generic;

using Engine.Systems.Activation.Components;

using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.UI.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Localization;
using System.Linq;

using PlayGen.Unity.Utilities.BestFit;

using UnityEngine;
using UnityEngine.EventSystems;
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
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
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
			if ((_selectionOptions.activeInHierarchy || _moveState) && !IsInvoking("OptionsDelay"))
			{
				if (Input.GetMouseButtonUp(0))
				{
					_moveState = false;
					if (_selectionOptions.activeInHierarchy)
					{
						var optionAnim = _selectionOptions.GetComponent<Animation>();
						var clipName = optionAnim.clip.name;
						optionAnim[clipName].time = optionAnim[clipName].length;
						optionAnim[clipName].speed = -1;
						optionAnim.Play();
						Invoke("DisableOptions", 0.33f);
						GetComponent<Canvas>().sortingOrder -= 100;
					}
					foreach (var con in transform.root.GetComponentsInChildren<ItemContainerBehaviour>(true))
					{
						con.RemoveHighlight();
					}
					Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				}
			}
		}

		public void OnPointerClick(ItemContainerBehaviour container, Director director)
		{
			if (!_selectionOptions.activeInHierarchy && CanActivate && !IsInvoking("OptionsDelay"))
			{
				_leftButton.transform.localScale = Vector3.one;
				_rightButton.transform.localScale = Vector3.one;
				_middleButton.transform.localScale = Vector3.one;
				_descriptionText.transform.parent.localScale = Vector3.one;
				GetComponent<Canvas>().sortingOrder += 100;
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				_selectionOptions.SetActive(true);
				if (container.CanRelease && CanActivate && CurrentLocation.Value != null)
				{
					_leftButton.SetActive(true);
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_leftButton.GetComponent<Button>().onClick.AddListener(Use);
					_leftButton.GetComponentInChildren<Text>().text = Localization.Get("USE_BUTTON");
					_leftButton.GetComponentInChildren<TextLocalization>().Key = "USE_BUTTON";
					_rightButton.SetActive(true);
					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.GetComponent<Button>().onClick.AddListener(() => Take(container));
					_rightButton.GetComponentInChildren<Text>().text = Localization.Get("TAKE_BUTTON");
					_rightButton.GetComponentInChildren<TextLocalization>().Key = "TAKE_BUTTON";
					_middleButton.SetActive(true);
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.AddListener(() => Move(container, director));
					_middleButton.GetComponentInChildren<Text>().text = Localization.Get("MOVE_BUTTON");
					_middleButton.GetComponentInChildren<TextLocalization>().Key = "MOVE_BUTTON";
				}
				else if (!container.CanRelease && CanActivate && CurrentLocation.Value != null)
				{
					_leftButton.SetActive(false);
					_rightButton.SetActive(false);
					_middleButton.SetActive(true);
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.AddListener(Use);
					_middleButton.GetComponentInChildren<Text>().text = Localization.Get("USE_BUTTON");
					_middleButton.GetComponentInChildren<TextLocalization>().Key = "USE_BUTTON";
				}
				else if (CurrentLocation.Value == null)
				{
					_leftButton.SetActive(false);
					_rightButton.SetActive(false);
					_middleButton.SetActive(true);
					_leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
					_middleButton.GetComponent<Button>().onClick.RemoveAllListeners();
					((RectTransform)_middleButton.transform).anchorMin = new Vector2(-0.2f, 1.2f);
					((RectTransform)_middleButton.transform).anchorMax = new Vector2(1.2f, 2f);
					((RectTransform)_middleButton.transform).anchoredPosition = Vector2.zero;
					((RectTransform)_middleButton.transform).sizeDelta = Vector2.zero;
					_middleButton.GetComponent<Button>().onClick.AddListener(() => Move(container, director));
					_middleButton.GetComponentInChildren<Text>().text = Localization.Get("PLACE_BUTTON");
					_middleButton.GetComponentInChildren<TextLocalization>().Key = "PLACE_BUTTON";
				}
				else
				{
					_leftButton.SetActive(false);
					_rightButton.SetActive(false);
					_middleButton.SetActive(true);
				}
				var bestFitSize = _selectionOptions.GetComponentsInChildren<Button>().BestFit(true, new List<string> { Localization.Get("USE_BUTTON"), Localization.Get("MOVE_BUTTON"), Localization.Get("TAKE_BUTTON") });
				_descriptionText.fontSize = (int)(bestFitSize * 0.6f);
				var optionAnim = _selectionOptions.GetComponent<Animation>();
				var clipName = optionAnim.clip.name;
				optionAnim[clipName].time = 0;
				optionAnim[clipName].speed = 1;
				optionAnim.Play();
			}
		}

		private void DisableOptions()
		{
			_leftButton.transform.localScale = Vector3.one;
			_rightButton.transform.localScale = Vector3.one;
			_middleButton.transform.localScale = Vector3.one;
			_descriptionText.transform.parent.localScale = Vector3.one;
			_selectionOptions.SetActive(false);
		}

		private void Use()
		{
			PlayerCommands.ActivateItem(Id);
		}

		private void Take(ItemContainerBehaviour container)
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

		private void Move(ItemContainerBehaviour container, Director director)
		{
			Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
			_moveState = true;
			_selectionOptions.SetActive(false);
			GetComponent<Canvas>().sortingOrder -= 100;
			var currentLocation = director.Player.CurrentLocationEntity;
			var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
			if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
			{
				foreach (var systemContainer in subsystemBehaviour.GetComponentsInChildren<ItemContainerBehaviour>())
				{
					if (systemContainer != container)
					{
						systemContainer.Highlight(this, container.ContainerIndex);
					}
				}
			}
		}
	}
}