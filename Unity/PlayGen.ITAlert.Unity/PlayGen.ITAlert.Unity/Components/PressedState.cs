using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayGen.ITAlert.Unity.Components
{
    public class PressedState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _isDownFrame;
        private bool _isUpFrame;
        
        public bool IsDown { get; private set; }
        public bool IsDownFrame { get; private set; }
        public bool IsUpFrame { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDown = true;
            _isDownFrame = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            IsDown = false;
            IsUpFrame = true;
            _isUpFrame = true;
        }

        // These values are buffered so they remain true for an entire update frame
        public void Update()
        {
            if (_isDownFrame)
            {
                IsDownFrame = true;
                _isDownFrame = false;
            }
            else if (IsDownFrame)
            {
                IsDownFrame = false;
            }

            if (_isUpFrame)
            {
                IsUpFrame = true;
                _isUpFrame = false;
            }
            else if (IsUpFrame)
            {
                IsUpFrame = false;
            }
        }
    }
}