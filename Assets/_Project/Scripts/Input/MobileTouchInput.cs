using HueSeek.Paint;
using HueSeek.Player;
using UnityEngine;

namespace HueSeek.Input
{
    /// <summary>
    /// Mobile touch mapping: virtual joystick, camera drag, paint mode, pose radial.
    /// </summary>
    public class MobileTouchInput : MonoBehaviour
    {
        [SerializeField] private ClaylingController _player;
        [SerializeField] private PaintSystem _paintSystem;
        [SerializeField] private Detection.SeekerToolkit _seekerToolkit;
        [SerializeField] private float _brushRadius = 0.08f;

        private Vector2 _moveInput;
        private Vector2 _lookDelta;
        private bool _paintModeActive;

        public Vector2 MoveInput => _moveInput;
        public Vector2 LookDelta => _lookDelta;

        public void SetMoveInput(Vector2 input) => _moveInput = input;
        public void SetLookDelta(Vector2 delta) => _lookDelta = delta;

        public void Initialize(ClaylingController player, PaintSystem paintSystem, Detection.SeekerToolkit seekerToolkit)
        {
            _player = player;
            _paintSystem = paintSystem;
            _seekerToolkit = seekerToolkit;
        }

        public void TogglePaintMode()
        {
            _paintModeActive = !_paintModeActive;
            _paintSystem?.SetPaintMode(_paintModeActive);
        }

        public void OnSampleLongPress(Ray ray) => _paintSystem?.TrySampleColor(ray);

        public void OnPaintDrag(Ray ray, float pressure)
        {
            if (_player == null || _paintSystem == null) return;
            _paintSystem.TryApplyStroke(ray, _brushRadius, pressure, _player.PlayerId);
            _player.GetComponent<DetectionRiskTracker>()?.RegisterRepaint(pressure);
        }

        public void OnSeekerTap(Ray ray)
        {
            if (_seekerToolkit == null) return;
            if (!_seekerToolkit.TryTag(ray))
                _seekerToolkit.TryInspect(ray);
        }

        private void Update()
        {
            _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _player?.Move(_moveInput, Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.P))
                TogglePaintMode();

            var camera = Camera.main;
            if (camera == null) return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButton(0))
                OnPaintDrag(ray, pressure: 1f);

            if (Input.GetMouseButtonDown(1))
                OnSeekerTap(ray);
        }
    }
}
