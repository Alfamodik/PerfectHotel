using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.AspectRatioFitter;

namespace Game.UI.Hud
{
    public sealed class SplashScreenHudView : BaseHud
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _fillBarImage;
        [SerializeField] private TMP_Text _appVersionText;
        [SerializeField] private TMP_Text _deviceIDText;
        [SerializeField] private RectTransform _aspectRatioTransform;
        [SerializeField] private AspectRatioFitter _aspectRatio;

        public Image Icon => _icon;
        public Image FillBarImage => _fillBarImage;
        public TMP_Text AppVersionText => _appVersionText;

        protected override void OnEnable()
        {
            SetDeviceID();
            RefreshImageLayout();
        }

        protected override void OnDisable()
        {
            
        }

        private void SetDeviceID()
        {
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            _deviceIDText.text = deviceID;
        }

        public void RefreshImageLayout()
        {
            if (_icon != null)
                _icon.preserveAspect = false;

            if (_aspectRatioTransform == null || _aspectRatio == null)
                return;

            ResetTransform();

            var sprite = _icon != null ? _icon.sprite : null;
            _aspectRatio.aspectRatio = sprite != null ? sprite.rect.width / sprite.rect.height : 1f;
            _aspectRatio.aspectMode = AspectMode.EnvelopeParent;
        }

        private void ResetTransform()
        {
            _aspectRatioTransform.anchorMin = Vector2.zero;
            _aspectRatioTransform.anchorMax = Vector2.one;
            _aspectRatioTransform.anchoredPosition = Vector2.zero;
            _aspectRatioTransform.sizeDelta = Vector2.zero;
        }
    }
}
