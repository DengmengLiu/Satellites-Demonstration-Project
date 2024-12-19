using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeEffect : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 2f;   // 淡入/淡出时间，单位为秒
    public bool enableFadeOut = false; // 是否启用淡出

    private TextMeshProUGUI tmpText;   // TextMeshPro 组件引用
    private Coroutine fadeCoroutine;   // 记录当前的协程

    void Awake()
    {
       
    }

    void OnEnable()
    {
        // 获取 TextMeshPro 组件
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
        // 开始淡入效果
        StartFade();
    }

    public void StartFade()
    {
        // 停止任何正在进行的淡入/淡出协程
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 开始淡入淡出效果
        fadeCoroutine = StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        // 淡入过程
        yield return StartCoroutine(Fade(0f, 1f));

        // 如果启用了淡出，进行淡出
        if (enableFadeOut)
        {
            yield return StartCoroutine(Fade(1f, 0f));
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = tmpText.color;

        while (elapsedTime < fadeDuration)
        {
            // 计算当前透明度
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            tmpText.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终的透明度精确设置
        color.a = endAlpha;
        tmpText.color = color;
    }
}
