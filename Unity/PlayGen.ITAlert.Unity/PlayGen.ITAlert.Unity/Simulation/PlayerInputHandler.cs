using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class PlayerInputHandler : MonoBehaviour
	{
		private List<RaycastHit2D> _lastClicked = new List<RaycastHit2D>();

		[SerializeField]
		private Director _director;

		#region Initialization 

		private void Update()
		{
			HandleInput();
		}

		#endregion

		private void HandleInput()
		{
			if (_director.Active)
			{
				if (Input.GetMouseButtonDown(0))
				{
					OnClick(true);
				}

				if (Input.GetMouseButtonUp(0))
				{
					OnClick(false);
				}
			}
		}

		#region Clicks

		private void OnClick(bool clickDown)
		{
			var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if (hits.Count(d => d.collider.tag.Equals(Tags.PauseScreen)) == 0)
			{
				var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
				var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
				var itemContainerHits = hits.Where(d => d.collider.tag.Equals(Tags.ItemContainer)).ToArray();

				if (subsystemHits.Length == 1)
				{
					OnClickSubsystem(subsystemHits[0], clickDown);
				}
				else if (itemHits.Length == 1 && itemContainerHits.Length == 1)
				{
					OnClickItemInContainer(itemHits[0], itemContainerHits[0], clickDown);
				}
				else if (itemContainerHits.Length == 1)
				{
					OnClickItemContainer(itemContainerHits[0], clickDown);
				}
				else if (itemHits.Length == 1)
				{
					OnClickItem(itemHits[0], clickDown);
				}
			}

			if (!clickDown)
			{
				foreach (var clicked in _lastClicked)
				{
					switch (clicked.collider.tag)
					{
						case Tags.Subsystem:
							SubsystemClickReset(clicked);
							break;
						case Tags.Item:
							ItemClickReset(clicked);
							break;
						case Tags.ItemContainer:
							ItemContainerClickReset(clicked);
							break;
					}

				}
				_lastClicked.Clear();
			}
		}

		private void OnClickSubsystem(RaycastHit2D subsystemHit, bool down)
		{
			var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
			if (down)
			{
				subsystem.OnClickDown();
				_lastClicked.Add(subsystemHit);
			}
			else
			{
				var clickConfirm = subsystem.OnClickUp();
				if (clickConfirm)
				{
					PlayerCommands.Move(subsystem.Id);
				}
			}
		}

		private void OnClickItem(RaycastHit2D itemHit, bool down)
		{
			if (!down)
			{
				ItemClickReset(itemHit);
			}
		}

		private void OnClickItemInContainer(RaycastHit2D itemHit, RaycastHit2D containerHit, bool down)
		{
			var item = itemHit.collider.GetComponent<ItemBehaviour>();
			var itemdrag = itemHit.collider.GetComponent<ItemDragBehaviour>();
			var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
			if (down)
			{
				if (container.OnClickDown())
				{
					itemdrag.OnClickDown(containerHit);
				}
				_lastClicked.Add(itemHit);
				_lastClicked.Add(containerHit);
			}
			else
			{
				container.OnClickUp(item, itemdrag.OnClickUp());
			}
		}

		private void OnClickItemContainer(RaycastHit2D containerHit, bool down)
		{
			var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
			if (down)
			{
				container.OnClickDown();
				_lastClicked.Add(containerHit);
			}
			else
			{
				container.OnClickUp();
			}
		}

		private void SubsystemClickReset(RaycastHit2D subsystemHit)
		{
			var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
			//	// Debug.Log(string.Format("mouse input: {0} bound: min {1}, max {2}", Camera.main.ScreenToWorldPoint(Input.mousePosition), _minDragBounds, _maxDragBounds));
			subsystem.ClickReset();
		}

		private void ItemClickReset(RaycastHit2D itemHit)
		{
			var item = itemHit.collider.GetComponent<ItemDragBehaviour>();
			item.ClickReset();
		}

		private void ItemContainerClickReset(RaycastHit2D containerHit)
		{
			var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
			container.ClickReset();
		}

		#endregion
	}
}