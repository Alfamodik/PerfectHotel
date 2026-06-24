using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Hud
{
    /// <summary>
    /// Секретная кнопка сброса прогресса, видимая только при удержании клавиши
    /// </summary>
    public class SecretResetButton : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private Button _resetButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeSpeed = 5f;
        [SerializeField] private KeyCode _showKey = KeyCode.F12;
        
        private bool _isVisible;
        private float _targetAlpha;
        
        private void Start()
        {
            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(OnResetButtonClick);
            }
            
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            
            // Начально скрыта
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        
        private void Update()
        {
            // Показываем кнопку при нажатии F12
            if (Input.GetKeyDown(_showKey))
            {
                ToggleVisibility();
            }
            
            // Плавное появление/исчезновение
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, Time.deltaTime * _fadeSpeed);
        }
        
        private void ToggleVisibility()
        {
            _isVisible = !_isVisible;
            _targetAlpha = _isVisible ? 1f : 0f;
            _canvasGroup.blocksRaycasts = _isVisible;
            _canvasGroup.interactable = _isVisible;
            
            Debug.Log($"Секретная кнопка сброса: {(_isVisible ? "ВКЛЮЧЕНА" : "ВЫКЛЮЧЕНА")}");
        }
        
        private void OnResetButtonClick()
        {
            Debug.LogWarning("НАЖАТА СЕКРЕТНАЯ КНОПКА СБРОСА ПРОГРЕССА!");
            
            // Показываем подтверждение
            if (Application.isPlaying)
            {
                // Можно добавить диалог подтверждения
                // Пока делаем сразу
                ResetProgress();
            }
        }
        
        private void ResetProgress()
        {
            // Вызываем статический метод из ResetProgressCheat
            Utils.ResetProgressCheat.ResetAllProgress();
        }
        
        /// <summary>
        /// Включить/выключить видимость кнопки из кода
        /// </summary>
        public void SetVisible(bool visible)
        {
            _isVisible = visible;
            _targetAlpha = visible ? 1f : 0f;
            _canvasGroup.blocksRaycasts = visible;
            _canvasGroup.interactable = visible;
        }
        
        /// <summary>
        /// Показать на короткое время (для отладки)
        /// </summary>
        public void ShowTemporarily(float duration = 3f)
        {
            SetVisible(true);
            Invoke(nameof(Hide), duration);
        }
        
        private void Hide()
        {
            SetVisible(false);
        }
        
        /// <summary>
        /// Быстрое создание кнопки через код
        /// </summary>
        [ContextMenu("Создать секретную кнопку")]
        public void CreateSecretButton()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Не найден Canvas на сцене!");
                return;
            }
            
            // Создаем объект кнопки
            var buttonObj = new GameObject("SecretResetButton");
            buttonObj.transform.SetParent(canvas.transform, false);
            
            // Добавляем компоненты
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.9f, 0.1f);
            rectTransform.anchorMax = new Vector2(0.95f, 0.15f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            
            var canvasGroup = buttonObj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            
            var image = buttonObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(1f, 0.5f, 0.5f, 0.7f);
            
            var button = buttonObj.AddComponent<UnityEngine.UI.Button>();
            
            // Создаем текст
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "RESET";
            text.color = Color.white;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            
            Debug.Log("Секретная кнопка создана! Назначьте её в компоненте SecretResetButton.");
        }
    }
}