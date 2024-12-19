using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvironmentManager: MonoBehaviour
{
    public Material milkywaySkybox;
    public Material sunnydaySkybox;

    public void SwapSkyboxMilkyWay()
    {
        Debug.Log("milkywaySkybox");
        RenderSettings.skybox = milkywaySkybox;
    }
    public void SwapSkyboxSunnyDay()
    {
        Debug.Log("sunnydaySkybox");
        RenderSettings.skybox = sunnydaySkybox;
    }
}
