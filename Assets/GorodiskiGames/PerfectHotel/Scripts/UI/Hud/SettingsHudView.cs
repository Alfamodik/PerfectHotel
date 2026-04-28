using Game.Localization;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    public sealed class SettingsHudView : BaseHud
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _developerButton;
        [SerializeField] private Button _restorePurchasesButton;
        [SerializeField] private Toggle _joystickVisibilityToggle;

        public Button CloseButton => _closeButton;
        public Button DeveloperButton => _developerButton;
        public Button RestorePurchasesButton => _restorePurchasesButton;

        public Toggle JoystickVisibilityToggle => _joystickVisibilityToggle;

        protected override void OnEnable()
        {
            LocalizedStaticText.Apply(this);
            YG2.onSwitchLang += OnSwitchLanguage;
        }

        protected override void OnDisable()
        {
            YG2.onSwitchLang -= OnSwitchLanguage;
        }

        private void OnSwitchLanguage(string language)
        {
            LocalizedStaticText.Apply(this);
        }
    }
}
