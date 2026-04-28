using System;
using Game.Localization;
using Game.Level.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using YG;

namespace Game.UI.Hud
{
    public sealed class PlayerSlotView : BaseHudWithModel<PlayerModel>
    {
        public Action<PlayerModel> ON_CLICK;

        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private TMP_Text _selectedText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        [SerializeField] private ScrollView _attributesScroll;

        public ScrollView AttributesScroll => _attributesScroll;

        protected override void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
            YG2.onSwitchLang += OnSwitchLanguage;
        }

        protected override void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            YG2.onSwitchLang -= OnSwitchLanguage;
        }

        protected override void OnModelChanged(PlayerModel model)
        {
            _labelText.text = LocalizedText.Key(model.Label);
            _selectedText.text = LocalizedText.Key("SELECTED");
            _icon.sprite = model.Icon;
            _selectedText.gameObject.SetActive(model.IsSelected);
        }

        private void OnButtonClick()
        {
            ON_CLICK.SafeInvoke(Model);
        }

        private void OnSwitchLanguage(string language)
        {
            if (Model != null)
                OnModelChanged(Model);
        }
    }
}

