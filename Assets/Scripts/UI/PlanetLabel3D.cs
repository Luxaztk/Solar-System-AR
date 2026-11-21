using SolarSystemAR.Core;
using UnityEngine;

namespace SolarSystemAR.UI
{
    public sealed class PlanetLabel3D : MonoBehaviour
    {
        [SerializeField] private PlanetData data;
        [SerializeField] private TextMesh label;
        [SerializeField] private Camera targetCamera;

        private void Awake()
        {
            if (label != null && data != null)
            {
                label.text = data.DisplayName;
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera != null)
            {
                transform.rotation = Quaternion.LookRotation(
                    transform.position - targetCamera.transform.position,
                    targetCamera.transform.up);
            }
        }

#if UNITY_EDITOR
        public void Configure(PlanetData planetData, TextMesh textMesh)
        {
            data = planetData;
            label = textMesh;
            if (label != null && data != null)
            {
                label.text = data.DisplayName;
            }
        }
#endif
    }
}
