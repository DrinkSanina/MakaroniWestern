using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    bool isFullScreen;
    Resolution[] rsl;
    List<string> resolutions;
    public Dropdown dropdown;

    //Fullscreeen mode on/off
    public void FullScreenToggle()
    { 
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }
    /*-----------------------------------------*/


    //Changing screen resolution
    public void Awake()
    {
        resolutions = new List<string>();
        rsl = Screen.resolutions;
        foreach (var i in rsl)
        {
            resolutions.Add(i.width + "x" + i.height);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(resolutions);
        resolutions.Clear();
    }

    public void Resolution()
    {
        Screen.SetResolution(rsl[dropdown.value].width, rsl[dropdown.value].height, isFullScreen);
    }


}
