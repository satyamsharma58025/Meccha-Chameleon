using HueSeek.Detection;
using HueSeek.Paint;
using HueSeek.Player;
using HueSeek.Taunt;
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
        [SerializeField] private SeekerToolkit _seekerToolkit;
        [SerializeField] private TauntSystem _tauntSystem;
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
            var didPaint = _paintSystem.TryApplyStroke(ray, _brushRadius, pressure, _player.PlayerId);
            if (didPaint)
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
            var horizontal = UnityEngine.Input.GetAxis("Horizontal");
            var vertical = UnityEngine.Input.GetAxis("Vertical");

            if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
            {
                horizontal = 0f;
                vertical = 0f;

                if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
                    horizontal += 1f;
                if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                    horizontal -= 1f;
                if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
                    vertical += 1f;
                if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
                    vertical -= 1f;
            }

            _moveInput = new Vector2(horizontal, vertical);
            _player?.Move(_moveInput, Time.deltaTime);

            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
                TogglePaintMode();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
                _paintSystem?.SetActiveTool(BrushTool.Freehand);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
                _paintSystem?.SetActiveTool(BrushTool.BucketFill);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
                _paintSystem?.SetActivePattern(PatternStampType.Stripes);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
                _paintSystem?.SetActivePattern(PatternStampType.Dots);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
                _paintSystem?.SetActivePattern(PatternStampType.Checker);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                _paintSystem?.AdjustMetallic(-0.1f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                _paintSystem?.AdjustMetallic(0.1f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                _paintSystem?.AdjustRoughness(-0.1f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
                _paintSystem?.AdjustRoughness(0.1f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
                _player?.SetPose(PlayerPose.Crouch);
            if (UnityEngine.Input.GetKeyDown(KeyCode.X))
                _player?.SetPose(PlayerPose.LieFlat);
            if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                _player?.SetPose(PlayerPose.ClingWall);
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
                _player?.SetPose(PlayerPose.Sit);
            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
                _player?.SetPose(PlayerPose.ContortSilhouette);
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
                _tauntSystem?.PerformTaunt();

            var camera = Camera.main;
            if (camera == null) return;

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (UnityEngine.Input.GetMouseButton(0))
                OnPaintDrag(ray, pressure: 1f);

            if (UnityEngine.Input.GetMouseButtonDown(1))
                OnSeekerTap(ray);
        }
    }
}
