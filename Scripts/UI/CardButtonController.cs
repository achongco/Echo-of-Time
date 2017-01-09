using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CardButtonController : MonoBehaviour, IPointerClickHandler{

    public int index;

    DeckUIControl dc;
    public bool isDeckCard;     //Determines if it is a deck card or a library card

    void Awake(){
        dc = GameObject.FindGameObjectWithTag("GameController").GetComponent<DeckUIControl>();
    }

    public void OnPointerClick(PointerEventData eventData){
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CardButtonScript cbs;
            if (isDeckCard)
            {
                cbs = dc.deckCards[index].GetComponent<CardButtonScript>();
            }
            else
            {
                cbs = dc.libCards[index].GetComponent<CardButtonScript>();
            }
            dc.DisplayPreview(cbs.card.name);   //REPLACE THIS WITH PROPER DATA FOR CARDS
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click received");
            if (isDeckCard)
                dc.RemoveDeckCard(index);
            else
                dc.RemoveLibraryCard(index);
        }
    }
}
