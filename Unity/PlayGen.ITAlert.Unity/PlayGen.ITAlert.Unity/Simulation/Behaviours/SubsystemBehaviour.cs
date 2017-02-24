using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Resources;
using PlayGen.ITAlert.Simulation.UI.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class SubsystemBehaviour : NodeBehaviour
	{
		private readonly Vector2[] _itemContainerPositions = new Vector2[4]
		{
			new Vector2(30, -120),
			new Vector2(-30, -120),
			new Vector2(30, 130),
			new Vector2(-30, 130),
		};

		private readonly Vector2[] _itemContainerOffsets = new Vector2[4]
		{
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0),
			new Vector2(1, 0),
		};

		private readonly Vector2[] _itemContainerPivots = new Vector2[4]
		{
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0),
			new Vector2(1, 0),
		};

		#region game elements

		[SerializeField]
		private Text _nameText;


		[SerializeField]
		private Image _cpuImage;
		[SerializeField]
		private Image _memoryImage;

		[SerializeField]
		private SpriteRenderer _iconRenderer;
		[SerializeField]
		private SpriteRenderer _filled;

		public BoxCollider2D DropCollider
		{
			get; private set;
		}

		[SerializeField]
		private GameObject _connectionSquare;

		public GameObject ConnectionSquare => _connectionSquare;

		[SerializeField]
		private List<GameObject> _itemContainers;

		[SerializeField]
		private RectTransform _rectTransform;

		private float _itemZ;

		#endregion


		#region movement constants

		private static float _connectionScaleCoefficient;
		// TODO: read from sim constants
		private static readonly int PathPointsInSubsystem = SimulationConstants.SubsystemPositions;
		private const int SquareSideCount = 4;
		private static readonly float PointsPerSide = (float) PathPointsInSubsystem / SquareSideCount;

		#endregion

		#region Components

		// required
		private CPUResource _cpuResource;
		private MemoryResource _memoryResource;
		private Name _name;

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
			_connectionScaleCoefficient = 1 / _connectionSquare.transform.localScale.x;

			DropCollider = GetComponent<BoxCollider2D>();

			// TODO: these should probably be UIEntities
			_itemZ = ((GameObject) Resources.Load("Item")).GetComponent<RectTransform>().position.z;
		}

		protected override void OnInitialize()
		{
			_itemContainers = new List<GameObject>();
			if (Entity.TryGetComponent(out _cpuResource)
				&& Entity.TryGetComponent(out _memoryResource)
				&& Entity.TryGetComponent(out _name))
			{
				Entity.TryGetComponent(out _itemStorage);

				SetPosition();
				CreateItemContainers();
				UpdateItemContainers();

				OnStateUpdated();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}

		}

		private void CreateItemContainers()
		{
			ForEachItemContainer((i, itemContainer) =>
			{
				var itemContainerObject = Director.InstantiateEntity(UIConstants.ItemContainerPrefab);
				itemContainerObject.transform.SetParent(this.transform, false);
				itemContainerObject.name = $"ItemContainer_{i}";
				_itemContainers.Add(itemContainerObject);

				var itemContainerBehaviour = itemContainerObject.GetComponent<ItemContainerBehaviour>();
				itemContainerBehaviour.Initialize(itemContainer, Director);
				SetItemPosition(i, _itemContainers[i]);

			});
		}

		private void SetItemPosition(int index, GameObject go)
		{
			var rectTransform = go.GetComponent<RectTransform>();
			rectTransform.anchorMin = _itemContainerOffsets[index];
			rectTransform.anchorMax = _itemContainerOffsets[index];
			rectTransform.pivot = _itemContainerPivots[index];
			rectTransform.anchoredPosition = new Vector3(_itemContainerPositions[index].x, _itemContainerPositions[index].y, _itemZ);
		}

		private void SetPosition()
		{
			var coordinate = Entity.GetComponent<Coordinate2DProperty>();
			var width = _rectTransform.rect.width * _rectTransform.localScale.x;
			var height = _rectTransform.rect.height * _rectTransform.localScale.y;

			var subsystemZ = ((GameObject)Resources.Load("Subsystem")).transform.position.z;
			transform.position = new Vector3(UIConstants.CurrentNetworkOffset.x + (width * UIConstants.SubsystemSpacingMultiplier * coordinate.X), UIConstants.CurrentNetworkOffset.y + (height * UIConstants.SubsystemSpacingMultiplier * coordinate.Y), subsystemZ);
		}

		#endregion

		#region State Update
		protected override void OnStateUpdated()
		{
			_cpuImage.fillAmount = 1f - _cpuResource.GetUtilisation();
			_memoryImage.fillAmount = 1f - _memoryResource.GetUtilisation();

			if (_nameText.text != _name.Value)
			{
				_nameText.text = _name.Value;
			}

			//_iconRenderer.color = Color.white;

			UpdateItemContainers();
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

		private void UpdateItemContainers()
		{
			ForEachItemContainer((i, itemContainer) =>
			{
				UIEntity item;
				if (itemContainer.Item != null
					&& Director.TryGetEntity(itemContainer.Item.Value, out item))
				{
					item.GameObject.transform.SetParent(this.transform, false);
					SetItemPosition(i, item.GameObject);
				}
			});
		}

		#region static properties

		public void FadeInUpdate()
		{
			if (_filled.color.a < 1)
			{
				_filled.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
				_iconRenderer.color -= new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			}
		}

		public void FadeOutUpdate()
		{
			if (_filled.color.a > 0)
			{
				_filled.color -= new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
				_iconRenderer.color += new Color(0, 0, 0, Time.fixedDeltaTime * 0.5f);
			}
		}

		#endregion

		#region Visitor Movement

		protected override Vector3 GetPositionFromPathPoint(int pathPoint)
		{
			var localPosition = GetPositionOnSquare(pathPoint);
			localPosition.y *= -1; // Flip Y so displays correctly relative to the view
			localPosition *= _connectionSquare.transform.localScale.x;
			localPosition += _connectionSquare.transform.position; // Move relative to this subsystem

			return localPosition;
		}

		private Vector3 GetPositionOnSquare(int pathPoint)
		{
			var squarePermimiterSideScale = 1f;
			var offsetPositionAlong = GetOffsetPositionAlongSquare(pathPoint, squarePermimiterSideScale);

			// Position on square perimeter
			// Top Left = (-1, -1), Bottom Right = (1, 1)
			var sideLength = _connectionScaleCoefficient * _connectionSquare.GetComponent<RectTransform>().rect.width * transform.localScale.x;
			var halfSide = sideLength / 2;
			//var step = sideLength / PointsPerSide * 2;
			var localPositionAlong = (offsetPositionAlong % squarePermimiterSideScale) * sideLength;

			// TOP
			if (offsetPositionAlong < squarePermimiterSideScale)
			{
				return new Vector3((localPositionAlong) - halfSide, -halfSide, 0);
			}
			// RIGHT
			else if (offsetPositionAlong < 2 * squarePermimiterSideScale)
			{
				return new Vector3(halfSide, (localPositionAlong) - halfSide, 0);
			}
			// BOTTOM
			else if (offsetPositionAlong < 3 * squarePermimiterSideScale)
			{
				return new Vector3(halfSide - (localPositionAlong), halfSide, 0);
			}
			// LEFT
			else if (offsetPositionAlong < 4 * squarePermimiterSideScale)
			{
				return new Vector3(-halfSide, halfSide - (localPositionAlong), 0);
			}
			else
			{
				throw new Exception("This should never be hit.");
			}
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



	}
}