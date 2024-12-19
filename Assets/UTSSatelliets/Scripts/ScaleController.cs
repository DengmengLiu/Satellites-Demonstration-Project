using System.Collections;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    // 设置目标缩放值和缩放时间
    public Vector3 targetScale = Vector3.one; // 目标缩放比例
    public float scaleDuration = 1.0f; // 缩放持续时间（秒）

    private Coroutine scalingCoroutine;
    void Start()
    {
        StartScaling();

    }
    /// <summary>
    /// 事件调用的方法，开始缩放
    /// </summary>
    public void StartScaling()
    {
        // 如果有正在运行的缩放协程，先停止
        if (scalingCoroutine != null)
        {
            StopCoroutine(scalingCoroutine);
        }
        // 启动新的缩放协程
        scalingCoroutine = StartCoroutine(ScaleOverTime(targetScale, scaleDuration));
    }

    /// <summary>
    /// 协程：逐渐缩放到目标比例
    /// </summary>
    private IEnumerator ScaleOverTime(Vector3 target, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(initialScale, target, t);
            yield return null; // 等待下一帧
        }

        transform.localScale = target; // 确保最终设置为目标比例
    }
}
