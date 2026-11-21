using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SolarSystemAR.AR
{
    /// <summary>
    /// Places one Sun on a detected horizontal plane and keeps it attached to
    /// an AR anchor. A lightweight runtime UI provides instructions and reset.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARAnchorManager))]
    public sealed class ARPlacementManager : MonoBehaviour
    {
        private static readonly List<ARRaycastHit> RaycastHits = new();

        [Header("Placement")]
        [SerializeField]
        private GameObject m_SunPrefab;

        [SerializeField, Min(0.05f)]
        private float m_PlaceholderDiameter = 0.15f;

        [Header("Runtime UI")]
        [SerializeField]
        private bool m_ShowRuntimeUi = true;

        private ARRaycastManager m_RaycastManager;
        private ARAnchorManager m_AnchorManager;
        private ARPlaneManager m_PlaneManager;
        private ARAnchor m_PlacementAnchor;
        private GameObject m_SunInstance;
        private Material m_RuntimeSunMaterial;
        private bool m_IsCreatingAnchor;
        private GUIStyle m_StatusStyle;
        private GUIStyle m_ButtonStyle;
        private string m_StatusMessage =
            "Di chuyển thiết bị để tìm mặt phẳng, sau đó chạm để đặt Mặt Trời.";

        public bool HasPlacement => m_PlacementAnchor != null && m_SunInstance != null;

        public GameObject SunPrefab
        {
            get => m_SunPrefab;
            set => m_SunPrefab = value;
        }

        private void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_AnchorManager = GetComponent<ARAnchorManager>();
            m_PlaneManager = GetComponent<ARPlaneManager>();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (HasPlacement || m_IsCreatingAnchor || Touch.activeTouches.Count == 0)
            {
                return;
            }

            var touch = Touch.activeTouches[0];
            if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            {
                return;
            }

            if (!m_RaycastManager.Raycast(
                    touch.screenPosition,
                    RaycastHits,
                    TrackableType.PlaneWithinPolygon))
            {
                m_StatusMessage = "Chưa chạm đúng mặt phẳng. Hãy quét thêm rồi thử lại.";
                return;
            }

            CreatePlacementAnchorAsync(RaycastHits[0].pose);
        }

        private async void CreatePlacementAnchorAsync(Pose hitPose)
        {
            m_IsCreatingAnchor = true;
            m_StatusMessage = "Đang tạo anchor...";

            var result = await m_AnchorManager.TryAddAnchorAsync(hitPose);
            if (this == null)
            {
                return;
            }

            m_IsCreatingAnchor = false;
            if (result.status.IsError() || result.value == null)
            {
                m_StatusMessage = $"Không thể tạo anchor ({result.status}). Hãy thử lại.";
                Debug.LogWarning($"[ARPlacement] Anchor creation failed: {result.status}");
                return;
            }

            m_PlacementAnchor = result.value;
            m_SunInstance = CreateSunInstance(m_PlacementAnchor.transform);
            SetPlaneVisualization(false);
            m_StatusMessage = "Đã đặt Mặt Trời. Nhấn Reset để đặt lại.";
            Debug.Log($"[ARPlacement] Sun placed at {hitPose.position} with anchor " +
                      $"{m_PlacementAnchor.trackableId}.");
        }

        private GameObject CreateSunInstance(Transform anchorTransform)
        {
            GameObject sun;
            if (m_SunPrefab != null)
            {
                sun = Instantiate(m_SunPrefab, anchorTransform);
            }
            else
            {
                sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sun.name = "Sun Placeholder";
                sun.transform.SetParent(anchorTransform, false);
                sun.transform.localScale = Vector3.one * m_PlaceholderDiameter;

                // Keep an explicit runtime reference so IL2CPP does not strip
                // SphereCollider before GameObject.CreatePrimitive can add it.
                var collider = sun.GetComponent<SphereCollider>();
                if (collider != null)
                {
                    Destroy(collider);
                }

                ConfigurePlaceholderMaterial(sun.GetComponent<Renderer>());
            }

            // Keep the sphere above the detected surface instead of embedding
            // half of it below the plane.
            sun.transform.localPosition = Vector3.up * (m_PlaceholderDiameter * 0.5f);
            sun.transform.localRotation = Quaternion.identity;
            return sun;
        }

        private void ConfigurePlaceholderMaterial(Renderer sunRenderer)
        {
            if (sunRenderer == null)
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                Debug.LogWarning("[ARPlacement] No compatible shader found for placeholder Sun.");
                return;
            }

            var sunColor = new Color(1f, 0.38f, 0.02f, 1f);
            m_RuntimeSunMaterial = new Material(shader)
            {
                name = "Runtime Sun Placeholder Material",
                color = sunColor,
            };

            if (m_RuntimeSunMaterial.HasProperty("_BaseColor"))
            {
                m_RuntimeSunMaterial.SetColor("_BaseColor", sunColor);
            }

            if (m_RuntimeSunMaterial.HasProperty("_EmissionColor"))
            {
                m_RuntimeSunMaterial.EnableKeyword("_EMISSION");
                m_RuntimeSunMaterial.SetColor("_EmissionColor", sunColor * 2.5f);
            }

            sunRenderer.sharedMaterial = m_RuntimeSunMaterial;
        }

        public void ResetPlacement()
        {
            if (m_SunInstance != null)
            {
                Destroy(m_SunInstance);
                m_SunInstance = null;
            }

            if (m_PlacementAnchor != null)
            {
                if (!m_AnchorManager.TryRemoveAnchor(m_PlacementAnchor))
                {
                    Destroy(m_PlacementAnchor.gameObject);
                }

                m_PlacementAnchor = null;
            }

            if (m_RuntimeSunMaterial != null)
            {
                Destroy(m_RuntimeSunMaterial);
                m_RuntimeSunMaterial = null;
            }

            SetPlaneVisualization(true);
            m_StatusMessage = "Đã reset. Chạm vào mặt phẳng để đặt lại Mặt Trời.";
            Debug.Log("[ARPlacement] Placement reset.");
        }

        private void SetPlaneVisualization(bool visible)
        {
            if (m_PlaneManager == null)
            {
                return;
            }

            if (visible && !m_PlaneManager.enabled)
            {
                m_PlaneManager.enabled = true;
            }

            foreach (var plane in m_PlaneManager.trackables)
            {
                foreach (var planeRenderer in plane.GetComponentsInChildren<Renderer>(true))
                {
                    planeRenderer.enabled = visible;
                }
            }

            if (!visible)
            {
                m_PlaneManager.enabled = false;
            }
        }

        private void OnGUI()
        {
            if (!m_ShowRuntimeUi)
            {
                return;
            }

            EnsureGuiStyles();
            var safeArea = Screen.safeArea;
            var margin = Mathf.Max(16f, Screen.width * 0.025f);
            var top = Screen.height - safeArea.yMax + margin;
            var statusRect = new Rect(
                safeArea.xMin + margin,
                top,
                safeArea.width - margin * 2f,
                Mathf.Max(84f, Screen.height * 0.07f));

            GUI.Box(statusRect, m_StatusMessage, m_StatusStyle);

            if (!HasPlacement)
            {
                return;
            }

            var buttonWidth = Mathf.Min(360f, safeArea.width * 0.42f);
            var buttonHeight = Mathf.Max(96f, Screen.height * 0.075f);
            var buttonRect = new Rect(
                safeArea.xMax - margin - buttonWidth,
                Screen.height - safeArea.yMin - margin - buttonHeight,
                buttonWidth,
                buttonHeight);

            if (GUI.Button(buttonRect, "RESET", m_ButtonStyle))
            {
                ResetPlacement();
            }
        }

        private void EnsureGuiStyles()
        {
            if (m_StatusStyle != null)
            {
                return;
            }

            var fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.022f), 22, 52);
            m_StatusStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                wordWrap = true,
                normal = { textColor = Color.white },
            };

            m_ButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
            };
        }
    }
}
