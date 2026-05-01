using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    public class ManualTimerBeforeAdsYG : MonoBehaviour
    {
        [SerializeField] private GameObject secondsPanelObject;
        [SerializeField] private GameObject[] secondObjects;

        [Space(20)]
        [SerializeField] private UnityEvent onShowTimer;
        [SerializeField] private UnityEvent onHideTimer;

        private static ManualTimerBeforeAdsYG _instance;

        private int _objSecCounter;
        private Coroutine _timerAdShowCoroutine;
        private Coroutine _backupTimerClosureCoroutine;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            HideTimer(false);
        }

        private void OnDestroy()
        {
            if (_instance != this)
                return;

            HideTimer(true);
            _instance = null;
        }

        public static void ShowInterstitial()
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<ManualTimerBeforeAdsYG>(FindObjectsInactive.Include);

            if (_instance == null)
            {
                Debug.LogWarning($"{nameof(ManualTimerBeforeAdsYG)} is not found in scene. Showing interstitial without timer.");
                YG2.InterstitialAdvShow();
                return;
            }

            _instance.ShowInterstitialWithTimer();
        }

        private void ShowInterstitialWithTimer()
        {
            if (_timerAdShowCoroutine != null || YG2.nowAdsShow)
                return;

            if (!YG2.isTimerAdvCompleted)
            {
                YG2.InterstitialAdvShow();
                return;
            }

            if (secondObjects == null || secondObjects.Length == 0)
            {
                Debug.LogError("Fill in the array 'secondObjects'");
                YG2.InterstitialAdvShow();
                return;
            }

            onShowTimer?.Invoke();
            _objSecCounter = 0;

            if (secondsPanelObject != null)
                secondsPanelObject.SetActive(true);

            YG2.PauseGame(true);
            _timerAdShowCoroutine = StartCoroutine(TimerAdShow());
        }

        private IEnumerator TimerAdShow()
        {
            while (_objSecCounter < secondObjects.Length)
            {
                for (int i = 0; i < secondObjects.Length; i++)
                {
                    if (secondObjects[i] != null)
                        secondObjects[i].SetActive(false);
                }

                if (secondObjects[_objSecCounter] != null)
                    secondObjects[_objSecCounter].SetActive(true);

                _objSecCounter++;
                yield return new WaitForSecondsRealtime(1.0f);
            }

            YG2.InterstitialAdvShow();
            _backupTimerClosureCoroutine = StartCoroutine(BackupTimerClosure());

            while (!YG2.nowInterAdv)
                yield return null;

            HideTimer(false);
        }

        private IEnumerator BackupTimerClosure()
        {
            yield return new WaitForSecondsRealtime(2f);

            if (_timerAdShowCoroutine != null && !YG2.nowInterAdv)
                HideTimer(true);
        }

        private void HideTimer(bool resumeGame)
        {
            if (secondsPanelObject != null)
                secondsPanelObject.SetActive(false);

            if (secondObjects != null)
            {
                foreach (GameObject obj in secondObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }

            onHideTimer?.Invoke();
            _objSecCounter = 0;

            if (_timerAdShowCoroutine != null)
            {
                StopCoroutine(_timerAdShowCoroutine);
                _timerAdShowCoroutine = null;
            }

            if (_backupTimerClosureCoroutine != null)
            {
                StopCoroutine(_backupTimerClosureCoroutine);
                _backupTimerClosureCoroutine = null;
            }

            if (resumeGame)
                YG2.PauseGame(false);
        }
    }
}
