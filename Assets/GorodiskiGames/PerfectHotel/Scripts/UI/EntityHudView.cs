using Game;
using Game.Level.Place;
using Game.Localization;
using Game.UI.Hud;
using TMPro;
using UnityEngine;
using Utilities;
using YG;

namespace Game.Level.Entity
{
    public class EntityHudView : BaseHudWithModel<EntityModel>
    {
        [SerializeField] private GameObject _lockedIcon;
        [SerializeField] private GameObject _purchaseIcon;
        [SerializeField] private GameObject _updateIcon;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _priceText;

        private string _info;
        private string _price;

        protected override void OnEnable()
        {
            YG2.onSwitchLang += OnSwitchLanguage;
        }

        protected override void OnDisable()
        {
            YG2.onSwitchLang -= OnSwitchLanguage;
        }

        public void Locked()
        {
            _lockedIcon.SetActive(true);
            _purchaseIcon.SetActive(false);
            _updateIcon.SetActive(false);
        }

        public void ReadyToPuchase()
        {
            _lockedIcon.SetActive(false);
            _purchaseIcon.SetActive(true);
            _updateIcon.SetActive(false);
        }

        public void ReadyToUpdate()
        {
            _lockedIcon.SetActive(false);
            _purchaseIcon.SetActive(false);
            _updateIcon.SetActive(true);
        }

        protected override void OnApplyModel(EntityModel model)
        {
            base.OnApplyModel(model);
        }

        protected override void OnModelChanged(EntityModel model)
        {
            _info = LocalizedText.Key(model.Type.ToString().ToUpper());
            _price = GameConstants.CashIcon + " " + MathUtil.NiceCash(model.PricePurchase);

            if (model.Type == EntityType.Area || model.Type == EntityType.Elevator)
            {
                if (model.IsLocked)
                {
                    _info = LocalizedText.Key("LEVEL") + " " + model.TargetPurchaseValue;
                    _price = "";
                }
                else
                {
                    if (model.Type == EntityType.Area)
                        _info = LocalizedText.Key("NEW AREA");
                    else if (model.Type == EntityType.Elevator)
                        _info = LocalizedText.Key("NEW HOTEL");
                }
            }

            if (model.IsPurchased)
            {
                _price = GameConstants.CashIcon + " " + MathUtil.NiceCash(model.PriceUpdate);

                int lvl = model.LvlNext + 1;
                _info = LocalizedText.Key("LVL") + " " + lvl;

                if (model.Type == EntityType.Reception)
                    _info = LocalizedText.Key("RECEPTIONIST");
                else if (model.Type == EntityType.Cleaner)
                    _info = LocalizedText.Key("SPEED");
            }

            _infoText.text = _info;
            _priceText.text = _price;
        }

        private void OnSwitchLanguage(string language)
        {
            if (Model != null)
                OnModelChanged(Model);
        }
    }
}


