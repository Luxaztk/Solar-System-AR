using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolarSystemAR.UI
{
    public sealed class QuizController : MonoBehaviour
    {
        [Serializable]
        private sealed class QuizCollection { public QuizQuestion[] questions; }

        [Serializable]
        private sealed class QuizQuestion
        {
            public string prompt;
            public string[] options;
            public int correctIndex;
            public string explanation;
        }

        [SerializeField] private TextAsset quizData;
        [SerializeField] private Text questionText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button[] optionButtons;
        private QuizQuestion[] questions = Array.Empty<QuizQuestion>();
        private int currentIndex;

        private void Start()
        {
            if (quizData != null)
            {
                questions = JsonUtility.FromJson<QuizCollection>(quizData.text)?.questions ??
                            Array.Empty<QuizQuestion>();
            }
            ShowCurrent();
        }

        public void Choose(int optionIndex)
        {
            if (questions.Length == 0)
            {
                return;
            }

            var current = questions[currentIndex];
            feedbackText.text = optionIndex == current.correctIndex
                ? $"Chính xác! {current.explanation}"
                : $"Chưa đúng. {current.explanation}";
            currentIndex = (currentIndex + 1) % questions.Length;
            Invoke(nameof(ShowCurrent), 1.4f);
        }

        private void ShowCurrent()
        {
            if (questions.Length == 0)
            {
                questionText.text = "Chưa có dữ liệu câu hỏi.";
                return;
            }

            var current = questions[currentIndex];
            questionText.text = current.prompt;
            feedbackText.text = string.Empty;
            for (var i = 0; i < optionButtons.Length; i++)
            {
                var optionIndex = i;
                var visible = i < current.options.Length;
                optionButtons[i].gameObject.SetActive(visible);
                if (!visible) continue;
                optionButtons[i].GetComponentInChildren<Text>().text = current.options[i];
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => Choose(optionIndex));
            }
        }

#if UNITY_EDITOR
        public void Configure(TextAsset data, Text question, Text feedback, Button[] buttons)
        {
            quizData = data;
            questionText = question;
            feedbackText = feedback;
            optionButtons = buttons;
        }
#endif
    }
}
