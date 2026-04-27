
using System;
using System.Collections.Generic;

namespace YG
{
    [Serializable]
    public sealed class IntSaveEntryYG
    {
        public string key;
        public int value;
    }

    [Serializable]
    public sealed class LongSaveEntryYG
    {
        public string key;
        public long value;
    }

    [Serializable]
    public sealed class StringSaveEntryYG
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        public bool noAdsPurchased;
        public string gameModelJson = string.Empty;
        public List<IntSaveEntryYG> intValues = new List<IntSaveEntryYG>();
        public List<LongSaveEntryYG> longValues = new List<LongSaveEntryYG>();
        public List<StringSaveEntryYG> stringValues = new List<StringSaveEntryYG>();

        public int GetInt(string key, int defaultValue = 0)
        {
            EnsureCollections();

            for (int i = 0; i < intValues.Count; i++)
            {
                if (intValues[i].key == key)
                    return intValues[i].value;
            }

            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            EnsureCollections();

            for (int i = 0; i < intValues.Count; i++)
            {
                if (intValues[i].key == key)
                {
                    intValues[i].value = value;
                    return;
                }
            }

            intValues.Add(new IntSaveEntryYG { key = key, value = value });
        }

        public long GetLong(string key, long defaultValue = 0)
        {
            EnsureCollections();

            for (int i = 0; i < longValues.Count; i++)
            {
                if (longValues[i].key == key)
                    return longValues[i].value;
            }

            return defaultValue;
        }

        public void SetLong(string key, long value)
        {
            EnsureCollections();

            for (int i = 0; i < longValues.Count; i++)
            {
                if (longValues[i].key == key)
                {
                    longValues[i].value = value;
                    return;
                }
            }

            longValues.Add(new LongSaveEntryYG { key = key, value = value });
        }

        public string GetString(string key, string defaultValue = "")
        {
            EnsureCollections();

            for (int i = 0; i < stringValues.Count; i++)
            {
                if (stringValues[i].key == key)
                    return stringValues[i].value;
            }

            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            EnsureCollections();

            for (int i = 0; i < stringValues.Count; i++)
            {
                if (stringValues[i].key == key)
                {
                    stringValues[i].value = value;
                    return;
                }
            }

            stringValues.Add(new StringSaveEntryYG { key = key, value = value });
        }

        private void EnsureCollections()
        {
            if (intValues == null)
                intValues = new List<IntSaveEntryYG>();
            if (longValues == null)
                longValues = new List<LongSaveEntryYG>();
            if (stringValues == null)
                stringValues = new List<StringSaveEntryYG>();
        }
    }
}
