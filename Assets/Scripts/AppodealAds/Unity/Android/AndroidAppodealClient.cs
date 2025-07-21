using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AndroidAppodealClient : IAppodealAdsClient
	{
		private bool isShow;

		private AndroidJavaClass appodealClass;

		private AndroidJavaClass appodealUnityClass;

		private AndroidJavaClass appodealBannerClass;

		private AndroidJavaObject appodealBannerInstance;

		private AndroidJavaObject userSettings;

		private AndroidJavaObject activity;

		private AndroidJavaObject popupWindow;

		private AndroidJavaObject resources;

		private AndroidJavaObject displayMetrics;

		private AndroidJavaObject window;

		private AndroidJavaObject decorView;

		private AndroidJavaObject attributes;

		private AndroidJavaObject rootView;

		public AndroidJavaClass getAppodealClass()
		{
			if (appodealClass == null)
			{
				appodealClass = new AndroidJavaClass("com.appodeal.ads.Appodeal");
			}
			return appodealClass;
		}

		public AndroidJavaClass getAppodealUnityClass()
		{
			if (appodealUnityClass == null)
			{
				appodealUnityClass = new AndroidJavaClass("com.appodeal.unity.AppodealUnity");
			}
			return appodealUnityClass;
		}

		public AndroidJavaObject getAppodealBannerInstance()
		{
			if (appodealBannerInstance == null)
			{
				appodealBannerClass = new AndroidJavaClass("com.appodeal.unity.AppodealUnityBannerView");
				appodealBannerInstance = appodealBannerClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			}
			return appodealBannerInstance;
		}

		public AndroidJavaObject getActivity()
		{
			if (activity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return activity;
		}

		public void initialize(string appKey, int adTypes)
		{
			initialize(appKey, adTypes, hasConsent: true);
		}

		public void initialize(string appKey, int adTypes, bool hasConsent)
		{
			getAppodealClass().CallStatic("disableNetwork", getActivity(), "mobvista");
			getAppodealClass().CallStatic("setFramework", "unity", Appodeal.getPluginVersion());
			if ((adTypes & 0x40) > 0)
			{
				getAppodealClass().CallStatic("setFramework", "unity", Appodeal.getPluginVersion(), false, false);
				getAppodealClass().CallStatic("disableNetwork", getActivity(), "amazon_ads", 4);
			}
			getAppodealClass().CallStatic("initialize", getActivity(), appKey, adTypes, hasConsent);
		}

		public bool show(int adTypes)
		{
			return getAppodealUnityClass().CallStatic<bool>("show", new object[2]
			{
				getActivity(),
				adTypes
			});
		}

		public bool show(int adTypes, string placement)
		{
			return getAppodealUnityClass().CallStatic<bool>("show", new object[3]
			{
				getActivity(),
				adTypes,
				placement
			});
		}

		public bool showBannerView(int YAxis, int XAxis, string Placement)
		{
			return getAppodealBannerInstance().Call<bool>("showBannerView", new object[4]
			{
				getActivity(),
				XAxis,
				YAxis,
				Placement
			});
		}

		public bool isLoaded(int adTypes)
		{
			return getAppodealClass().CallStatic<bool>("isLoaded", new object[1]
			{
				adTypes
			});
		}

		public void cache(int adTypes)
		{
			getAppodealClass().CallStatic("cache", getActivity(), adTypes);
		}

		public void hide(int adTypes)
		{
			getAppodealClass().CallStatic("hide", getActivity(), adTypes);
		}

		public void hideBannerView()
		{
			getAppodealBannerInstance().Call("hideBannerView", getActivity());
		}

		public bool isPrecache(int adTypes)
		{
			return getAppodealClass().CallStatic<bool>("isPrecache", new object[1]
			{
				adTypes
			});
		}

		public void setAutoCache(int adTypes, bool autoCache)
		{
			getAppodealClass().CallStatic("setAutoCache", adTypes, autoCache);
		}

		public void onResume()
		{
			getAppodealClass().CallStatic("onResume", getActivity(), 4);
		}

		public void setSmartBanners(bool value)
		{
			getAppodealClass().CallStatic("setSmartBanners", value);
			getAppodealBannerInstance().Call("setSmartBanners", value);
		}

		public void setBannerAnimation(bool value)
		{
			getAppodealClass().CallStatic("setBannerAnimation", value);
		}

		public void setBannerBackground(bool value)
		{
			UnityEngine.Debug.LogWarning("Not Supported by Android SDK");
		}

		public void setTabletBanners(bool value)
		{
			getAppodealClass().CallStatic("set728x90Banners", value);
		}

		public void setTesting(bool test)
		{
			getAppodealClass().CallStatic("setTesting", test);
		}

		public void setLogLevel(Appodeal.LogLevel logging)
		{
			switch (logging)
			{
			case Appodeal.LogLevel.None:
				getAppodealClass().CallStatic("setLogLevel", new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("none"));
				break;
			case Appodeal.LogLevel.Debug:
				getAppodealClass().CallStatic("setLogLevel", new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("debug"));
				break;
			case Appodeal.LogLevel.Verbose:
				getAppodealClass().CallStatic("setLogLevel", new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("verbose"));
				break;
			}
		}

		public void setChildDirectedTreatment(bool value)
		{
			getAppodealClass().CallStatic("setChildDirectedTreatment", value);
		}

		public void disableNetwork(string network)
		{
			getAppodealClass().CallStatic("disableNetwork", getActivity(), network);
		}

		public void disableNetwork(string network, int adTypes)
		{
			getAppodealClass().CallStatic("disableNetwork", getActivity(), network, adTypes);
		}

		public void disableLocationPermissionCheck()
		{
			getAppodealClass().CallStatic("disableLocationPermissionCheck");
		}

		public void disableWriteExternalStoragePermissionCheck()
		{
			getAppodealClass().CallStatic("disableWriteExternalStoragePermissionCheck");
		}

		public void setTriggerOnLoadedOnPrecache(int adTypes, bool onLoadedTriggerBoth)
		{
			getAppodealClass().CallStatic("setTriggerOnLoadedOnPrecache", adTypes, onLoadedTriggerBoth);
		}

		public void muteVideosIfCallsMuted(bool value)
		{
			getAppodealClass().CallStatic("muteVideosIfCallsMuted", value);
		}

		public void showTestScreen()
		{
			getAppodealClass().CallStatic("startTestActivity", getActivity());
		}

		public string getVersion()
		{
			return getAppodealClass().CallStatic<string>("getVersion", new object[0]);
		}

		public bool canShow(int adTypes)
		{
			return getAppodealClass().CallStatic<bool>("canShow", new object[1]
			{
				adTypes
			});
		}

		public bool canShow(int adTypes, string placement)
		{
			return getAppodealClass().CallStatic<bool>("canShow", new object[2]
			{
				adTypes,
				placement
			});
		}

		public void setSegmentFilter(string name, bool value)
		{
			getAppodealClass().CallStatic("setSegmentFilter", name, value);
		}

		public void setSegmentFilter(string name, int value)
		{
			getAppodealClass().CallStatic("setSegmentFilter", name, value);
		}

		public void setSegmentFilter(string name, double value)
		{
			getAppodealClass().CallStatic("setSegmentFilter", name, value);
		}

		public void setSegmentFilter(string name, string value)
		{
			getAppodealClass().CallStatic("setSegmentFilter", name, value);
		}

		public void setExtraData(string key, bool value)
		{
			getAppodealClass().CallStatic("setExtraData", key, value);
		}

		public void setExtraData(string key, int value)
		{
			getAppodealClass().CallStatic("setExtraData", key, value);
		}

		public void setExtraData(string key, double value)
		{
			getAppodealClass().CallStatic("setExtraData", key, value);
		}

		public void setExtraData(string key, string value)
		{
			getAppodealClass().CallStatic("setExtraData", key, value);
		}

		public void trackInAppPurchase(double amount, string currency)
		{
			getAppodealClass().CallStatic("trackInAppPurchase", getActivity(), amount, currency);
		}

		public string getRewardCurrency(string placement)
		{
			AndroidJavaObject androidJavaObject = getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[1]
			{
				placement
			});
			return androidJavaObject.Get<string>("second");
		}

		public double getRewardAmount(string placement)
		{
			AndroidJavaObject androidJavaObject = getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[1]
			{
				placement
			});
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Get<AndroidJavaObject>("first");
			return androidJavaObject2.Call<double>("doubleValue", new object[0]);
		}

		public string getRewardCurrency()
		{
			AndroidJavaObject androidJavaObject = getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[0]);
			return androidJavaObject.Get<string>("second");
		}

		public double getRewardAmount()
		{
			AndroidJavaObject androidJavaObject = getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Get<AndroidJavaObject>("first");
			return androidJavaObject2.Call<double>("doubleValue", new object[0]);
		}

		public void getUserSettings()
		{
			userSettings = getAppodealClass().CallStatic<AndroidJavaObject>("getUserSettings", new object[1]
			{
				getActivity()
			});
		}

		public void setUserId(string id)
		{
			userSettings.Call<AndroidJavaObject>("setUserId", new object[1]
			{
				id
			});
		}

		public void setAge(int age)
		{
			userSettings.Call<AndroidJavaObject>("setAge", new object[1]
			{
				age
			});
		}

		public void setGender(UserSettings.Gender gender)
		{
			switch (gender)
			{
			case UserSettings.Gender.OTHER:
				userSettings.Call<AndroidJavaObject>("setGender", new object[1]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("OTHER")
				});
				break;
			case UserSettings.Gender.MALE:
				userSettings.Call<AndroidJavaObject>("setGender", new object[1]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("MALE")
				});
				break;
			case UserSettings.Gender.FEMALE:
				userSettings.Call<AndroidJavaObject>("setGender", new object[1]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("FEMALE")
				});
				break;
			}
		}

		public void setInterstitialCallbacks(IInterstitialAdListener listener)
		{
			getAppodealClass().CallStatic("setInterstitialCallbacks", new AppodealInterstitialCallbacks(listener));
		}

		public void setNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
		{
			getAppodealClass().CallStatic("setNonSkippableVideoCallbacks", new AppodealNonSkippableVideoCallbacks(listener));
		}

		public void setRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			getAppodealClass().CallStatic("setRewardedVideoCallbacks", new AppodealRewardedVideoCallbacks(listener));
		}

		public void setBannerCallbacks(IBannerAdListener listener)
		{
			getAppodealClass().CallStatic("setBannerCallbacks", new AppodealBannerCallbacks(listener));
		}

		public void requestAndroidMPermissions(IPermissionGrantedListener listener)
		{
			getAppodealClass().CallStatic("requestAndroidMPermissions", getActivity(), new AppodealPermissionCallbacks(listener));
		}
	}
}
