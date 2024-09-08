using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer inner;
    [SerializeField] private SpriteRenderer outer;
    [SerializeField] private RectTransform self;
    [SerializeField] private float baseLineThicknessFactor;

    public void SetPos(float x, float y, float angle)
    {
        self.SetLocalPositionAndRotation(new(x, y), Quaternion.Euler(0, 0, angle));
    }

    public void SetLineThickness(float factor)
    {
        float dist = 1 - baseLineThicknessFactor;
        inner.transform.localScale = new(1 - dist * factor, 1 - dist * factor);
    }

    public void SetScale(float size)
    {
        self.localScale = new(size, size, 1);
    }

    public void SetColor(Color innerColor, Color lineColor)
    {
        inner.color = innerColor;
        outer.color = lineColor;
    }

}