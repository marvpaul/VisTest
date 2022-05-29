using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Dynamic;
using System;
using System.IO;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class Visualizer : MonoBehaviour
{

    public Text presetText;

    private AndroidJavaObject visualizerPlugin = null;
    private AndroidJavaObject activityContext = null;
    private AndroidJavaClass activityClass = null;

    public void Awake() {
    StartCoroutine(RequestPermissionsRoutine());        
    if (!Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO") )
    {
        Permission.RequestUserPermission("android.permission.RECORD_AUDIO");
        
    }

    if (!Permission.HasUserAuthorizedPermission("android.permission.MODIFY_AUDIO_SETTINGS"))
    {
        Permission.RequestUserPermission("android.permission.MODIFY_AUDIO_SETTINGS");
    }
    if (!Permission.HasUserAuthorizedPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
    {
        Permission.RequestUserPermission("android.permission.WRITE_EXTERNAL_STORAGE");
    }
    #if UNITY_ANDROID
        activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        visualizerPlugin = new AndroidJavaObject("com.mkgames.visualizer.VisualizerHelper"); 
    #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
#else
    /***** Ready to run you app *****/
#endif
    }

    bool _locationPermissionAsked, _locationPermissionAsked2 = false;

    public IEnumerator RequestPermissionsRoutine()
    {
        #if UNITY_ANDROID
        yield return new WaitForEndOfFrame();
        while (true)
            {
                if (!Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO") && !_locationPermissionAsked)
                {
                    _locationPermissionAsked = true; 
                    Permission.RequestUserPermission("android.permission.RECORD_AUDIO");
                    continue;
                } else {
                    _locationPermissionAsked = true;
                }

                if (!Permission.HasUserAuthorizedPermission("android.permission.MODIFY_AUDIO_SETTINGS") && !_locationPermissionAsked2)
                {
                    _locationPermissionAsked2 = true; 
                    Permission.RequestUserPermission("android.permission.MODIFY_AUDIO_SETTINGS");
                    continue;
                } else {
                    _locationPermissionAsked2 = true;
                }
                if(_locationPermissionAsked && _locationPermissionAsked2 ){
                    break;
                }
        }
        #else
        yield return null;
        #endif
    }

    public float[] spectrum=new float[1024];

    void Update()
    { 
        spectrum = visualizerPlugin.CallStatic<float[]>("getFFT");
        Debug.Log(spectrum.Length);
    }
}
