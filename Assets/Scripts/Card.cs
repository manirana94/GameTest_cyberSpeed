using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is card controller script which will be attached to card which we want to flip
/// </summary>
public class Card : MonoBehaviour
{
    public int Value { get; private set; } //Card Value to match the state
    public bool IsMatched { get; private set; } //Bool to raise win condition at last
    public bool IsFlipped { get; private set; } //bool to check card flip


    private Image cardImage; //Card Image Component Reference (will load dynamically)
    private Sprite CardSpriteFront; //Front image Reference (will load dynamically)
    private Sprite blankSprite; // Blank image Reference (will load dynamically)


    private void Awake()
    {
        //Loading blank image + card image component reference.

        cardImage = GetComponent<Image>();
        blankSprite = Resources.Load<Sprite>("blank");

        if (blankSprite == null)
        {
            Debug.LogError("Blank image not found in Resources.");
        }

        if (cardImage == null)
        {
            Debug.LogError("Card image not set dynamically please set in inspector.");
        }
    }

    //This function used to initialize each card value + images.
    public void Initialize(int value, System.Action<Card> onClickCallback)
    {
        Value = value;
        IsMatched = false;
        IsFlipped = false;

        // Loading the front image dynamically based on cardValue
        int imageIndex = value;
        CardSpriteFront = Resources.Load<Sprite>("image_" + imageIndex);

        if (CardSpriteFront != null)
        {
            // Setting card image to as blank image in start
            // cardImage.sprite = blankSprite != null ? blankSprite : CardSpriteFront;
            cardImage.sprite =  CardSpriteFront;
        }
        else
        {
            Debug.LogError("Card front image not found: image_" + imageIndex);
        }

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            if (!this.IsMatched && !this.IsFlipped && !GridGenerator.ReadyToStart)
            {
                IsFlipped = true;
                cardImage.sprite = CardSpriteFront; // Show the front image
                GameManager.Instance.PlayButtonClickSound();
                onClickCallback?.Invoke(this);
            }
        });
        
        
    }

    /// <summary>
    /// Some basic functions to Show, Hide, Flip the image
    /// </summary>
    public void ShowCard()
    {
        CardAnimation();
        this.IsFlipped = true;
        this.cardImage.enabled = true;
    }

    public void HideCard()
    {
        this.IsFlipped = false;
        this.cardImage.enabled = false;
        SetMatched();
    }

    public void FlipBackCard()
    {
        this.IsFlipped = false;
        this.cardImage.sprite = blankSprite;
    }

    public void SetMatched()
    {
        this.IsMatched = true;
    }

    #region CardZoom

    public float zoomDuration = 2.0f;
    public float zoomScale = 1.1f;
    
    public void CardAnimation()
    {
        
        this.transform.localScale = Vector3.zero;

        this.transform.DOScale(Vector3.one * zoomScale, zoomDuration)
            .SetEase(Ease.OutBack);
    }

    #endregion

    
}