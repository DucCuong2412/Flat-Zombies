using Assets.GmSoft.Scripts;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Path = System.IO.Path;

public class AdvertisementManager : MonoBehaviour
{
    public static AdvertisementManager Instance;
    [HideInInspector]
    public SDKConfig sdkConfig;
    [HideInInspector]
    public bool enable = true;
    [HideInInspector]
    public bool prerollAds = false;
    public bool checkInternetConnection = true;
    private bool loadedRewardVideo = false;
    private const string NETWORK_ERROR_MSG = "Network error.\nPlease check your internet connection and try again.";
    private const string LOADING_ADS_MSG = "Loading";
    private const string REWARD_ADS_NOT_AVAILABLE_MSG = "Reward video not available.";
    [HideInInspector] public float showMsgTime = 1f;
    [HideInInspector] public float minTimeShowInterAds = 15;
    private float interAdsTimer;
    [HideInInspector] public float minTimeShowRewardAds = 30;
    [HideInInspector] public float rewardAdsTimer;
    public GameObject messagePanel;
    public GameObject messageWaitAdsPanel;
    public Action ResumeAction = null;
    public Action RewardSuccessAction = null;
    public string targetSceneName;
    [HideInInspector]
    public Advertisement advertisementSDK;
    private const float PRELOAD_REWARD_AD_INTERVAL = 5f;
    private bool loadedTargetScene;

    private bool tryShowingAds = false;
    private float tryShowingAdsTimer = 0f;
    private const float TIME_TRY_SHOWING_ADS = 5f;
    public bool isTestAds = false, beginWaitAds = false, isWaitAds = false;
    float countWaitAds;
    TMP_Text countWaitAdsTxt;

    
    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
        DontDestroyOnLoad(gameObject);
        countWaitAdsTxt = messageWaitAdsPanel.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        LoadTargetScene();
#endif
        InvokeRepeating(nameof(AutoPreloadRewardAd), 0, PRELOAD_REWARD_AD_INTERVAL);
    }

    public void SetConfig(SDKConfig sdkConfig)
    {
        this.sdkConfig = sdkConfig;
    }

    private void AutoPreloadRewardAd()
    {
        if (!enable) return;
        if (loadedRewardVideo) return;
        PreloadRewardAd();
    }

    public void InitSdk()
    {
        if (advertisementSDK == null)
        {
            Debug.LogError("Not found advertisement sdk!");
            return;
        }
        advertisementSDK.OnResumeGame += OnResumeGame;
        advertisementSDK.OnPauseGame += OnPauseGame;
        advertisementSDK.OnPreloadRewardedVideo += OnPreloadRewardedVideo;
        advertisementSDK.OnRewardedVideoSuccess += OnRewardedVideoSuccess;
        advertisementSDK.OnRewardedVideoFailure += OnRewardedVideoFailure;
        advertisementSDK.OnRewardGame += OnRewardGame;

        //interAdsTimer = sdkConfig.timeShowInter;
        interAdsTimer = 0;
        rewardAdsTimer = sdkConfig.timeShowReward;

        minTimeShowInterAds = sdkConfig.timeShowInter;
        minTimeShowRewardAds = sdkConfig.timeShowReward;

        PreloadRewardAd();
        if (prerollAds && enable && sdkConfig != null && sdkConfig.enableAds)
        {
            ShowPrerollAd();
        }
        else
        {
            LoadTargetScene();
        }
    }

    public void ShowPrerollAd()
    {
        ShowAds(() =>
        {
            LoadTargetScene();
        });
    }

    public void LoadTargetScene()
    {
        if (loadedTargetScene) return;
        if (!GmSoft.Instance.IsAllowPlay()) return;
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        loadedTargetScene = true;
    }

    public void ShowAds(Action ResumeAction)
    {
#if UNITY_EDITOR
        Debug.Log($"Skip inter in editor.");
        ResumeAction?.Invoke();
        return;
#endif
        if (!enable || !CanShowInterAds() || Application.platform != RuntimePlatform.WebGLPlayer)
        {
            ResumeAction?.Invoke();
            return;
        }
        ShowMessagePanel(LOADING_ADS_MSG);
        if (checkInternetConnection)
        {
            CheckForInternetConnection((bool connected) =>
            {
                if (!connected)
                {
                    ShowMessagePanel(NETWORK_ERROR_MSG, showMsgTime);
                    return;
                }
                else
                {
                    if (isWaitAds)
                    {
                        StartCoroutine(WaitShowAds(3, ResumeAction));
                    }
                    else
                    {
                        ShowAd(ResumeAction);
                    }
                }
            });
        }
        else
        {
            if (isWaitAds)
            {
                StartCoroutine(WaitShowAds(3, ResumeAction));
            }
            else
            {
                ShowAd(ResumeAction);
            }
        }
    }

    public void ShowReward(Action CompleteAction)
    {
#if UNITY_EDITOR
        Debug.Log($"Skip reward in editor.");
        CompleteAction?.Invoke();
        return;
#endif
        if (!enable || Application.platform != RuntimePlatform.WebGLPlayer)
        {
            CompleteAction?.Invoke();
            return;
        }
        ShowMessagePanel(LOADING_ADS_MSG);
        RewardSuccessAction = null;
        if (checkInternetConnection)
        {
            CheckForInternetConnection((bool connected) =>
            {
                if (!connected)
                {
                    ShowMessagePanel(NETWORK_ERROR_MSG, showMsgTime);
                    return;
                }
            });
        }
        if (!loadedRewardVideo)
        {
            ShowMessagePanel(REWARD_ADS_NOT_AVAILABLE_MSG, showMsgTime);
            return;
        }
        if (isWaitAds)
        {
            StartCoroutine(WaitShowRewardAds(3, CompleteAction));
        }
        else
        {
            ShowRewardAds(CompleteAction);
        }
    }

    private void Update()
    {
        interAdsTimer += Time.deltaTime;
        rewardAdsTimer += Time.deltaTime;
        if (beginWaitAds && countWaitAds > 0)
        {
            countWaitAds -= Time.deltaTime;
            countWaitAdsTxt.text = $"Ad showing in {Mathf.CeilToInt(countWaitAds)}...";
        }
        if (tryShowingAds)
        {
            tryShowingAdsTimer += Time.deltaTime;
            if (tryShowingAdsTimer >= TIME_TRY_SHOWING_ADS)
            {
                gmsoft.Analytics.LogEvent($"show_ads_failed_timmer");
                tryShowingAdsTimer = 0;
                tryShowingAds = false;
                Debug.Log($"resume on try show ads failed................................");
                OnResumeGame();
            }
        }
    }

    public bool CanShowInterAds()
    {
        return enable && interAdsTimer >= minTimeShowInterAds;
    }

    public bool CanShowRewardAds()
    {
        return loadedRewardVideo && enable && GetNextRewardAdsTime() <= 0;
    }

    public float GetNextRewardAdsTime()
    {
        return Mathf.Max(0, minTimeShowRewardAds - rewardAdsTimer);
    }

    private void ResetRewardAdsTimer()
    {
        rewardAdsTimer = 0;
    }

    private void OnRewardGame()
    {
        Debug.Log($"On reward game.");
        gmsoft.Analytics.LogEvent($"{sdkConfig.sdkType}_completeReward_{sdkConfig.gameId}");
        RewardSuccessAction?.Invoke();
    }

    private void OnRewardedVideoFailure()
    {
        Debug.Log($"On reward video failure.");
        Invoke("PreloadRewardAd", 0.2f);
        ResetRewardAdsTimer();
    }

    private void OnRewardedVideoSuccess()
    {
        Debug.Log($"On reward video success.");
        if (advertisementSDK.GetType() != typeof(GameDistribution))
        {
            gmsoft.Analytics.LogEvent($"{sdkConfig.sdkType}_completeReward_{sdkConfig.gameId}");
            RewardSuccessAction?.Invoke();
        }
        Invoke("PreloadRewardAd", 0.2f);
        ResetRewardAdsTimer();
    }

    public void PreloadRewardAd()
    {
#if UNITY_EDITOR
        return;
#endif
        Debug.Log($"On preload reward ad.");
        advertisementSDK?.PreloadRewardedAd();
    }

    private void OnPreloadRewardedVideo(int loaded)
    {
        loadedRewardVideo = loaded == 1;
        string loadedRewardVideoStr = loadedRewardVideo ? "success" : "failure";
        Debug.Log($"On preload reward video: {loadedRewardVideoStr}.");
    }

    private void OnPauseGame()
    {
        Debug.Log($"On pause game.");
        AudioListener.volume = 0;
        tryShowingAds = false;
    }

    private void OnResumeGame()
    {
        Debug.Log($"On resume game.");
        Application.ExternalEval("window.focus();");
        AudioListener.volume = 1f;
        ResumeAction?.Invoke();
        ResumeAction = null;
        HideMassagePanel();
    }

    public void CheckForInternetConnection(Action<bool> OnCompleted)
    {
        StartCoroutine(CheckForInternetConnectionCoroutine(OnCompleted));
    }

    private IEnumerator CheckForInternetConnectionCoroutine(Action<bool> OnCompleted)
    {
        string url = Path.Combine(Application.streamingAssetsPath, $"test_connection.txt");
        if (!url.StartsWith("http"))
        {
            url = "file://" + url;
        }
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || string.IsNullOrEmpty(www.downloadHandler.text)) OnCompleted?.Invoke(false);
        OnCompleted?.Invoke(true);
    }

    public void ShowMessagePanel(string message, float time)
    {
        if(isWaitAds) return;
        messagePanel.SetActive(true);
        TMP_Text tmp = messagePanel.GetComponentInChildren<TMP_Text>();
        tmp.text = message;
        StartCoroutine(HideMessagePanel(time));
    }

    public void ShowMessagePanel(string message)
    {
        if(isWaitAds) return;
        messagePanel.SetActive(true);
        TMP_Text tmp = messagePanel.GetComponentInChildren<TMP_Text>();
        tmp.text = message;
    }

    public void HideMassagePanel()
    {
        messagePanel.SetActive(false);
    }

    private IEnumerator HideMessagePanel(float time)
    {
        yield return new WaitForSeconds(time);
        messagePanel.SetActive(false);
    }

    private IEnumerator WaitShowAds(float time, Action ResumeAction)
    {
        countWaitAds = time;
        beginWaitAds = true;
        messageWaitAdsPanel.SetActive(true);
        yield return new WaitForSeconds(time);
        ShowAd(ResumeAction);


    }
    private void ShowAd(Action ResumeAction)
    {
        this.ResumeAction = ResumeAction;
        interAdsTimer = 0;
        tryShowingAds = true;
        Debug.Log($"try showing ads............");
        advertisementSDK.ShowAd();
        gmsoft.Analytics.LogEvent($"{sdkConfig.sdkType}_showInter_{sdkConfig.gameId}");
        messageWaitAdsPanel.SetActive(false);
        beginWaitAds = false;
    }

    private IEnumerator WaitShowRewardAds(float time, Action CompleteAction)
    {
        countWaitAds = time;
        beginWaitAds = true;
        messageWaitAdsPanel.SetActive(true);
        yield return new WaitForSeconds(time);
        ShowRewardAds(CompleteAction);


    }
    private void ShowRewardAds(Action CompleteAction)
    {
        RewardSuccessAction = CompleteAction;
        tryShowingAds = true;
        advertisementSDK.ShowRewardedAd();
        messageWaitAdsPanel.SetActive(false);
        gmsoft.Analytics.LogEvent($"{sdkConfig.sdkType}_showReward_{sdkConfig.gameId}");
        Debug.Log($"Show reward.");
        loadedRewardVideo = false;
    }
}
