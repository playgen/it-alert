using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
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
		private const float ItemContainerOffset = 25f;


		private static readonly Vector2[] CornerItemOffsets = new[]
		{
			new Vector2(-2 * ItemContainerOffset, ItemContainerOffset + 4),
			new Vector2(2 * ItemContainerOffset, ItemContainerOffset + 4),
			new Vector2(-2 * ItemContainerOffset, -1 * ItemContainerOffset - 4),
			new Vector2(2 * ItemContainerOffset, -1 * ItemContainerOffset - 4),
		};

		private Vector3[] _itemPositions;

		#region game elements

		private Text _nameText;


		private Image _cpuImage;

		private Image _memoryImage;

		private SpriteRenderer _iconRenderer;
		private SpriteRenderer _filled;

		public BoxCollider2D DropCollider
		{
			get; private set;
		}

		// everything is centered around this
		private GameObject _connectionSquare;

		public GameObject ConnectionSquare => _connectionSquare;

		private List<GameObject> _itemContainers;

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
			gameObject.transform.SetParent(Director.Graph.transform, false);
		}

		public void Awake()
		{
			_iconRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
			_filled = transform.Find("Filled").GetComponent<SpriteRenderer>();

			//var material = Resources.Load<Material>("DefaultMaterial");
			//_iconRenderer.transparentMaterial = material;
			//_filled.transparentMaterial = material;

			_nameText = transform.Find("Name").GetComponent<Text>();


			_cpuImage = transform.Find("CPU/Amount").gameObject.GetComponent<Image>();
			_memoryImage = transform.Find("Memory/Amount").gameObject.GetComponent<Image>();

			_connectionSquare = transform.Find("ConnectionSquare").gameObject;
			_connectionScaleCoefficient = 1 / _connectionSquare.transform.localScale.x;

			DropCollider = GetComponent<BoxCollider2D>();

			// TODO: these should probably be UIEntities
			_itemContainers = new List<GameObject>();
		}

		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _cpuResource)
				&& Entity.TryGetComponent(out _memoryResource)
				&& Entity.TryGetComponent(out _name))
			{
				Entity.TryGetComponent(out _itemStorage);

				SetPosition();
				CreateItemContainers();

				OnStateUpdated();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}

		}

		private void SetPosition()
		{
			var coordinate = Entity.GetComponent<Coordinate2DProperty>();
			var rectTransform = GetComponent<RectTransform>();
			var width = rectTransform.rect.width * rectTransform.localScale.x;
			var height = rectTransform.rect.height * rectTransform.localScale.y;

			var subsystemZ = ((GameObject)Resources.Load("Subsystem")).transform.position.z;
			transform.position = new Vector3(UIConstants.CurrentNetworkOffset.x + (width * UIConstants.SubsystemSpacingMultiplier * coordinate.X), UIConstants.CurrentNetworkOffset.y + (height * UIConstants.SubsystemSpacingMultiplier * coordinate.Y), subsystemZ);

			var itemZ = ((GameObject)Resources.Load("Item")).transform.position.z;
			_itemPositions = CornerItemOffsets.Select(c => new Vector3(c.x + transform.position.x, c.y + transform.position.y, itemZ)).ToArray();
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
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

		private void CreateItemContainers()
		{
			if (_itemStorage != null)
			{
				for (var i = 0; i < _itemStorage.Items.Length; i++)
				{
					var itemContainer = _itemStorage.Items[i];
					if (itemContainer != null)
					{
						var itemContainerObject = Director.InstantiateEntity(UIConstants.ItemContainerPrefab);
						itemContainerObject.transform.SetParent(this.transform, false);
						_itemContainers.Add(itemContainerObject);

						var itemContainerBehaviour = itemContainerObject.GetComponent<ItemContainerBehaviour>();
						itemContainerBehaviour.Initialize(itemContainer);

						itemContainerObject.transform.position = _itemPositions[i];
					}
				}
			}
		}

		private void UpdateItemContainers()
		{
			if (_itemStorage != null)
			{
				for (var i = 0; i < _itemStorage.Items.Length; i++)
				{
					var itemContainer = _itemStorage.Items[i];
					UIEntity item;
					if (itemContainer?.Item != null
						&& Director.TryGetEntity(itemContainer.Item.Value, out item))
					{
						var itemBehaviour = (ItemBehaviour)item.EntityBehaviour;
						itemBehaviour.ScaleUp = false;
						item.GameObject.transform.position = _itemPositions[i];
					}
				}
			}
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