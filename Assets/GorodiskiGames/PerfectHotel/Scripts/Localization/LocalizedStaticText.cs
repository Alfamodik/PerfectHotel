using TMPro;
using UnityEngine;

namespace Game.Localization
{
    public static class LocalizedStaticText
    {
        public static void Apply(Component root)
        {
            if (root == null)
                return;

            Apply(root.transform);
        }

        public static void Apply(Transform root)
        {
            if (root == null)
                return;

            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
                text.text = LocalizedText.Static(text.text);
        }
    }
}
