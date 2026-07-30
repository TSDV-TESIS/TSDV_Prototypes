using CameraScripts;
using Events;
using Events.Scriptables;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject goBackButton;
    [SerializeField] private GameObjectEventChannelSO onNewSelected;
    [SerializeField] private RectTransform creditsImage;
    [SerializeField] private float panningDuration;
    [SerializeField] private Vector2 verticalMoveRateRange;
    [SerializeField] private Vector2 verticalMoveRange;
    [SerializeField] private float verticalSpeed;

    private float width;
    private bool isGoingRight = true;
    private bool isGoingUp = true;
    private float panningTimer;
    private float verticalTimer;
    private float verticalDuration;

    private Vector2 range;

    private void OnEnable()
    {
        onNewSelected?.RaiseEvent(goBackButton);
        width = creditsImage.rect.width;
        range = new Vector2(width / 4, -width / 4);
        panningTimer = 0;
        verticalTimer = 0;
        verticalDuration = Random.Range(verticalMoveRateRange.x, verticalMoveRateRange.y);
    }

    private void Update()
    {
        CreditsMove();
    }

    private void CreditsMove()
    {
        panningTimer += Time.deltaTime;
        verticalTimer += Time.deltaTime;

        float xPos = isGoingRight ? Mathf.Lerp(range.x, range.y, panningTimer / panningDuration) : Mathf.Lerp(range.y, range.x, panningTimer / panningDuration);
        float yPos = isGoingUp ? creditsImage.anchoredPosition.y + verticalSpeed : creditsImage.anchoredPosition.y - verticalSpeed;

        creditsImage.anchoredPosition = new Vector2(xPos, Mathf.Clamp(yPos, verticalMoveRange.x, verticalMoveRange.y));

        if (panningTimer >= panningDuration)
        {
            panningTimer = 0;
            isGoingRight = !isGoingRight;
        }

        if (verticalTimer >= verticalDuration)
        {
            verticalTimer = 0;
            verticalDuration = Random.Range(verticalMoveRateRange.x, verticalMoveRateRange.y);
            isGoingUp = !isGoingUp;
        }
    }

    public void OnDisable()
    {
        onNewSelected?.RaiseEvent(creditsButton);
    }
}