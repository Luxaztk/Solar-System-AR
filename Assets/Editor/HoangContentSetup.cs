using System;
using System.Collections.Generic;
using System.IO;
using SolarSystemAR.Core;
using SolarSystemAR.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SolarSystemAR.Editor
{
    public static class HoangContentSetup
    {
        private const string DataFolder = "Assets/Data/Planets";
        private const string MaterialFolder = "Assets/Art/Materials/Planets";
        private const string PrefabFolder = "Assets/Prefabs/Planets";
        private const string ScenePath = "Assets/Scenes/HoangFeatureDemo.unity";
        private const string ScreenshotPath = "docs/hoang/screenshots/hoang-ui-preview.png";
        private const string SourceUrl = "https://nssdc.gsfc.nasa.gov/planetary/factsheet/";

        private readonly struct Definition
        {
            public Definition(string id, string name, string summary, float au, float radius,
                float orbitDays, float rotationHours, int moons, string hex, bool rings, float scale)
            {
                Id = id; Name = name; Summary = summary; Au = au; Radius = radius;
                OrbitDays = orbitDays; RotationHours = rotationHours; Moons = moons;
                Color = ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;
                Rings = rings; Scale = scale;
            }

            public string Id { get; }
            public string Name { get; }
            public string Summary { get; }
            public float Au { get; }
            public float Radius { get; }
            public float OrbitDays { get; }
            public float RotationHours { get; }
            public int Moons { get; }
            public Color Color { get; }
            public bool Rings { get; }
            public float Scale { get; }
        }

        private static readonly Definition[] Definitions =
        {
            new("sun", "Mặt Trời", "Ngôi sao ở trung tâm, chứa phần lớn khối lượng của Hệ Mặt Trời.", 0f, 695700f, 0f, 609.1f, 0, "#FDB813", false, 1.45f),
            new("mercury", "Sao Thủy", "Hành tinh nhỏ nhất và gần Mặt Trời nhất, có bề mặt nhiều hố va chạm.", 0.387f, 2439.7f, 88f, 1407.6f, 0, "#A6A6A6", false, 0.36f),
            new("venus", "Sao Kim", "Hành tinh đá với khí quyển dày và hiệu ứng nhà kính rất mạnh.", 0.723f, 6051.8f, 224.7f, 5832.5f, 0, "#E3BB76", false, 0.55f),
            new("earth", "Trái Đất", "Hành tinh quê hương của chúng ta và là nơi duy nhất đã biết có sự sống.", 1f, 6371f, 365.26f, 23.93f, 1, "#2E86DE", false, 0.58f),
            new("mars", "Sao Hỏa", "Một thế giới đá lạnh, màu đỏ do khoáng vật sắt bị oxy hóa.", 1.524f, 3389.5f, 687f, 24.62f, 2, "#C1440E", false, 0.43f),
            new("jupiter", "Sao Mộc", "Hành tinh lớn nhất, một khí khổng lồ với các dải mây và Vết Đỏ Lớn.", 5.203f, 69911f, 4332.6f, 9.93f, 95, "#D8CA9D", true, 1.05f),
            new("saturn", "Sao Thổ", "Khí khổng lồ nổi bật với hệ vành đai băng và đá rộng lớn.", 9.537f, 58232f, 10759.2f, 10.7f, 146, "#EAD6A0", true, 0.95f),
            new("uranus", "Sao Thiên Vương", "Hành tinh băng khổng lồ quay gần như nằm nghiêng trên quỹ đạo.", 19.191f, 25362f, 30688.5f, 17.24f, 27, "#7DE3F4", true, 0.72f),
            new("neptune", "Sao Hải Vương", "Hành tinh xa nhất, một thế giới xanh với gió rất mạnh.", 30.069f, 24622f, 60182f, 16.11f, 14, "#4169E1", true, 0.70f),
        };

        [MenuItem("Solar System AR/Generate Hoang Week 1-4 Content")]
        public static void Generate()
        {
            EnsureFolders();
            var data = CreatePlanetData();
            var prefabs = CreatePrefabs(data);
            CreateDemoScene(data, prefabs);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RenderScreenshot();
            Debug.Log("[SolarSystemAR] Hoang week 1-4 content generated and verified.");
        }

        public static void GenerateFromCommandLine()
        {
            Generate();
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "Data");
            EnsureFolder("Assets/Data", "Planets");
            EnsureFolder("Assets/Art/Materials", "Planets");
            EnsureFolder("Assets/Prefabs", "Planets");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static PlanetData[] CreatePlanetData()
        {
            var result = new PlanetData[Definitions.Length];
            for (var i = 0; i < Definitions.Length; i++)
            {
                var definition = Definitions[i];
                var path = $"{DataFolder}/{definition.Id}.asset";
                var asset = AssetDatabase.LoadAssetAtPath<PlanetData>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<PlanetData>();
                    AssetDatabase.CreateAsset(asset, path);
                }

                asset.Configure(definition.Id, definition.Name, definition.Summary, definition.Au,
                    definition.Radius, definition.OrbitDays, definition.RotationHours,
                    definition.Moons, definition.Color, definition.Rings, SourceUrl);
                EditorUtility.SetDirty(asset);
                result[i] = asset;
            }

            return result;
        }

        private static GameObject[] CreatePrefabs(IReadOnlyList<PlanetData> data)
        {
            var result = new GameObject[Definitions.Length];
            for (var i = 0; i < Definitions.Length; i++)
            {
                var definition = Definitions[i];
                var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                root.name = definition.Name;
                root.transform.localScale = Vector3.one * definition.Scale;
                root.GetComponent<MeshRenderer>().sharedMaterial = CreateMaterial(definition);
                root.AddComponent<PlanetRuntimeView>().Configure(data[i], 4f + i * 1.5f);

                if (definition.Rings && definition.Id == "saturn")
                {
                    var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    ring.name = "Vành đai";
                    ring.transform.SetParent(root.transform, false);
                    ring.transform.localScale = new Vector3(1.65f, 0.035f, 1.65f);
                    Object.DestroyImmediate(ring.GetComponent<Collider>());
                    ring.GetComponent<MeshRenderer>().sharedMaterial = CreateRingMaterial();
                }

                var labelObject = new GameObject("Nhãn 3D", typeof(TextMesh), typeof(PlanetLabel3D));
                labelObject.transform.SetParent(root.transform, false);
                labelObject.transform.localPosition = new Vector3(0f, 0.75f / definition.Scale, 0f);
                labelObject.transform.localScale = Vector3.one / definition.Scale;
                var label = labelObject.GetComponent<TextMesh>();
                label.text = definition.Name;
                label.fontSize = 48;
                label.characterSize = 0.035f;
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.color = Color.white;
                labelObject.GetComponent<PlanetLabel3D>().Configure(data[i], label);

                var path = $"{PrefabFolder}/{definition.Id}.prefab";
                result[i] = PrefabUtility.SaveAsPrefabAsset(root, path);
                Object.DestroyImmediate(root);
            }

            return result;
        }

        private static Material CreateMaterial(Definition definition)
        {
            var path = $"{MaterialFolder}/{definition.Id}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader) { name = definition.Name };
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = definition.Color;
            if (definition.Id == "sun")
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", definition.Color * 2.2f);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateRingMaterial()
        {
            const string path = MaterialFolder + "/saturn_ring.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader) { name = "Vành đai Sao Thổ" };
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = new Color(0.72f, 0.62f, 0.42f, 1f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void CreateDemoScene(IReadOnlyList<PlanetData> data, IReadOnlyList<GameObject> prefabs)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "HoangFeatureDemo";

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex("#071426");
            camera.transform.SetPositionAndRotation(new Vector3(0f, 1.4f, -10.8f), Quaternion.Euler(5f, 0f, 0f));

            var lightObject = new GameObject("Key Light", typeof(Light));
            var light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 2.1f;
            lightObject.transform.rotation = Quaternion.Euler(38f, -35f, 0f);

            for (var i = 0; i < prefabs.Count; i++)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[i], scene);
                var column = i % 5;
                var row = i / 5;
                instance.transform.position = new Vector3((column - 2f) * 2.2f, 1.45f - row * 2.7f, 0f);
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetParent(null);
            var canvas = CreateCanvas(camera);
            var infoPanel = CreateInfoPanel(canvas.transform);
            cameraObject.AddComponent<PlanetPreviewSelector>().Configure(camera, infoPanel);
            CreateDiscoveryBar(canvas.transform);
            CreateQuizPanel(canvas.transform);
            CreateMenuAndTutorial(canvas.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings();
        }

        private static Canvas CreateCanvas(Camera camera)
        {
            var canvasObject = new GameObject("Hoang UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static PlanetInfoPanel CreateInfoPanel(Transform parent)
        {
            var root = Panel("Planet Info Panel", parent, Hex("#0B2038", 0.96f));
            SetRect(root.GetComponent<RectTransform>(), new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.36f), Vector2.zero, Vector2.zero);
            var accent = Image("Accent", root.transform, Hex("#35D0FF"));
            SetRect(accent.rectTransform, new Vector2(0f, 0f), new Vector2(0.018f, 1f), Vector2.zero, Vector2.zero);
            var title = Text("Tên thiên thể", root.transform, 42, FontStyle.Bold, TextAnchor.MiddleLeft);
            SetRect(title.rectTransform, new Vector2(0.06f, 0.76f), new Vector2(0.82f, 0.96f), Vector2.zero, Vector2.zero);
            var summary = Text("Mô tả", root.transform, 25, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(summary.rectTransform, new Vector2(0.06f, 0.47f), new Vector2(0.94f, 0.76f), Vector2.zero, Vector2.zero);
            var facts = Text("Dữ liệu", root.transform, 23, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(facts.rectTransform, new Vector2(0.06f, 0.05f), new Vector2(0.94f, 0.47f), Vector2.zero, Vector2.zero);
            var close = Button("×", root.transform, Hex("#163754"));
            SetRect(close.GetComponent<RectTransform>(), new Vector2(0.84f, 0.78f), new Vector2(0.96f, 0.96f), Vector2.zero, Vector2.zero);
            var component = root.AddComponent<PlanetInfoPanel>();
            component.Configure(root, title, summary, facts, accent, close);
            root.SetActive(false);
            return component;
        }

        private static void CreateDiscoveryBar(Transform parent)
        {
            var root = Panel("Discovery Progress", parent, Hex("#0B2038", 0.9f));
            SetRect(root.GetComponent<RectTransform>(), new Vector2(0.05f, 0.91f), new Vector2(0.95f, 0.97f), Vector2.zero, Vector2.zero);
            var label = Text("Đã khám phá 0/9", root.transform, 25, FontStyle.Bold, TextAnchor.MiddleLeft);
            SetRect(label.rectTransform, new Vector2(0.04f, 0f), new Vector2(0.44f, 1f), Vector2.zero, Vector2.zero);
            var sliderObject = new GameObject("Progress", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(root.transform, false);
            SetRect(sliderObject.GetComponent<RectTransform>(), new Vector2(0.46f, 0.24f), new Vector2(0.95f, 0.76f), Vector2.zero, Vector2.zero);
            var background = Image("Background", sliderObject.transform, Hex("#183A59"));
            SetRect(background.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var fillArea = Rect("Fill Area", sliderObject.transform);
            SetRect(fillArea, Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));
            var fill = Image("Fill", fillArea, Hex("#35D0FF"));
            SetRect(fill.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var slider = sliderObject.GetComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.minValue = 0f; slider.maxValue = 1f; slider.interactable = false;
            root.AddComponent<DiscoveryProgressController>().Configure(slider, label, 9);
        }

        private static void CreateQuizPanel(Transform parent)
        {
            var root = Panel("Quiz Panel", parent, Hex("#0B2038", 0.98f));
            SetRect(root.GetComponent<RectTransform>(), new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.78f), Vector2.zero, Vector2.zero);
            var heading = Text("THỬ THÁCH NHANH", root.transform, 26, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(heading.rectTransform, new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.96f), Vector2.zero, Vector2.zero);
            var question = Text("Câu hỏi", root.transform, 32, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(question.rectTransform, new Vector2(0.08f, 0.64f), new Vector2(0.92f, 0.84f), Vector2.zero, Vector2.zero);
            var buttons = new Button[3];
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i] = Button($"Lựa chọn {i + 1}", root.transform, Hex("#163754"));
                var top = 0.59f - i * 0.16f;
                SetRect(buttons[i].GetComponent<RectTransform>(), new Vector2(0.10f, top - 0.12f), new Vector2(0.90f, top), Vector2.zero, Vector2.zero);
            }
            var feedback = Text(string.Empty, root.transform, 23, FontStyle.Italic, TextAnchor.MiddleCenter);
            SetRect(feedback.rectTransform, new Vector2(0.08f, 0.03f), new Vector2(0.92f, 0.16f), Vector2.zero, Vector2.zero);
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/quiz.vi.json");
            root.AddComponent<QuizController>().Configure(json, question, feedback, buttons);
            root.SetActive(false);
        }

        private static void CreateMenuAndTutorial(Transform parent)
        {
            var main = Panel("Main Menu", parent, Hex("#071426", 0.94f));
            SetRect(main.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var eyebrow = Text("KHÁM PHÁ NGOÀI ĐỜI THẬT", main.transform, 23, FontStyle.Bold, TextAnchor.MiddleCenter);
            eyebrow.color = Hex("#35D0FF");
            SetRect(eyebrow.rectTransform, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.80f), Vector2.zero, Vector2.zero);
            var title = Text("SOLAR\nSYSTEM AR", main.transform, 76, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.72f), Vector2.zero, Vector2.zero);
            var subtitle = Text("Đi bộ qua Hệ Mặt Trời theo tỉ lệ", main.transform, 28, FontStyle.Normal, TextAnchor.MiddleCenter);
            subtitle.color = Hex("#B9CAE0");
            SetRect(subtitle.rectTransform, new Vector2(0.1f, 0.40f), new Vector2(0.9f, 0.48f), Vector2.zero, Vector2.zero);
            var start = Button("BẮT ĐẦU", main.transform, Hex("#08A8D6"));
            SetRect(start.GetComponent<RectTransform>(), new Vector2(0.16f, 0.26f), new Vector2(0.84f, 0.34f), Vector2.zero, Vector2.zero);
            var skip = Button("Bỏ qua hướng dẫn", main.transform, Hex("#163754"));
            SetRect(skip.GetComponent<RectTransform>(), new Vector2(0.16f, 0.17f), new Vector2(0.84f, 0.24f), Vector2.zero, Vector2.zero);

            var tutorial = Panel("Tutorial Panel", parent, Hex("#071426", 0.97f));
            SetRect(tutorial.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var tutorialTitle = Text("3 BƯỚC KHÁM PHÁ", tutorial.transform, 46, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(tutorialTitle.rectTransform, new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.86f), Vector2.zero, Vector2.zero);
            var steps = Text("1  Quét mặt phẳng và đặt Mặt Trời\n\n2  Đi theo khoảng cách chỉ dẫn\n\n3  Chạm hành tinh để xem thông tin", tutorial.transform, 31, FontStyle.Normal, TextAnchor.MiddleLeft);
            SetRect(steps.rectTransform, new Vector2(0.12f, 0.36f), new Vector2(0.88f, 0.72f), Vector2.zero, Vector2.zero);
            var continueButton = Button("BẮT ĐẦU KHÁM PHÁ", tutorial.transform, Hex("#08A8D6"));
            SetRect(continueButton.GetComponent<RectTransform>(), new Vector2(0.13f, 0.20f), new Vector2(0.87f, 0.29f), Vector2.zero, Vector2.zero);
            tutorial.SetActive(false);
            main.AddComponent<AppMenuController>().Configure(main, tutorial, start, skip, continueButton);
        }

        private static GameObject Panel(string name, Transform parent, Color color)
        {
            var image = Image(name, parent, color);
            return image.gameObject;
        }

        private static Image Image(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text Text(string value, Transform parent, int size, FontStyle style, TextAnchor anchor)
        {
            var go = new GameObject(value.Replace("\n", " "), typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button Button(string label, Transform parent, Color color)
        {
            var image = Image(label + " Button", parent, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = Text(label, image.transform, 27, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 4f), new Vector2(-12f, -4f));
            return button;
        }

        private static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = min; rect.anchorMax = max;
            rect.offsetMin = offsetMin; rect.offsetMax = offsetMax;
        }

        private static Color Hex(string hex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            color.a = alpha;
            return color;
        }

        private static void AddSceneToBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.TrueForAll(item => item.path != ScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        private static void RenderScreenshot()
        {
            var camera = Object.FindAnyObjectByType<Camera>();
            if (camera == null) return;
            var target = new RenderTexture(540, 960, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            var texture = new Texture2D(540, 960, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, 540, 960), 0, 0);
            texture.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(ScreenshotPath) ?? "docs/hoang");
            File.WriteAllBytes(ScreenshotPath, texture.EncodeToPNG());
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(target);
            AssetDatabase.Refresh();
        }
    }
}
