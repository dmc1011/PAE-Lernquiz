using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationStatistics : MonoBehaviour
{

    [SerializeField] private Image colorDisplay;
    [SerializeField] private TextMeshProUGUI textLabel;

    [SerializeField] private Color wrongColor;
    [SerializeField] private Color neutralColor;
    [SerializeField] private Color correctColor;

    private bool isInAnimation = false;
    private float currentAnimationTime = 0;
    private const float animationTime = 1.5f;
    private int currentNumAnswers = 0;
    private int currentNumCorrectAnswers = 0;

    private void Update()
    {
        if (isInAnimation)
        {
            float correctPercentage = currentNumCorrectAnswers / (float)currentNumAnswers;
            float t = Mathf.Pow(currentAnimationTime / animationTime, 1.5f);
            Color targetColor = Functions.Lerp3(wrongColor, neutralColor, correctColor, correctPercentage);

            if (currentAnimationTime < animationTime)
            {
                colorDisplay.transform.localScale = new Vector3(1, correctPercentage * t, 1);
                colorDisplay.color = Color.Lerp(wrongColor, targetColor, t);
                textLabel.text = "Richtig: " + Mathf.Round(100 * correctPercentage * t).ToString() + "% (" + Mathf.Round(currentNumCorrectAnswers * t) + "/" + currentNumAnswers + ")";
            }
            else
            {
                colorDisplay.transform.localScale = new Vector3(1, correctPercentage, 1);
                colorDisplay.color = targetColor;
                textLabel.text = "Richtig: " + Mathf.Round(100 * correctPercentage).ToString() + "% (" + currentNumCorrectAnswers + "/" + currentNumAnswers + ")";
                isInAnimation = false;
            }
            currentAnimationTime += Time.deltaTime;
        }
    }

    public void Set(int numAnswers, int numCorrectAnswers)
    {
        currentNumAnswers = numAnswers;
        currentNumCorrectAnswers = numCorrectAnswers;
        isInAnimation = true;
    }
}
