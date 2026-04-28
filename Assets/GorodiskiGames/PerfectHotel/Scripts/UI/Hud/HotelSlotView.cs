using System;
using Game.Config;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    public sealed class HotelSlotView : BaseHud
    {
        public event Action<int> ON_SLOT_CLICK;

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _youAreHereHolder;

        private HotelConfig _config;
        private int _slotHotelSceneIndex;

        protected override void OnEnable()
        {
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
            ON_SLOT_CLICK?.Invoke(_slotHotelSceneIndex);
        }

        public void Initialize(HotelConfig config, int currentHotelSceneIndex)
        {
            _config = config;
            _slotHotelSceneIndex = config.SceneIndex;
            _icon.sprite = config.Icon;
            _labelText.text = LocalizedText.Key(config.Label);

            bool isHotelsMatch = _slotHotelSceneIndex == currentHotelSceneIndex;

            _button.gameObject.SetActive(!isHotelsMatch);
            _youAreHereHolder.SetActive(isHotelsMatch);
        }

        private void OnSwitchLanguage(string language)
        {
            if (_config != null)
                _labelText.text = LocalizedText.Key(_config.Label);
        }
    }
}

