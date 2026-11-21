using UnityEngine;
using UnityEngine.UI;

namespace SolarSystemAR.UI
{
    public sealed class AppMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OpenTutorial);
            skipButton.onClick.AddListener(BeginExplore);
            continueButton.onClick.AddListener(BeginExplore);
        }

        private void OpenTutorial()
        {
            mainMenu.SetActive(false);
            tutorialPanel.SetActive(true);
        }

        private void BeginExplore()
        {
            mainMenu.SetActive(false);
            tutorialPanel.SetActive(false);
        }

#if UNITY_EDITOR
        public void Configure(
            GameObject menu,
            GameObject tutorial,
            Button start,
            Button skip,
            Button continueValue)
        {
            mainMenu = menu;
            tutorialPanel = tutorial;
            startButton = start;
            skipButton = skip;
            continueButton = continueValue;
        }
#endif
    }
}
