using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Hex = UnityEngine.GameObject;

public class Background : MonoBehaviour
{

    [SerializeField] private Hex hexagonObject;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Color accentColor1;
    [SerializeField] private Color accentColor2;
    [SerializeField] private Color accentColor3;
    [SerializeField] private Transform parentTransform;
    private List<Hex> hexagonList = new();
    private List<SpriteRenderer> innerList = new();
    private List<LineRenderer> outerList = new();
    private List<float> times = new();
    private List<Vector3> targetPositions = new();
    private List<Vector3> initialPositions = new();

    private bool inwards = true;
    private float outwardsTimeMultiplier = 2.0f;
    private float time = 0;
    private int numHex = 0;
    private int numHexX = 10;
    private int numHexY = 9;

    private float width = 0;
    private float height = 0;
    private float dpiRatio = 0;

    public bool SequenceIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        background.color = new(accentColor3.r, accentColor3.g, accentColor3.b);

        {
            Rect rect = transform.GetComponent<RectTransform>().rect;
            width = rect.width;
            height = rect.height;
            dpiRatio = width / Screen.width;
            print(width);
            print(height);
            print(Screen.width / Screen.height);

        }

        // Hexgrid
        for (int y = 0; y < numHexY; y++)
        {
            float fy = (y) / (float)numHexY;

            for (int x = 0; x < numHexX - ((y % 2 == 0) ? 1 : 0); x++)
            {

                float fx = (x + (y % 2 == 0 ? 0.5f : 0.0f) - 0.5f) / ((float)numHexX - 2);

                targetPositions.Add(new((fx - 0.5f) * numHexX / 2, (fy - 0.5f) * numHexY / 2));
                print(fx);

                //targetPositions.Add(new(
                //    0.95f * (x + ((y % 2 == 0) ? 0.5f : 0)) - 3 * 0.95f, 
                //    0.825f * (y - 3) - 3 * 0.825f)
                //);
                initialPositions.Add(new(RandomWithHole(-10.0f, 10.0f, 2.5f), Random.Range(5.0f, 10.0f)));

                times.Add(Random.Range(0.75f, 1.5f));
                if(inwards)
                    AddHex(new(initialPositions.Last().x, initialPositions.Last().y), Random.Range(-45.0f, 45.0f));
                else
                    AddHex(new(targetPositions.Last().x, targetPositions.Last().y), 0);
            }
        }
        SequenceIsRunning = true;
        numHex = hexagonList.Count;
    }

    // Gets a random number with |random| > minAbsDistanceToZero
    private float RandomWithHole(float min, float max, float minAbsDistanceToZero)
    {
        float r = Random.Range(min, max);
        for(int i = 0; i < 9; i++) // try 10 times
        {
            if (Mathf.Abs(r) < minAbsDistanceToZero)
            {
                r = Random.Range(min, max);
            }
        }
        return r;
    }

    public float TriggerEndSequence()
    {
        inwards = false;
        time = 0;
        SequenceIsRunning = true;
        return 1.5f / outwardsTimeMultiplier;
    }

    private void AddHex(Vector3 position, float rotation)
    {
        hexagonList.Add(Instantiate(hexagonObject, position, Quaternion.Euler(0, 0, 0), parentTransform));
        hexagonList.Last().transform.localScale = new Vector3(1.0f / numHexX, 1.0f / numHexX * 0.5625f);
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
        float delta = Time.deltaTime * (inwards ? 1.0f : outwardsTimeMultiplier);
        time += delta;

        // For dynamic colors
        var innerEnum = innerList.GetEnumerator();
        var outerEnum = outerList.GetEnumerator();
        int index = 0;
        while (innerEnum.MoveNext() && outerEnum.MoveNext())
        {
            float mix = (float)index / numHex; // lerp depending on position
            float mixSinus = Mathf.Pow(Mathf.Sin(index + time) / 2 + 0.5f, 4.0f) / 10; // lerp depending on time
            var c = Color.Lerp(accentColor2, Color.Lerp(accentColor1, accentColor3, mix), 1-mixSinus);
            innerEnum.Current.color = new Color(c.r, c.g, c.b); // MS: idk why i have to do this... But assigning the color directly does not work.
            SetLineColor(outerEnum.Current, Color.Lerp(accentColor1, Color.Lerp(accentColor2, accentColor3, mix), 1 - 2 * mixSinus));
            index++;
        }

        // For moving the hexahedra
        var hexEnum = hexagonList.GetEnumerator();
        var targetPositionEnum = targetPositions.GetEnumerator();
        var initialPositionEnum = initialPositions.GetEnumerator();
        var timeEnum = times.GetEnumerator();
        int numRunningSequences = 0;
        while (hexEnum.MoveNext() && targetPositionEnum.MoveNext() && initialPositionEnum.MoveNext() && timeEnum.MoveNext())
        {
            float relativeTimeLeft = time / timeEnum.Current;
            if (relativeTimeLeft < 1.5) // dont do anything if time is 1.5x over.
            {
                Hex hex = hexEnum.Current;
                Vector3 targetPos = targetPositionEnum.Current;
                Vector3 initialPos = initialPositionEnum.Current;

                if (relativeTimeLeft >= 1.0f) // already arrived at target position.
                {
                    if (!inwards)
                        hex.transform.SetPositionAndRotation(new Vector3(initialPos[0], initialPos[1]), Quaternion.Euler(0, 0, 0));
                    else
                        hex.transform.SetPositionAndRotation(new Vector3(targetPos[0], targetPos[1]), Quaternion.Euler(0, 0, 0));
                }
                else // move to target and rotate
                {
                    numRunningSequences++;
                    float scaledRelativeTimeLeft = Mathf.Pow(relativeTimeLeft, inwards ? 0.5f : 2.0f);
                    if (!inwards) scaledRelativeTimeLeft = 1 - scaledRelativeTimeLeft;
                    hex.transform.SetPositionAndRotation(
                        new Vector3(
                            (1 - scaledRelativeTimeLeft) * initialPos[0] + scaledRelativeTimeLeft * targetPos[0],
                            (1 - scaledRelativeTimeLeft) * initialPos[1] + scaledRelativeTimeLeft * targetPos[1]),
                        Quaternion.Euler(0, 0, relativeTimeLeft < 1 ? hex.transform.rotation.eulerAngles[2] + 100 * delta : 0)
                    );
                }
            }
        }
        if (numRunningSequences == 0) SequenceIsRunning = false;

    }
}
