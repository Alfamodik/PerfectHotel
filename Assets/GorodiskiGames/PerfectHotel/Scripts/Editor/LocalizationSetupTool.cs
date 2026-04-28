using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using YG.LanguageLegacy;

namespace Game.Editor
{
    public static class LocalizationSetupTool
    {
        private const string AutoRunSessionKey = "PerfectHotel.LocalizationSetupTool.AutoRun.v3";

        private static readonly Dictionary<string, string> RuByEn = new Dictionary<string, string>
        {
            { "BUY", "КУПИТЬ" },
            { "GET", "ВЫБРАТЬ" },
            { "SELECT", "ВЫБРАТЬ" },
            { "SELECTED", "ВЫБРАНО" },
            { "RESTORE PURCHASES", "ВОССТАНОВИТЬ ПОКУПКИ" },
            { "SHOP", "МАГАЗИН" },
            { "NO ADS", "БЕЗ РЕКЛАМЫ" },
            { "CASH", "ДЕНЬГИ" },
            { "FREE", "БЕСПЛАТНО" },
            { "SOME TEXT", "АКЦИЯ" },
            { "SHOW JOYSTICK", "ПОКАЗЫВАТЬ ДЖОЙСТИК" },
            { "Upgrade Room", "УЛУЧШЕНИЕ НОМЕРА" },
            { "Level Up", "НОВЫЙ УРОВЕНЬ" },
            { "Reward", "НАГРАДА" },
            { "REMOVE FORCED ADS INTERRUPTIONS AND BANNER ADS", "ОТКЛЮЧАЕТ РЕКЛАМНЫЕ ПАУЗЫ И БАННЕРЫ" },
            { "DISABLES FORCED ADS AND BANNERS", "ОТКЛЮЧАЕТ РЕКЛАМНЫЕ ПАУЗЫ И БАННЕРЫ" }
        };

        [InitializeOnLoadMethod]
        private static void AutoRunSetupLocalization()
        {
            if (SessionState.GetBool(AutoRunSessionKey, false))
                return;

            SessionState.SetBool(AutoRunSessionKey, true);
            EditorApplication.delayCall += AutoRunSetupPrefabLocalization;
        }

        private static void AutoRunSetupPrefabLocalization()
        {
            SetupPrefabLocalization();
        }

        [MenuItem("Tools/Perfect Hotel/Setup Localization")]
        public static void SetupLocalization()
        {
            var changedAssets = 0;

            changedAssets += SetupPrefabLocalization();
            changedAssets += SetupSceneLocalization();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Localization setup completed. Changed assets: {changedAssets}");
        }

        private static int SetupPrefabLocalization()
        {
            var changedAssets = 0;

            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[]
                     {
                         "Assets/GorodiskiGames/PerfectHotel/ResourcesStatic/Prefabs/UI",
                         "Assets/GorodiskiGames/PerfectHotel/Resources/Prefabs"
                     }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (SetupPrefab(path))
                    changedAssets++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Prefab localization setup completed. Changed assets: {changedAssets}");
            return changedAssets;
        }

        private static int SetupSceneLocalization()
        {
            var changedAssets = 0;

            foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets/GorodiskiGames/PerfectHotel/Scenes" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (SetupScene(path))
                    changedAssets++;
            }

            return changedAssets;
        }

        private static bool SetupPrefab(string path)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            var changed = SetupTexts(root);

            if (changed)
                PrefabUtility.SaveAsPrefabAsset(root, path);

            PrefabUtility.UnloadPrefabContents(root);
            return changed;
        }

        private static bool SetupScene(string path)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var changed = false;

            foreach (var root in scene.GetRootGameObjects())
                changed |= SetupTexts(root);

            if (changed)
                EditorSceneManager.SaveScene(scene);

            return changed;
        }

        private static bool SetupTexts(GameObject root)
        {
            var changed = false;
            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                var en = Normalize(text.text);
                if (!RuByEn.TryGetValue(en, out var ru))
                    continue;

                var language = text.GetComponent<LanguageYG>();
                var textChanged = false;
                if (language == null)
                {
                    language = text.gameObject.AddComponent<LanguageYG>();
                    textChanged = true;
                }

                var enText = EnglishBySource(en);
                textChanged |= SetIfDifferent(ref language.textMPComponent, text);
                textChanged |= SetIfDifferent(ref language.text, enText);
                textChanged |= SetIfDifferent(ref language.en, enText);
                textChanged |= SetIfDifferent(ref language.ru, ru);

                if (textChanged)
                {
                    changed = true;
                    EditorUtility.SetDirty(language);
                    EditorUtility.SetDirty(text.gameObject);
                }
            }

            return changed;
        }

        private static bool SetIfDifferent<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            return true;
        }

        private static string EnglishBySource(string value)
        {
            return value == "REMOVE FORCED ADS INTERRUPTIONS AND BANNER ADS"
                ? "DISABLES FORCED ADS AND BANNERS"
                : value;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
