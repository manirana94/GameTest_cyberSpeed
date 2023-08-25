using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [Header("Reference to Objects")] public GameObject cardPrefab; // Reference to the card prefab
    public Transform cardsParent; // All cards parents

    [Header("Set Col And Row")] public int gridSizeX = 4; // Number of columns
    public int gridSizeY = 4; // Number of rows

    private List<int> StoringCardValues; // List to store card values
    public List<Card> StoringOpenCards; // List to store currently open cards

    //Used to set GridLayoutGroup both Horizontal and Vertical constraints manually
    private Vector2 horizontalScale = new Vector2(1f, 1f); // Horizontal scaling factor (width)
    private Vector2 verticalScale = new Vector2(1f, 1f); // Vertical scaling factor (height)


    private List<List<Card>> listoflist = new List<List<Card>>();

    public static bool
        ReadyToStart = true; //Will control that use will not be able to play during we are showing user card front side

    private void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        StoringCardValues = GenerateCardValues();
        StoringOpenCards = new List<Card>();

        //Will set GridLayout constraints in this function
        SetGridLayoutConstraints();

        // ShuffleCards function will return the card values after shuffle
        List<int> shuffledValues = ShuffleCards(StoringCardValues);

        // Creating and placing all cards in the grid
        Enumerable.Range(0, gridSizeY)
            .SelectMany(y => Enumerable.Range(0, gridSizeX)
                .Select(x =>
                {
                    int index = y * gridSizeX + x;
                    int cardValue = shuffledValues[index];


                    GameObject cardObject = Instantiate(cardPrefab, cardsParent);
                    Card card = cardObject.GetComponent<Card>();

                    card.Initialize(cardValue, OnCardClicked);

                    // Positioning the cards in the grid
                    cardObject.transform.localPosition = new Vector3(x, -y, 0);

                    // Applying scaling to the grid element's RectTransform
                    RectTransform elementRect = cardObject.GetComponent<RectTransform>();
                    elementRect.sizeDelta = new Vector2(elementRect.sizeDelta.x * horizontalScale.x,
                        elementRect.sizeDelta.y * verticalScale.y);

                    return card;
                }))
            .ToList(); // This ToList() call ensures that the cards are instantiated
        StartCoroutine(DisableCardsAgain());
    }

    /// <summary>
    /// This function used to flip card back after showing for few seconds
    /// </summary>
    /// <returns></returns>
    public IEnumerator DisableCardsAgain()
    {
        yield return new WaitForSeconds(2f);

        foreach (Transform cardsIn in cardsParent)
        {
            cardsIn.GetComponent<Card>().FlipBackCard();
            cardsIn.GetComponent<Card>().CardAnimation();
        }

        ReadyToStart = false;
    }

    List<int> GenerateCardValues()
    {
        List<int> values = new List<int>();

        int numUniqueCards = gridSizeX * gridSizeY / 2;

        for (int i = 0; i < numUniqueCards; i++)
        {
            values.Add(i);
            values.Add(i);
        }

        return values;
    }

    List<int> ShuffleCards(List<int> values)
    {
        List<int> shuffledValues = new List<int>(values);

        for (int i = 0; i < shuffledValues.Count; i++)
        {
            int temp = shuffledValues[i];
            int randomIndex = Random.Range(i, shuffledValues.Count);
            shuffledValues[i] = shuffledValues[randomIndex];
            shuffledValues[randomIndex] = temp;
        }

        return shuffledValues;
    }

    public void SetGridLayoutConstraints()
    {
        GridLayoutGroup gridLayout = cardsParent.GetComponent<GridLayoutGroup>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayout.constraintCount = gridSizeY;

        // Set the start axis to horizontal or vertical based on user-defined scaling
        if (horizontalScale.x != 1f)
        {
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridSizeX;
        }
        else if (verticalScale.y != 1f)
        {
            gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gridLayout.constraintCount = gridSizeY;
        }
    }


//This function will be called in the call back of card button click
    void OnCardClicked(Card card)
    {
        card.ShowCard();
        StoringOpenCards.Add(card);
        if (StoringOpenCards.Count == 2)
        {
            listoflist.Add(new List<Card>(StoringOpenCards));
            StoringOpenCards.Clear();
            foreach (List<Card> cardPair in listoflist)
            {
                bool isMatch = cardPair[0].Value == cardPair[1].Value;

                if (isMatch)
                {
                    // Match found
                    StartCoroutine(HideMatchedCards(cardPair[0], cardPair[1]));
                }
                else
                {
                    // No match
                    StartCoroutine(FlipBackCards(cardPair[0], cardPair[1]));
                }
            }

            listoflist.RemoveAt(0);
        }
    }


    private IEnumerator HideMatchedCards(Card card1, Card card2)
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.PlayMatchedSound();
        card1.HideCard();
        card2.HideCard();

        // Check if all cards have been matched
        if (AllCardsMatched())
        {
            GameManager.Instance.WinTextPanel.SetActive(true);
            GameManager.Instance.PlayCoinsSound();
            GameManager.Instance.ScoreText.GetComponent<ZoomAnimation>().enabled = true;
            GameManager.Instance.AddScore(50);

            Debug.Log("You've matched all cards! Victory!");
        }
    }

    bool AllCardsMatched()
    {
        foreach (Transform cardTransform in cardsParent)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (card != null && !card.IsMatched)
            {
                return false;
            }
        }

        // If all cards are matched, return true
        return true;
    }

    private IEnumerator FlipBackCards(Card card1, Card card2)
    {
        yield return new WaitForSeconds(1f);


        GameManager.Instance.PlayNotMatchedSound();
        card1.FlipBackCard();
        card2.FlipBackCard();
    }
}