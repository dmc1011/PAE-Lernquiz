using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class HexagonBackground : MonoBehaviour
{

    [SerializeField] private Color accentColor1;
    [SerializeField] private Color accentColor2;
    [SerializeField] private Color accentColor3;
    [SerializeField] private float lineThicknessFactor;
    [SerializeField] private int numX;
    [SerializeField] private Hexagon hexagonPrefab;
    [SerializeField] private RectTransform self;
    [SerializeField] private float animationTimeMin;
    [SerializeField] private float animationTimeMax;

    private List<Hexagon> hexagonList = new();
    private List<Vector3> targetPositions = new();
    private List<Vector3> initialPositions = new();
    private List<float> times = new();

    private int numAll = 0;
    private bool inwards = true;
    private float outwardsTimeMultiplier = 2.0f;
    private float time = 0;
    private int numY = 0;

    public bool sequenceIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        if(self.rect.width > self.rect.height) // widescreen
        {
            numX = Mathf.RoundToInt(numX * self.rect.width / self.rect.height);
        }

        numY = Mathf.RoundToInt(numX * self.rect.height / self.rect.width);

        // Hexgrid
        for (int y = 0; y < numY; y++)
        {
            float fy = (y) / (float)numX; // numX is correct here, because the width is used for scaling the rects.

            for (int x = 0; x < numX - ((y % 2 == 0) ? 1 : 0); x++)
            {

                float fx = (x + (y % 2 == 0 ? 0.5f : 0.0f)) / ((float)numX - 1);

                initialPositions.Add(new(Random.Range(0.0f, 1.0f) > 0.5f ? -self.rect.width : self.rect.width, Random.Range(0, self.rect.height)));
                targetPositions.Add(new((fx - 0.5f) * self.rect.width, (fy - 0.5f * (self.rect.height / self.rect.width)) * self.rect.width));
                
                times.Add(Random.Range(animationTimeMin, animationTimeMax));
                if (inwards)
                    AddHex(new(initialPositions.Last().x, initialPositions.Last().y), Random.Range(-45.0f, 45.0f));
                else
                    AddHex(new(targetPositions.Last().x, targetPositions.Last().y), 0);
            }
        }
        sequenceIsRunning = true;

    }

    private void AddHex(Vector3 position, float rotation)
    {
        hexagonList.Add(Instantiate(hexagonPrefab, position, Quaternion.Euler(0, 0, rotation), self));
        hexagonList.Last().SetScale(self.rect.width / (numX - 1.25f));
        hexagonList.Last().SetLineThickness(lineThicknessFactor);
        hexagonList.Last().SetColor(accentColor1, accentColor2);
        numAll++;
    }

    public float TriggerEndSequence()
    {
        inwards = false;
        time = 0;
        sequenceIsRunning = true;
        return animationTimeMax / outwardsTimeMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime * (inwards ? 1.0f : outwardsTimeMultiplier);
        time += delta;

        var hexEnum = hexagonList.GetEnumerator();
        var targetPositionEnum = targetPositions.GetEnumerator();
        var initialPositionEnum = initialPositions.GetEnumerator();
        var timeEnum = times.GetEnumerator();
        int index = 0;
        int numRunningSequences = 0;

        while (hexEnum.MoveNext() && targetPositionEnum.MoveNext() && initialPositionEnum.MoveNext() && timeEnum.MoveNext())
        {
            // Colors
            float mix = (float)index / numAll;
            float mixSinus = Mathf.Pow(Mathf.Sin(index + time) / 2 + 0.5f, 4.0f) / 10; // lerp depending on time
            var c = Color.Lerp(accentColor2, Color.Lerp(accentColor1, accentColor3, mix), 1 - mixSinus);
            hexEnum.Current.SetColor(c, Color.Lerp(accentColor1, Color.Lerp(accentColor2, accentColor3, mix), 1 - 2 * mixSinus));
            index++;

            // Movement
            float relativeTimeLeft = time / timeEnum.Current;
            if (relativeTimeLeft < 1.5) // dont do anything if time is 1.5x over.
            {
                Hexagon hex = hexEnum.Current;
                Vector3 targetPos = targetPositionEnum.Current;
                Vector3 initialPos = initialPositionEnum.Current;
                
                if (relativeTimeLeft >= 1.0f) // already arrived at target position.
                {
                    if (!inwards)
                        hex.SetPos(initialPos[0], initialPos[1], 0);
                    else
                        hex.SetPos(targetPos[0], targetPos[1], 0);
                }
                else // move to target and rotate
                {
                    numRunningSequences++;
                    float scaledRelativeTimeLeft = Mathf.Pow(relativeTimeLeft, inwards ? 0.5f : 2.0f);
                    if (!inwards) scaledRelativeTimeLeft = 1 - scaledRelativeTimeLeft;
                    hex.SetPos(
                        (1 - scaledRelativeTimeLeft) * initialPos[0] + scaledRelativeTimeLeft * targetPos[0],
                        (1 - scaledRelativeTimeLeft) * initialPos[1] + scaledRelativeTimeLeft * targetPos[1],
                        relativeTimeLeft < 1 ? hex.transform.rotation.eulerAngles[2] + 100 * delta : 0
                    );
                }
            }
            if (numRunningSequences == 0) sequenceIsRunning = false;
        }

    }
}
