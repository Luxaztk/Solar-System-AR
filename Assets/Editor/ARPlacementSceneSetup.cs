using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using SolarSystemAR.AR;

namespace SolarSystemAR.Editor
{
    /// <summary>
    /// Creates the plane-detection scene and its visualization prefab from the
    /// validated AR camera scene. The setup is idempotent and can also be run
    /// manually from the Solar System AR menu.
    /// </summary>
    internal static class ARPlacementSceneSetup
    {
        private const string CameraScenePath = "Assets/Scenes/ARCameraTest.unity";
        private const string PlacementScenePath = "Assets/Scenes/ARPlacement.unity";
        private const string PlanePrefabPath = "Assets/Prefabs/ARPlaneVisualizer.prefab";
        private const string SunPrefabPath = "Assets/Prefabs/SunPlaceholder.prefab";
        private const string SunMaterialPath =
            "Assets/Art/Materials/SunPlaceholder.mat";
        private const string MobileRendererPath = "Assets/Settings/Mobile_Renderer.asset";
        private const string AndroidBuildPath =
            "Builds/Android/Solar-System-AR-v0.1.apk";
        private const string AndroidApplicationIdentifier = "com.solarsystemar.app";
        private const string DebugPlaneMaterialPath =
            "Packages/com.unity.xr.arfoundation/Assets/Materials/DebugPlane.mat";

        [InitializeOnLoadMethod]
        private static void ScheduleFirstRun()
        {
            EditorApplication.delayCall += SetupWhenReady;
        }

        private static void SetupWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += SetupWhenReady;
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(PlacementScenePath) == null)
            {
                CreateOrUpdatePlacementScene();
            }
        }

        [MenuItem("Solar System AR/Setup AR Placement Scene")]
        public static void CreateOrUpdatePlacementScene()
        {
            var cameraScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(CameraScenePath);
            if (cameraScene == null)
            {
                Debug.LogError($"[SolarSystemAR] Missing source scene: {CameraScenePath}");
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(PlacementScenePath) == null &&
                !AssetDatabase.CopyAsset(CameraScenePath, PlacementScenePath))
            {
                Debug.LogError($"[SolarSystemAR] Could not create scene: {PlacementScenePath}");
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(PlacementScenePath, OpenSceneMode.Single);
            var xrOrigin = Object.FindAnyObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                Debug.LogError("[SolarSystemAR] ARPlacement scene has no XR Origin.");
                return;
            }

            var planeManager = GetOrAddComponent<ARPlaneManager>(xrOrigin.gameObject);
            GetOrAddComponent<ARRaycastManager>(xrOrigin.gameObject);
            GetOrAddComponent<ARAnchorManager>(xrOrigin.gameObject);
            var placementManager = GetOrAddComponent<ARPlacementManager>(xrOrigin.gameObject);
            planeManager.requestedDetectionMode =
                PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
            planeManager.planePrefab = CreateOrLoadPlanePrefab();
            placementManager.SunPrefab = CreateOrLoadSunPrefab();

            EditorUtility.SetDirty(xrOrigin.gameObject);
            EditorUtility.SetDirty(planeManager);
            EditorUtility.SetDirty(placementManager);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            UpdateBuildScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("[SolarSystemAR] AR placement scene configured successfully.");
        }

        [MenuItem("Solar System AR/Configure AR Renderer")]
        public static void ConfigureARRenderer()
        {
            var rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(MobileRendererPath);
            if (rendererData == null)
            {
                throw new FileNotFoundException("Mobile renderer asset was not found.", MobileRendererPath);
            }

            EnsureRendererFeature<ARBackgroundRendererFeature>(rendererData);
            EnsureRendererFeature<ARCommandBufferSupportRendererFeature>(rendererData);
            EditorUtility.SetDirty(rendererData);
            AssetDatabase.SaveAssets();
            Debug.Log("[SolarSystemAR] AR background renderer features configured on Mobile_Renderer.");
        }

        [MenuItem("Solar System AR/Use OpenGLES3 for Android")]
        public static void ConfigureOpenGLES3ForAndroid()
        {
            PlayerSettings.SetGraphicsAPIs(
                BuildTarget.Android,
                new[] { GraphicsDeviceType.OpenGLES3 });
            AssetDatabase.SaveAssets();
            Debug.Log("[SolarSystemAR] Android Graphics API set to OpenGLES3 only.");
        }

        [MenuItem("Solar System AR/Build Android Prototype v0.1")]
        public static void BuildAndroidTestApk()
        {
            CreateOrUpdatePlacementScene();
            ConfigureARRenderer();
            ConfigureOpenGLES3ForAndroid();
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Android,
                AndroidApplicationIdentifier);
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode =
                Mathf.Max(PlayerSettings.Android.bundleVersionCode, 3);

            var buildDirectory = Path.GetDirectoryName(AndroidBuildPath);
            if (!string.IsNullOrEmpty(buildDirectory))
            {
                Directory.CreateDirectory(buildDirectory);
            }

            var enabledScenes = System.Array.FindAll(
                EditorBuildSettings.scenes,
                scene => scene.enabled);
            var scenePaths = System.Array.ConvertAll(enabledScenes, scene => scene.path);
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = AndroidBuildPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"Android build failed with result {report.summary.result}.");
            }

            Debug.Log($"[SolarSystemAR] Android test APK built: {AndroidBuildPath}");
        }

        private static void EnsureRendererFeature<T>(UniversalRendererData rendererData)
            where T : ScriptableRendererFeature
        {
            foreach (var rendererFeature in rendererData.rendererFeatures)
            {
                if (rendererFeature is T)
                {
                    rendererFeature.SetActive(true);
                    EditorUtility.SetDirty(rendererFeature);
                    return;
                }
            }

            var feature = ScriptableObject.CreateInstance<T>();
            feature.name = typeof(T).Name;
            feature.SetActive(true);
            AssetDatabase.AddObjectToAsset(feature, rendererData);
            rendererData.rendererFeatures.Add(feature);
            EditorUtility.SetDirty(feature);
        }

        private static GameObject CreateOrLoadPlanePrefab()
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlanePrefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var planeObject = new GameObject(
                "AR Plane Visualizer",
                typeof(ARPlaneMeshVisualizer),
                typeof(MeshCollider),
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(LineRenderer));

            var meshRenderer = planeObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial =
                AssetDatabase.LoadAssetAtPath<Material>(DebugPlaneMaterialPath);

            var lineRenderer = planeObject.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial =
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");
            lineRenderer.loop = true;
            lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, 0.005f);
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = false;

            var prefab = PrefabUtility.SaveAsPrefabAsset(planeObject, PlanePrefabPath);
            Object.DestroyImmediate(planeObject);
            return prefab;
        }

        private static GameObject CreateOrLoadSunPrefab()
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SunPrefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new BuildFailedException(
                    "Universal Render Pipeline/Lit shader is unavailable.");
            }

            var sunColor = new Color(1f, 0.38f, 0.02f, 1f);
            var material = new Material(shader)
            {
                name = "Sun Placeholder",
                color = sunColor,
            };
            material.SetColor("_BaseColor", sunColor);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", sunColor * 2.5f);
            AssetDatabase.CreateAsset(material, SunMaterialPath);

            var sunObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunObject.name = "Sun Placeholder";
            sunObject.transform.localScale = Vector3.one * 0.15f;
            Object.DestroyImmediate(sunObject.GetComponent<SphereCollider>());
            sunObject.GetComponent<Renderer>().sharedMaterial = material;

            var prefab = PrefabUtility.SaveAsPrefabAsset(sunObject, SunPrefabPath);
            Object.DestroyImmediate(sunObject);
            return prefab;
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static void UpdateBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(PlacementScenePath, true),
                new EditorBuildSettingsScene(CameraScenePath, true),
            };
        }
    }
}
