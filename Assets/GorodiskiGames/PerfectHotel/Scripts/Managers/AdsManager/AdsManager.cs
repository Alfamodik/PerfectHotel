using System;
using Game.Config;
using Game.Core;
using Utilities;
using YG;



namespace Game.Managers
{
    public enum AdsProviderType
    {
        Fake,
        AdMob
    }

    public enum AdsPlacement
    {
        Default,
        CashForAds,
        CashForAdsEquipmentHud,
        EquipmentForAds
    }

    public sealed class AdsManager : IDisposable
    {
        public Action ON_INTERSTITIAL_SHOW;
        public Action ON_INTERSTITIAL_WATCHED;
        public Action ON_INTERSTITIAL_LOADED;
        public Action ON_INTERSTITIAL_FAILED_TO_LOAD;

        public Action ON_REWARDED_WATCHED;

        private bool _isNoAds;

        public void Initialize(bool isNoAds, GameConfig config)
        {
            _isNoAds = isNoAds;

            YG2.onOpenInterAdv += OnInterstitialShow;
            YG2.onCloseInterAdv += OnInterstitialWatched;

            YG2.onCloseRewardedAdv += OnRewardedWatched;
            YG2.onGetSDKData += OnSDKDataReceived;

            if (YG2.saves.noAdsPurchased)
                SetNoAds();
        }

        public void Dispose()
        {
            YG2.onOpenInterAdv -= OnInterstitialShow;
            YG2.onCloseInterAdv -= OnInterstitialWatched;
            YG2.onCloseRewardedAdv -= OnRewardedWatched;
            YG2.onGetSDKData -= OnSDKDataReceived;
        }

        public void ShowInterstitial()
        {
            if (_isNoAds)
                return;

            if (YG2.isTimerAdvCompleted)
                ManualTimerBeforeAdsYG.ShowInterstitial();
        }

        public void ShowRewarded()
        {
            YG2.RewardedAdvShow("UniversalRewarded");
        }

        public void ShowBanner()
        {
        }

        public void HideBanner()
        {
        }

        private void OnRewardedWatched()
        {
            Log.Info($"Rewarded watched");
            ON_REWARDED_WATCHED.SafeInvoke();
            YG2.SaveProgress();
        }

        private void OnInterstitialWatched()
        {
            Log.Info($"Interstitial watched");
            ON_INTERSTITIAL_WATCHED.SafeInvoke();
        }

        private void OnInterstitialShow()
        {
            Log.Info($"Interstitial show");
            ON_INTERSTITIAL_SHOW.SafeInvoke();
        }

        private void OnSDKDataReceived()
        {
            if (YG2.saves.noAdsPurchased)
                SetNoAds();
        }

        public void SetNoAds()
        {
            _isNoAds = true;
            HideBanner();
        }
    }
}

