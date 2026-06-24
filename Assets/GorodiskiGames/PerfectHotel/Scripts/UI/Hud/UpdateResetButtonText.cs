using TMPro;
using UnityEngine;

namespace Game.UI.Hud
{
    /// <summary>
    /// Вспомогательный скрипт для обновления текста кнопки Reset
    /// Добавьте этот скрипт на кнопку Reset в DeveloperHud префабе
    /// </summary>
    public class UpdateResetButtonText : MonoBehaviour
    {
        [SerializeField] private TMP_Text resetButtonText;
        
        private void Start()
        {
            if (resetButtonText != null)
            {
                // Обновляем текст через LocalizedText
                resetButtonText.text = Localization.LocalizedText.Static("RESET PROGRESS");
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Обновить текст кнопки")]
        private void UpdateTextInEditor()
        {
            if (resetButtonText != null)
            {
                resetButtonText.text = "RESET PROGRESS";
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
        }
#endif
    }
}