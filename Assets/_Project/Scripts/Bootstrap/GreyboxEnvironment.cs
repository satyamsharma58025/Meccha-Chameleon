using UnityEngine;

namespace HueSeek.Bootstrap
{
    /// <summary>
    /// Procedural Sunroom Greenhouse greybox — sample surfaces with distinct colors/textures.
    /// </summary>
    public static class GreyboxEnvironment
    {
        public static void Build(Transform parent)
        {
            CreateFloor(parent);
            CreateWalls(parent);
            CreateFoliage(parent);
            CreateProps(parent);
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            go.layer = ClaylingFactory.EnvironmentLayer;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", color);
            go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static void CreateFloor(Transform parent)
        {
            CreateCube(parent, "Floor", new Vector3(0f, -0.05f, 0f), new Vector3(24f, 0.1f, 24f),
                new Color(0.35f, 0.55f, 0.32f));
        }

        private static void CreateWalls(Transform parent)
        {
            CreateCube(parent, "Wall_N", new Vector3(0f, 2f, 12f), new Vector3(24f, 4f, 0.3f),
                new Color(0.92f, 0.95f, 0.88f));
            CreateCube(parent, "Wall_S", new Vector3(0f, 2f, -12f), new Vector3(24f, 4f, 0.3f),
                new Color(0.88f, 0.93f, 0.85f));
            CreateCube(parent, "Wall_E", new Vector3(12f, 2f, 0f), new Vector3(0.3f, 4f, 24f),
                new Color(0.75f, 0.85f, 0.95f));
            CreateCube(parent, "Wall_W", new Vector3(-12f, 2f, 0f), new Vector3(0.3f, 4f, 24f),
                new Color(0.95f, 0.88f, 0.75f));
        }

        private static void CreateFoliage(Transform parent)
        {
            for (var i = 0; i < 8; i++)
            {
                var angle = i * 45f * Mathf.Deg2Rad;
                var pos = new Vector3(Mathf.Cos(angle) * 6f, 0.6f, Mathf.Sin(angle) * 6f);
                var pot = CreateCube(parent, $"Pot_{i}", pos, new Vector3(0.8f, 0.6f, 0.8f),
                    new Color(0.55f, 0.35f, 0.22f));
                CreateCube(parent, $"Plant_{i}", pos + Vector3.up * 0.9f, new Vector3(1.2f, 1.2f, 1.2f),
                    Color.HSVToRGB(0.28f + i * 0.02f, 0.55f, 0.45f));
            }
        }

        private static void CreateProps(Transform parent)
        {
            CreateCube(parent, "Bench", new Vector3(-4f, 0.25f, 3f), new Vector3(3f, 0.5f, 1f),
                new Color(0.62f, 0.48f, 0.32f));
            CreateCube(parent, "GlassPanel", new Vector3(5f, 1.5f, -5f), new Vector3(0.1f, 3f, 4f),
                new Color(0.7f, 0.9f, 0.95f, 0.6f));
            CreateCube(parent, "WaterTub", new Vector3(3f, 0.2f, 6f), new Vector3(2f, 0.4f, 2f),
                new Color(0.2f, 0.55f, 0.75f));
        }
    }
}
