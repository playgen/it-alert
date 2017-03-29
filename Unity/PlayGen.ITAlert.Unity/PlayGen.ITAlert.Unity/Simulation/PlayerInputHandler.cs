using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class PlayerInputHandler : MonoBehaviour
	{
        private List<RaycastHit2D> _lastClicked = new List<RaycastHit2D>();

		#region Initialization 

		private void Update()
		{
			HandleInput();
		}

		#endregion

		private void HandleInput()
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

		#region Clicks

		private void OnClick(bool clickDown)
		{
			var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			var subsystemHits = hits.SingleOrDefault(d => d.collider.tag.Equals(Tags.Subsystem));
			var itemHits = hits.SingleOrDefault(d => d.collider.tag.Equals(Tags.Item));
			var itemContainerHits = hits.SingleOrDefault(d => d.collider.tag.Equals(Tags.ItemContainer));

			if (subsystemHits)
			{
				OnClickSubsystem(subsystemHits, clickDown);
			}
			else if (itemHits && itemContainerHits)
			{
                OnClickItemInContainer(itemHits, itemContainerHits, clickDown);
			}
			else if (itemContainerHits)
			{
                OnClickItemContainer(itemContainerHits, clickDown);
			}
			else if (itemHits)
			{
                OnClickItem(itemHits, clickDown);
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
            var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
            if (down)
            {
                item.OnClickDown();
                _lastClicked.Add(itemHit);
                _lastClicked.Add(containerHit);
            }
            else
            {
                container.OnClickUp(item);
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
            subsystem.ClickReset();
        }

        private void ItemClickReset(RaycastHit2D itemHit)
        {
            var item = itemHit.collider.GetComponent<ItemBehaviour>();
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