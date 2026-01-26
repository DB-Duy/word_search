using UnityEngine;

namespace Shared.View.Fps
{
    public class FpsView : MonoBehaviour
    {
        private float _deltaTime = 0.0f;
        private GUIStyle _style;
        private Rect _rect;

        private void Awake() => _Init();

        private void _Init()
        {
            int w = Screen.width, h = Screen.height;
            _rect = new Rect(10, 10, 100, 20);

            _style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / 100 + 15,
                normal =
                {
                    textColor = Color.green
                }
            };
        }

        private void Update()
        {
            if(Time.timeScale <= 0) return;
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            if (_style == null) return;
            var fps = 1.0f / _deltaTime;
            var msec = _deltaTime * 1000.0f;
            var text = $"{msec:0.0} ms ({fps:0.} fps)";
            GUI.Label(_rect, text, _style);
        }
    }
}