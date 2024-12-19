using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    private PlayableDirector director;
    public GameObject controlPanel;

    void Awake() {
    director = GetComponent<PlayableDirector>();
    director.played += Director_Played;
    director.stopped += Director_Stopped;
    
    if (controlPanel == null) {
        Debug.LogError("controlPanel is not assigned");
    }
}
    private void Director_Played(PlayableDirector obj)
{
    Debug.Log("Timeline Played");
    SetControlPanelVisibility(false);
}

private void Director_Stopped(PlayableDirector obj)
{
    Debug.Log("Timeline Stopped");
    SetControlPanelVisibility(true);
}
    public void SetControlPanelVisibility(bool visible)
{
    CanvasGroup canvasGroup = controlPanel.GetComponent<CanvasGroup>();
    if (canvasGroup != null)
    {
        canvasGroup.alpha = visible ? 1 : 0; // 设置透明度
        canvasGroup.interactable = visible;  // 控制是否可交互
        canvasGroup.blocksRaycasts = visible; // 控制是否能点击
    }
    else
    {
        controlPanel.SetActive(visible); // 如果没有CanvasGroup，仍然使用SetActive
    }
}

   public void StartTimeline()
{
    Debug.Log("StartTimeline called");
    director.Play();
}
}
