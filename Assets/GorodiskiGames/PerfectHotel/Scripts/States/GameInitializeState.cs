using Game.Config;
using Game.Domain;
using Game.Managers;
using Injection;
using YG;

namespace Game.States
{
    public partial class GameInitializeState : GameState
    {
        [Inject] private GameStateManager _gameStateManager;
        [Inject] private Context _context;
        [Inject] private AdsManager _adsManager;
        [Inject] private IAPManager _IAPManager;
        [Inject] private LoginManager _loginManager;

        private GameConfig _config;
        private GameModel _model;

        public override void Initialize()
        {
            _config = GameConfig.Load();
            _model = GameModel.Load(_config);

            _context.Install(_config);
            _context.ApplyInstall();

            _IAPManager.ON_PRODUCT_PURCHASED += OnProductPurchased;
            _IAPManager.Initialize(_config);

            if (YG2.saves.noAdsPurchased)
                _model.IsNoAds = true;

            _loginManager.Initialize(_model);
            _adsManager.Initialize(_model.IsNoAds, _config);

            _gameStateManager.SwitchToState(new GameLoadLevelState());
        }

        public override void Dispose()
        {
            _IAPManager.ON_PRODUCT_PURCHASED -= OnProductPurchased;
        }

        private void OnProductPurchased(string productID)
        {
            if (productID == "no_ads")
            {
                _model.IsNoAds = true;
                _model.Save();
                _adsManager.SetNoAds();
            }
        }
    }
}
