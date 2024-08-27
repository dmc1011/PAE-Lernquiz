using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SideMenu : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform sideMenuRectTransform;
    [SerializeField] private RectTransform sideMenuGearIconTransform;
    [SerializeField] private CircleCollider2D sideMenuGearIconCollider;
    [SerializeField] private Image sideMenuHandleInner;
    [SerializeField] private Image sideMenuHandleOuter;
    [SerializeField] private Color colorInner;
    [SerializeField] private Color colorOuter;
    [SerializeField] private GameObject headerBorder;
    [SerializeField] private bool hasHeaderBorder;

    // Positioning
    private float width;
    private float startPositionX;
    private float startingAnchoredPositionX;
    private float onScreenPosition;
    private float offScreenPosition;
    private bool isOnScreen = false;
    private bool isDragged = false;
    private bool isGearIconPressed = false;

    // Animation
    private bool isInAnimation = false;
    private float targetPositionForCurrentAnimation = 0.0f;
    private float startPositionForCurrentAnimation = 0.0f;
    private float currentAnimationTime = 0.0f;
    private const float defaultAnimationTimeTarget = 0.4f;
    private float currentAnimationTimeTarget = defaultAnimationTimeTarget;

    // For EvaluationScreens
    private bool isEvaluation = false;
    private bool isHelp = false;
    private bool hasNoGearIcon = true; 

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Evaluation")
        {
            isEvaluation = true;
        }
        if(SceneManager.GetActiveScene().name == "Help")
        {
            isHelp = true;
        }
        hasNoGearIcon = (isHelp || isEvaluation);

        if(!hasHeaderBorder)
            headerBorder.SetActive(false);


        width = Screen.width;
        onScreenPosition = width * 0.5f;
        offScreenPosition = width * 1.5f;
        sideMenuRectTransform.anchoredPosition = new Vector2(offScreenPosition, 0);
    }

    void Update()
    {
        if (isInAnimation)
        {
            currentAnimationTime += Time.deltaTime;
            if (currentAnimationTime < currentAnimationTimeTarget)
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(
                    Mathf.Lerp(startPositionForCurrentAnimation, targetPositionForCurrentAnimation,
                    Mathf.Pow(currentAnimationTime / currentAnimationTimeTarget, 2.0f)), 0);
                Animations();
            } else
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(targetPositionForCurrentAnimation, 0);  // Ensure exact final position
                Animations();
                isInAnimation = false;
                isOnScreen = sideMenuRectTransform.anchoredPosition.x == onScreenPosition;
            }
        }
        else
        {
            if(!isDragged && sideMenuRectTransform.anchoredPosition.x != onScreenPosition && sideMenuRectTransform.anchoredPosition.x != offScreenPosition)
            {
                StartAnimation(isAfterHalfPoint());
            }
        }

        // This fires independent of any raycasts and can replace the "OnPointer" callbacks in a more robust way.
        if (Input.GetButtonUp("Fire1"))
            PointerUp();

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragged)
        {
            startPositionX = eventData.position.x;
            startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;
            isDragged = true;
        }
        else
        {
            isGearIconPressed = false;
            sideMenuRectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x), onScreenPosition, offScreenPosition),
                0);
            Animations();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasNoGearIcon && sideMenuGearIconCollider.OverlapPoint(eventData.position))
        {
            isGearIconPressed = true;
        }
        startPositionX = eventData.position.x;
        startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;
    }

    private void PointerUp()
    {
        isDragged = false;
        if (isGearIconPressed)
        {
            ToggleMenu();
            isGearIconPressed = false;
        }
    }

    private void StartAnimation(bool shouldOpen, bool toggleUsed = false)
    {
        targetPositionForCurrentAnimation = shouldOpen ? onScreenPosition : offScreenPosition;
        startPositionForCurrentAnimation = sideMenuRectTransform.anchoredPosition.x;

        if(toggleUsed)
        {
            currentAnimationTimeTarget = defaultAnimationTimeTarget;
        }
        else
        {
            // Relative distance from the center ( 0 = exactly centered, 1 = exactly at start or end)
            float distanceFromBorders = 2 * Mathf.Abs(startPositionForCurrentAnimation - width) / width;
            currentAnimationTimeTarget = defaultAnimationTimeTarget * Mathf.Pow(1 - distanceFromBorders, 0.5f);
        }
        currentAnimationTime = 0.0f;
        isInAnimation = true;
        isOnScreen = shouldOpen;
    }

    private void Animations()
    {
        if(hasNoGearIcon) // There is no gear icon inside evaluations
        {
            return;
        }

        float t = sideMenuRectTransform.anchoredPosition.x / (Mathf.Abs(onScreenPosition - offScreenPosition)) - 0.5f;
        // Rotate Gear
        sideMenuGearIconTransform.rotation = Quaternion.Euler(0, 0, 360 * t);

        // Dynamically blend inner handle
        sideMenuHandleInner.color = Color.Lerp(colorOuter, colorInner, t);
        sideMenuHandleOuter.color = Color.Lerp(colorOuter, colorInner, t);
    }

    private bool isAfterHalfPoint()
    {
        return sideMenuRectTransform.anchoredPosition.x < width;
    }

    public void ToggleMenu()
    {
        StartAnimation(!isOnScreen, true);
    }
}