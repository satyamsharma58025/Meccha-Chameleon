using UnityEngine;

namespace HueSeek.Player
{
    /// <summary>
    /// Brief outline flash when detection risk triggers (movement/repaint in Seeker LOS).
    /// </summary>
    public class ShimmerEffect : MonoBehaviour
    {
        [SerializeField] private Color _shimmerColor = new(1f, 0.85f, 0.2f, 1f);

        private DetectionRiskTracker _risk;
        private Renderer _renderer;
        private Material _instanceMaterial;
        private float _shimmerTimer;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void Bind(DetectionRiskTracker risk)
        {
            _risk = risk;
            _risk.OnShimmerTriggered.AddListener(OnShimmer);
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _instanceMaterial = _renderer.material;
                _instanceMaterial.EnableKeyword("_EMISSION");
            }
        }

        private void OnShimmer(float duration) => _shimmerTimer = duration;

        private void Update()
        {
            if (_shimmerTimer <= 0f || _instanceMaterial == null) return;

            _shimmerTimer -= Time.deltaTime;
            var pulse = Mathf.Sin(Time.time * 30f) * 0.5f + 0.5f;
            _instanceMaterial.SetColor(EmissionColor, _shimmerColor * pulse * 2f);
        }

        private void OnDestroy()
        {
            if (_risk != null)
                _risk.OnShimmerTriggered.RemoveListener(OnShimmer);
        }
    }
}
