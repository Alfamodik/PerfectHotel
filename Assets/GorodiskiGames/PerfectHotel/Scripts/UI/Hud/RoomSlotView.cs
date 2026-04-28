using System;
using Game.Localization;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    public sealed class RoomSlotView : BaseHud
    {
        public event Action<int> ON_SLOT_CLICK;

        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;

        private int _index;

        public void Initialize(Sprite icon, int index)
        {
            _icon.sprite = icon;
            _index = index;
        }

        protected override void OnEnable()
        {
            LocalizedStaticText.Apply(this);
            _button.onClick.AddListener(OnSlotButtonClick);
            YG2.onSwitchLang += OnSwitchLanguage;
        }

        protected override void OnDisable()
        {
            _button.onClick.RemoveListener(OnSlotButtonClick);
            YG2.onSwitchLang -= OnSwitchLanguage;
        }

        void OnSlotButtonClick()
        {
            ON_SLOT_CLICK?.Invoke(_index);
        }

        private void OnSwitchLanguage(string language)
        {
            LocalizedStaticText.Apply(this);
        }
    }
}

