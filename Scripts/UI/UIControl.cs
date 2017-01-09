using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{

    //Public Variables
    public Image TimeBar;                   //Bar that fills as time goes by. Stops once player is able to access 'deck'
    public GameObject PauseUI;				//Image and/or text that appears when game is paused
    public GameObject DeckMenuUI;           //Menu interface when player accesses deck in battle
    public GameObject[] QueueButtons;       //For controlling when they appear and don't appear
    public GameObject[] QueuePanels;        //For displaying what is currently queued 
    public GameObject[] CardButtons;        //For accessing card buttons
    public GameObject Mana;
    public GameObject Queue;
    public GameObject player;
    public GameObject battleUI;
    public GameObject dialogueUI;
    public GameObject combatHUD;
    //External scripts 
    SceneFlowController sceneFC;


    //Private Variables
    private bool isPaused = false;			//Determine if game is paused or not
    private bool inDeckMenu = false;        //Determine if player is going through deck menu and pauses game accordingly
    private bool deckIsHidden = true;       //Determine if Deck Menu is hidden while accessable
    private int cardsInHand = 0;            //Counts number of cards in hand
    private int examineIndex = 0;           //Keeps track of what card is being examined
    public GameObject[] queuedCards;        //Array of cards that were queued
    private Stack queuedIndex;//Keeps index of queued cards 
    Dictionary<int, int> queueMap;
    private int queueIndex = 0;
    private CardCombo combo;
    /*
     * CHANGE FOR TUNING OR BALANCE
     */
    static float FILL_TIME = .002f;         //Set amount to fill time bar per FixedUpdate call

    public int MAX_IN_HAND = 10;            //Max amount of cards allowed in hand
    static int MAX_QUEUE = 5;               //Max amount of cards allowed in queue
    public int MAX_BASIC = 4;

    KeyCode pauseKey = KeyCode.Return;      //Sets key to pause the game
    KeyCode deckAccessKey = KeyCode.Z;		//Sets key to access the player's deck

    //Navigation Keys
    KeyCode leftNav = KeyCode.LeftArrow;    //Navigates left in the Deck Interface
    KeyCode rightNav = KeyCode.RightArrow;  //Navigates right in the Deck Interface
    KeyCode altLeftNav = KeyCode.A;         //Alternate button for left navigation
    KeyCode altRightNav = KeyCode.D;        //Alternate button for right navigation

    //Selection Keys
    KeyCode selectKey = KeyCode.C;          //Selection key while in Menu
    KeyCode deselectKey = KeyCode.X;        //Deselects first queued card

    //Action Key
    KeyCode playCard = KeyCode.T;

    // Use this for initialization
    void Awake()
    {
        ClearCardList();
        Reset();
    }

    void Start()
    {
        dialogueUI = GameObject.FindGameObjectWithTag("UIController");
        sceneFC = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneFlowController>();
        //LoadCards();
        //Time.timeScale = 0.0f;
        //SetDeckState(true);
        //ShiftRemainingCards();
    }

    void Update()
    {
        HealthScript character = player.GetComponent<HealthScript>();
        Mana.GetComponent<Text>().text = character.currentMana.ToString();
        WatchForInputs();
        HighlightSelected();
        if (character.health <= 0)
        {
            Reset();
            character.health = character.maxhealth;
            character.currentMana = character.maxMana;
            ClearCardList();
            
            sceneFC.NextSequence();
            Destroy(GameObject.FindGameObjectWithTag("Board"));
        }

        if (Input.GetKeyDown(playCard))
        {
            ParseCards();
        }

    }

    void FixedUpdate()
    {
        FillTimeBar();
    }


    //Resets all UI information
    void Reset()
    {
        InitalizeVar();
        DeactivateInterface();
        DeactivateButtons();
        resetMana();
    }

    void RemoveNonBasics()
    {
        //if (queueIndex < 1)
        //    return;

        //string cardName = queuedCards[queueIndex-1].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().cardName;
        //if (!string.Equals(cardName, "Fire") && !string.Equals(cardName, "Earth") && !string.Equals(cardName, "Air") && !string.Equals(cardName, "Water") && !string.Equals(cardName, "Light") && !string.Equals(cardName, "Dark"))
        //{
        //    Debug.Log(cardName);
        //    Debug.Log("Removing nonbasic");
        //    for (int i = 0; i < cardsInHand; i++) 
        //    {
        //        string cardbuttonName = CardButtons[i].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().cardName;
        //        Debug.Log(cardbuttonName);
        //        if (string.Equals(cardName, cardbuttonName))
        //        {
        //            Debug.Log("Setting deactive at " + i);
        //            CardButtons[i].SetActive(false);
        //            CardButtons[i].GetComponent<CardButtonScript>().card = null;
        //            cardsInHand--;
                    
        //        }
        //    }

        //}



    }

    //Initializes variables
    void InitalizeVar()
    {
        queueIndex = 0;
        TimeBar.fillAmount = 0;
        examineIndex = 0;
        Time.timeScale = 1.0f;
        isPaused = false;
        deckIsHidden = true;
        inDeckMenu = false;
    }

    //Clears stack and list of active and queued cards
    void ClearCardList()
    {
        //queuedCards = new GameObject[0];
        //queuedIndex = new Stack();
        //queueIndex = 0;
    }

    void resetMana()
    {

        //HealthScript character = player.GetComponent<HealthScript>();
        //character.currentMana = character.maxMana;
    }

    //Deactivate menu interfaces
    void DeactivateInterface()
    {
        PauseUI.SetActive(false);
        DeckMenuUI.SetActive(false);
    }

    //Deactivates all the card buttons
    void DeactivateButtons()
    {
        //foreach (GameObject button in CardButtons)
        //{
        //    button.GetComponent<Button>().interactable = true;
        //    button.SetActive(false);
        //}
        //foreach (GameObject panel in QueuePanels)
        //    panel.SetActive(false);

    }

    void ReactivateButtons()
    {
        //for(int i = 0; i < cardsInHand; i++)
        //{
        //    CardButtons[i].GetComponent<Button>().interactable = true;
        //    CardButtons[i].SetActive(true);
        //}
        //foreach (GameObject panel in QueuePanels)
        //{
        //    panel.SetActive(false);
        //    panel.GetComponentInChildren<Text>().text = "";
        //}
            

    }

    //Watches for all possible inputs
    void WatchForInputs()
    {
        WatchForPause();
        WatchForToggleView();
        WatchForAccessDeck();
        WatchForKeyNav();
        WatchForKeySelect();
        WatchForKeyDeselect();
    }

    //Sets the state of the Deck Interface to active or not active
    void SetDeckState(bool val)
    {
        //DeckMenuUI.SetActive(val);
        //deckIsHidden = !val;
    }

    //Watches for if player enters pause button and reacts accordingly
    void WatchForPause()
    {
        if (Input.GetKeyDown(pauseKey) && !isPaused)
        {
            Time.timeScale = 0.0f;
            isPaused = true;
            PauseUI.SetActive(true);
        }
        else if (Input.GetKeyDown(pauseKey) && isPaused)
        {
            Time.timeScale = 1.0f;
            isPaused = false;
            PauseUI.SetActive(false);
        }
    }

    //Controls when player can access deck
    void WatchForAccessDeck()
    {
        //if (TimeBar.fillAmount >= 1.0f && !inDeckMenu && Input.GetKeyDown(deckAccessKey))
        //{
        //    AccessDeck();
        //}
    }

    void AccessDeck()
    {
        //inDeckMenu = true;
        //Time.timeScale = 0.0f;
        //ReactivateButtons();
        //SetDeckState(true);
    }

    //Hides deck from view while in Deck Menu Interface
    void WatchForToggleView()
    {
        //if (inDeckMenu && (Input.GetKeyDown(deckAccessKey)) && !deckIsHidden)
        //    SetDeckState(false);
        //else if (inDeckMenu && (Input.GetKeyDown(deckAccessKey)) && deckIsHidden)
        //    SetDeckState(true);
    }

    //Looks out for button keyboard navigation of menu
    void WatchForKeyNav()
    {
        //if (inDeckMenu && (Input.GetKeyDown(rightNav) || Input.GetKeyDown(altRightNav)))
        //{
        //    CheckForNextActive(1);
        //}
        //if (inDeckMenu && ((Input.GetKeyDown(leftNav)) || Input.GetKeyDown(altLeftNav)))
        //{
        //    CheckForNextActive(-1);
        //}
    }

    //Watch for key to select card
    void WatchForKeySelect()
    {

        //if (!deckIsHidden && Input.GetKeyDown(selectKey))
        //{
        //    SelectCard(examineIndex);
        //}
    }

    void WatchForKeyDeselect()
    {
        //if (!deckIsHidden && Input.GetKeyDown(deselectKey))
        //{
        //    DequeueCard();
        //}
    }

    //Controls which card is highlighted. Mostly for keyboard selection.
    void HighlightSelected()
    {
        //if (!deckIsHidden && examineIndex != -1)
        //    CardButtons[examineIndex].GetComponent<Button>().Select();
    }

    //Made public so deck can be closed by button
    public void CloseDeck()
    {
        //Reset();
        //queuedCards = new GameObject[queuedIndex.Count];
        //for (int i = queuedCards.Length - 1; i >= 0; i--)
        //{
        //    int index = (int)queuedIndex.Pop();
        //    queuedCards[i] = CardButtons[index];
        //}



        //if (queueIndex < queuedCards.Length)
        //    Queue.GetComponent<Text>().text = queuedCards[queueIndex].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().cardName;
        //else
        //    Queue.GetComponent<Text>().text = "";
        
    }

    public void checkCombo()
    {
        //queueMap = new Dictionary<int, int>();
        //Debug.Log("Checking combo");
        //if (combo == null)
        //{
        //    combo = new CardCombo();
        //}
        //List<GameObject> cardQueue = deconstuctStack();
        //for (int i = 0; i < cardQueue.Count; i++)
        //{
        //    GameObject card = cardQueue[i];
           
        //    combo.addCombo(card, i);

        //}
        //ReactivateButtons();
        //ComboPoint comboCard = combo.getCombo();

        ////CardButtons[i].SetActive(true); //Necessary Line

        //if (comboCard != null && cardsInHand < 10)
        //{
        //    //Adding combo card
        //    Debug.Log("Add combo card");
            
        //    int newCardIndex = cardsInHand;
        //    //Replace one index with the new card
        //    CardButtons[newCardIndex].GetComponent<Image>().sprite = comboCard.newCard.GetComponent<CardScript>().sprite;
        //    SetCardButtonAttr(CardButtons[newCardIndex], comboCard.newCard);
        //    DisplayCardDetails(CardButtons[newCardIndex]); //Replace string argurment with details from Card
        //    CardButtons[newCardIndex].SetActive(true);
        //    cardsInHand++;
        //    combo.reset();
        //}

        //ClearCardList();
    }

    List<GameObject> deconstuctStack()
    {
        List<GameObject> queuestack = new List<GameObject>();
        
        //for (int i = 0; i < CardButtons.Length; i++)
        //{

        //    if (!CardButtons[i].GetComponent<Button>().interactable)
        //    {
        //        queuestack.Add(CardButtons[i]);
        //        queueMap.Add(queuestack.Count - 1, i);
        //    }
            
        //}

        return queuestack;
    }

    Stack reconstructStack(ArrayList list)
    {
        Stack queuestack = new Stack();
        //for (int i = list.Count - 1; i >= 0; i--)
        //{
        //    queuestack.Push(list[i]);
        //}
        return queuestack;
    }

    //Parse Top Card of Queue
    void ParseCards()
    {
       
        CharacterScript character = player.GetComponent<CharacterScript>();

        if (character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            character.parseCard();
        }
        //if (queueIndex < queuedCards.Length)
        //    Queue.GetComponent<Text>().text = queuedCards[queueIndex].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().cardName;
        //else
        //    Queue.GetComponent<Text>().text = "";

        //RemoveNonBasics();
    }

    //Shows card image in Deck Interface
    public void ExaminedCard(int data)
    {
        //examineIndex = data;
    }

    //Controls filling of time bar
    void FillTimeBar()
    {
        if (TimeBar.fillAmount < 1.0f)
            TimeBar.fillAmount += FILL_TIME;
    }

    //Checks for next active button in given direction
    void CheckForNextActive(int dir)
    {
        //Debug.Log(examineIndex);
        //if (cardsInHand == 0)
        //{
        //    examineIndex = -1;
        //    return;
        //}
        //else if (examineIndex == 0 && dir == 1)
        //{
        //    for (int i = 1; i < CardButtons.Length; i++)
        //        if (!QueuePanels[i].activeSelf)
        //        {
        //            examineIndex = i;
        //            return;
        //        }
        //}
        //else
        //{
        //    for (int i = examineIndex + dir; i != examineIndex; i += dir)
        //    {
        //        //Debug.Log(i);
        //        if (i < 0) { i = CardButtons.Length + dir; }
        //        if (i >= CardButtons.Length) { i = 0; }
        //        if (!QueuePanels[i].activeSelf)
        //        {
        //            examineIndex = i;
        //            return;
        //        }
        //    }
        //}
    }
    

    void ShiftRemainingCards()
    {

        //List<GameObject> remainingCards = new List<GameObject>();
        //if (cardsInHand == 0)
        //    return;

        //for (int i = 0; i < queuedCards.Length; i++)
        //{
        //    queuedCards[i].GetComponent<CardButtonScript>().card = null;
        //}

        //for (int i = 0; i < MAX_BASIC; i++)
        //{
        //    if (CardButtons[i].GetComponent<CardButtonScript>().card != null)
        //        remainingCards.Add(CardButtons[i].GetComponent<CardButtonScript>().card);
        //}


        //for (int i = 0; i < remainingCards.Count; i++)
        //{
        //    CardButtons[i].SetActive(true); //Necessary Line
            
        //    SetCardButtonAttr(CardButtons[i], remainingCards[i]);       //MIGHT BE CAUSING BUG

        //    DisplayCardDetails(CardButtons[i]); //Replace string argurment with details from Card
        //}

        //for (int i = remainingCards.Count; i < MAX_IN_HAND; i++)
        //{
        //    CardButtons[i].GetComponent<CardButtonScript>().card = null;
        //}


    }

    //Activates buttons and associates them with cards in hand.
    //Amount of activated buttons should be dependent on how many cards are in hand
    //Number of cards in hand can be different since hands can range from 0 to 10 cards
    void LoadCards()
    {
        //Reset Mana during this phase
        
        //BNDeck deck = GameObject.FindGameObjectWithTag("PlayerStorage").GetComponent<BNDeck>();
      
        //int cards_in_hand = MAX_BASIC;         
        //for (int i = cardsInHand; i < cards_in_hand; i++)
        //{
        //    GameObject card = deck.drawCard();
        //    CardButtons[i].GetComponent<Image>().sprite = card.GetComponent<CardScript>().sprite;
        //    SetCardButtonAttr(CardButtons[i], card);
        //    DisplayCardDetails(CardButtons[i]); //Replace string argurment with details from Card
        //}
        //cardsInHand = cards_in_hand;


    }


    //Possibly use index to access specific index of list of cards in hand
    //Action performed when card is selected. Index is for the index of the button selected
    public void SelectCard(int index)
    {
        //;
        //if (examineIndex != -1 && queuedIndex.Count < MAX_QUEUE)
        //{
        //    HealthScript character = player.GetComponent<HealthScript>();
        //    if ((character.currentMana - CardButtons[examineIndex].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().manaCost >= 0) && combatHUD.activeSelf)
        //    {
        //        Debug.Log("blah");
        //        character.currentMana -= CardButtons[examineIndex].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().manaCost;
        //        QueuePanels[index].SetActive(true);
        //        QueuePanels[index].GetComponentInChildren<Text>().text = (queuedIndex.Count + 1).ToString();
        //        CardButtons[index].GetComponent<Button>().interactable = false;
        //        queuedIndex.Push(index);
                
        //        CheckForNextActive(1);
        //    }else
        //        Queue.GetComponent<Text>().text = "Not Enough Mana";

        //    if (!combatHUD.activeSelf)
        //    {
        //        Debug.Log("hello");
        //        int textIndex = 10;
        //        DialogueHandler dialogue = dialogueUI.GetComponent<DialogueHandler>();
        //        int currentLine = getCardDialogueLine(CardButtons[examineIndex].GetComponent<CardButtonScript>().card.GetComponent<CardScript>());
                
        //        dialogue.LoadTextAsset(textIndex);
        //        dialogue.setCurrentLine(currentLine);
        //        dialogue.TypingLine();

        //    }
            

        //}
    }

    int getCardDialogueLine(CardScript card)
    {
        int currentLine = 0;
        //Debug.Log(card.cardName);
        //switch(card.cardName)
        //{
            
        //    case ("Air"):
        //        currentLine = 0;
        //        break;
        //    case ("Dark"):
        //        currentLine = 1;
        //        break;
        //    case ("Earth"):
        //        currentLine = 2;
        //        break;
        //    case ("Fire"):
        //        currentLine = 3;
        //        break;
        //    case ("Light"):
        //        currentLine = 4;
        //        break;
        //    case ("Water"):
        //        currentLine = 5;
        //        break;

        //}
        return currentLine;
    }

    //Dequeues card and reactivates last card for interaction
    public void DequeueCard()
    {
        //if (queuedIndex.Count > 0)
        //{
        //    HealthScript character = player.GetComponent<HealthScript>();
        //    character.currentMana += CardButtons[(int)queuedIndex.Peek()].GetComponent<CardButtonScript>().card.GetComponent<CardScript>().manaCost;
        //    cardsInHand++;
        //    int returnedIndex = (int)queuedIndex.Pop();
        //    QueuePanels[returnedIndex].SetActive(false);
        //    CardButtons[returnedIndex].GetComponent<Button>().interactable = true;
        //    if (examineIndex == -1)
        //        examineIndex = returnedIndex;
        //}
    }

    //FOR TESTING PURPOSES: Use this function to change the text of the card to the name of the card
    //Eventually add image functionality and remove name text?
    //Displays image of card
    void DisplayCardDetails(GameObject button)
    {
        //CardButtonScript cbs = button.GetComponent<CardButtonScript>();
        //button.transform.GetChild(0).GetComponent<Text>().text = cbs.card.name;
    }

    //Set attributes for card buttons. This should be called when card list is updated to update the button appearance
    void SetCardButtonAttr(GameObject button, GameObject card)
    {
        //CardButtonScript cbs = button.GetComponent<CardButtonScript>();
        //cbs.card = card;
    }
}
