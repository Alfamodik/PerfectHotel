using System;
using System.Collections.Generic;
using Core;
using Game.Config;
using Game.Level.Inventory;
using Game.Level.Place;
using UnityEngine;
using YG;

namespace Game.Domain
{
    [Serializable]
    public sealed class GameModel : Observable
    {
        public static GameModel Load(GameConfig config)
        {
            try
            {
                var data = YG2.saves.gameModelJson;
                if (string.IsNullOrEmpty(data))
                    return CreateDefault(config);

                var result = JsonUtility.FromJson<GameModel>(data);
                if (result == null)
                    return CreateDefault(config);

                result.EnsureRuntimeState();
                result.ApplyCloudPurchases();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                YG2.saves.gameModelJson = string.Empty;
                YG2.SaveProgress();
                return CreateDefault(config);
            }
        }

        public int Player;
        public long Cash;
        public long TotalEarnedCash;
        public int Hotel;
        public bool IsNoAds;
        public List<InventoryType> InventoryTypes;
        public bool JoystickVisibility;

        public GameModel()
        {
            InventoryTypes = new List<InventoryType>();
        }

        private void Prepare(GameConfig config)
        {
            Cash = config.DefaultCash;
            TotalEarnedCash = 0;
            Hotel = config.DefaultHotel;
            IsNoAds = YG2.saves.noAdsPurchased;
            JoystickVisibility = false;
        }

        private static GameModel CreateDefault(GameConfig config)
        {
            var model = new GameModel();
            model.Prepare(config);
            return model;
        }

        private void EnsureRuntimeState()
        {
            if (InventoryTypes == null)
                InventoryTypes = new List<InventoryType>();
        }

        private void ApplyCloudPurchases()
        {
            if (YG2.saves.noAdsPurchased)
                IsNoAds = true;
        }

        public void AddCash(long amount)
        {
            if (amount <= 0)
                return;

            Cash += amount;
            TotalEarnedCash += amount;

            Save();
            SetChanged();
        }

        public void Save()
        {
            if (IsNoAds)
                YG2.saves.noAdsPurchased = true;

            var data = JsonUtility.ToJson(this);
            YG2.saves.gameModelJson = data;
            YG2.SaveProgress();
        }

        public void Remove()
        {
            YG2.saves.gameModelJson = string.Empty;
            YG2.SaveProgress();
        }

        public string GenerateEntityID(int hotel, EntityType type, int number)
        {
            return "Hotel" + hotel + type.ToString() + number;
        }

        public int LoadLvl()
        {
            string hotelLvlWord = "HotelLvl";
            string key = hotelLvlWord + Hotel;
            return YG2.saves.GetInt(key, 1);
        }
        public void SaveLvl(int lvl)
        {
            string hotelLvlWord = "HotelLvl";
            string key = hotelLvlWord + Hotel;
            YG2.saves.SetInt(key, lvl);
            YG2.SaveProgress();
        }

        public int LoadProgress()
        {
            string hotelProgressWord = "HotelProgress";
            string key = hotelProgressWord + Hotel;
            return YG2.saves.GetInt(key, 0);
        }
        public void SaveProgress(int progress)
        {
            string hotelProgressWord = "HotelProgress";
            string key = hotelProgressWord + Hotel;
            YG2.saves.SetInt(key, progress);
            YG2.SaveProgress();
        }

        public bool LoadPlaceIsUsed(string id)
        {
            string isUsedWord = "IsUsed";
            string key = isUsedWord + id;
            int value = YG2.saves.GetInt(key, 0);
            if (value == 1) return true;
            else return false;
        }
        public void SavePlaceIsUsed(string id, int isUsed)
        {
            string isUsedWord = "IsUsed";
            string key = isUsedWord + id;
            YG2.saves.SetInt(key, isUsed);
            YG2.SaveProgress();
        }

        public bool LoadPlaceIsPurchased(string id)
        {
            string areaOneID = GenerateEntityID(Hotel, EntityType.Area, 1);
            string roomZeroID = GenerateEntityID(Hotel, EntityType.Room, 0);

            string isPurchasedWord = "IsPurchased";
            string key = isPurchasedWord + id;
            int value = YG2.saves.GetInt(key, 0);
            if (roomZeroID == id || areaOneID == id || value == 1) return true;
            else return false;
        }
        public void SavePlaceIsPurchased(string id)
        {
            string isPurchasedWord = "IsPurchased";
            string key = isPurchasedWord + id;
            YG2.saves.SetInt(key, 1);
            YG2.SaveProgress();
        }

        public void SavePlaceLvl(string id, int lvl)
        {
            string lvlWord = "Lvl";
            string key = lvlWord + id;
            YG2.saves.SetInt(key, lvl);
            YG2.SaveProgress();
        }
        public int LoadPlaceLvl(string id)
        {
            string lvlWord = "Lvl";
            string key = lvlWord + id;
            return YG2.saves.GetInt(key, 0);
        }


        public void SavePlaceVisualIndex(string id, int visualIndex)
        {
            string visualIndexWord = "VisualIndex";
            string key = visualIndexWord + id;
            YG2.saves.SetInt(key, visualIndex);
            YG2.SaveProgress();
        }
        public int LoadPlaceVisualIndex(string id)
        {
            string visualIndexWord = "VisualIndex";
            string key = visualIndexWord + id;
            return YG2.saves.GetInt(key, 0);
        }

        public void SavePlaceCash(string id, long cash)
        {
            string cashWord = "Cash";
            string key = cashWord + id;
            YG2.saves.SetLong(key, cash);
            YG2.SaveProgress();
        }
        public long LoadPlaceCash(string id)
        {
            string cashWord = "Cash";
            string key = cashWord + id;
            return YG2.saves.GetLong(key, 0);
        }

        public void SaveVisitsCount(string id, int numberOfVisits)
        {
            string visitsCountWord = "VisitsCount";
            string key = visitsCountWord + id;
            YG2.saves.SetInt(key, numberOfVisits);
            YG2.SaveProgress();
        }
        public int LoadVisitsCount(string id)
        {
            string visitsCountWord = "VisitsCount";
            string key = visitsCountWord + id;
            return YG2.saves.GetInt(key, 0);
        }


        public void SaveWatchAdsTimes()
        {
            var times = LoadWatchAdsTimes();
            times++;
            YG2.saves.SetInt(GameConstants.kWatchAdsTimes, times);
            YG2.SaveProgress();
        }

        public int LoadWatchAdsTimes()
        {
            return YG2.saves.GetInt(GameConstants.kWatchAdsTimes, 0);
        }

        public void SaveWatchAdsTimes(int playerIndex)
        {
            var times = LoadWatchAdsTimes(playerIndex);
            times++;
            YG2.saves.SetInt(GameConstants.kWatchAdsTimes + playerIndex, times);
            YG2.SaveProgress();
        }

        public int LoadWatchAdsTimes(int playerIndex)
        {
            return YG2.saves.GetInt(GameConstants.kWatchAdsTimes + playerIndex, 0);
        }

        public int LoadLoginDays()
        {
            return YG2.saves.GetInt(GameConstants.kLoginDays, 1);
        }

        public void SaveLoginDay()
        {
            var days = LoadLoginDays();
            days++;
            YG2.saves.SetInt(GameConstants.kLoginDays, days);
            YG2.SaveProgress();
        }

        public void ResetLoginDays()
        {
            YG2.saves.SetInt(GameConstants.kLoginDays, 1);
            YG2.SaveProgress();
        }
    }
}
