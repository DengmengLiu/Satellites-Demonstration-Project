using System.Collections;
using UnityEngine;

public class AnimatedLine : MonoBehaviour
{
    public Transform startPoint;    // 起点物体
    public Transform endPoint;      // 终点物体
    public float animationDuration = 1f;  // 动画持续时间
    public Material material;

    private LineRenderer lineRenderer;
    private float currentTime = 0f;

    void Start()
    {
        // 获取或添加 LineRenderer 组件
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 初始化 LineRenderer 设置
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.material = material; // 设置默认材质

        // 开始绘制动画
        StartCoroutine(AnimateLine());
    }

    IEnumerator AnimateLine()
    {
        currentTime = 0f;
        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        while (currentTime < animationDuration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / animationDuration);

            // 设置线条的两个点：起点 和 动态计算的中间点
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, Vector3.Lerp(startPos, endPos, t));

            yield return null;
        }

        // 确保线条最终连接到终点
        lineRenderer.SetPosition(1, endPos);
    }
}
