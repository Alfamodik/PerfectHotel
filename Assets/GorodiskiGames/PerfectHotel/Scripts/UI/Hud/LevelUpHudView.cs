using Game.Domain;
using Game.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.UI.Hud
{
    public sealed class LevelUpHudView : BaseHud
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _lvlText;
        [SerializeField] private TMP_Text _rewardText;

        public Button CloseButton => _closeButton;

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
            _lvlText.text = lvl.ToString();
        }

        internal void SetReward(int reward)
        {
            _rewardText.text = GameConstants.CashIcon + " " + reward.ToString();
        }

        private void OnSwitchLanguage(string language)
        {
            LocalizedStaticText.Apply(this);
        }
    }
}

