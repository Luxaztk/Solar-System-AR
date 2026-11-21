using SolarSystemAR.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SolarSystemAR.UI
{
    public sealed class PlanetPreviewSelector : MonoBehaviour
    {
        [SerializeField] private Camera raycastCamera;
        [SerializeField] private PlanetInfoPanel infoPanel;

        private void Update()
        {
            if (!TryGetPointerDown(out var screenPosition) ||
                EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var cameraToUse = raycastCamera != null ? raycastCamera : Camera.main;
            if (cameraToUse == null)
            {
                return;
            }

            if (Physics.Raycast(cameraToUse.ScreenPointToRay(screenPosition), out var hit))
            {
                var planet = hit.collider.GetComponentInParent<PlanetRuntimeView>();
                if (planet != null)
                {
                    infoPanel.Show(planet.Data);
                }
            }
        }

        private static bool TryGetPointerDown(out Vector2 position)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                return true;
            }

            position = default;
            return false;
        }

#if UNITY_EDITOR
        public void Configure(Camera cameraValue, PlanetInfoPanel panel)
        {
            raycastCamera = cameraValue;
            infoPanel = panel;
        }
#endif
    }
}
