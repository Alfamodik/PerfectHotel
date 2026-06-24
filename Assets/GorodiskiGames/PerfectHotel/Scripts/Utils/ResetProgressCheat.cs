using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Game.Utils
{
    /// <summary>
    /// Секретный чит для сброса прогресса
    /// Работает при удержании Backspace + клавиши R
    /// </summary>
    public class ResetProgressCheat : MonoBehaviour
    {
        [Header("Настройки")]
        [Tooltip("Время удержания клавиш для активации (секунды)")]
        [SerializeField] private float holdTime = 3f;
        
        [Tooltip("Показывать сообщение при активации")]
        [SerializeField] private bool showDebugMessage = true;
        
        private float _holdTimer;
        private bool _isBackspaceHeld;
        private bool _cheatActivated;
        
        private void Update()
        {
            CheckResetCheat();
        }
        
        private void CheckResetCheat()
        {
            // ПРОСТОЙ ВАРИАНТ: Удерживать R 3 секунды
            if (Input.GetKey(KeyCode.R))
            {
                _holdTimer += Time.deltaTime;
                
                // Показываем прогресс в консоли
                if (!_cheatActivated && _holdTimer > 0.5f)
                {
                    if (Mathf.FloorToInt(_holdTimer) > Mathf.FloorToInt(_holdTimer - Time.deltaTime))
                    {
                        Debug.Log($"Удерживайте R для сброса: {Mathf.FloorToInt(_holdTimer)}/{holdTime} сек");
                    }
                }
                
                if (_holdTimer >= holdTime && !_cheatActivated)
                {
                    ActivateResetCheat();
                }
            }
            else
            {
                // Сбрасываем таймер если R отпущена
                if (_holdTimer > 0 && _holdTimer < holdTime)
                {
                    Debug.Log("R отпущена, сброс отменен");
                }
                _holdTimer = 0f;
            }
            
            // Быстрые комбинации для разработчиков
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                ActivateResetCheat();
            }
            
            // WebGL альтернатива: комбинация 1-2-3
            CheckNumberCombination();
            
            // Проверяем комбинацию для мобильных устройств (если есть тапы)
            CheckMobileCheat();
        }
        
        private void CheckNumberCombination()
        {
            // Альтернативная комбинация для WebGL: 1, 2, 3 быстро
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                StartCoroutine(Check123Combo());
            }
        }
        
        private System.Collections.IEnumerator Check123Combo()
        {
            float timeout = 1f;
            float timer = 0f;
            bool got2 = false;
            bool got3 = false;
            
            while (timer < timeout)
            {
                timer += Time.deltaTime;
                
                if (!got2 && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
                {
                    got2 = true;
                    Debug.Log("Получена 2... ждем 3");
                }
                
                if (got2 && !got3 && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
                {
                    got3 = true;
                    Debug.Log("Комбинация 1-2-3 получена!");
                    ActivateResetCheat();
                    yield break;
                }
                
                yield return null;
            }
            
            if (got2 && !got3)
            {
                Debug.Log("Таймаут комбинации 1-2-3");
            }
        }
        
        private void CheckMobileCheat()
        {
            // Для тестирования в редакторе или на мобильных устройствах
            // Тройное касание по экрану в углу
            if (Input.touchCount == 3)
            {
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);
                var touch3 = Input.GetTouch(2);
                
                // Проверяем что все касания в правом верхнем углу
                bool allInCorner = true;
                foreach (var touch in Input.touches)
                {
                    var viewportPos = Camera.main.ScreenToViewportPoint(touch.position);
                    if (viewportPos.x < 0.8f || viewportPos.y < 0.8f)
                    {
                        allInCorner = false;
                        break;
                    }
                }
                
                if (allInCorner && !_cheatActivated)
                {
                    ActivateResetCheat();
                }
            }
        }
        
        private void ActivateResetCheat()
        {
            if (_cheatActivated)
                return;
                
            _cheatActivated = true;
            
            if (showDebugMessage)
            {
                Debug.LogWarning("ЧИТ АКТИВИРОВАН: СБРОС ПРОГРЕССА!");
            }
            
            // Полный сброс прогресса
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            
            // Сброс PlayerPrefs
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();
            
            // Сбрасываем локальные данные модели
            PlayerPrefs.DeleteAll();
            
            // Сообщение в консоль
            Debug.Log("Прогресс полностью сброшен. Перезагрузка игры...");
            
            // Перезагрузка сцены
            SceneManager.LoadScene(0);
        }
        
        /// <summary>
        /// Публичный метод для сброса прогресса (можно вызывать из других скриптов)
        /// </summary>
        public static void ResetAllProgress()
        {
            Debug.LogWarning("РУЧНОЙ СБРОС ПРОГРЕССА");
            
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();
            
            PlayerPrefs.DeleteAll();
            
            SceneManager.LoadScene(0);
        }
        
        /// <summary>
        /// Метод для сброса прогресса через консоль (в веб-версии)
        /// </summary>
        public void ConsoleReset()
        {
            Debug.Log("Вызов сброса прогресса из консоли");
            ResetAllProgress();
        }
    }
}