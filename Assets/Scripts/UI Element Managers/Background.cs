using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


using Hex = UnityEngine.GameObject;

public class Background : MonoBehaviour
{

    [SerializeField] private Hex hexagonObject;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Color accentColor1;
    [SerializeField] private Color accentColor2;
    [SerializeField] private Color accentColor3;
    private List<Hex> hexagonList = new();
    private List<SpriteRenderer> innerList = new();
    private List<LineRenderer> outerList = new();
    private List<Vector3> targetPositions = new();
    private List<Vector3> initialPositions = new();

    private bool inwards = true;
    private float time = 0;
    private float max_time = 2;
    private int numHex = 0;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        background.color = new(accentColor3.r, accentColor3.g, accentColor3.b);

        // Hexgrid
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                targetPositions.Add(new(0.95f * (x + ((y % 2 == 0) ? 0.5f : 0)) - 3 * 0.95f, 0.825f * (y - 3) - 3 * 0.825f));
                initialPositions.Add(new(Random.Range(-10.0f, 10.0f), Random.Range(0.0f, 10.0f)));
                AddHex(new(initialPositions.Last().x, initialPositions.Last().y), Random.Range(-45.0f, 45.0f));
            }
        }
        numHex = hexagonList.Count;
    }

    private void AddHex(Vector3 position, float rotation, float size = 1)
    {
        hexagonList.Add(Instantiate(hexagonObject, position, Quaternion.Euler(0, 0, rotation)));
        hexagonList[0].transform.localScale = new Vector3(1, 1);
        innerList.Add(hexagonList.Last().GetComponentInChildren<SpriteRenderer>());
        outerList.Add(hexagonList.Last().GetComponentInChildren<LineRenderer>());
    }

    private void SetLineColor(LineRenderer line, Color c)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new(new Color(c.r, c.g, c.b), 0.0f), new(new Color(c.r, c.g, c.b), 1.0f) },
            new GradientAlphaKey[] { new(1.0f, 0.0f), new(1.0f, 1.0f) }
        );
        line.colorGradient = gradient;
    }

    [System.Obsolete] // needed for "SetColors" because the new way to set colors is stupidly complicated. Worst interface ever.
    void Update()
    {
        float delta = Time.deltaTime;
        time += delta;
        float relativeTimeLeft = time / max_time;
        relativeTimeLeft = relativeTimeLeft < 0 ? 0 : relativeTimeLeft > 1 ? 1 : relativeTimeLeft;

        // For dynamic Colors
        var innerEnum = innerList.GetEnumerator();
        var outerEnum = outerList.GetEnumerator();
        int index = 0;
        while (innerEnum.MoveNext() && outerEnum.MoveNext())
        {
            float mix = (float)index / numHex;
            float mixSinus = Mathf.Pow(Mathf.Sin(index + time) / 2 + 0.5f, 4.0f) / 10;
            SpriteRenderer inner = innerEnum.Current;
            LineRenderer outer = outerEnum.Current;
            inner.color = new Color(
                (1 - mixSinus) * ((1 - mix) * accentColor1.r + mix * accentColor3.r) + mixSinus * accentColor2.r,
                (1 - mixSinus) * ((1 - mix) * accentColor1.g + mix * accentColor3.g) + mixSinus * accentColor2.g,
                (1 - mixSinus) * ((1 - mix) * accentColor1.b + mix * accentColor3.b) + mixSinus * accentColor2.b
                );
            SetLineColor(outer,
                new Color(
                (1 - mixSinus) * ((1 - mix) * accentColor2.r + mix * accentColor3.r) + mixSinus * accentColor1.r,
                (1 - mixSinus) * ((1 - mix) * accentColor2.g + mix * accentColor3.g) + mixSinus * accentColor1.g,
                (1 - mixSinus) * ((1 - mix) * accentColor2.b + mix * accentColor3.b) + mixSinus * accentColor1.b
                )
                );
            index++;
        }

        // For moving the hexahedra
        if (time / max_time < 1.5) // Only until everything is sorted out.
        {
            var hexEnum = hexagonList.GetEnumerator();
            var targetPositionEnum = targetPositions.GetEnumerator();
            var initialPositionEnum = initialPositions.GetEnumerator();
            float scaledRelativeTimeLeft = relativeTimeLeft * relativeTimeLeft;
            if (!inwards) scaledRelativeTimeLeft *= -1;
            float invScaledRelativeTimeLeft = 1 - scaledRelativeTimeLeft;
            while (hexEnum.MoveNext() && targetPositionEnum.MoveNext() && initialPositionEnum.MoveNext())
            {
                Hex hex = hexEnum.Current;
                Vector3 targetPos = targetPositionEnum.Current;
                Vector3 initialPos = initialPositionEnum.Current;
                hex.transform.SetPositionAndRotation(
                    new Vector3(
                        invScaledRelativeTimeLeft * initialPos[0] + scaledRelativeTimeLeft * targetPos[0],
                        invScaledRelativeTimeLeft * initialPos[1] + scaledRelativeTimeLeft * targetPos[1]),
                    Quaternion.Euler(0, 0,
                        relativeTimeLeft < 1 ? hex.transform.rotation.eulerAngles[2] + 100 * delta : 0
                    )
                );
            }
        }


    }
}
