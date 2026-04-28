using System;
using Game.Localization;
using TMPro;
using UnityEngine;
using YG;

namespace Game.UI.Hud
{
    public sealed class RoomUpgradeHudView : BaseHud
    {
        public event Action ON_APP_QUIT;

        [SerializeField] private GameObject _roomSlotPrefab;
        [SerializeField] private TMP_Text _lvlText;
        [SerializeField] private RectTransform _container;

        public RectTransform Container => _container;
        public GameObject RoomSlotPrefab => _roomSlotPrefab;
        private int _lvl;

        protected override void OnEnable()
        {
            LocalizedStaticText.Apply(this);
            YG2.onSwitchLang += OnSwitchLanguage;
        }

        protected override void OnDisable()
        {
            YG2.onSwitchLang -= OnSwitchLanguage;
        }

        internal void SetLvl(int lvl)
        {
            _lvl = lvl;
            int lvlNice = _lvl + 1;
            _lvlText.text = LocalizedText.Level(lvlNice);
        }

        private void OnSwitchLanguage(string language)
        {
            LocalizedStaticText.Apply(this);
            SetLvl(_lvl);
        }

        private void OnApplicationQuit()
        {
            ON_APP_QUIT?.Invoke();
        }
    }
}

