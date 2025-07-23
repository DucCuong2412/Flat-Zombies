using System.Collections;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class GameAdsManager : MonoBehaviour
{
    public static GameAdsManager Instance;
    [SerializeField]
    private Text rewardedAdText;
    [SerializeField]
    private Text massage;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void ShowAd()
    {
        AdvertisementManager.Instance.ShowAds(() => { });
    }

    public void ShowRewardedAd()
    {
        if (!AdvertisementManager.Instance.CanShowRewardAds()) return;
        AdvertisementManager.Instance.ShowReward(delegate
        {
            rewardedAdText.text = "show reward done";
            StartCoroutine(HideMassagePanel(rewardedAdText.gameObject, 10));
        });
        //GmSoft.Instance.ShowRewardedAd();
    }
    public void OnRewardedVideoFailure()
    {
        rewardedAdText.text = "rewarded video fail";
        StartCoroutine(HideMassagePanel(rewardedAdText.gameObject, 10));
    }

    public void PreloadRewardedAd()
    {
        //GmSoft.Instance.PreloadRewardedAd();
        AdvertisementManager.Instance.PreloadRewardAd();
        massage.text = "preload reward ad";
        StartCoroutine(HideMassagePanel(massage.gameObject, 10));
    }
    private IEnumerator HideMassagePanel(GameObject obj, float time)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
