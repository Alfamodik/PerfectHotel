using UnityEngine;

namespace Game.Utils
{
    public class CreateResetProgressPrefabHelper : MonoBehaviour
    {
        [Header("Instructions:")]
        [TextArea(5, 10)]
        public string Instructions = @"Создайте префаб ResetProgressHud в Assets/GorodiskiGames/PerfectHotel/ResourcesStatic/Prefabs/UI/Huds/

Структура:
- ResetProgressHud (Canvas)
  - Panel (Image с затемнением)
  - Dialog (Panel с UI элементами)
    - Title (TextMeshPro - 'СБРОС ПРОГРЕССА')
    - Description (TextMeshPro - текст подтверждения)
    - Buttons (Horizontal Layout Group)
      - Cancel (Button + Text - 'ОТМЕНА')
      - Confirm (Button + Text - 'СБРОСИТЬ')
      
Настройки:
- Canvas: Render Mode = Screen Space Overlay
- На ResetProgressHud повесить компонент ResetProgressHudView
- Назначить ссылки на кнопки и тексты
- В HudManager добавить префаб в список дополнительных худи";

        private void Start()
        {
            Debug.Log("Создание префаба ResetProgressHud");
            Debug.Log(Instructions);
        }
    }
}