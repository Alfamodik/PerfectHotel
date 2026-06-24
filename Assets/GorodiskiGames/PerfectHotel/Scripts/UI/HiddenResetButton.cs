using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    /// <summary>
    /// Скрытая кнопка сброса в правом верхнем углу экрана
    /// Активируется тройным тапом по углу
    /// </summary>
    public class HiddenResetButton : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private Button _hiddenButton;
        [SerializeField] private float _tapTimeout = 1f; // Время между тапами
        [SerializeField] private int _requiredTaps = 3;  // Количество тапов
        
        private float _lastTapTime;
        private int _tapCount;
        
        private void Start()
        {
            if (_hiddenButton != null)
            {
                _hiddenButton.onClick.AddListener(OnHiddenButtonClick);
            }
            
            // Делаем кнопку полностью прозрачной, но кликабельной
            if (TryGetComponent<Image>(out var image))
            {
                image.color = new Color(0, 0, 0, 0.01f); // Почти невидимая
            }
            
            // Устанавливаем в правый верхний угол
            var rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.9f, 0.9f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }
        
        private void Update()
        {
            // Сбрасываем счетчик тапов если прошло слишком много времени
            if (Time.time - _lastTapTime > _tapTimeout)
            {
                _tapCount = 0;
            }
        }
        
        private void OnHiddenButtonClick()
        {
            _tapCount++;
            _lastTapTime = Time.time;
            
            Debug.Log($"Скрытый тап: {_tapCount}/{_requiredTaps}");
            
            if (_tapCount >= _requiredTaps)
            {
                ResetAllProgress();
                _tapCount = 0;
            }
        }
        
        public void OnPointerClick()
        {
            // Альтернативный метод для EventTrigger
            OnHiddenButtonClick();
        }
        
        private void ResetAllProgress()
        {
            Debug.LogWarning("АКТИВИРОВАН СКРЫТЫЙ СБРОС ПРОГРЕССА!");
            
            // Сбрасываем через YG2
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            
            // PlayerPrefs
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();
            
            PlayerPrefs.DeleteAll();
            
            // Перезагрузка
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        
        [ContextMenu("Создать скрытую кнопку")]
        public void CreateHiddenButton()
        {
            // Ищем GameView
            var gameView = FindObjectOfType<Game.UI.GameView>();
            if (gameView == null)
            {
                Debug.LogError("Не найден GameView на сцене!");
                return;
            }
            
            // Создаем объект
            var buttonObj = new GameObject("HiddenResetButton");
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            
            // Добавляем на GameView
            buttonObj.transform.SetParent(gameView.transform, false);
            
            // Настраиваем позицию и размер
            rectTransform.anchorMin = new Vector2(0.95f, 0.95f);
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            
            // Добавляем компоненты
            var button = buttonObj.AddComponent<Button>();
            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.01f); // Почти невидимая
            
            // Добавляем этот же скрипт
            var hiddenButtonScript = buttonObj.AddComponent<HiddenResetButton>();
            hiddenButtonScript._hiddenButton = button;
            
            Debug.Log("Скрытая кнопка создана в правом верхнем углу!");
            Debug.Log("Тапните 3 раза по углу экрана для сброса прогресса.");
        }
    }
}