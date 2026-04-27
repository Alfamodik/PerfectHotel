using System;
using Game.Config;
using UnityEngine;
using YG;
using YG.Utils.Pay;

namespace Game.Managers
{
    public sealed class IAPManager : IDisposable
    {
        private const string _fallbackPrice = "BUY";
        private const string _noAdsProductID = "no_ads";

        public event Action ON_INITIALIZED;
        public event Action ON_PURCHASE_CLICKED;
        public event Action<string> ON_PURCHASE_FAILED;
        public event Action ON_PURCHASE_PROCESS_COMPLETE;
        public event Action ON_RESTORE_PURCHASES;
        public event Action<string> ON_RESTORE_PURCHASES_END;
        public event Action<string> ON_PRODUCT_PURCHASED;

        public void Initialize(GameConfig config)
        {
            YG2.onGetPayments += OnPaymentsInitialized;
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
            YG2.onPurchaseFailed += OnPurchaseFailed;
            YG2.onGetSDKData += OnSDKDataReceived;

            if (YG2.purchases != null && YG2.purchases.Length > 0)
                OnPaymentsInitialized();
        }

        public void Dispose()
        {
            YG2.onGetPayments -= OnPaymentsInitialized;
            YG2.onPurchaseSuccess -= OnPurchaseSuccess;
            YG2.onPurchaseFailed -= OnPurchaseFailed;
            YG2.onGetSDKData -= OnSDKDataReceived;
        }

        public string GetPrice(string productID)
        {
            var product = YG2.PurchaseByID(productID);
            if (product == null)
                return _fallbackPrice;

            if (!string.IsNullOrEmpty(product.price))
                return product.price;

            if (!string.IsNullOrEmpty(product.priceValue))
                return product.priceValue;

            return _fallbackPrice;
        }

        public string GetTitle(string productID)
        {
            var product = YG2.PurchaseByID(productID);
            return product != null ? product.title : string.Empty;
        }

        public void OnPurchaseClicked(string productID)
        {
            if (string.IsNullOrEmpty(productID))
            {
                OnPurchaseFailed(productID);
                return;
            }

            ON_PURCHASE_CLICKED?.Invoke();
            Debug.Log($"YG2 purchase started. Product ID: {productID}");
            YG2.BuyPayments(productID);
        }

        public void RestorePurchases()
        {
            ON_RESTORE_PURCHASES?.Invoke();
            YG2.ConsumePurchases();
            ON_RESTORE_PURCHASES_END?.Invoke("RESTORE PURCHASES REQUESTED");
        }

        public bool IsProductPurchased(string productID)
        {
            return IsNoAdsProduct(productID) && YG2.saves.noAdsPurchased;
        }

        public Purchase GetMetaDataById(string id)
        {
            return YG2.PurchaseByID(id);
        }

        private void OnPaymentsInitialized()
        {
            ON_INITIALIZED?.Invoke();
        }

        private void OnSDKDataReceived()
        {
            ON_INITIALIZED?.Invoke();
        }

        private void OnPurchaseSuccess(string productID)
        {
            SaveProductPurchased(productID);
            Debug.Log($"YG2 purchase success. Product ID: {productID}");
            ON_PRODUCT_PURCHASED?.Invoke(productID);
            ON_PURCHASE_PROCESS_COMPLETE?.Invoke();
        }

        private void OnPurchaseFailed(string productID)
        {
            var info = $"Purchase failed. Product ID: {productID}";
            Debug.Log(info);
            ON_PURCHASE_FAILED?.Invoke(info);
            ON_PURCHASE_PROCESS_COMPLETE?.Invoke();
        }

        private void SaveProductPurchased(string productID)
        {
            if (!IsNoAdsProduct(productID))
                return;

            YG2.saves.noAdsPurchased = true;
            YG2.SaveProgress();
        }

        private bool IsNoAdsProduct(string productID)
        {
            return string.Equals(productID, _noAdsProductID, StringComparison.Ordinal);
        }
    }
}
