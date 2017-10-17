using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class PlayerInputHandler : MonoBehaviour
	{
		private readonly List<RaycastResult> _lastClicked = new List<RaycastResult>();

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
			if (clickDown)
			{
				ClearClicks();
			}

			var hits = new List<RaycastResult>();
			EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, hits);
			if (hits.Count(d => d.gameObject.tag.Equals(Tags.PauseScreen)) == 0)
			{
				var subsystemHits = hits.Where(d => d.gameObject.tag.Equals(Tags.Subsystem)).ToArray();
				var itemHits = hits.Where(d => d.gameObject.tag.Equals(Tags.Item)).ToArray();
				var itemContainerHits = hits.Where(d => d.gameObject.tag.Equals(Tags.ItemContainer)).ToArray();

				if (subsystemHits.Length == 1 && itemHits.Length == 1 && itemContainerHits.Length == 1)
				{
					OnClickItemInContainer(itemHits[0], itemContainerHits[0], clickDown);
				}
				else if (subsystemHits.Length == 1)
				{
					OnClickSubsystem(subsystemHits[0], clickDown);
				}
				else if (itemHits.Length > 0 && itemContainerHits.Length == 1)
				{
					foreach (var item in itemHits)
					{
						OnClickItemInContainer(item, itemContainerHits[0], clickDown);
					}
				}
				else if (itemContainerHits.Length == 1)
				{
					OnClickItemContainer(itemContainerHits[0], clickDown);
				}
			}

			if (!clickDown)
			{
				ClearClicks();
			}
		}

		public void ClearClicks()
		{
			foreach (var clicked in _lastClicked)
			{
				if (clicked.gameObject != null)
				{
					switch (clicked.gameObject.tag)
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
			}
			_lastClicked.Clear();
		}


		private void OnClickSubsystem(RaycastResult subsystemHit, bool down)
		{
			var subsystem = subsystemHit.gameObject.GetComponentsInParent<SubsystemBehaviour>(true).FirstOrDefault();
            if (subsystem)
            {
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
		}

		private void OnClickItemInContainer(RaycastResult itemHit, RaycastResult containerHit, bool down)
		{
			var item = itemHit.gameObject.GetComponentsInParent<ItemBehaviour>(true).FirstOrDefault();
			var itemdrag = itemHit.gameObject.GetComponentsInParent<ItemDragBehaviour>(true).FirstOrDefault();
			var container = containerHit.gameObject.GetComponent<ItemContainerBehaviour>();
            if (container)
            {
                if (down)
                {
                    if (container.OnClickDown())
                    {
                        if (itemdrag?.OnClickDown(containerHit) ?? true)
                        {
                            _lastClicked.Add(itemHit);
                        }
                    }
                    _lastClicked.Add(containerHit);
                }
                else
                {
                    if (item)
                    {
                        if (itemdrag == null && _lastClicked.Any(d => d.gameObject.name.Equals(itemHit.gameObject.name)))
                        {
                            container.OnClickUp(item, container.ContainerIndex);
                        }
                        else if (itemdrag != null && itemdrag.OnClickUp(out var containerIndex, out var dragged))
                        {
                            container.OnClickUp(item, containerIndex, dragged);
                        }
                    }
                }
            }
		}

		private void OnClickItemContainer(RaycastResult containerHit, bool down)
		{
			var container = containerHit.gameObject.GetComponent<ItemContainerBehaviour>();
            if (container)
            {
                if (down)
                {
                    container.OnClickDown();
                    _lastClicked.Add(containerHit);
                }
                else
                {
                    var itemsPreviouslyHit = _lastClicked.Where(d => d.gameObject.tag.Equals(Tags.Item)).ToArray();

                    if (itemsPreviouslyHit.Length == 1)
                    {
                        OnClickItemInContainer(itemsPreviouslyHit[0], containerHit, false);
                        return;
                    }
                    container.OnClickUp();
                }
            }
		}

		private void SubsystemClickReset(RaycastResult subsystemHit)
		{
			var subsystem = subsystemHit.gameObject.GetComponentsInParent<SubsystemBehaviour>(true).FirstOrDefault();
            //	// Debug.Log(string.Format("mouse input: {0} bound: min {1}, max {2}", Camera.main.ScreenToWorldPoint(Input.mousePosition), _minDragBounds, _maxDragBounds));
            if (subsystem)
            {
                subsystem.ClickReset();
            }
		}

		private void ItemClickReset(RaycastResult itemHit)
		{
			var item = itemHit.gameObject.GetComponentsInParent<ItemDragBehaviour>(true).FirstOrDefault();
			item?.ClickReset();
			item?.PositionReset();
		}

		private void ItemContainerClickReset(RaycastResult containerHit)
		{
			var container = containerHit.gameObject.GetComponent<ItemContainerBehaviour>();
			container.ClickReset();
		}

		#endregion
	}
}