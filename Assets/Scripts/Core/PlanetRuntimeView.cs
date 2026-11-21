using UnityEngine;

namespace SolarSystemAR.Core
{
    public sealed class PlanetRuntimeView : MonoBehaviour
    {
        [SerializeField] private PlanetData data;
        [SerializeField, Min(0f)] private float rotationDegreesPerSecond = 8f;

        public PlanetData Data => data;

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationDegreesPerSecond * Time.deltaTime, Space.Self);
        }

#if UNITY_EDITOR
        public void Configure(PlanetData planetData, float speed)
        {
            data = planetData;
            rotationDegreesPerSecond = speed;
        }
#endif
    }
}
