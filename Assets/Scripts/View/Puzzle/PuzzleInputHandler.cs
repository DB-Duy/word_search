using System;
using UnityEngine;

namespace View.Puzzle
{
    public struct PuzzleTouchData
    {
        public Vector2 Position;
    }

    public class PuzzleInputHandler : MonoBehaviour
    {
        private Action<PuzzleTouchData> _onPointerDown;
        private Action<PuzzleTouchData> _onPointerUp;
        private Action<PuzzleTouchData> _onPointerMove;

        public void RegisterOnPointerDown(Action<PuzzleTouchData> onPointerDown)
        {
            _onPointerDown += onPointerDown;
        }

        public void RegisterOnPointerUp(Action<PuzzleTouchData> onPointerUp)
        {
            _onPointerUp += onPointerUp;
        }

        public void RegisterOnPointerMove(Action<PuzzleTouchData> onPointerMove)
        {
            _onPointerMove += onPointerMove;
        }

        public void OnPointerDown(PuzzleTouchData eventData)
        {
            _onPointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PuzzleTouchData eventData)
        {
            _onPointerUp?.Invoke(eventData);
        }

        public void OnPointerMove(PuzzleTouchData eventData)
        {
            _onPointerMove?.Invoke(eventData);
        }

        private void Update()
        {
            // touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                PuzzleTouchData pointerData = new PuzzleTouchData()
                {
                    Position = touch.position
                };

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnPointerDown(pointerData);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        OnPointerMove(pointerData);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnPointerUp(pointerData);
                        break;
                }
            }
        }
    }
}