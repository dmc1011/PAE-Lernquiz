using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SideMenu : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform sideMenuRectTransform;
    [SerializeField] private RectTransform sideMenuGearIconTransform;
    [SerializeField] private Image sideMenuHandleInner;
    [SerializeField] private Image sideMenuHandleOuter;
    [SerializeField] private Button sideMenuButton;
    [SerializeField] private Color colorInner;
    [SerializeField] private Color colorOuter;
    [SerializeField] private GameObject headerBorder;
    [SerializeField] private bool hasHeaderBorder;
    [SerializeField] private RectTransform canvasRectTransform;

    // Positioning
    private float width;
    private float startPositionX;
    private float startingAnchoredPositionX;
    private float onScreenPosition;
    private float offScreenPosition;
    private float dpiRatio;
    private bool isOnScreen = false;
    private bool isDragged = false;
    //private bool isGearIconPressed = false;

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

 
        sideMenuRectTransform.sizeDelta = new(canvasRectTransform.sizeDelta.x, canvasRectTransform.sizeDelta.y);
        width = canvasRectTransform.rect.width;
        dpiRatio = width / Screen.width;
        onScreenPosition = -width; // Negativer Wert, da AnchorX = 0 und das Menu ist rechts auﬂerhalb.
        offScreenPosition = onScreenPosition + width;
        sideMenuRectTransform.anchoredPosition = new Vector2(offScreenPosition, 0);

        if(!hasNoGearIcon)
            sideMenuButton.onClick.AddListener(delegate { ToggleMenu(); });
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
            sideMenuRectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x) * dpiRatio, onScreenPosition, offScreenPosition),
                0);
            Animations();
        }
    }

    private void PointerUp()
    {
        isDragged = false;
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
            float distanceFromBorders = (width + startPositionForCurrentAnimation) / width - 0.5f;
            distanceFromBorders = Mathf.Abs(Mathf.Abs(2 * distanceFromBorders) - 1);
            currentAnimationTimeTarget = defaultAnimationTimeTarget * Mathf.Pow(distanceFromBorders, 0.5f);
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

        float t = (width + sideMenuRectTransform.anchoredPosition.x) / width;
        // Rotate Gear
        sideMenuGearIconTransform.rotation = Quaternion.Euler(0, 0, 360 * t);

        // Dynamically blend inner handle
        sideMenuHandleInner.color = Color.Lerp(colorOuter, colorInner, t);
        sideMenuHandleOuter.color = Color.Lerp(colorOuter, colorInner, t);
    }

    private bool isAfterHalfPoint()
    {
        return sideMenuRectTransform.anchoredPosition.x + width < width / 2;
    }

    public void ToggleMenu()
    {
        StartAnimation(!isOnScreen, true);
    }
}