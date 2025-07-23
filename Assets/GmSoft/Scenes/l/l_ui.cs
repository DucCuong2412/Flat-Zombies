using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class l_ui : MonoBehaviour
{
    public Image imageHover;
    //public Text nameProject;
    public TMP_Text textPro;
    // Start is called before the first frame update
    private void Awake()
    {
        string appName = Application.productName;
        textPro.text = appName;
        gmsoft.Analytics.LogEvent("lock_sence_open");
    }
    
    public void EventEnter()
    {
        Debug.Log($"Enter: {imageHover.name}");
        imageHover.gameObject.SetActive(true);
    }
    public void EventExit()
    {
        Debug.Log($"Exit: {imageHover.name}");
        imageHover.gameObject.SetActive(false);
    }
    public void EventDown()
    {
        gmsoft.Analytics.LogEvent("lock_sence_play");
        #if UNITY_WEBGL && !UNITY_EDITOR
                        JsPlugins.WebGLUtils.Redirect("https://1games.io/");
        #endif
    }
}
