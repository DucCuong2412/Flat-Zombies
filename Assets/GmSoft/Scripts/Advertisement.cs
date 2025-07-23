using System;
using UnityEngine;

namespace Assets.GmSoft.Scripts
{
    public abstract class Advertisement : MonoBehaviour
    {
        public Action OnResumeGame;
        public Action OnPauseGame;
        public Action OnRewardGame;
        public Action OnRewardedVideoSuccess;
        public Action OnRewardedVideoFailure;
        public Action<int> OnPreloadRewardedVideo;

        public abstract void ShowAd();
        public abstract void ShowRewardedAd();
        public abstract void PreloadRewardedAd();
    }
}
