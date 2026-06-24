using System;
using YG;

namespace Game.Localization
{
    public static class LocalizedText
    {
        public static string CurrentLanguage => YG2.lang;
        public static bool IsRussian => string.Equals(CurrentLanguage, "ru", StringComparison.OrdinalIgnoreCase);

        public static string Get(string en, string ru)
        {
            return IsRussian ? ru : en;
        }

        public static string Key(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (!IsRussian)
                return key;

            switch (key.ToUpperInvariant())
            {
                case "BUY": return "КУПИТЬ";
                case "GET": return "ВЫБРАТЬ";
                case "RESTORE PURCHASES": return "ВОССТАНОВИТЬ ПОКУПКИ";
                case "SELECT": return "ВЫБРАТЬ";
                case "SELECTED": return "ВЫБРАНО";
                case "FREE": return "БЕСПЛАТНО";
                case "CASH": return "ДЕНЬГИ";
                case "NO ADS": return "БЕЗ РЕКЛАМЫ";
                case "SHOP": return "МАГАЗИН";
                case "LEVEL": return "УРОВЕНЬ";
                case "LVL": return "УР.";
                case "NEW AREA": return "НОВАЯ ЗОНА";
                case "NEW HOTEL": return "НОВЫЙ ОТЕЛЬ";
                case "RECEPTION": return "РЕСЕПШЕН";
                case "RECEPTIONIST": return "АДМИНИСТРАТОР";
                case "ROOM": return "НОМЕР";
                case "WC": return "ТУАЛЕТ";
                case "CLEANER": return "УБОРЩИК";
                case "UTILITY": return "СКЛАД";
                case "AREA": return "ЗОНА";
                case "ELEVATOR": return "ЛИФТ";
                case "LOADER": return "НОСИЛЬЩИК";
                case "SODA": return "ГАЗИРОВКА";
                case "SPEED": return "СКОРОСТЬ";
                case "MANEUVER": return "МАНЕВР";
                case "CAPACITY": return "ВМЕСТИМОСТЬ";
                case "TED": return "ТЕД";
                case "UMA": return "УМА";
                case "BEER": return "БИР";
                case "FOX": return "ФОКС";
                case "GUSTAV": return "ГУСТАВ";
                case "PATRICK": return "ПАТРИК";
                case "CONCIERGE": return "КОНСЬЕРЖ";
                default: return key;
            }
        }

        public static string Static(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            switch (value.Trim())
            {
                case "SHOW JOYSTICK":
                case "ПОКАЗЫВАТЬ ДЖОЙСТИК":
                    return Get("SHOW JOYSTICK", "ПОКАЗЫВАТЬ ДЖОЙСТИК");
                case "RESET PROGRESS":
                case "СБРОС ПРОГРЕССА":
                    return Get("RESET PROGRESS", "СБРОС ПРОГРЕССА");
                case "RESET PROGRESS DESCRIPTION":
                case "ОПИСАНИЕ СБРОСА ПРОГРЕССА":
                    return Get("Are you sure you want to reset all progress? All game data will be deleted.", "Вы уверены, что хотите сбросить весь прогресс? Все игровые данные будут удалены.");
                case "RESET":
                case "СБРОСИТЬ":
                    return Get("RESET", "СБРОСИТЬ");
                case "CANCEL":
                case "ОТМЕНА":
                    return Get("CANCEL", "ОТМЕНА");
                case "SOME TEXT":
                case "SPECIAL":
                case "АКЦИЯ":
                    return Get("SPECIAL", "АКЦИЯ");
                case "Upgrade Room":
                case "УЛУЧШЕНИЕ НОМЕРА":
                    return Get("Upgrade Room", "УЛУЧШЕНИЕ НОМЕРА");
                case "Level Up":
                case "НОВЫЙ УРОВЕНЬ":
                    return Get("Level Up", "НОВЫЙ УРОВЕНЬ");
                case "Reward":
                case "НАГРАДА":
                    return Get("Reward", "НАГРАДА");
                case "SHOP":
                case "МАГАЗИН":
                    return Key("SHOP");
                case "NO ADS":
                case "БЕЗ РЕКЛАМЫ":
                    return Key("NO ADS");
                case "CASH":
                case "ДЕНЬГИ":
                    return Key("CASH");
                case "FREE":
                case "БЕСПЛАТНО":
                    return Key("FREE");
                case "GET":
                case "ВЫБРАТЬ":
                    return Key("GET");
                case "SELECT":
                    return Key("SELECT");
                case "SELECTED":
                case "ВЫБРАНО":
                    return Key("SELECTED");
                case "REMOVE FORCED ADS INTERRUPTIONS AND BANNER ADS":
                case "DISABLES FORCED ADS AND BANNERS":
                case "УБРАТЬ ПРЕРЫВАНИЯ РЕКЛАМОЙ И БАННЕРЫ":
                case "ОТКЛЮЧАЕТ РЕКЛАМНЫЕ ПАУЗЫ И БАННЕРЫ":
                    return Get("DISABLES FORCED ADS AND BANNERS", "ОТКЛЮЧАЕТ РЕКЛАМНЫЕ ПАУЗЫ И БАННЕРЫ");
                default:
                    return value;
            }
        }

        public static string Level(int level)
        {
            return Get("Level ", "Уровень ") + level;
        }

        public static string DeveloperProgress(int progress)
        {
            return Get("PROGRESS ", "ПРОГРЕСС ") + progress;
        }

        public static string DeveloperLevel(int level)
        {
            return Get("LEVEL ", "УРОВЕНЬ ") + level;
        }

        public static string ReachLevelToUnlock(int level)
        {
            return Get($"Reach Level {level} To Unlock", $"Достигните уровня {level}, чтобы открыть");
        }

        public static string OpenHotel(string hotelLabel)
        {
            return Get($"OPEN {hotelLabel} HOTEL", $"ОТКРОЙТЕ ОТЕЛЬ {hotelLabel}");
        }

        public static string WatchAdsTimes(string targetTimesLabel)
        {
            return Get($"WATCH ADS {targetTimesLabel} TIMES", $"ПОСМОТРИТЕ РЕКЛАМУ {targetTimesLabel} РАЗ");
        }

        public static string WatchAdsTimesLeft(string targetTimesLabel, string timesLeftLabel)
        {
            return Get(
                $"WATCH ADS {targetTimesLabel} TIMES\n{timesLeftLabel} LEFT",
                $"ПОСМОТРИТЕ РЕКЛАМУ {targetTimesLabel} РАЗ\nОСТАЛОСЬ {timesLeftLabel}");
        }

        public static string LoginDaysLeft(string targetDaysLabel, string straightWord, string daysLeftLabel)
        {
            return Get(
                $"LOGIN TO THE GAME {targetDaysLabel} DAYS {straightWord}\n{daysLeftLabel} DAYS LEFT",
                $"ЗАЙДИТЕ В ИГРУ {targetDaysLabel} ДНЕЙ {straightWord}\nОСТАЛОСЬ ДНЕЙ: {daysLeftLabel}");
        }

        public static string Straight()
        {
            return Get("STRAIGHT", "ПОДРЯД");
        }

        public static string PurchaseProcessing()
        {
            return Get("PURCHASE PROCESSING...", "ОБРАБОТКА ПОКУПКИ...");
        }

        public static string RestoringPurchases()
        {
            return Get("RESTORING PURCHASES...", "ВОССТАНОВЛЕНИЕ ПОКУПОК...");
        }

        public static string RestorePurchasesRequested()
        {
            return Get("RESTORE PURCHASES REQUESTED", "ЗАПРОС НА ВОССТАНОВЛЕНИЕ ПОКУПОК ОТПРАВЛЕН");
        }

        public static string PurchaseFailed(string productID)
        {
            return Get($"Purchase failed. Product ID: {productID}", $"Покупка не удалась. ID товара: {productID}");
        }

        public static string TimeUnitHours()
        {
            return Get("hr", "ч");
        }

        public static string TimeUnitMinutes()
        {
            return Get("min", "мин");
        }

        public static string TimeUnitSeconds()
        {
            return Get("sec", "с");
        }
    }
}
