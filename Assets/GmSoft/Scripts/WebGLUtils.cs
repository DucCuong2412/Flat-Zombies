using UnityEngine;
using System.Runtime.InteropServices;

namespace JsPlugins
{
    public class WebGLUtils
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                [DllImport("__Internal")]
                public static extern void Redirect(string url);
#endif
    }
}