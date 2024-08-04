using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnimator : MonoBehaviour
{

    [SerializeField] private Image inner;
    [SerializeField] private Image outline;
    [SerializeField] private Image outlineBlur;

    [SerializeField] private Color innerColor;
    [SerializeField] private Color outlineColor;
    [SerializeField] private Color neonColor;

    private float time = 0;
    private float neonFlicker = 0;
    private float timeForNextFlicker = 0;

    void Start()
    {
        inner.color = new Color(innerColor.r, innerColor.g, innerColor.b);
        outline.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b);
        outlineBlur.color = new Color(neonColor.r, neonColor.g, neonColor.b);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (timeForNextFlicker < time)
        {
            neonFlicker += 1;
            timeForNextFlicker = time + Random.Range(3.0f, 10.0f);
        }
        if (Random.value < 0.05) neonFlicker += 0.2f;
        inner.color = Color.Lerp(innerColor, neonColor, 0.1f * (Mathf.Sin(time / 2) / 2 + 0.5f));
        outline.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, outlineColor.a * (0.8f + 0.2f * (Mathf.Sin(time) / 2 + 0.5f)));
        outlineBlur.color = new Color(neonColor.r, neonColor.g, neonColor.b, neonColor.a * (0.35f + 0.65f * Mathf.Clamp(neonFlicker, 0, 0.5f)));
        neonFlicker *= 0.9f;
    }
}
