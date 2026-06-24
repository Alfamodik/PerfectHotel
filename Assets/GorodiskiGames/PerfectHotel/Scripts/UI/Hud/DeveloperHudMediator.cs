using Game.Core.UI;
using Game.Level;
using Game.Managers;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Game.UI.Hud
{
    public sealed class DeveloperHudMediator : Mediator<DeveloperHudView>
    {
        private const int _cash = 100000;

        [Inject] private HudManager _hudManager;
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;
        [Inject] private LevelView _levelView;
        [Inject] private AdsManager _adsManager;

        private bool _isDebugBuild;

        protected override void Show()
        {
            _gameView.Joystick.gameObject.SetActive(false);

            // Для Debug сборки показываем все кнопки
            // Для релиза - только основное
            _view.AddCashButton.gameObject.SetActive(GameConstants.IsDebugBuild());
            // Reset Button всегда доступна для сброса прогресса
            _view.ResetButton.gameObject.SetActive(true);

            _view.SetProgressSliderLimits(0, _levelView.MaxProgress(_gameManager.Model.LoadLvl()));
            _view.LoadProgress(_gameManager.Model.LoadProgress());

            _view.SetLvlSliderLimits(1, _levelView.MaxLevels);
            _view.LoadLvl(_gameManager.Model.LoadLvl());

            _view.SAVE += Save;

            _view.CloseButton.onClick.AddListener(OnCloseButtonClick);
            _view.AddCashButton.onClick.AddListener(OnAddCashButtonClick);
            _view.ResetButton.onClick.AddListener(OnResetButtonClick);
            _view.LoadGameplayButton.onClick.AddListener(OnLoadGameplayButtonClick);
            _view.RewAdsButton.onClick.AddListener(OnRewAdsButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _hudManager.HideAdditional<DeveloperHudMediator>();
        }

        protected override void Hide()
        {
            _gameView.Joystick.gameObject.SetActive(true);

            _view.SAVE -= Save;

            _view.CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            _view.AddCashButton.onClick.RemoveListener(OnAddCashButtonClick);
            _view.ResetButton.onClick.RemoveListener(OnResetButtonClick);
            _view.LoadGameplayButton.onClick.RemoveListener(OnLoadGameplayButtonClick);
            _view.RewAdsButton.onClick.RemoveListener(OnRewAdsButtonClick);
        }

        private void OnLoadGameplayButtonClick()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        private void Save()
        {
            _gameManager.Model.SaveProgress(_view.Progress);
            _gameManager.Model.SaveLvl(_view.Lvl);
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();

            _gameManager.FireProgressChanged(_gameManager.Model.LoadProgress());
            _gameManager.FireLevelChanged(_gameManager.Model.LoadLvl());
        }

        private void OnAddCashButtonClick()
        {
            _gameManager.Model.Cash += _cash;
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();
        }

        private void OnResetButtonClick()
        {
            // Показываем подтверждение перед сбросом
            ShowResetConfirmation();
        }

        private void ShowResetConfirmation()
        {
            // Можно использовать простой MessageBox или добавить UI
            // Пока делаем прямое подтверждение через debug сообщение
            // В будущем можно добавить диалог подтверждения
            
            // Сразу сбрасываем прогресс
            YG2.SetDefaultSaves();
            YG2.SaveProgress();

            // Также очищаем PlayerPrefs для видимости джойстика
            const string JoystickVisibilityPrefsKey = "JoystickVisibility";
            PlayerPrefs.SetInt(JoystickVisibilityPrefsKey, 1);
            PlayerPrefs.Save();

            OnLoadGameplayButtonClick();
        }

        private void OnRewAdsButtonClick()
        {
            _adsManager.ShowRewarded();
        }
    }
}


