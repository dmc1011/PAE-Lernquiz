using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private RectTransform popupTransform;
    [SerializeField] private Image none;
    [SerializeField] private Image bronze;
    [SerializeField] private Image silver;
    [SerializeField] private Image gold;
    [SerializeField] private RectTransform iconContainer;
    [SerializeField] private TextMeshProUGUI title_element;
    [SerializeField] private TextMeshProUGUI text_element;

    enum AnimationMode
    {
        Start, Wait, End, Idle
    }

    public enum Grade
    {
        None, Bronze, Silver, Gold
    }

    AnimationMode animationMode = AnimationMode.Idle;

    Vector3 animation_time = new(1.0f, 5.0f, 1.0f); // Animationtime for "Start", "Wait", "End"
    float currentAnimationPosition = 0;
    private Vector2 onScreenPosition;
    private Vector2 offScreenPositionLeft;
    private Vector2 offScreenPositionRight;
    private bool startAnimation = false;

    private void Start()
    {
        var reference_screen_height = 1920; // Added so the achievement is always in the same place relative to the UI-Box.
        onScreenPosition = new(0, reference_screen_height * 0.5f - popupTransform.rect.height);
        offScreenPositionLeft = new(-Screen.width - popupTransform.rect.width / 2, reference_screen_height * 0.5f - popupTransform.rect.height);
        offScreenPositionRight = new(Screen.width + popupTransform.rect.width / 2, reference_screen_height * 0.5f - popupTransform.rect.height);
        popupTransform.anchoredPosition = offScreenPositionLeft;
        animationMode = AnimationMode.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(startAnimation)
        {
            animationMode = AnimationMode.Start;
            currentAnimationPosition = animation_time[0];
            startAnimation = false;
        }

        if(currentAnimationPosition > 0)
        {
            switch (animationMode)
            {
                case AnimationMode.Start:
                    {
                        float t = 1 - currentAnimationPosition / animation_time[0];
                        popupTransform.anchoredPosition = Vector2.Lerp(offScreenPositionLeft, onScreenPosition, Mathf.Pow(t, 2.5f));
                        currentAnimationPosition -= Time.deltaTime;
                        if (currentAnimationPosition <= 0)
                        {
                            popupTransform.anchoredPosition = onScreenPosition;
                            currentAnimationPosition = animation_time[1];
                            animationMode = AnimationMode.Wait;
                        }
                    }
                    break;

                case AnimationMode.Wait:
                    {
                        float t = 1 - currentAnimationPosition / animation_time[1];
                        float t4 = t * 4;
                        float t10 = t * 10;

                        if (t10 < 1)
                        {
                            // MS: This is the half way between linear and sqrt.
                            // -> cannot use linear because it looks bad
                            // -> cannot use sqrt because of the "jump" for t << 1
                            float t_nonlinear = Mathf.Lerp(t10, Mathf.Pow(t10, 0.5f), Mathf.Pow(t10, 0.5f));
                            iconContainer.rotation = Quaternion.Euler(0, 0, (360.0f / 5.0f) * t_nonlinear);
                        } else
                        {
                            iconContainer.rotation = Quaternion.Euler(0, 0, (360.0f / 5.0f));
                        }

                        if(t4 < 1)
                        {
                            float size = 1 + 0.05f * Mathf.Sin(t4 * Mathf.PI);
                            iconContainer.localScale = new(size, size, size);
                        } else
                        {
                            iconContainer.localScale = new(1, 1, 1);
                        }

                        currentAnimationPosition -= Time.deltaTime;
                        if (currentAnimationPosition <= 0)
                        {
                            iconContainer.rotation = Quaternion.Euler(0, 0, 0);
                            currentAnimationPosition = animation_time[2];
                            animationMode = AnimationMode.End;
                        }
                    }
                    break;

                case AnimationMode.End:
                    {
                        float t = 1 - currentAnimationPosition / animation_time[2];
                        popupTransform.anchoredPosition = Vector2.Lerp(onScreenPosition, offScreenPositionRight, Mathf.Pow(t, 2.5f));
                        currentAnimationPosition -= Time.deltaTime;
                        if (currentAnimationPosition <= 0)
                        {
                            popupTransform.anchoredPosition = offScreenPositionRight;
                            currentAnimationPosition = 0;
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
        startAnimation = true;
    }

    public void SetData(Grade achievementGrade, string title, string text)
    {
        none.gameObject.SetActive(achievementGrade == Grade.None);
        bronze.gameObject.SetActive(achievementGrade == Grade.Bronze);
        silver.gameObject.SetActive(achievementGrade == Grade.Silver);
        gold.gameObject.SetActive(achievementGrade == Grade.Gold);

        if(achievementGrade == Grade.None)
        {
            title_element.color = Color.gray;
            text_element.color = Color.gray;
        }
        title_element.text = title;
        text_element.text = text;
    }

    public void SetData(Achievement achievement)
    {
        SetData(
            achievement.isAchieved ? achievement.grade : Grade.None,
            achievement.name + " " + achievement.grade,
            achievement.description
        );
    }

}
