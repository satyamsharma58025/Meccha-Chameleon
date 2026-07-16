using UnityEngine;

namespace HueSeek.Paint
{
    /// <summary>
    /// GPU stroke stamping into a RenderTexture bound to the Clayling paint shader.
    /// </summary>
    [DisallowMultipleComponent]
    public class PaintTextureAccumulator : MonoBehaviour
    {
        private static readonly int PaintMapId = Shader.PropertyToID("_PaintMap");
        private static readonly int BrushUvId = Shader.PropertyToID("_BrushUV");
        private static readonly int BrushRadiusId = Shader.PropertyToID("_BrushRadius");
        private static readonly int BrushColorId = Shader.PropertyToID("_BrushColor");
        private static readonly int BrushPressureId = Shader.PropertyToID("_BrushPressure");

        [SerializeField] private int _resolution = 512;

        private RenderTexture _paintMap;
        private Material _stampMaterial;
        private Material _targetMaterial;
        private Renderer _targetRenderer;

        public RenderTexture PaintMap => _paintMap;

        public void Initialize(Renderer renderer, Material paintMaterial, int resolution = 512)
        {
            _targetRenderer = renderer;
            _resolution = resolution;
            _targetMaterial = paintMaterial;

            ReleaseResources();
            _paintMap = new RenderTexture(_resolution, _resolution, 0, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "ClaylingPaintMap"
            };
            _paintMap.Create();

            _stampMaterial = new Material(Shader.Find("HueSeek/PaintStampBlit"));
            _targetMaterial.SetTexture(PaintMapId, _paintMap);
            _targetRenderer.sharedMaterial = _targetMaterial;

            Clear();
        }

        public void ApplyStroke(PaintStroke stroke, Vector2 uv)
        {
            if (_paintMap == null || _stampMaterial == null) return;

            _stampMaterial.SetVector(BrushUvId, new Vector4(uv.x, uv.y, 0f, 0f));
            _stampMaterial.SetFloat(BrushRadiusId, Mathf.Max(0.01f, stroke.BrushRadius));
            _stampMaterial.SetColor(BrushColorId, stroke.Color);
            _stampMaterial.SetFloat(BrushPressureId, stroke.BrushPressure);

            var temp = RenderTexture.GetTemporary(_paintMap.width, _paintMap.height, 0, _paintMap.format);
            Graphics.Blit(_paintMap, temp, _stampMaterial);
            Graphics.Blit(temp, _paintMap);
            RenderTexture.ReleaseTemporary(temp);

            _targetMaterial.SetFloat("_Metallic", stroke.Material.Metallic);
            _targetMaterial.SetFloat("_Roughness", stroke.Material.Roughness);
            _targetMaterial.SetFloat("_Imperfection", stroke.Material.ImperfectionNoise);
        }

        public void Clear()
        {
            if (_paintMap == null) return;

            var prev = RenderTexture.active;
            RenderTexture.active = _paintMap;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = prev;
        }

        private void OnDestroy() => ReleaseResources();

        private void ReleaseResources()
        {
            if (_paintMap != null)
            {
                _paintMap.Release();
                Destroy(_paintMap);
                _paintMap = null;
            }

            if (_stampMaterial != null)
            {
                Destroy(_stampMaterial);
                _stampMaterial = null;
            }
        }
    }
}
