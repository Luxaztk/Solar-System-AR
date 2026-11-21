using System.Collections.Generic;
using SolarSystemAR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SolarSystemAR.UI
{
    public sealed class DiscoveryProgressController : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text progressText;
        [SerializeField, Min(1)] private int totalBodies = 9;
        private readonly HashSet<string> discoveredIds = new();

        public void MarkDiscovered(PlanetData data)
        {
            if (data != null && discoveredIds.Add(data.Id))
            {
                Refresh();
            }
        }

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            var ratio = Mathf.Clamp01((float)discoveredIds.Count / totalBodies);
            progressSlider.value = ratio;
            progressText.text = $"Đã khám phá {discoveredIds.Count}/{totalBodies}";
        }

#if UNITY_EDITOR
        public void Configure(Slider slider, Text label, int count)
        {
            progressSlider = slider;
            progressText = label;
            totalBodies = count;
        }
#endif
    }
}
