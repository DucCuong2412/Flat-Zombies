using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealDemo : MonoBehaviour, IPermissionGrantedListener, IInterstitialAdListener, IBannerAdListener, INonSkippableVideoAdListener, IRewardedVideoAdListener
{
	private string appKey = "fee50c333ff3825fd6ad6d38cff78154de3025546d47a84f";

	private string interstitialLabel = "CACHE INTERSTITIAL";

	private string rewardedLabel = "Loading";

	private int buttonWidth;

	private int buttonHeight;

	private int toggleSize;

	private GUIStyle buttonStyle;

	private bool testingToggle;

	private bool loggingToggle;

	public void Awake()
	{
		Appodeal.requestAndroidMPermissions(this);
	}

	public void Init()
	{
		if (loggingToggle)
		{
			Appodeal.setLogLevel(Appodeal.LogLevel.Verbose);
		}
		else
		{
			Appodeal.setLogLevel(Appodeal.LogLevel.None);
		}
		Appodeal.setTesting(testingToggle);
		UserSettings userSettings = new UserSettings();
		userSettings.setAge(25).setGender(UserSettings.Gender.OTHER).setUserId("best_user_ever");
		Appodeal.disableNetwork("appnext");
		Appodeal.disableNetwork("amazon_ads", 4);
		Appodeal.disableLocationPermissionCheck();
		Appodeal.disableWriteExternalStoragePermissionCheck();
		Appodeal.setTriggerOnLoadedOnPrecache(3, onLoadedTriggerBoth: true);
		Appodeal.setSmartBanners(value: true);
		Appodeal.setBannerAnimation(value: false);
		Appodeal.setTabletBanners(value: false);
		Appodeal.setBannerBackground(value: false);
		Appodeal.setChildDirectedTreatment(value: false);
		Appodeal.muteVideosIfCallsMuted(value: true);
		Appodeal.setAutoCache(3, autoCache: false);
		Appodeal.setExtraData(ExtraData.APPSFLYER_ID, "1527256526604-2129416");
		int @int = PlayerPrefs.GetInt("result_gdpr", 1);
		Appodeal.initialize(appKey, 195, @int == 1);
		Appodeal.setBannerCallbacks(this);
		Appodeal.setInterstitialCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setSegmentFilter("newBoolean", value: true);
		Appodeal.setSegmentFilter("newInt", 1234567890);
		Appodeal.setSegmentFilter("newDouble", 123.123456789);
		Appodeal.setSegmentFilter("newString", "newStringFromSDK");
	}

	private void OnGUI()
	{
		InitStyles();
		if (GUI.Toggle(new Rect(Screen.width / 10, Screen.height / 10 - Screen.height / 15, toggleSize * 3, toggleSize), testingToggle, new GUIContent("Testing")))
		{
			testingToggle = true;
		}
		else
		{
			testingToggle = false;
		}
		if (GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 10 - Screen.height / 15, toggleSize * 3, toggleSize), loggingToggle, new GUIContent("Logging")))
		{
			loggingToggle = true;
		}
		else
		{
			loggingToggle = false;
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10, buttonWidth, buttonHeight), "INITIALIZE", buttonStyle))
		{
			Init();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + Screen.height / 10, buttonWidth, buttonHeight), interstitialLabel, buttonStyle))
		{
			showInterstitial();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 2 * Screen.height / 10, buttonWidth, buttonHeight), rewardedLabel, buttonStyle))
		{
			showRewardedVideo();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 3 * Screen.height / 10, buttonWidth, buttonHeight), "SHOW BANNER", buttonStyle))
		{
			showBanner();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 4 * Screen.height / 10, buttonWidth, buttonHeight), "HIDE BANNER", buttonStyle))
		{
			hideBanner();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 5 * Screen.height / 10, buttonWidth, buttonHeight), "SHOW BANNER VIEW", buttonStyle))
		{
			showBannerView();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 6 * Screen.height / 10, buttonWidth, buttonHeight), "HIDE BANNER VIEW", buttonStyle))
		{
			hideBannerView();
		}
		if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10 + 7 * Screen.height / 10, buttonWidth, buttonHeight), "SHOW TEST SCREEN", buttonStyle))
		{
			Appodeal.showTestScreen();
		}
	}

	private void InitStyles()
	{
		if (buttonStyle == null)
		{
			buttonWidth = Screen.width - Screen.width / 5;
			buttonHeight = Screen.height / 15;
			toggleSize = Screen.height / 20;
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontSize = buttonHeight / 2;
			buttonStyle.normal.textColor = Color.red;
			buttonStyle.hover.textColor = Color.red;
			buttonStyle.active.textColor = Color.red;
			buttonStyle.focused.textColor = Color.red;
			buttonStyle.active.background = MakeTexure(buttonWidth, buttonHeight, Color.grey);
			buttonStyle.focused.background = MakeTexure(buttonWidth, buttonHeight, Color.grey);
			buttonStyle.normal.background = MakeTexure(buttonWidth, buttonHeight, Color.white);
			buttonStyle.hover.background = MakeTexure(buttonWidth, buttonHeight, Color.white);
			GUI.skin.toggle = buttonStyle;
		}
	}

	private Texture2D MakeTexure(int width, int height, Color color)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public void showInterstitial()
	{
		if (Appodeal.isLoaded(3) && !Appodeal.isPrecache(3))
		{
			Appodeal.show(3);
		}
		else
		{
			Appodeal.cache(3);
		}
	}

	public void showRewardedVideo()
	{
		UnityEngine.Debug.Log("Reward currency: " + Appodeal.getRewardParameters().Key + ", amount: " + Appodeal.getRewardParameters().Value);
		if (Appodeal.canShow(128))
		{
			Appodeal.show(128);
		}
	}

	public void showBanner()
	{
		Appodeal.show(8, "banner_button_click");
	}

	public void showBannerView()
	{
		Appodeal.showBannerView(Screen.currentResolution.height - Screen.currentResolution.height / 10, -2, "banner_view");
	}

	public void hideBanner()
	{
		Appodeal.hide(4);
	}

	public void hideBannerView()
	{
		Appodeal.hideBannerView();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			Appodeal.onResume();
		}
	}

	public void onBannerLoaded(bool precache)
	{
		MonoBehaviour.print("banner loaded");
	}

	public void onBannerFailedToLoad()
	{
		MonoBehaviour.print("banner failed");
	}

	public void onBannerShown()
	{
		MonoBehaviour.print("banner opened");
	}

	public void onBannerClicked()
	{
		MonoBehaviour.print("banner clicked");
	}

	public void onInterstitialLoaded(bool isPrecache)
	{
		interstitialLabel = "SHOW INTERSTITIAL";
		MonoBehaviour.print("Appodeal. Interstitial loaded");
	}

	public void onInterstitialFailedToLoad()
	{
		MonoBehaviour.print("Appodeal. Interstitial failed");
	}

	public void onInterstitialShown()
	{
		interstitialLabel = "CACHE INTERSTITIAL";
		MonoBehaviour.print("Appodeal. Interstitial opened");
	}

	public void onInterstitialClosed()
	{
		MonoBehaviour.print("Appodeal. Interstitial closed");
	}

	public void onInterstitialClicked()
	{
		MonoBehaviour.print("Appodeal. Interstitial clicked");
	}

	public void onNonSkippableVideoLoaded(bool isPrecache)
	{
		UnityEngine.Debug.Log("NonSkippable Video loaded");
	}

	public void onNonSkippableVideoFailedToLoad()
	{
		UnityEngine.Debug.Log("NonSkippable Video failed to load");
	}

	public void onNonSkippableVideoShown()
	{
		UnityEngine.Debug.Log("NonSkippable Video opened");
	}

	public void onNonSkippableVideoClosed(bool isFinished)
	{
		UnityEngine.Debug.Log("NonSkippable Video, finished:" + isFinished);
	}

	public void onNonSkippableVideoFinished()
	{
		UnityEngine.Debug.Log("NonSkippable Video finished");
	}

	public void onRewardedVideoLoaded(bool isPrecache)
	{
		rewardedLabel = "SHOW REWARDED";
		MonoBehaviour.print("Appodeal. Video loaded");
	}

	public void onRewardedVideoFailedToLoad()
	{
		MonoBehaviour.print("Appodeal. Video failed");
	}

	public void onRewardedVideoShown()
	{
		rewardedLabel = "Loading";
		MonoBehaviour.print("Appodeal. Video shown");
	}

	public void onRewardedVideoClosed(bool finished)
	{
		MonoBehaviour.print("Appodeal. Video closed");
	}

	public void onRewardedVideoFinished(double amount, string name)
	{
		MonoBehaviour.print("Appodeal. Reward: " + amount + " " + name);
	}

	public void writeExternalStorageResponse(int result)
	{
		if (result == 0)
		{
			UnityEngine.Debug.Log("WRITE_EXTERNAL_STORAGE permission granted");
		}
		else
		{
			UnityEngine.Debug.Log("WRITE_EXTERNAL_STORAGE permission grant refused");
		}
	}

	public void accessCoarseLocationResponse(int result)
	{
		if (result == 0)
		{
			UnityEngine.Debug.Log("ACCESS_COARSE_LOCATION permission granted");
		}
		else
		{
			UnityEngine.Debug.Log("ACCESS_COARSE_LOCATION permission grant refused");
		}
	}
}
