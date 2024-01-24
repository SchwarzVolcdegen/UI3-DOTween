using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using DG.Tweening;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public Button playButton;
    public Button pauseButton;
    public Button resetButton;
    public Button rescaleButton;
    public Button randomSpriteButton;
    public Button jumpButton;
    public Button spinButton;
    public Button fadeButton;
    public Button runAllButton;
    public Button animatedButton;
    public GameObject animationControlUI;
    public Vector3 targetScale;
    public Sprite[] sprites;

    [SerializeField] private Button currentActionControlButton;


    [SerializeField] private bool pauseAnimationFlag = false;

    [SerializeField] private bool resetButtonFlag = true;


    [SerializeField] private Quaternion originalRotation;

    [SerializeField] private Vector3 originalScale;

    [SerializeField] private Vector3 originalLocation;

    [SerializeField] private Vector3 highestJumpLocation;

    [SerializeField] private Sprite originalSprite;

    [SerializeField] private Color originalButtonImageColor;
    //private float animationRuntime = 4f;

    [SerializeField] private List<UnityAction> buttonActionListeners = new List<UnityAction>();
    // Start is called before the first frame update


    void OnEnable()
    {
        originalRotation = animatedButton.transform.rotation;
        originalScale = animatedButton.transform.localScale;
        originalSprite = animatedButton.GetComponent<Image>().sprite;
        originalButtonImageColor = animatedButton.GetComponent<Image>().color;
        originalLocation = animatedButton.transform.position;
    }
    void Start()
    {
        AddListenerToButton(playButton);
        AddListenerToButton(pauseButton);
        AddListenerToButton(resetButton);
        AddListenerToButton(rescaleButton);
        AddListenerToButton(randomSpriteButton);
        AddListenerToButton(jumpButton);
        AddListenerToButton(spinButton);
        AddListenerToButton(fadeButton);
        AddListenerToButton(animatedButton);
        AddListenerToButton(runAllButton);
    }

    // Update is called once per frame
    void Update()
    {
        resetButton.interactable = resetButtonFlag;
    }

    void OnDisable()
    {
        RemoveAllListenerOfButton(playButton);
        RemoveAllListenerOfButton(pauseButton);
        RemoveAllListenerOfButton(resetButton);
        RemoveAllListenerOfButton(rescaleButton);
        RemoveAllListenerOfButton(randomSpriteButton);
        RemoveAllListenerOfButton(jumpButton);
        RemoveAllListenerOfButton(spinButton);
        RemoveAllListenerOfButton(fadeButton);
        RemoveAllListenerOfButton(animatedButton);
        RemoveAllListenerOfButton(runAllButton);
    }

    private void ButtonClicked(Button button)
    {
        Debug.Log(button.name + " is Clicked!");

        if (button.gameObject.tag == "Action Control Button")
        {
            currentActionControlButton = button;
            ActiveUI(animationControlUI);
            resetButtonFlag = true;
        }
        else if (button.gameObject.tag == "Animation Control Button")
        {
            switch (button.name)
            {
                case "Play Button":
                    resetButtonFlag = true;
                    pauseAnimationFlag = false;
                    RunAnimation();
                    break;
                case "Pause/Resume Button":
                    pauseAnimationFlag = !pauseAnimationFlag;
                    if (pauseAnimationFlag)
                        DOTween.PauseAll();
                    else
                        DOTween.PlayAll();
                    break;
                case "Reset Button":
                    resetButtonFlag = false;
                    ResetButtonState(animatedButton);
                    break;
            }
        }
        else if (button.gameObject.tag == "Untagged")
        {
            DisableUI(animationControlUI);
        }
    }

    void RunAnimation()
    {
        Debug.Log(currentActionControlButton.name + " animation is playing");

        switch (currentActionControlButton.name)
        {
            case "Scale Button":
                ScalingButton(animatedButton, targetScale, 1f);
                break;
            case "Random Sprite Button":
                ChangingToRandomSprites(animatedButton, sprites, 1f);
                break;
            case "Jump Button":
                JumpButton(animatedButton, 1);
                break;
            case "Spin Button":
                SpinButton(animatedButton, 1);
                break;
            case "Fade Button":
                FadeButton(animatedButton, 1);
                break;
            case "Run All":
                RunAllAnimation(animatedButton);
                break;
        }
    }


    private void ActiveUI(GameObject UI)
    {
        UI.SetActive(true);
        Debug.Log(UI.name + " is active!");
    }

    private void DisableUI(GameObject UI)
    {
        UI.SetActive(false);
        Debug.Log(UI.name + " is disabled!");
    }

    private void AddListenerToButton(Button button)
    {
        UnityAction listener = () => ButtonClicked(button);
        button.onClick.AddListener(listener);
        buttonActionListeners.Add(listener);
    }

    private void RemoveAllListenerOfButton(Button button)
    {
        foreach (var listener in buttonActionListeners)
        {
            button.onClick.RemoveListener(listener);
        }
        buttonActionListeners.Clear();
    }

    private void ResetButtonState(Button button)
    {
        button.transform.rotation = originalRotation;
        button.transform.localScale = originalScale;
        button.GetComponent<Image>().sprite = originalSprite;
        button.GetComponent<Image>().color = new Color(originalButtonImageColor.r, originalButtonImageColor.g, originalButtonImageColor.b);
        button.transform.position = originalLocation;
    }

    private Tween ScalingButton(Button button, Vector3 targetScale, float duration)
    {
        Vector3 originalScale = button.transform.localScale;

        return button.transform.DOScale(targetScale, duration).OnComplete(() => button.transform.DOScale(originalScale, duration));
    }

    private Tween ChangingToRandomSprites(Button button, Sprite[] sprites, float duration)
    {
        int randomIndex = Random.Range(0, sprites.Length);
        Sprite newSprite = sprites[randomIndex];
        Color buttonColor = button.GetComponent<Image>().color;

        return button.GetComponent<Image>().DOBlendableColor(buttonColor, duration)
            .OnComplete(() => button.GetComponent<Image>().sprite = newSprite);
    }

    private Tween JumpButton(Button button, float duration)
    {
        Vector3 originalPosition = button.transform.position;
        highestJumpLocation = originalPosition;
        highestJumpLocation.y = originalPosition.y + 300;

        return button.transform.DOMove(highestJumpLocation, duration).OnComplete(() => button.transform.DOMove(originalPosition, duration));
    }

    private Tween SpinButton(Button button, float duration)
    {
        float totalRotation = 360f; // Assuming you want to spin the object 360 degrees

        return button.transform.DORotate(new Vector3(0, 0, totalRotation), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() => button.transform.rotation = originalRotation);
    }

    private Tween FadeButton(Button button, float duration)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color originalColor = buttonImage.color;

        return buttonImage.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0), duration)
            .SetEase(Ease.Linear).OnComplete(() => buttonImage.DOColor(originalColor, 1));
    }

    private void RunAllAnimation(Button button)
    {

        Sequence sequence = DOTween.Sequence();

        sequence.Append(ScalingButton(button, targetScale, 1))
            .Append(ChangingToRandomSprites(button, sprites, 1))
            .Append(JumpButton(button, 1))
            .Append(SpinButton(button, 1))
            .Append(FadeButton(button, 1));
    }

}
