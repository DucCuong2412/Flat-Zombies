using Assets.GMDev.Utilities;
using Assets.GmSoft.Scripts;
using Best.HTTP;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[HideMonoScript]
public class GmSoft : Advertisement
{
    public enum LockReason
    {
        UnKnow = 0,
        Host_Diff = 1,
        Host_Disable = 2,
        Time_Span_Diff = 3,
        MISS_SDK_VERSION = 4,
        LOCK_INTERACT_GAME = 5
    }

    [Button(SdfIconType.QuestionSquareFill, IconAlignment.RightEdge, Stretch = false, ButtonAlignment = 1f), PropertyOrder(-1)]
    private void Help()
    {
#if UNITY_EDITOR
        GmSoftHelper.OpenWindow();
#endif
    }

    public static GmSoft Instance;

    private PublisherSite publisher = PublisherSite.AZGAMES;
    [PropertySpace(SpaceBefore = 10), HideInInspector]
    public bool ENABLE_ADS = false;
    [HideInInspector]
    public string PUB_ID = "PUB_ID";
    [HideInInspector]
    public bool DEBUG = false;
    [HideInInspector]
    public int UNLOCK_TIMER = 15;
    [HideInInspector]
    public int TIME_SHOW_INTER = 60;
    [HideInInspector]
    public string sdkType;

    [DllImport("__Internal")]
    private static extern void SDK_GMEvent(string eventName, string msg = null);

    [DllImport("__Internal")]
    private static extern void SDK_Init(bool enableAds, string gameKey, string pubId, bool debug, int unlockTimer, int timeShowInter);

    [DllImport("__Internal")]
    private static extern void SDK_PreloadAd();

    [DllImport("__Internal")]
    private static extern void SDK_InitParam();

    [DllImport("__Internal")]
    private static extern void SDK_ShowAd(string adType);

    [DllImport("__Internal")]
    private static extern void SDK_SendEvent(string options);

    private bool _isRewardedVideoLoaded = false;

    private SDKConfig sdkConfig;

    public Sprite azGamesLogoDefault;
    public Sprite oneGamesLogoDefault;

    [HideInInspector]
    public Sprite logo;
    [HideInInspector]
    public Sprite gameImage;

    private bool _allowPlay = true;

    private bool allowPlay
    {
        get
        {
            return _allowPlay;
        }
        set
        {
            if (_allowPlay == value) return;
            if (_allowPlay && !value)
            {
                OnNotAllowPlay();
            }
            _allowPlay = value;
        }
    }

    [HideInInspector]
    public bool enableLog;

    private string currentTimeSpanRequest;

    private string unityHostName;

    public FpsLogger fpsLogger;

    public Advertisement weegoo;
    public GameDistribution gameDistribution;

    [HideInInspector]
    public LockReason lockReason = LockReason.UnKnow;

    public UnityAction OnLoadedLogo;
    public UnityAction OnLoadedGameImage;

    [HideInInspector] public bool loadedLogo;
    [HideInInspector] public bool loadedGameImage;

    private bool paramReady;
    private int playCount;
    private int interactCount;
    private bool banned;

    private bool gameStarted = false;

    private void Awake()
    {
        if (GmSoft.Instance == null) GmSoft.Instance = this;
        else Destroy(this);
        DontDestroyOnLoad(this);
#if !UNITY_EDITOR
        SDK_GMEvent("awake-game");
#endif
        Init();
    }

    private void Start()
    {
        logo = azGamesLogoDefault;
#if !UNITY_EDITOR
        SDK_GMEvent("start-game");
#endif
        gameStarted = true;
        if (paramReady)
        {
            InitAdsSdk();
        }
        else
        {
            Init();
        }
    }

    /// <summary>
    /// gọi khi người chơi bấm nút chơi game
    /// playCount: so lan choi game
    /// </summary>
    public void OnPlayGame()
    {
        playCount++;
#if !UNITY_EDITOR
        SDK_GMEvent("play-game", playCount.ToString());
#endif
    }

    public void OnInteractGame()
    {
        interactCount++;
        //neu ham initparam bị xoa ở code js. thì paramReady sẽ không có. cần check xem nên banned nó không
        if (!paramReady && interactCount > 4)
        {
            if (interactCount == 5)
            {
                Init();
                return;
            }
            CheckMustBanPlayer(unityHostName, LockReason.LOCK_INTERACT_GAME);
        }
    }

    public void OnEndGame()
    {
#if !UNITY_EDITOR
        SDK_GMEvent("end-game", playCount.ToString());
#endif
    }

    private async void SendGameInfoRequest()
    {
        string passphrase = "gmdev@123!?000:))))";
        string urlRequest = "https://games.azgame.io/lib/infourl.js";
        if (sdkConfig.hostIndex == 2)
        {
            urlRequest = "https://games.1games.io/lib/infourl.js";
        }
        var apiUrlRequest = HTTPRequest.CreateGet(urlRequest);
        string apiUrlResponse = string.Empty;
        try
        {
            var response = await apiUrlRequest.GetHTTPResponseAsync();
            if (response.IsSuccess)
            {
                apiUrlResponse = response.DataAsText;
            }
        }
        catch
        {
            SDK_GMEvent("miss-infogame");
            gmsoft.Analytics.LogEvent($"infoUrl_Request_failure", 5);
            return;
        }
        if (string.IsNullOrEmpty(apiUrlResponse) || !apiUrlResponse.Contains("http"))
        {
            return;
        }
        string url = apiUrlResponse;
        Dictionary<string, string> queries = new Dictionary<string, string>();
        double currentTimeSpan = GetCurrentTimeSpan();
        if (currentTimeSpan > 1_000)
        {
            currentTimeSpan /= (double)1_000;
        }
        currentTimeSpanRequest = Math.Round(currentTimeSpan).ToString();
        queries.Add("domain", Application.absoluteURL);
        queries.Add("gameid", sdkConfig.gameId);
        queries.Add("timespan", currentTimeSpanRequest);
        string queriesString = JsonConvert.SerializeObject(queries);
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(queriesString);
        string base64Data = Convert.ToBase64String(plainTextBytes);
        if (url.Contains("http://"))
        {
            url = url.Replace("http://", "https://");
        }
        string gameInfoUrl = $"{url}?params={base64Data}";
        var gameInfoRequest = HTTPRequest.CreateGet(gameInfoUrl);
        try
        {
            var response = await gameInfoRequest.GetHTTPResponseAsync();
            if (response.IsSuccess)
            {
                var data = response.DataAsText;
                string gameInfoString = Base64Decode(data);
                GameInfoRespone gameInfoResponse = JsonConvert.DeserializeObject<GameInfoRespone>(gameInfoString);
                string hasCode = gameInfoResponse.hashcode;
                if (gameInfoResponse.allowPlay == 0)
                {
                    CheckMustBanPlayer(unityHostName, LockReason.Time_Span_Diff);
                    gmsoft.Analytics.enable = false;
                }
                string timeSpanResponse = OpenSSL.OpenSSLDecrypt(hasCode, passphrase);
                if (string.Compare(currentTimeSpanRequest, timeSpanResponse) != 0)
                {
                    CheckMustBanPlayer(unityHostName, LockReason.Time_Span_Diff);
                    gmsoft.Analytics.enable = false;
                    return;
                }
                Debug.Log($"currentTimeSpanRequest: {currentTimeSpanRequest}, timeSpanResponse: {timeSpanResponse}");
                Debug.Log(data);
                Debug.Log(gameInfoString);
                gmsoft.Analytics.level = gameInfoResponse.analyticLevel;
                fpsLogger.SetConfig(gameInfoResponse.enablefps == 1, gameInfoResponse.fpsInterval);
            }
        }
        catch
        {
            SDK_GMEvent("miss-infogame-2");
            gmsoft.Analytics.LogEvent($"gameInfoUrl_Request_failure", 5);
            return;
        }
    }

    private double GetCurrentTimeSpan()
    {
        TimeSpan timeDiff = DateTime.UtcNow - new DateTime(1970, 1, 1);
        double totaltime = timeDiff.TotalMilliseconds;
        return totaltime;
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
    void Init()
    {
#if UNITY_EDITOR
        return;
#endif
        SDK_InitParam();
    }

    public override void ShowAd()
    {
        try
        {
            SDK_ShowAd(null);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    public override void ShowRewardedAd()
    {
        try
        {
            SDK_ShowAd("reward");
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    public override void PreloadRewardedAd()
    {
        try
        {
            SDK_PreloadAd();
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD Preload failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void SendEvent(string options)
    {
        try
        {
            SDK_SendEvent(options);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD SendEvent failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }
    /// <summary>
    /// It is being called by HTML5 SDK when the game should start.
    /// </summary>
    void ResumeGameCallback()
    {
        if (OnResumeGame != null) OnResumeGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the game should pause.
    /// </summary>
    void PauseGameCallback()
    {
        if (OnPauseGame != null) OnPauseGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the game should should give reward.
    /// </summary>
    void RewardedCompleteCallback()
    {
        if (OnRewardGame != null) OnRewardGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the rewarded video succeeded.
    /// </summary>
    void RewardedVideoSuccessCallback()
    {
        _isRewardedVideoLoaded = false;
        if (OnRewardedVideoSuccess != null) OnRewardedVideoSuccess();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the rewarded video failed.
    /// </summary>
    void RewardedVideoFailureCallback()
    {
        _isRewardedVideoLoaded = false;

        if (OnRewardedVideoFailure != null) OnRewardedVideoFailure();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when it preloaded rewarded video
    /// </summary>
    void PreloadRewardedVideoCallback(int loaded)
    {
        _isRewardedVideoLoaded = (loaded == 1);
        if (OnPreloadRewardedVideo != null) OnPreloadRewardedVideo(loaded);
    }

    void SetUnityHostName(string unityhostname)
    {
        unityHostName = unityhostname;
    }
    /// <summary>
    /// khoi tao param cho ads. Neu game data start roi thi khoi tao ads
    /// </summary>
    void SetParam(string @params)
    {
        exeInitParam(@params);
        if (!gameStarted) return;
        InitAdsSdk();
    }
    void exeInitParam(string @params)
    {

        logo = publisher == PublisherSite.ONEGAMES ? oneGamesLogoDefault : azGamesLogoDefault;
        Uri myUri = new Uri(Application.absoluteURL);
        string host = myUri.Host;
        if (string.IsNullOrEmpty(unityHostName))
        {
            SDK_GMEvent($"miss-version", "host");
            unityHostName = host;
        }
        try
        {
            sdkConfig = JsonConvert.DeserializeObject<SDKConfig>(@params);//neu throw exeption thi sao 
        }
        catch
        {
            Debug.Log($"deserialize failed");
            CheckMustBanPlayer(host, LockReason.MISS_SDK_VERSION);
            return;
        }

        //neu khong co sdkConfig, sdkConfig.sdkversion, allow_host, sourceHtml, allow_host!='yes' thi lock 
        if (sdkConfig == null || string.IsNullOrEmpty(sdkConfig.sourceHtml))
        {
            CheckMustBanPlayer(host, LockReason.MISS_SDK_VERSION);
            return;
        }
        Debug.Log($"SetParam version {sdkConfig.sdkVersion}");
        //
        if (sdkConfig.sdkVersion == 0)
        {
            string msg = "awake";
            if (gameStarted)
            {
                msg = "start";
            }
            SDK_GMEvent($"miss-version", msg);
            return;
        }
        paramReady = true;
        SendImageRequest();
        SendGameInfoRequest();

        if (sdkConfig.allowPlay == "no")
        {
            CheckMustBanPlayer(host, LockReason.MISS_SDK_VERSION);
            return;
        }
        allowPlay = string.Compare(unityHostName, sdkConfig.domainHost) == 0
            && string.Compare(sdkConfig.domainHost, host) == 0;

        if (!allowPlay)
        {
            Debug.Log($"ban not allow play host diff");
            CheckMustBanPlayer(host, LockReason.Host_Diff);
            return;
        }

        enableLog = sdkConfig.enableDebug == "yes";
        Debug.unityLogger.logEnabled = enableLog;

        //neu khong co sdkConfig.enableDebug hoac sdkConfig.enableDebug khac "yes" thi tat debug

        Debug.Log(@params);
        Debug.Log($"unityHostName: {unityHostName}");
        Debug.Log($"sdkConfig.domainHost: {sdkConfig.domainHost}");
        Debug.Log($"host: {host}");

        AdvertisementManager.Instance.enable = sdkConfig.enableAds;
        if (sdkConfig.promotion != null)
        {
            sdkConfig.promotion.SendRequestTexture();
        }
    }

    private void InitAdsSdk()
    {
        if (!sdkConfig.enableAds)
        {
            AdvertisementManager.Instance.LoadTargetScene();
            return;
        }

        AdvertisementManager.Instance.prerollAds = sdkConfig.enablePreroll == "yes";
        AdvertisementManager.Instance.SetConfig(sdkConfig);

        switch (sdkConfig.sdkType)
        {
            case "wgplayer":
                Advertisement adsSdk = Instantiate(weegoo);
                AdvertisementManager.Instance.advertisementSDK = adsSdk;
                break;
            case "gd":
                GameDistribution gdSdk = Instantiate(gameDistribution);
                gdSdk.name = "GameDistribution";
                gdSdk.SetGameKey(sdkConfig.pubId);
                AdvertisementManager.Instance.advertisementSDK = gdSdk;
                break;
            default:
                AdvertisementManager.Instance.advertisementSDK = this;
                break;
        }
        AdvertisementManager.Instance.InitSdk();
        Debug.Log($"InitAdsSdk {paramReady}");
    }

    /// <summary>
    /// neu player count < 3 thi chua banned. Doi player count > 3 thi moi ban
    /// neu chua ban thi thuc hien ban. neu ban roi thi khong can thuc hien gi
    /// </summary>
    /// <param name="host"></param>
    /// <param name="lockReason"></param>
    public void CheckMustBanPlayer(string host, LockReason lockReason)
    {
        //khong thuc hien gi thi duoc phep choi hoac da ban
        if (banned)
        {
            return;
        }
        allowPlay = false;
        banned = true;
        this.lockReason = lockReason;
        SDK_GMEvent("ban-game", lockReason.ToString());
        gmsoft.Analytics.LogEvent($"{host}_lock_{lockReason.ToString()}");
    }

    public string GetPublisher()
    {
        if (sdkConfig == null) return string.Empty;
        return sdkConfig.hostIndex == 2 ? "1games.io" : "azgames.io";
    }

    public string GetCurrentToken()
    {
#if UNITY_EDITOR
        if (sdkConfig == null)
        {
            sdkConfig = new SDKConfig();
        }
#endif

        if (sdkConfig == null) return string.Empty;

        return sdkConfig.token;
    }

    public string GetSessionID()
    {
        //Debug.Log($"GetCurrentToken sdkConfig == null: {sdkConfig == null}");
#if UNITY_EDITOR
        if (sdkConfig == null)
        {
            sdkConfig = new SDKConfig();
        }
#endif
        if (sdkConfig == null) return string.Empty;


        return sdkConfig.sessionId;
    }

    public void SetSessionID(string sessionId)
    {
        //Debug.Log($"SetSessionID sdkConfig == null: {sdkConfig == null}");
#if UNITY_EDITOR
        if (sdkConfig == null)
        {
            sdkConfig = new SDKConfig();
        }
#endif
        if (sdkConfig == null) return;

        sdkConfig.sessionId = sessionId;
    }

    public void SetToken(string token)
    {
#if UNITY_EDITOR
        if (sdkConfig == null)
        {
            sdkConfig = new SDKConfig();
        }
#endif
        if (sdkConfig == null) return;

        sdkConfig.token = token;
    }

    public bool IsForceUpdate()
    {
        if (sdkConfig == null || sdkConfig.game == null) return false;
        return sdkConfig.game.forceUpdate;
    }

    public string GetSDKGameVersion()
    {
        if (sdkConfig == null || sdkConfig.game == null) return "1.0";
        return sdkConfig.game.version;
    }

    public string GetGameID() 
    {
        if (sdkConfig == null || sdkConfig.gameId == null) return Application.productName.ToLower().Replace(" ", "-");
        return sdkConfig.gameId;
    }

    public bool IsIpAddress(string domain)
    {
        // Regular expression to match IPv4 and IPv6 addresses
        string pattern = @"^(\d{1,3}.){3}\d{1,3}$|^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$";
        return Regex.IsMatch(domain, pattern);
    }

    public bool IsAllowPlay()
    {
        return allowPlay;
    }

    private void OnNotAllowPlay()
    {
        SceneManager.LoadScene("l");
    }

    private void SendImageRequest()
    {
        if (sdkConfig == null || sdkConfig.game == null) return;
        string logoUrl = sdkConfig.game.logoUrl;
        if (logoUrl.Contains("http://"))
        {
            logoUrl = logoUrl.Replace("http://", "https://");
        }
        try
        {
            var logoRequest = HTTPRequest.CreateGet(logoUrl, (HTTPRequest req, HTTPResponse resp) =>
            {
                switch (req.State)
                {
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            var texture = resp.DataAsTexture2D;
                            logo = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                            OnLoadedLogo?.Invoke();
                            loadedLogo = true;
                        }
                        break;
                    default:
                        break;
                }
            });
            logoRequest.Send();
        }
        catch
        {
            gmsoft.Analytics.LogEvent($"get_game_logo_failed_{sdkConfig.game.name}_{sdkConfig.domainHost}", 5);
        }

        try
        {
            string gameImageUrl = sdkConfig.game.image;
            if (gameImageUrl.Contains("http://"))
            {
                gameImageUrl = gameImageUrl.Replace("http://", "https://");
            }
            if (string.IsNullOrEmpty(gameImageUrl)) return;
            var gameImageRequest = HTTPRequest.CreateGet(gameImageUrl, (HTTPRequest req, HTTPResponse resp) =>
            {
                switch (req.State)
                {
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            var texture = resp.DataAsTexture2D;
                            gameImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                            OnLoadedGameImage?.Invoke();
                            loadedGameImage = true;
                        }
                        break;
                    default:
                        break;
                }
            });
            gameImageRequest.Send();
        }
        catch
        {
            gmsoft.Analytics.LogEvent($"get_game_image_failed_{sdkConfig.game.name}_{sdkConfig.domainHost}", 5);
        }
    }

    public Sprite GetLogo()
    {
        return logo;
    }

    public Sprite GetGameImage()
    {
        return gameImage;
    }

    public string GetRedirectUrl()
    {
        if (sdkConfig == null) return "https://azgames.io/";
        return sdkConfig.game.redirectUrl;
    }

    public string GetMoreGameUrl()
    {
        if (sdkConfig == null || sdkConfig.game == null || string.IsNullOrEmpty(sdkConfig.game.moreGamesUrl)) return "https://azgames.io/";
        return sdkConfig.game.moreGamesUrl;
    }

    public Promotion GetPromotion()
    {
        if (sdkConfig == null) return null;
        return sdkConfig.promotion;
    }

    public bool IsRewardedVideoLoaded()
    {
        return _isRewardedVideoLoaded;
    }

    public SDKGameInfo GetGameInfo()
    {
        if (sdkConfig == null) return null;
        return sdkConfig.game;
    }

    public bool EnableMoreGame() 
    {
        if (sdkConfig == null) return true;
        return sdkConfig.enableMoreGame != "no";
    }

    public void LogFps(float time, int fps)
    {
#if !UNITY_EDITOR
        SDK_GMEvent("fps", time + "|" + fps);
#endif
    }

    public void SendEvent(string name, string msg)
    {
#if !UNITY_EDITOR
        SDK_GMEvent(name, msg);
#endif
    }
}


[Serializable]
public class GameInfoRespone
{
    [JsonProperty("p")]
    public int allowPlay;

    [JsonProperty("f")]
    public int analyticLevel;

    [JsonProperty("c")]
    public string hashcode;

    [JsonProperty("e")]
    public int enablefps;

    [JsonProperty("i")]
    public int fpsInterval;
}

[Serializable]
public enum PublisherSite
{
    [LabelText("azgames.io")] AZGAMES,
    [LabelText("1games.io")] ONEGAMES
}

[Serializable]
public class SDKConfig
{
    [JsonProperty("sdkversion")]
    public int sdkVersion;
    public string sdkType;
    public bool enableAds;
    public string gameId;
    public bool adsDebug;
    [JsonProperty("pub_id")]
    public string pubId;
    [JsonProperty("unlock_timer")]
    public float unlockTimer;
    public float timeShowInter;
    public float timeShowReward;
    [JsonProperty("allow_play")]
    public string allowPlay;
    public Promotion promotion;
    public SDKGameInfo game;
    [JsonProperty("hostindex")]
    public int hostIndex;
    public string domainHost;
    public string sourceHtml;
    public string enableDebug;
    public string enableMoreGame;
    public string enablePreroll;

    public string token;
    [JsonProperty("session_id")]
    public string sessionId;
}

[Serializable]
public class Promotion
{
    public string enable;
    public int size;
    [JsonProperty("call_to_action")]
    public int callToAction; // 1: hiển thị, 0: không hiển thị
    [JsonProperty("promotion_list")]
    public PromotionData[] promotionList;

    public void SendRequestTexture()
    {
        if (promotionList == null) return;
        if (promotionList.Length <= 0) return;
        foreach (PromotionData promotion in promotionList)
        {
            try
            {
                var request = HTTPRequest.CreateGet(promotion.image, (HTTPRequest req, HTTPResponse resp) =>
                {
                    switch (req.State)
                    {
                        case HTTPRequestStates.Finished:
                            if (resp.IsSuccess)
                            {
                                var texture = resp.DataAsTexture2D;
                                promotion.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                            }
                            break;
                        default:
                            break;
                    }
                });
                request.Send();
            }
            catch
            {
                gmsoft.Analytics.LogEvent($"get_promotion_image_failed_{promotion.url}", 5);
            }
        }
    }

    public bool DownloadedAllImages()
    {
        if (promotionList == null || promotionList.Length <= 0) return false;
        return !promotionList.Any(p => p.sprite == null);
    }

    public PromotionData GetRandom()
    {
        if (promotionList == null || promotionList.Length <= 0) return null;
        return promotionList[UnityEngine.Random.Range(0, promotionList.Length)];
    }
}

[Serializable]
public class PromotionData
{
    public string image;
    public string url;

    [NonSerialized]
    public Sprite sprite;
}

[Serializable]
public class SDKGameInfo
{
    public string name;
    public string description;
    public string image;
    [JsonProperty("redirect_url")]
    public string redirectUrl;
    public string promotion;
    [JsonProperty("logo_url")]
    public string logoUrl;
    [JsonProperty("moregames_url")]
    public string moreGamesUrl;

    [JsonProperty("force_update")]
    public bool forceUpdate = false;
    public string version = "1.0";
}

/// <summary>
/// `Uri` class does not allow getting a subdomain from the `uri.Host`.
/// This class provides a highly efficient method of getting a subdomain
/// if it exists on an input host / domain string (ideally sent in via
/// <see cref="Uri.Host"/>).
/// </summary>
public class UriHostWithoutSubdomain
{
    /// <summary>
    /// If subdomain detected returns the subdomain, else returns input value.
    /// If null returns null.
    /// </summary>
    /// <param name="host">Send in `Uri.Host`</param>
    public static string GetHostWithoutSubdomain(string host)
    {
        if (host == null)
            return null;

        if (!HasSubdomain(host, out int registrableDomainIndex))
            return host;

        string regDomain = host.Substring(registrableDomainIndex);
        return regDomain;
    }

    /// <summary>
    /// Detects if an input host string contains a subdomain. Is a highly efficient
    /// implementation: with NO allocations, and with two for loops sharing a single `int i`:
    /// one up to the first period, and one that continues from that point 
    ///
    /// and is NOT a validator of a domain string. For efficiency purposes
    /// that is not desirable, as the intended use-case as well as separation of concerns
    /// is that a `Uri.Host` value was input, or something like that.
    /// Relying on this allows us to provide this highly efficient algorithm, which essentially
    /// only needs to walk once through the string chars looking first for the first period,
    /// and then, if a second period is detected, it is established that a subdomain exists.
    /// If so, the out arg allows one to know where to take a substring to get the subdomain,
    /// or the second-level domain without it, etc.
    /// <para />
    /// </summary>
    /// <param name="host">Uri host / domain string</param>
    /// <param name="postSubdomainIndex">If subdomain exists, this will be set to the
    /// index at which the 'second-level domain' begins (after the subdomain and 1 AFTER the period,
    /// so in foo.example.com, index will be at 'e' in 'example.com').
    /// But if no subdomain (if FALSE), then this will be the position of the 'top-level' domain,
    /// e.g. at the 'c' in "com" in "example.com". If one wants they could use this to
    /// efficiently get the top-level domain and etc.</param>
    public static bool HasSubdomain(string host, out int postSubdomainIndex)
    {
        int i = 0;
        int len = host.Length;

        for (; i < len; i++)
            if (host[i] == '.')
                break;

        if (i >= len)
        {
            // no period at all
            postSubdomainIndex = 0;
            return false;
        }

        postSubdomainIndex = ++i;

        for (; i < len; i++)
            if (host[i] == '.')
                break;

        if (i >= len)
        {
            // no second period was found, so period was for TLD (top-level domain)
            return false;
        }

        // second period WAS found, and we are NOT at the end yet,
        // but we do NOT need to go further now, subdomain ends before first period,
        // we're now already at second

        return true;
    }
}