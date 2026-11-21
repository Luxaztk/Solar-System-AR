using System;
using System.Globalization;
using System.IO;
using System.Text;
using SolarSystemAR.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolarSystemAR.Editor
{
    /// <summary>Builds an isolated scale test scene and compares scene transforms with calculated positions.</summary>
    internal static class ScaleTestSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/ScaleTest.unity";
        private const string MaterialPath = "Assets/Art/Materials/ScaleTestSphere.mat";
        private const string ReportPath = "docs/scale-test-report.md";
        private const float PositionTolerance = 0.0001f;

        private static readonly Planet[] Planets =
        {
            new Planet("Mercury", 0.39f),
            new Planet("Venus", 0.72f),
            new Planet("Earth", 1.00f),
            new Planet("Mars", 1.52f),
            new Planet("Jupiter", 5.20f),
            new Planet("Saturn", 9.58f),
            new Planet("Uranus", 19.20f),
            new Planet("Neptune", 30.05f),
        };

        private static readonly TestCase[] TestCases =
        {
            new TestCase("Origin zero, +X, 1 m/AU", Vector3.zero, Vector3.right, 1f),
            new TestCase("Offset origin, +Z, 0.5 m/AU", new Vector3(3f, 1f, -2f), Vector3.forward, 0.5f),
            new TestCase("Offset origin, diagonal, 2 m/AU", new Vector3(-4f, 0.75f, 6f), new Vector3(3f, 7f, 4f), 2f),
        };

        [MenuItem("Solar System AR/Build ScaleTest Scene and Run Checks")]
        public static void BuildSceneAndRunChecks()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SetActiveScene(scene);

            var sphereMaterial = LoadOrCreateSphereMaterial();
            var spheres = new Transform[Planets.Length];
            for (var index = 0; index < Planets.Length; index++)
            {
                spheres[index] = CreatePlanetSphere(Planets[index], sphereMaterial);
            }

            CreateCamera();
            CreateLight();

            var report = RunChecks(spheres);
            RestoreDefaultLayout(spheres);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new IOException($"Could not save the scale test scene to {ScenePath}.");
            }

            File.WriteAllText(ReportPath, report, new UTF8Encoding(false));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (report.Contains("FAIL"))
            {
                Debug.LogError($"[ScaleTest] Validation failed. See {ReportPath}");
            }
            else
            {
                Debug.Log($"[ScaleTest] Scene saved to {ScenePath}; all transform checks passed. Report: {ReportPath}");
            }
        }

        private static Transform CreatePlanetSphere(Planet planet, Material material)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = $"{planet.Name} ({planet.AU:F2} AU)";
            sphere.transform.position = ScaleCalculator.CalculatePosition(
                Vector3.zero,
                Vector3.right,
                planet.AU);
            sphere.transform.localScale = Vector3.one * 0.18f;

            var collider = sphere.GetComponent<Collider>();
            if (collider != null)
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }

            sphere.GetComponent<Renderer>().sharedMaterial = material;
            return sphere.transform;
        }

        private static Material LoadOrCreateSphereMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material != null)
            {
                return material;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
            {
                throw new InvalidOperationException("Could not find a compatible sphere shader.");
            }

            material = new Material(shader)
            {
                name = "ScaleTestSphere",
                color = new Color(0.32f, 0.72f, 0.95f, 1f),
            };
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("ScaleTest Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.035f, 0.07f, 1f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            camera.fieldOfView = 42f;
            cameraObject.transform.position = new Vector3(15f, 11f, -34f);
            cameraObject.transform.LookAt(new Vector3(15f, 0f, 0f));
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }

        private static string RunChecks(Transform[] spheres)
        {
            var output = new StringBuilder();
            output.AppendLine("# ScaleTest verification report");
            output.AppendLine();
            output.AppendLine("Generated by `ScaleTestSceneBuilder` in Unity Editor. Each case sets the eight scene `Transform.position` values using `ScaleCalculator.CalculatePosition`, then compares those transforms with independently evaluated `origin + normalizedDirection * (AU * metresPerAU)`. Position tolerance: 0.0001 Unity units.");
            output.AppendLine();
            output.AppendLine("Sphere diameter is 0.18 m for visibility only; it is not used in distance calculations. The scene stores the default 1 m/AU, +X layout after the checks.");

            var allPassed = true;
            foreach (var testCase in TestCases)
            {
                var direction = new Vector3(testCase.Direction.x, 0f, testCase.Direction.z).normalized;
                output.AppendLine();
                output.AppendLine($"## {testCase.Name}");
                output.AppendLine();
                output.AppendLine($"Origin: `{Format(testCase.Origin)}`; input direction: `{Format(testCase.Direction)}`; normalized horizontal direction: `{Format(direction)}`; scale: `{testCase.MetresPerAU:F2} m/AU`.");
                output.AppendLine();
                output.AppendLine("| Body | AU | Expected distance (m) | Expected Transform.position | Actual Transform.position | Result |");
                output.AppendLine("| --- | ---: | ---: | --- | --- | --- |");

                var previousProjectedDistance = float.NegativeInfinity;
                for (var index = 0; index < Planets.Length; index++)
                {
                    var planet = Planets[index];
                    var expectedDistance = planet.AU * testCase.MetresPerAU;
                    var expectedPosition = testCase.Origin + direction * expectedDistance;
                    spheres[index].position = ScaleCalculator.CalculatePosition(
                        testCase.Origin,
                        testCase.Direction,
                        planet.AU,
                        testCase.MetresPerAU);

                    var actualPosition = spheres[index].position;
                    var positionMatches = Vector3.Distance(actualPosition, expectedPosition) <= PositionTolerance;
                    var projectedDistance = Vector3.Dot(actualPosition - testCase.Origin, direction);
                    var ordered = projectedDistance > previousProjectedDistance;
                    var distanceMatches = Mathf.Abs(projectedDistance - expectedDistance) <= PositionTolerance;
                    var passed = positionMatches && ordered && distanceMatches;
                    allPassed &= passed;
                    previousProjectedDistance = projectedDistance;

                    output.AppendLine(
                        $"| {planet.Name} | {planet.AU:F2} | {expectedDistance:F3} | `{Format(expectedPosition)}` | `{Format(actualPosition)}` | {(passed ? "PASS" : "FAIL")} |");
                }
            }

            output.AppendLine();
            output.AppendLine($"## Overall result: {(allPassed ? "PASS" : "FAIL")}");
            output.AppendLine();
            output.AppendLine("Distance comes from AU × metresPerAU once. Transform positions are compared in world space after changing origin, horizontal direction, and scale.");
            return output.ToString();
        }

        private static void RestoreDefaultLayout(Transform[] spheres)
        {
            for (var index = 0; index < Planets.Length; index++)
            {
                spheres[index].position = ScaleCalculator.CalculatePosition(
                    Vector3.zero,
                    Vector3.right,
                    Planets[index].AU);
            }
        }

        private static string Format(Vector3 value)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "({0:F3}, {1:F3}, {2:F3})",
                value.x,
                value.y,
                value.z);
        }

        private sealed class Planet
        {
            public Planet(string name, float au)
            {
                Name = name;
                AU = au;
            }

            public string Name { get; }
            public float AU { get; }
        }

        private sealed class TestCase
        {
            public TestCase(string name, Vector3 origin, Vector3 direction, float metresPerAU)
            {
                Name = name;
                Origin = origin;
                Direction = direction;
                MetresPerAU = metresPerAU;
            }

            public string Name { get; }
            public Vector3 Origin { get; }
            public Vector3 Direction { get; }
            public float MetresPerAU { get; }
        }
    }
}
