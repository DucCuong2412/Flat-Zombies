using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/UI/BannerAppodeal")]
public class BannerAppodeal : MonoBehaviour, IInterstitialAdListener
{
	public enum TypeBanner
	{
		Banner,
		BannerTop,
		BannerBottom,
		Interstitial,
		NonSkippableVideo,
		RewardedVideo
	}

	public static bool visible;

	public static int stepsLeftRequest;

	public static int markTimeShow;

	public static bool solveRequest = true;

	[Tooltip("Тип баннера для запроса")]
	public TypeBanner type;

	[Tooltip("Идентификатор/ключ полученный при создании приложения")]
	public string appKey;

	[Tooltip("Согласие на передачу линых данных по условиям GDPR")]
	public bool GDPR = true;

	[Tooltip("Показывать рекламу только при запуске сцены, без таймера")]
	public bool showStartScene;

	[Tooltip("Отключение сетей")]
	public string[] disableNetwork;

	[Space(6f)]
	[Tooltip("Интервал для показа первого баннера, в скндх")]
	public int firstTimeShow;

	[Tooltip("Интервал для показа баннера, в скндх")]
	public int intervalShow;

	[Tooltip("Сколько раз показать один и тот же баннер")]
	public int stepRequest = 2;

	[Tooltip("Кнопки, при нажатии на которых показать баннер")]
	public Button[] buttonShow = new Button[0];

	[Space(6f)]
	[Tooltip("Скрыть баннер закрытии сцены, при выполнении OnDestroy()")]
	public bool hideOnDestroy;

	[Space(6f)]
	[Tooltip("Тестовый режим. В тестовом режиме будет показ тестовой рекламы и отладочные данные будут записываться в logcat")]
	public bool testMode;

	private int adType;

	private void Start()
	{
		Show();
		for (int i = 0; i < buttonShow.Length; i++)
		{
			buttonShow[i].onClick.AddListener(Show);
		}
	}

	private void RequestBanner()
	{
		if (adType == 0)
		{
			if (type == TypeBanner.Banner)
			{
				adType = 4;
			}
			else if (type == TypeBanner.BannerTop)
			{
				adType = 16;
			}
			else if (type == TypeBanner.BannerBottom)
			{
				adType = 8;
			}
			else if (type == TypeBanner.Interstitial)
			{
				adType = 3;
			}
			else if (type == TypeBanner.NonSkippableVideo)
			{
				adType = 128;
			}
			else if (type == TypeBanner.RewardedVideo)
			{
				adType = 128;
			}
			Appodeal.setInterstitialCallbacks(this);
			Appodeal.setTesting(testMode);
		}
		if (stepsLeftRequest <= 0)
		{
			for (int i = 0; i < disableNetwork.Length; i++)
			{
				Appodeal.disableNetwork(disableNetwork[i]);
			}
			Appodeal.disableLocationPermissionCheck();
			Appodeal.initialize(appKey, adType, GDPR);
			Appodeal.hide(adType);
			UnityEngine.Debug.Log("BannerAppodeal.Request:" + stepsLeftRequest.ToString());
			stepsLeftRequest = stepRequest;
		}
	}

	public void Show(int interval)
	{
		RequestBanner();
		CancelInvoke("Show");
		int num = Mathf.FloorToInt(Time.time / Time.timeScale);
		int num2 = markTimeShow + interval - num;
		if (markTimeShow == 0)
		{
			num2 = firstTimeShow - num;
		}
		if (showStartScene && num2 <= 0 && Appodeal.isLoaded(adType))
		{
			Appodeal.show(adType);
			markTimeShow = Mathf.FloorToInt(Time.time / Time.timeScale);
			stepsLeftRequest--;
			UnityEngine.Debug.Log("BannerAppodeal.Show(): IsLoaded");
		}
		else if (!showStartScene && buttonShow.Length != 0)
		{
			if (num2 <= 0 && Appodeal.isLoaded(adType))
			{
				Appodeal.show(adType);
				markTimeShow = Mathf.FloorToInt(Time.time / Time.timeScale);
				stepsLeftRequest--;
				UnityEngine.Debug.Log("BannerAppodeal.Show: buttonShow");
			}
		}
		else if (!showStartScene)
		{
			if (num2 > 0)
			{
				Invoke("Show", (float)num2 * Time.timeScale);
				UnityEngine.Debug.Log("BannerAppodeal.Show: " + num2.ToString() + " sec.");
			}
			else if (Appodeal.isLoaded(adType))
			{
				Appodeal.show(adType);
				markTimeShow = Mathf.FloorToInt(Time.time / Time.timeScale);
				stepsLeftRequest--;
				Invoke("Show", (float)interval * Time.timeScale);
				UnityEngine.Debug.Log("BannerAppodeal.Show: IsLoaded");
			}
			else
			{
				Invoke("Show", 5f * Time.timeScale);
				UnityEngine.Debug.Log("BannerAppodeal.Show: 5 sec.");
			}
		}
	}

	public void Show()
	{
		Show(intervalShow);
	}

	public void onInterstitialLoaded(bool isPrecache)
	{
		UnityEngine.Debug.Log("BannerAppodeal.Interstitial loaded");
	}

	public void onInterstitialFailedToLoad()
	{
		UnityEngine.Debug.Log("BannerAppodeal:Interstitial failed");
	}

	public void onInterstitialShown()
	{
		UnityEngine.Debug.Log("BannerAppodeal:Interstitial opened");
	}

	public void onInterstitialClosed()
	{
		RequestBanner();
		UnityEngine.Debug.Log("BannerAppodeal:Interstitial closed");
	}

	public void onInterstitialClicked()
	{
		UnityEngine.Debug.Log("BannerAppodeal:Interstitial clicked");
	}

	public void OnDestroy()
	{
		if (hideOnDestroy)
		{
			Appodeal.hide(adType);
		}
		buttonShow = new Button[0];
		CancelInvoke();
	}
}
