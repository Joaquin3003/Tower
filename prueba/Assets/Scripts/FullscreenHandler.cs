using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class FullscreenHandler : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void ExitFullscreen();
#endif

    public void OnExitFullscreenButton()
    {
    #if UNITY_WEBGL
        ExitFullscreen();
    #endif
    }
}
