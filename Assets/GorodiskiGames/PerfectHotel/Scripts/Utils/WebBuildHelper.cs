using UnityEngine;

namespace Game.Utils
{
    public static class WebBuildHelper
    {
        /// <summary>
        /// Проверяет, должна ли быть видна кнопка сброса прогресса
        /// В веб-сборках всегда видна, в других - только в дебаге
        /// </summary>
        public static bool ShowResetProgressButton()
        {
#if UNITY_WEBGL
            // В WebGL сборке для Яндекс Игр показываем кнопку
            // Можно добавить проверку на тестовую среду Яндекс
            return true;
#else
            // В других платформах - только в дебаг-сборках
            return Debug.isDebugBuild;
#endif
        }
        
        /// <summary>
        /// Проверяет, является ли сборка веб-версией Яндекс Игр
        /// </summary>
        public static bool IsYandexWebBuild()
        {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }
        
        /// <summary>
        /// Проверяет, является ли это тестовой средой Яндекс Игр
        /// (по умолчанию всегда true для сброса прогресса)
        /// </summary>
        public static bool IsYandexTestEnvironment()
        {
#if UNITY_WEBGL
            // В WebGL для Яндекс Игр возвращаем true
            return true;
#else
            // В редакторе тоже true для тестирования
            return true;
#endif
        }
    }
}