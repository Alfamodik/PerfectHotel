using Game.Core.UI;
using Game.Managers;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Game.UI.Hud
{
    public sealed class ResetProgressHudMediator : Mediator<ResetProgressHudView>
    {
        [Inject] private HudManager _hudManager;
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;

        protected override void Show()
        {
            _gameView.Joystick.gameObject.SetActive(false);

            _view.ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
            _view.CancelButton.onClick.AddListener(OnCancelButtonClick);
        }

        protected override void Hide()
        {
            _gameView.Joystick.gameObject.SetActive(true);

            _view.ConfirmButton.onClick.RemoveListener(OnConfirmButtonClick);
            _view.CancelButton.onClick.RemoveListener(OnCancelButtonClick);
        }

        private void OnConfirmButtonClick()
        {
            // Полностью сбрасываем прогресс через YG2
            YG2.SetDefaultSaves();
            YG2.SaveProgress();

            // Также очищаем PlayerPrefs для видимости джойстика
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();

            // Сброс покупки No Ads в локальном сохранении
            if (_gameManager.Model != null)
            {
                _gameManager.Model.IsNoAds = false;
                _gameManager.Model.Remove();
            }

            // Перезагружаем игру
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        private void OnCancelButtonClick()
        {
            _hudManager.HideAdditional<ResetProgressHudMediator>();
        }
    }
}