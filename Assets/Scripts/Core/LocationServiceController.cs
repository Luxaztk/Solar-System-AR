using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace SolarSystemAR.Core
{
    /// <summary>
    /// Requests device location access and exposes a diagnostic GPS readout.
    /// GPS is for context and diagnostics only; it must not position planets.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocationServiceController : MonoBehaviour
    {
        [Header("Location Service")]
        [SerializeField]
        private bool m_RequestOnStart = true;

        [SerializeField, Min(1f)]
        private float m_InitializationTimeoutSeconds = 20f;

        [SerializeField, Min(1f)]
        private float m_DesiredAccuracyInMeters = 10f;

        [SerializeField, Min(0f)]
        private float m_UpdateDistanceInMeters = 10f;

        [Header("Diagnostics")]
        [SerializeField]
        private bool m_ShowDiagnosticPanel = true;

        private Coroutine m_InitializationRoutine;
        private bool m_HasRequestedLocation;
        private bool m_HasLocationFix;
        private LocationInfo m_LastLocation;
        private string m_StatusMessage = "Vị trí chưa được yêu cầu.";
        private GUIStyle m_PanelStyle;
        private GUIStyle m_ButtonStyle;

        /// <summary>Whether a GPS/location fix is currently available.</summary>
        public bool HasLocationFix => m_HasLocationFix;

        /// <summary>Latest diagnostic location. Check HasLocationFix before reading.</summary>
        public LocationInfo LastLocation => m_LastLocation;

        /// <summary>Human-readable permission and service state.</summary>
        public string StatusMessage => m_StatusMessage;

        private void Start()
        {
            if (m_RequestOnStart)
            {
                RequestLocationAccess();
            }
        }

        private void Update()
        {
            if (!m_HasRequestedLocation)
            {
                return;
            }

            var service = Input.location;
            if (service.status == LocationServiceStatus.Running)
            {
                if (!service.isEnabledByUser)
                {
                    m_HasLocationFix = false;
                    m_StatusMessage = "Dịch vụ vị trí đã tắt trên thiết bị.";
                    return;
                }

                m_LastLocation = service.lastData;
                m_HasLocationFix = m_LastLocation.timestamp > 0d;
                m_StatusMessage = m_HasLocationFix
                    ? "Đã nhận dữ liệu vị trí; độ chính xác do thiết bị báo."
                    : "Dịch vụ đang chạy, chờ bản ghi vị trí đầu tiên.";
            }
            else if (service.status == LocationServiceStatus.Failed)
            {
                m_HasLocationFix = false;
                m_StatusMessage = GetFailureMessage();
            }
        }

        /// <summary>
        /// Starts the Unity location service. On Android this triggers the
        /// operating-system permission prompt when permission has not been granted.
        /// </summary>
        public void RequestLocationAccess()
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                m_HasRequestedLocation = true;
                m_StatusMessage = "Dịch vụ vị trí đang bật.";
                return;
            }

            if (m_InitializationRoutine != null)
            {
                StopCoroutine(m_InitializationRoutine);
                m_InitializationRoutine = null;
            }

            m_HasLocationFix = false;
            m_HasRequestedLocation = true;
            m_StatusMessage = "Đang yêu cầu quyền và khởi tạo vị trí...";

            try
            {
                Input.location.Start(
                    m_DesiredAccuracyInMeters,
                    m_UpdateDistanceInMeters);
                m_InitializationRoutine = StartCoroutine(WaitForInitialization());
            }
            catch (System.Exception exception)
            {
                m_HasRequestedLocation = false;
                m_StatusMessage = $"Không thể khởi động dịch vụ vị trí: {exception.Message}";
                Debug.LogWarning($"[LocationService] {m_StatusMessage}", this);
            }
        }

        private IEnumerator WaitForInitialization()
        {
            var deadline = Time.realtimeSinceStartup + m_InitializationTimeoutSeconds;
            while (Input.location.status == LocationServiceStatus.Initializing &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            m_InitializationRoutine = null;
            switch (Input.location.status)
            {
                case LocationServiceStatus.Running:
                    m_StatusMessage = "Dịch vụ vị trí đã bật; đang chờ dữ liệu GPS.";
                    break;

                case LocationServiceStatus.Failed:
                    m_StatusMessage = GetFailureMessage();
                    Debug.LogWarning($"[LocationService] {m_StatusMessage}", this);
                    break;

                case LocationServiceStatus.Initializing:
                    Input.location.Stop();
                    m_StatusMessage = $"Hết thời gian chờ ({m_InitializationTimeoutSeconds:F0} giây). " +
                                      "Kiểm tra quyền và dịch vụ vị trí rồi thử lại.";
                    Debug.LogWarning($"[LocationService] {m_StatusMessage}", this);
                    break;

                default:
                    m_StatusMessage = "Dịch vụ vị trí chưa khởi động. Hãy thử yêu cầu lại.";
                    break;
            }
        }

        private static string GetFailureMessage()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                return "Quyền vị trí bị từ chối hoặc chưa được cấp. Cấp quyền trong Cài đặt ứng dụng rồi thử lại.";
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                return "Dịch vụ vị trí đang tắt trên thiết bị. Bật Location/GPS rồi thử lại.";
            }

            return "Không nhận được vị trí. Kiểm tra tín hiệu hoặc quyền ứng dụng rồi thử lại.";
        }

        private void OnDestroy()
        {
            if (m_InitializationRoutine != null)
            {
                StopCoroutine(m_InitializationRoutine);
            }

            if (m_HasRequestedLocation && Input.location.status != LocationServiceStatus.Stopped)
            {
                Input.location.Stop();
            }
        }

        private void OnGUI()
        {
            if (!m_ShowDiagnosticPanel)
            {
                return;
            }

            EnsureGuiStyles();
            var safeArea = Screen.safeArea;
            var margin = Mathf.Max(16f, Screen.width * 0.025f);
            var width = Mathf.Min(900f, safeArea.width - margin * 2f);
            var height = m_HasLocationFix ? 310f : 235f;
            var panelRect = new Rect(
                safeArea.xMin + margin,
                Screen.height - safeArea.yMin - height - margin,
                width,
                height);

            GUILayout.BeginArea(panelRect, m_PanelStyle);
            GUILayout.Label("GPS diagnostics", m_PanelStyle);
            GUILayout.Label($"Trạng thái: {GetServiceState()}", m_PanelStyle);
            GUILayout.Label($"Location bật trên thiết bị: {GetEnabledState()}", m_PanelStyle);
            GUILayout.Label(m_StatusMessage, m_PanelStyle);

            if (m_HasLocationFix)
            {
                GUILayout.Label($"Latitude: {m_LastLocation.latitude:F6}", m_PanelStyle);
                GUILayout.Label($"Longitude: {m_LastLocation.longitude:F6}", m_PanelStyle);
                GUILayout.Label($"Accuracy báo cáo: {m_LastLocation.horizontalAccuracy:F1} m", m_PanelStyle);
                GUILayout.Label($"Timestamp: {m_LastLocation.timestamp:F0}", m_PanelStyle);
            }

            if (GUILayout.Button("YÊU CẦU / THỬ LẠI", m_ButtonStyle, GUILayout.Height(58f)))
            {
                RequestLocationAccess();
            }

            GUILayout.EndArea();
        }

        private string GetServiceState()
        {
            return m_HasRequestedLocation
                ? Input.location.status.ToString()
                : "Chưa khởi động";
        }

        private string GetEnabledState()
        {
            if (!m_HasRequestedLocation)
            {
                return "Chưa kiểm tra";
            }

            return Input.location.isEnabledByUser ? "Bật / được phép" : "Tắt hoặc chưa được phép";
        }

        private void EnsureGuiStyles()
        {
            if (m_PanelStyle != null)
            {
                return;
            }

            var fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.018f), 20, 40);
            m_PanelStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
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
