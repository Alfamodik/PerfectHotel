using UnityEngine;
using YG;

namespace Game.Utils
{
    /// <summary>
    /// Вспомогательный скрипт для создания объекта сброса прогресса
    /// Добавьте этот скрипт на любой GameObject в сцене
    /// </summary>
    public class CreateResetHelper : MonoBehaviour
    {
        [Header("Настройки")]
        [Tooltip("Создать автоматически при старте")]
        [SerializeField] private bool createOnStart = true;
        
        [Tooltip("Тип сброса: 1-Скрытая кнопка, 2-Комбинации клавиш, 3-Кнопка по F12")]
        [SerializeField] private int resetType = 1;
        
        [Tooltip("Размер скрытой кнопки (от 0 до 1)")]
        [SerializeField] private float hiddenButtonSize = 0.05f;
        
        private void Start()
        {
            if (createOnStart)
            {
                CreateResetObject();
            }
        }
        
        [ContextMenu("Создать объект сброса")]
        public void CreateResetObject()
        {
            // Создаем пустой GameObject
            GameObject resetObject = new GameObject("ProgressResetHelper");
            
            // Не уничтожаем при загрузке сцен
            DontDestroyOnLoad(resetObject);
            
            switch (resetType)
            {
                case 1:
                    CreateHiddenButton(resetObject);
                    break;
                case 2:
                    CreateKeyCombinationCheat(resetObject);
                    break;
                case 3:
                    CreateSecretButton(resetObject);
                    break;
            }
            
            Debug.Log($"Создан объект сброса прогресса: {resetObject.name}");
            Debug.Log($"Тип: {GetResetTypeDescription(resetType)}");
        }
        
        private void CreateHiddenButton(GameObject parent)
        {
            // Добавляем скрытую кнопку
            var hiddenButton = parent.AddComponent<UI.Hud.HiddenResetButton>();
            
            // Создаем Canvas если его нет
            CreateCanvasIfNeeded();
            
            // Создаем кнопку UI
            CreateHiddenUIButton(parent);
        }
        
        private void CreateKeyCombinationCheat(GameObject parent)
        {
            // Добавляем чит с комбинациями клавиш
            parent.AddComponent<ResetProgressCheat>();
            
            Debug.Log("Чит сброса прогресса создан!");
            Debug.Log("Комбинации: Удерживайте R 3 секунды или Ctrl+Shift+R");
        }
        
        private void CreateSecretButton(GameObject parent)
        {
            // Добавляем секретную кнопку
            var secretButton = parent.AddComponent<UI.Hud.SecretResetButton>();
            
            Debug.Log("Секретная кнопка создана!");
            Debug.Log("Нажмите F12 чтобы показать/скрыть кнопку сброса");
        }
        
        private void CreateHiddenUIButton(GameObject parent)
        {
            // Создаем UI кнопку
            var buttonObj = new GameObject("HiddenResetUI");
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            
            // Находим Canvas
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas не найден! Создайте Canvas в сцене.");
                return;
            }
            
            // Родитель - Canvas
            buttonObj.transform.SetParent(canvas.transform, false);
            
            // Настраиваем позицию и размер
            rectTransform.anchorMin = new Vector2(1 - hiddenButtonSize, 1 - hiddenButtonSize);
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            
            // Добавляем компоненты
            var button = buttonObj.AddComponent<UnityEngine.UI.Button>();
            var image = buttonObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, 0.01f); // Почти невидимая
            
            // HiddenResetButton сам найдет кнопку через GetComponent
            // Или можно добавить EventTrigger для ручного управления
            
            Debug.Log($"Скрытая кнопка создана в правом верхнем углу (размер: {hiddenButtonSize * 100}%)");
            Debug.Log($"Тапните 3 раза по этой области для сброса прогресса");
        }
        
        private void CreateCanvasIfNeeded()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.Log("Canvas не найден, создаю новый...");
                var canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                Debug.Log("Canvas создан!");
            }
        }
        
        private string GetResetTypeDescription(int type)
        {
            return type switch
            {
                1 => "Скрытая кнопка (тройной тап в углу)",
                2 => "Комбинации клавиш (Backspace+R)",
                3 => "Секретная кнопка (F12)",
                _ => "Неизвестный тип"
            };
        }
        
        [ContextMenu("Тест сброса прогресса")]
        public void TestReset()
        {
            Debug.LogWarning("ТЕСТИРУЕМ СБРОС ПРОГРЕССА...");
            
            // Симулируем сброс
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();
            
            Debug.Log("Прогресс сброшен! (тестовый режим)");
            Debug.Log("В реальном режиме игра перезагрузится");
        }
    }
}