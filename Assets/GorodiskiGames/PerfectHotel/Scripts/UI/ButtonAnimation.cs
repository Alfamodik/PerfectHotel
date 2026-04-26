using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Hud
{
    public sealed class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Min(0f)] private float _toScaleIn = .95f;
        [SerializeField, Min(0f)] private float _durationIn = .05f;
        [SerializeField, Min(0f)] private float _durationOut = .1f;
        [SerializeField, Min(0f)] private float _amplitude = 1f;

        private RectTransform _rectTransform;
        private Button _button;
        private Vector3 _startScale;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _button = GetComponent<Button>();
            _startScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanAnimate())
                return;

            PlayScaleIn();
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!CanAnimate())
                return;

            PlayScaleOut();
        }

        private bool CanAnimate()
        {
            if (!_rectTransform)
                return false;

            return _button == null || _button.interactable;
        }

        public void PlayScaleIn()
        {
            _rectTransform.DOKill(this);
            _rectTransform.DOScale(_startScale * _toScaleIn, _durationIn).SetEase(Ease.InOutQuad).SetId(this);
        }

        public void PlayScaleOut()
        {
            _rectTransform.DOKill(this);
            _rectTransform.DOScale(_startScale, _durationOut).SetEase(Ease.OutBack, _amplitude).SetId(this);
        }
    }
}
