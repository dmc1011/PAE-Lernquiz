using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private RectTransform popupTransform;
    [SerializeField] private Image bronze;
    [SerializeField] private Image silver;
    [SerializeField] private Image gold;
    [SerializeField] private TextMeshProUGUI title_element;
    [SerializeField] private TextMeshProUGUI text_element;

    enum AnimationMode
    {
        Start, Wait, End, Idle
    }

    AnimationMode animationMode = AnimationMode.Idle;

    Vector3 animation_time = new(1.0f, 5.0f, 1.0f); // Animationtime for "Start", "Wait", "End"
    float current_animation_position = 0;
    private Vector2 onScreenPosition;
    private Vector2 offScreenPositionLeft;
    private Vector2 offScreenPositionRight;

    private void Start()
    {
        onScreenPosition = new(0, Screen.height * 0.4f);
        offScreenPositionLeft = new(-Screen.width, Screen.height * 0.4f);
        offScreenPositionRight = new(Screen.width, Screen.height * 0.4f);
        popupTransform.anchoredPosition = offScreenPositionLeft;
    }

    // Update is called once per frame
    void Update()
    {
        if(current_animation_position > 0)
        {
            switch (animationMode)
            {
                case AnimationMode.Start:
                    {
                        float t = 1 - current_animation_position / animation_time[0];
                        popupTransform.anchoredPosition = Vector2.Lerp(offScreenPositionLeft, onScreenPosition, Mathf.Pow(t, 2.5f));
                        current_animation_position -= Time.deltaTime;
                        if (current_animation_position <= 0)
                        {
                            popupTransform.anchoredPosition = onScreenPosition;
                            current_animation_position = animation_time[1];
                            animationMode = AnimationMode.Wait;
                        }
                    }
                    break;

                case AnimationMode.Wait:
                    {
                        current_animation_position -= Time.deltaTime;
                        if (current_animation_position <= 0)
                        {
                            current_animation_position = animation_time[2];
                            animationMode = AnimationMode.End;
                        }
                    }
                    break;

                case AnimationMode.End:
                    {
                        float t = 1 - current_animation_position / animation_time[2];
                        popupTransform.anchoredPosition = Vector2.Lerp(onScreenPosition, offScreenPositionRight, Mathf.Pow(t, 2.5f));
                        current_animation_position -= Time.deltaTime;
                        if (current_animation_position <= 0)
                        {
                            popupTransform.anchoredPosition = offScreenPositionRight;
                            current_animation_position = 0;
                            animationMode = AnimationMode.Idle;
                            Destroy(gameObject);
                        }
                    }
                    break;
            }

        }
    }

    public void Fire()
    {
        current_animation_position = animation_time[0];
        animationMode = AnimationMode.Start;
    }

    public void SetData(string grade, string title, string text)
    {
        bronze.gameObject.SetActive(grade == "bronze");
        silver.gameObject.SetActive(grade == "silver");
        gold.gameObject.SetActive(grade == "gold");
        title_element.text = title;
        text_element.text = text;
    }

}
