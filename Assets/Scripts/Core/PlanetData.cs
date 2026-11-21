using UnityEngine;

namespace SolarSystemAR.Core
{
    [CreateAssetMenu(fileName = "PlanetData", menuName = "Solar System AR/Planet Data")]
    public sealed class PlanetData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 5)] private string summary;
        [SerializeField, Min(0f)] private float distanceFromSunAu;
        [SerializeField, Min(0f)] private float meanRadiusKm;
        [SerializeField, Min(0f)] private float orbitalPeriodDays;
        [SerializeField, Min(0f)] private float rotationPeriodHours;
        [SerializeField, Min(0)] private int knownMoons;
        [SerializeField] private Color displayColor = Color.white;
        [SerializeField] private bool hasRings;
        [SerializeField] private string sourceUrl;

        public string Id => id;
        public string DisplayName => displayName;
        public string Summary => summary;
        public float DistanceFromSunAu => distanceFromSunAu;
        public float MeanRadiusKm => meanRadiusKm;
        public float OrbitalPeriodDays => orbitalPeriodDays;
        public float RotationPeriodHours => rotationPeriodHours;
        public int KnownMoons => knownMoons;
        public Color DisplayColor => displayColor;
        public bool HasRings => hasRings;
        public string SourceUrl => sourceUrl;

#if UNITY_EDITOR
        public void Configure(
            string newId,
            string newDisplayName,
            string newSummary,
            float newDistanceFromSunAu,
            float newMeanRadiusKm,
            float newOrbitalPeriodDays,
            float newRotationPeriodHours,
            int newKnownMoons,
            Color newDisplayColor,
            bool newHasRings,
            string newSourceUrl)
        {
            id = newId;
            displayName = newDisplayName;
            summary = newSummary;
            distanceFromSunAu = newDistanceFromSunAu;
            meanRadiusKm = newMeanRadiusKm;
            orbitalPeriodDays = newOrbitalPeriodDays;
            rotationPeriodHours = newRotationPeriodHours;
            knownMoons = newKnownMoons;
            displayColor = newDisplayColor;
            hasRings = newHasRings;
            sourceUrl = newSourceUrl;
        }
#endif
    }
}
