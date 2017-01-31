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
#pragma warning disable 649

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	// ReSharper disable once CheckNamespace
	public class SubsystemBehaviour : NodeBehaviour
	{
		private const float ItemContainerOffset = 0.3f;

		private static readonly Vector2[] CornerItemOffsets = new[]
		{
			new Vector2(-2 * ItemContainerOffset, ItemContainerOffset),
			new Vector2(2 * ItemContainerOffset, ItemContainerOffset),
			new Vector2(-2 * ItemContainerOffset, -1 * ItemContainerOffset),
			new Vector2(2 * ItemContainerOffset, -1 * ItemContainerOffset),
		};

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

		private BoxCollider2D _connectionSquareCollider;

		public BoxCollider2D ConnectionSquareCollider => _connectionSquareCollider;

		public float ConnectionSquareRadius => _connectionSquare.transform.localScale.x * (_connectionSquareCollider.size.x / 2) * transform.localScale.x;

		private List<GameObject> _itemContainers;

		#endregion

		private Vector2[] _itemPositions;

		#region movement constants

		private static float _connectionScaleCoefficient;
		// TODO: read from sim constants
		private static readonly int PathPointsInSubsystem = 24;
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

		#endregion

		#region Initialization

		public void Start()
		{

		}

		public void Awake()
		{
			_iconRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
			_filled = transform.Find("Filled").GetComponent<SpriteRenderer>();

			//var material = Resources.Load<Material>("DefaultMaterial");
			//_iconRenderer.transparentMaterial = material;
			//_filled.transparentMaterial = material;

			_nameText = transform.Find("Canvas/Name").GetComponent<Text>();


			_cpuImage = transform.Find("Canvas/CPU/Amount").gameObject.GetComponent<Image>();
			_memoryImage = transform.Find("Canvas/Memory/Amount").gameObject.GetComponent<Image>();

			_connectionSquare = transform.Find("Connections").gameObject;
			_connectionSquareCollider = _connectionSquare.GetComponent<BoxCollider2D>();
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
			transform.position = new Vector3(UIConstants.NetworkOffset.x + (UIConstants.SubsystemSpacing.x * coordinate.X), UIConstants.NetworkOffset.y + (UIConstants.SubsystemSpacing.y * coordinate.Y));
			_itemPositions = CornerItemOffsets.Select(c => c + (Vector2) _connectionSquare.transform.position).ToArray();
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

			_nameText.text = _name.Value;

			//TODO: reimplement
			//if (EntityState.Disabled)
			//{
			//	//SetName("SYSTEM OFFLINE");
			//	_iconRenderer.color = new Color(0,0,0,0.5f);
			//}
			//else if (EntityState.Infection.HasValue)
			//{
			//	Color infectedColor;
			//	ColorUtility.TryParseHtmlString("#E32730", out infectedColor);
			//	_iconRenderer.color = infectedColor - new Color(0, 0, 0, 1 - _iconRenderer.color.a);

			//	var virus = Director.GetEntity(EntityState.Infection.Value);
			//	//TODO: get center of square properly
			//	virus.GameObject.transform.position = transform.position;
			//}
			//else
			//{
			_iconRenderer.color = Color.white;
			//_nameText.text = Name.ToUpper();
			//}


			//if (EntityState.VisitorPositions.Count > 0)
			//{
			//	FadeInUpdate();
			//}
			//else
			//{
			//	FadeOutUpdate();
			//}
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
						var itemContainerObject = Director.InstantiateEntity(ItemContainerBehaviour.Prefab);
						itemContainerObject.transform.SetParent(this.transform, false);
						_itemContainers.Add(itemContainerObject);

						var itemContainerBehaviour = itemContainerObject.GetComponent<ItemContainerBehaviour>();
						itemContainerBehaviour.Initialize(itemContainer);

						itemContainerObject.transform.position = GetItemPosition(i);
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
						item.GameObject.transform.position = _itemPositions[i];
					}
				}
			}
		}

		#region static properties

		public Vector2 GetItemPosition(int itemContainerIndex)
		{
			return _itemPositions[itemContainerIndex];
		}

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
			var sideLength = _connectionScaleCoefficient * _connectionSquareCollider.bounds.size.x;
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