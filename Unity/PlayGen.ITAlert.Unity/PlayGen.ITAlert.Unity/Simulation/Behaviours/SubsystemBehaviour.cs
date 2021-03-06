using System;
using System.Collections.Generic;

using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;
using PlayGen.ITAlert.Simulation.UI.Components.Items;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.Unity.Utilities.Extensions;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class SubsystemBehaviour : NodeBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		private readonly Vector2[] _itemContainerPositions = {
			new Vector2(-59, 31.5f),
			new Vector2(59, 31.5f),
			new Vector2(-59, -31.5f),
			new Vector2(59, -31.5f)
		};

		#region game elements

		[SerializeField]
		private Text _nameText;


		[SerializeField]
		private Image _cpuImageLeft;
		[SerializeField]
		private Image _cpuImageRight;

		[SerializeField]
		private GameObject _cpuLeft;
		[SerializeField]
		private GameObject _cpuRight;
		[SerializeField]
		private GameObject _cpuConnection;

		[SerializeField]
		private Image _sprite;

		[SerializeField]
		private BlinkBehaviour _blink;

		[SerializeField]
		private GameObject _connectionSquare;

		public GameObject ConnectionSquare => _connectionSquare;

		[SerializeField]
		private List<GameObject> _itemContainers;

		private float _itemZ;

		#endregion


		#region movement constants

		//private static float _connectionScaleCoefficient;
		// TODO: read from sim constants
		private static readonly int PathPointsInSubsystem = SimulationConstants.SubsystemPositions;
		private const int SquareSideCount = 4;
		private static readonly float PointsPerSide = (float)PathPointsInSubsystem / SquareSideCount;

		#endregion

		#region Components

		// required
		private CPUResource _cpuResource;
		private MemoryResource _memoryResource;
		private Name _name;
		private Coordinate2DProperty _coordinate2D;
		private Visitors _visitors;

		//optional
		private ItemStorage _itemStorage;

		public ItemStorage ItemStorage => _itemStorage;

		#endregion

		#region Initialization


		public void Start()
		{
			//Canvas.ForceUpdateCanvases();
		}

		public void Awake()
		{
			//_connectionScaleCoefficient = 1 / _connectionSquare.transform.localScale.x;

			// TODO: these should probably be UIEntities
			_itemZ = ((GameObject)Resources.Load("Item")).RectTransform().position.z;
		}

		protected override void OnInitialize()
		{
			_itemContainers = new List<GameObject>();
			if (Entity.TryGetComponent(out _cpuResource)
				&& Entity.TryGetComponent(out _memoryResource)
				&& Entity.TryGetComponent(out _name)
				&& Entity.TryGetComponent(out _coordinate2D)
				&& Entity.TryGetComponent(out _visitors))
			{
				gameObject.name = $"{Name}_{_name.Value}";
				Entity.TryGetComponent(out _itemStorage);

				SetPosition();
				CreateItemContainers();
				UpdateItemContainers();

				OnStateUpdated();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Subsystem Entity Id {Entity.Id}");
			}
		}

		public override void ResetEntity()
		{
			_itemContainers.Clear();
		}

		private void CreateItemContainers()
		{
			ForEachItemContainer((i, itemContainer) =>
			{
				var itemContainerObject = Director.InstantiateEntity(UIConstants.ItemContainerPrefab);
				itemContainerObject.transform.SetParent(transform, false);
				itemContainerObject.name = $"ItemContainer_{i}";

				_itemContainers.Add(itemContainerObject);

				var itemContainerBehaviour = itemContainerObject.GetComponent<ItemContainerBehaviour>();
				itemContainerBehaviour.Initialize(itemContainer, Director, i, Id);
				SetItemPosition(i, _itemContainers[i]);

			});
		}

		private void SetItemPosition(int index, GameObject go)
		{
			var rectTransform = go.RectTransform();
			rectTransform.anchoredPosition = new Vector3(_itemContainerPositions[index].x, _itemContainerPositions[index].y, _itemZ);
		}

		private void SetPosition()
		{
			var subsystemZ = ((GameObject)Resources.Load("Subsystem")).transform.position.z;

			var relativeX = _coordinate2D.X - ((Director.NetworkDimensions.x - 1) / 2);
			var relativeY = _coordinate2D.Y - ((Director.NetworkDimensions.y - 1) / 2);

			gameObject.RectTransform().anchoredPosition = new Vector3(relativeX * Director.SubsystemSpacing.x, -1 * relativeY * Director.SubsystemSpacing.y, subsystemZ);
		}

		#endregion

		#region State Update
		protected override void OnStateUpdated()
		{
			base.OnStateUpdated();
			_cpuImageLeft.fillAmount = (1 - _cpuResource.GetUtilisation()) * 2;
			_cpuImageRight.fillAmount = 1 - (_cpuResource.GetUtilisation() * 2);

			if (_nameText.text != _name.Value)
			{
				_nameText.text = _name.Value;
			}

			UpdateItemContainers();
			UpdateResourceVisibility();
			UpdateDestination();
		}

		private void UpdateDestination()
		{
			if (Director.Player != null
				&& Director.Player.DestinationSystemId == Entity.Id
				&& _blink.enabled == false)
			{
				_blink.enabled = true;
			}
			else if (Director.Player != null
				&& Director.Player.DestinationSystemId != Entity.Id
				&& _blink.enabled)
			{
				_blink.enabled = false;
			}
		}

		private void ForEachItemContainer(Action<int, ItemContainer> action)
		{
			if (_itemStorage != null)
			{
				for (var i = 0; i < _itemStorage.Items.Length; i++)
				{
					var itemContainer = _itemStorage.Items[i];
					if (itemContainer != null)
					{
						action(i, itemContainer);
					}
				}
			}
		}

		private void UpdateResourceVisibility()
		{
			if (_visitors != null
				&& Director.Player != null
				&& _visitors.Values.Contains(Director.Player.Id))
			{
				_cpuLeft.SetActive(true);
				_cpuRight.SetActive(true);
				_cpuConnection.SetActive(true);
			}
			else
			{
				_cpuLeft.SetActive(false);
				_cpuRight.SetActive(false);
				_cpuConnection.SetActive(false);
			}
		}

		private void UpdateItemContainers()
		{
			ForEachItemContainer((i, itemContainer) =>
			{
				if (itemContainer.Item != null
					&& Director.TryGetEntity(itemContainer.Item.Value, out var item))
				{
					item.GameObject.transform.SetParent(transform, false);
					SetItemPosition(i, item.GameObject);
				}
			});
		}

		#region static properties

		public void FadeInUpdate()
		{
			if (_sprite.color.a < 1)
			{
				_sprite.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			}
		}

		public void FadeOutUpdate()
		{
			if (_sprite.color.a > 0)
			{
				_sprite.color -= new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			}
		}

		#endregion

		#region Visitor Movement

		protected override VisitorVectors GetPositionFromPathPoint(int pathPoint)
		{
			var visitorVectors = GetPositionOnSquare(pathPoint);
			var localPosition = visitorVectors.Position;
			//localPosition *= _connectionSquare.transform.localScale.x;
			localPosition += _connectionSquare.RectTransform().anchoredPosition; // Move relative to this subsystem
			visitorVectors.Position = localPosition;
			return visitorVectors;
		}

		private VisitorVectors GetPositionOnSquare(int pathPoint)
		{
			var squarePermimiterSideScale = 1f;
			var offsetPositionAlong = GetOffsetPositionAlongSquare(pathPoint, squarePermimiterSideScale);

			// Position on square perimeter
			// Top Left = (-1, -1), Bottom Right = (1, 1)
			var sideLength = _connectionSquare.RectTransform().rect.width - 8;
			var halfSide = sideLength / 2;
			//var step = sideLength / PointsPerSide * 2;
			var localPositionAlong = (offsetPositionAlong % squarePermimiterSideScale) * sideLength;

			// TOP
			if (offsetPositionAlong < squarePermimiterSideScale)
			{
				return new VisitorVectors
							{
					Position = new Vector2((localPositionAlong) - halfSide, halfSide),
					Rotation = new Vector3(0, 0, 180)
				};
			}
			// RIGHT
			if (offsetPositionAlong < 2 * squarePermimiterSideScale)
			{
				return new VisitorVectors
							{
					Position = new Vector2(halfSide, (localPositionAlong - halfSide) * -1),
					Rotation = new Vector3(0, 0, 90)
				};
			}
			// BOTTOM
			if (offsetPositionAlong < 3 * squarePermimiterSideScale)
			{
				return new VisitorVectors
							{
					Position = new Vector2(halfSide - (localPositionAlong), -halfSide),
					Rotation = new Vector3(0, 0, 0)
				};
			}
			// LEFT
			if (offsetPositionAlong < 4 * squarePermimiterSideScale)
			{
				return new VisitorVectors
							{
					Position = new Vector2(-halfSide, (halfSide - localPositionAlong) * -1),
					Rotation = new Vector3(0, 0, 270)
				};
			}
			throw new Exception("Subsystem movement has exceeded bounds: this should never be hit.");
		}

		private float GetOffsetPositionAlongSquare(int pathPoint, float scale)
		{
			pathPoint = pathPoint % PathPointsInSubsystem; // Wrap around
														   // Point along the perimeter as if it were a straight line.
			var halfScale = scale / 2;
			var progressAlongSide = pathPoint / PointsPerSide;
			var offsetPositionAlong = (progressAlongSide + halfScale) % (SquareSideCount * scale);  // Offsets the position so that entry points are half way along each square side.
			return offsetPositionAlong;
		}

		#endregion

		#endregion

		public void OnPointerClick(PointerEventData eventData)
		{
			PlayerCommands.Move(Id);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_blink.enabled = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_blink.enabled = false;
		}
	}
}