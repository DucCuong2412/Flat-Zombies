using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsLogger : MonoBehaviour
{
    [HideInInspector]
    public bool enable;
    [HideInInspector]
    public float interval;

    private float timer = 0;
    private int frames = 0;

    private bool focus;

    private int fps;
    private int count = 1;

    public void SetConfig(bool enable, float interval)
    {
        this.enable = enable;
        this.interval = interval;
    }

    private void Update()
    {
#if UNITY_EDITOR && !UNITY_WEBGL
                return;
#endif
        if (!enable) return;
        if (!focus) return;
        if (Time.timeScale <= 0.01f) return;
        timer += Time.unscaledDeltaTime;
        frames++;
        if (timer >= interval)
        {
            fps = (int)(frames / timer);
            GmSoft.Instance.LogFps(interval * count, fps);
            count++;
            frames = 0;
            timer = 0;
        }
    }

    public int GetFPS()
    {
        return fps;
    }

    private void OnApplicationFocus(bool focus)
    {
        this.focus = focus;
    }
}
