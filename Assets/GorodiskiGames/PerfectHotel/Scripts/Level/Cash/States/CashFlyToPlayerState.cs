using DG.Tweening;
using Game.Config;
using Injection;
using UnityEngine;

namespace Game.Level.Cash.States
{
    public sealed class CashFlyToPlayerState : CashState
    {
        [Inject] private GameManager _gameManager;
        [Inject] private GameConfig _config;

        private Sequence _sequence;

        public override void Initialize()
        {
            Vector3 startPosition = _cash.View.transform.position;
            Vector3 targetPosition = _gameManager.Player.View.AimPosition;
            Vector3 directionFromPlayer = (startPosition - targetPosition).normalized;

            if (directionFromPlayer == Vector3.zero)
                directionFromPlayer = -_gameManager.Player.View.transform.forward;

            var flyConfig = _config.CashFlyToPlayerAnimation;
            Vector3 backPosition = startPosition + directionFromPlayer * flyConfig.BackMoveDistance;
            Vector3 horizontalOffset = flyConfig.ArcHorizontal * _gameManager.Player.View.transform.right;
            Vector3 arcPosition = Vector3.Lerp(backPosition, targetPosition, 0.45f) + Vector3.up * flyConfig.ArcHeight + horizontalOffset;
            Vector3[] path = { arcPosition, targetPosition };

            _sequence = DOTween.Sequence();
            _sequence.Append(_cash.View.transform.DOMove(backPosition, flyConfig.BackMoveDuration).SetEase(flyConfig.BackMoveEase));
            _sequence.Append(_cash.View.transform.DOPath(path, flyConfig.FlyDuration, PathType.CatmullRom).SetEase(flyConfig.FlyEase));
            _sequence.OnComplete(_cash.FireRemoveCash);
        }

        public override void Dispose()
        {
            _sequence?.Kill();
        }
    }
}
