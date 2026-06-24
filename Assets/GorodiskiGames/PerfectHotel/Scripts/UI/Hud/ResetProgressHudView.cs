using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    public sealed class ResetProgressHudView : BaseHud
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;

        public Button ConfirmButton => _confirmButton;
        public Button CancelButton => _cancelButton;
        public TMP_Text TitleText => _titleText;
        public TMP_Text DescriptionText => _descriptionText;

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