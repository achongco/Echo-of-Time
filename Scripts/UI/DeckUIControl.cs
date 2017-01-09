using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class DeckUIControl : MonoBehaviour {

    //Static Variables
    private static float CARD_BUTTON_HEIGHT = 40f;              //Height of card buttons
    private static float PANEL_HEIGHT = 500f;                   //Height of scroll view panels
    private static int MAX_CARDS_IN_DECK = 54;                  //Max amount of cards allowed in deck
    private static int MIN_CARDS_IN_DECK = 32;                  //Min amount of cards allowed in deck
    private static int MAX_CARDS_IN_LIB = 500;                  //Max amount of cards allowed in library

    //Deck Submenu Interface
    public GameObject cardButton;                               //Prefab button for cards
    public GameObject cardPreview;                              //Preview of card
    public RectTransform deckContentPanel;                      //For adjusting size of scrollable deck window
    public RectTransform libraryContentPanel;                   //For adjusting size of scrollable library window

    public bool loadFromPrefab;
    public PremadeDeck premadeDeck;
    public GameObject[] deckCards = new GameObject[MAX_CARDS_IN_DECK]; //Stores information of deck buttons
    public GameObject[] libCards = new GameObject[MAX_CARDS_IN_LIB];   //Stores information of library cards
    public ArrayList playerDeckCards = new ArrayList();
    public ArrayList libCardsPrefabs = new ArrayList();
    
    //Private Variables
    private int deckSize;                                       //Number of cards in deck
    private int librarySize;                                    //Number of cards in library

    void Awake(){
        loadPrefabAssets();

     
    }

    void Start(){
        loadPlayerAssets();
        PopulateDeckScroll();
        PopulateLibraryScroll();
        UpdateAllButtonsInfo();
        
    }
   
   /*
   * DECK UI MANAGEMENT FUNCTIONS 
   */

    //Adjusts Deck Panel Size
    void SetPanelSize(float size, RectTransform panel){
        if(size>PANEL_HEIGHT)
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        else
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PANEL_HEIGHT);
    }

    //Generates all buttons that can possibly exist for deck so Destroy() never needs to get called
    void GenerateCardButtons(int max, RectTransform parentObject, GameObject[] cardArray, bool onLeft){
        //Debug.Log(cardArray.Length);
        for (int i = 0; i < cardArray.Length; i++){
            GameObject button = Instantiate(cardButton) as GameObject;
            button.transform.SetParent(parentObject);
            button.transform.localScale = new Vector3(1, 1, 1);
            //int captured = i;
            button.GetComponent<CardButtonController>().isDeckCard = onLeft;
            button.GetComponent<CardButtonController>().index = i;    //DELETE THIS LATER. JUST FOR TEST PURPOSES.
          
            cardArray[i] = button;
            cardArray[i].SetActive(false);
        }
    }

    //Updates UI Display
    void UpdateAllButtonsInfo()
    {
        deckSize = playerDeckCards.Count;
        librarySize = libCardsPrefabs.Count;
        UpdateButtonSide(deckCards,playerDeckCards, deckSize, MAX_CARDS_IN_DECK);
        UpdateButtonSide(libCards, libCardsPrefabs, librarySize, MAX_CARDS_IN_LIB);
    }

    /*
     * DECK MANAGEMENT FUNCTIONS 
     */

    void RemoveCard(ref int cardCount, ref int otherCount, ref RectTransform contentPanel, ref RectTransform otherPanel){
        
        UpdateAllButtonsInfo();
    }

    /*
     * XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
     * RELEVANT TO KIRSTEN STUFF MAYBE?!?
     * XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
     */

    //NOTES: Check static variables for min/max. Currently set to 32/54
    //Max cards in library is an arbitrary number
    
    public void loadPlayerAssets()
    {
        BNDeck deck = GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>();
        if(loadFromPrefab)
        {
            deck.initwithCards(premadeDeck.deckCards);
            playerDeckCards.AddRange(premadeDeck.deckCards);
        }else
        {
            deck.initwithCards(deckCards);
            playerDeckCards.AddRange(deckCards);
        }
        
        //GameObject[] playerDeck = deck.playerDeck.ToArray(typeof(GameObject)) as GameObject[];

        
        
    }
    public void loadPrefabAssets()
    {
        GameObject[] cardPrefabs = Resources.LoadAll<GameObject>("Cards") ;
        libCardsPrefabs.AddRange(cardPrefabs);
    }

    //Display image of card and flavor text in center of screen
    public void DisplayPreview(string flavorText, Image img = null)
    {
        //cardPreview.GetComponentInChildren<Image>() = 
        cardPreview.GetComponentInChildren<Text>().text = flavorText;
    }

    //Possibly retrieve data from CardButtonController like the index if you're making the deck an array/list
    //Remove card from deck
    public void RemoveDeckCard(int index)
    {
        Debug.Log("Remove deck card");


        BNDeck deck = GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>();
        deck.removeCardFromDeck(index);
        playerDeckCards = new ArrayList(deck.getPlayerDeck());
        RemoveCard(ref deckSize, ref librarySize, ref deckContentPanel, ref libraryContentPanel);
        if (deckSize > MIN_CARDS_IN_DECK){
            RemoveCard(ref deckSize, ref librarySize, ref deckContentPanel, ref libraryContentPanel);
        }
    }
    void addToPlayerDeck(GameObject card)
    {
        if(GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>().addCardToDeck(card))
        {
            BNDeck deck = GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>();

            playerDeckCards = new ArrayList(deck.getPlayerDeck());
        }
        UpdateAllButtonsInfo();
        
    }

    //Remove card from library
    public void RemoveLibraryCard(int index)
    {
        Debug.Log("Remove Library Card");
        GameObject card = libCards[index];
        GameObject cardToAdd = card.GetComponent<CardButtonScript>().card;
        addToPlayerDeck(cardToAdd);
    }

    //Set attributes for card buttons. This should be called when card list is updated to update the button appearance
    void SetCardButtonAttr(GameObject button, GameObject card)
    {
        CardButtonScript cbs = button.GetComponent<CardButtonScript>();
        cbs.card = card;
        cbs.cardName.text = cbs.card.name; //Access cbs CardName to set name
        //cbs.damage.text = cbs.card;
                //cbs.cardImage = Image... Whenever we get images
                 cbs.card = card;
    }

    //Maybe add argument for deck/library cards array
    void UpdateButtonSide(GameObject[] cardButtonArray, ArrayList prefabArray, int cardCount, int max)
    {
        for (int i = 0; i < cardCount; i++)
        {
            cardButtonArray[i].SetActive(true);
            SetCardButtonAttr(cardButtonArray[i], (GameObject)prefabArray[i]);
        }
        /*
        for (int i = cardCount; i < max; i++)
        {
            cardButtonArray[i].SetActive(false);
        }*/
       
        SetPanelSize((float)deckSize * CARD_BUTTON_HEIGHT, deckContentPanel);
        SetPanelSize((float)librarySize * CARD_BUTTON_HEIGHT, libraryContentPanel);
    }

 

    //Populates deck scrolling panel with buttons for cards
    void PopulateDeckScroll()
    {
        GenerateCardButtons(MAX_CARDS_IN_DECK, deckContentPanel, deckCards, true);
        int cards_in_deck;
        //Temporarily commented out since there's no Player GameObject in scene
        
        BNDeck deck = GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>();

        if (deck.playerDeck == null)
        {
            cards_in_deck = 0;
            deckSize = 0;
        }
        else
        {
            cards_in_deck = deck.playerDeck.Count;
            deckSize = cards_in_deck;
        }
       
        for (int i = 0; i < cards_in_deck; i++)
        {
            //GameObject card = (GameObject)deck.playerDeck[i];
            //SetCardButtonAttr(deckCards[i], card);
        }
        deckSize = cards_in_deck;
        
        deckSize = 50;
        librarySize = 0;
        //END FILLER. DELETE WHEN DONE

        //deckSize = deckList Length??
        //SetCardButtonAttr call?
    }

    //Populates library scrolling panel with buttons for cards
    void PopulateLibraryScroll()
    {
        GenerateCardButtons(MAX_CARDS_IN_LIB, libraryContentPanel, libCards, false);
       
    }


    //DELETE LATER. FOR TESTING ONLY

    public void TEMP_StartBattle(){
        //Application.LoadLevel(1);
    }
}
