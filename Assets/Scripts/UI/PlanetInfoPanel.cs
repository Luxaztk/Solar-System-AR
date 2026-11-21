using System.Globalization;
using SolarSystemAR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SolarSystemAR.UI
{
    public sealed class PlanetInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Text factsText;
        [SerializeField] private Image accentImage;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        public void Show(PlanetData data)
        {
            if (data == null)
            {
                return;
            }

            titleText.text = data.DisplayName;
            summaryText.text = data.Summary;
            factsText.text = BuildFacts(data);
            accentImage.color = data.DisplayColor;
            panelRoot.SetActive(true);
        }

        public void Hide()
        {
            panelRoot.SetActive(false);
        }

        private static string BuildFacts(PlanetData data)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return $"Khoảng cách: {data.DistanceFromSunAu.ToString("0.###", culture)} AU\n" +
                   $"Bán kính trung bình: {data.MeanRadiusKm.ToString("N0", culture)} km\n" +
                   $"Chu kỳ quỹ đạo: {data.OrbitalPeriodDays.ToString("N1", culture)} ngày\n" +
                   $"Chu kỳ tự quay: {data.RotationPeriodHours.ToString("N1", culture)} giờ\n" +
                   $"Số vệ tinh: {data.KnownMoons}\n" +
                   $"Vành đai: {(data.HasRings ? "Có" : "Không")}";
        }

#if UNITY_EDITOR
        public void Configure(
            GameObject root,
            Text title,
            Text summary,
            Text facts,
            Image accent,
            Button close)
        {
            panelRoot = root;
            titleText = title;
            summaryText = summary;
            factsText = facts;
            accentImage = accent;
            closeButton = close;
        }
#endif
    }
}
