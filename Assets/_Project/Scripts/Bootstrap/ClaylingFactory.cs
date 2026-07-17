using HueSeek.Detection;
using HueSeek.Input;
using HueSeek.Paint;
using HueSeek.Player;
using HueSeek.Taunt;
using UnityEngine;

namespace HueSeek.Bootstrap
{
    public static class ClaylingFactory
    {
        public const int AvatarLayer = 8;
        public const int EnvironmentLayer = 9;
        
        /// <summary>
        /// Calculate adaptive paint texture resolution based on available graphics memory.
        /// Lower resolution on mobile/limited VRAM to prevent texture thrashing.
        /// </summary>
        private static int GetAdaptivePaintResolution()
        {
            // Use 256x256 (64KB) on devices with <2GB graphics memory
            // Use 512x512 (256KB) on devices with 2GB+ graphics memory
            var memoryMB = SystemInfo.graphicsMemorySize;
            return memoryMB < 2048 ? 256 : 512;
        }

        public static ClaylingController CreatePlayer(
            Vector3 position,
            int playerId,
            Material paintMaterial,
            Transform cameraRig)
        {
            var root = new GameObject($"Clayling_{playerId}");
            root.transform.position = position;
            root.layer = AvatarLayer;

            var controller = root.AddComponent<CharacterController>();
            controller.height = 1.2f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.6f, 0f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            body.transform.localScale = new Vector3(0.7f, 0.6f, 0.7f);
            body.layer = AvatarLayer;
            Object.Destroy(body.GetComponent<Collider>());

            var meshCollider = root.AddComponent<CapsuleCollider>();
            meshCollider.height = 1.2f;
            meshCollider.radius = 0.35f;
            meshCollider.center = new Vector3(0f, 0.6f, 0f);

            var renderer = body.GetComponent<Renderer>();
            renderer.sharedMaterial = paintMaterial;

            var paintAccumulator = root.AddComponent<PaintTextureAccumulator>();
            paintAccumulator.Initialize(renderer, paintMaterial, GetAdaptivePaintResolution());

            var sampler = root.AddComponent<PaintSampler>();
            var paintSystem = root.AddComponent<PaintSystem>();
            paintSystem.Configure(sampler, renderer, paintAccumulator, AvatarLayer);

            var risk = root.AddComponent<DetectionRiskTracker>();
            var taunt = root.AddComponent<TauntSystem>();
            var clayling = root.AddComponent<ClaylingController>();
            clayling.Initialize(controller, paintSystem, risk, taunt);
            clayling.PlayerId = playerId;

            var seeker = root.AddComponent<SeekerToolkit>();
            var input = root.AddComponent<MobileTouchInput>();
            input.Initialize(clayling, paintSystem, seeker);

            var shimmer = body.AddComponent<ShimmerEffect>();
            shimmer.Bind(risk);

            return clayling;
        }

        public static Material CreatePaintMaterial()
        {
            var shader = Shader.Find("HueSeek/ClaylingPaint");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Lit");

            var mat = new Material(shader) { color = new Color(0.85f, 0.78f, 0.72f) };
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", new Color(0.85f, 0.78f, 0.72f));
            return mat;
        }
    }
}
