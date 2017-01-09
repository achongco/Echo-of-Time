using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OverworldUIControl : MonoBehaviour {

    const float NORMAL_ZOOM = 5f;
    const float ZOOM_IN = 1f;

    //Menus
    public GameObject map;
    public GameObject mapReference;         //For reference to nodes
    public GameObject MenuPanel;            //Menu Panel for the overworld
    public GameObject MenuParticles;        //Menu Panel particles for the overworld
    public GameObject MainMenuInterface;    //Main Menu of the overworld Menu
    public GameObject[] SubInterfaces;      //List of subinterfaces
    public GameObject[] SubIndicators;      //Indicates which sub interface the menu is at

    //Inventory Submenu Interfaces
    public GameObject[] itemButtons;        //Item buttons
    public GameObject itemPreview;          //Preview of item with text explanation
    int currentItem = -1;                   //Current Item being inspected

    //Map UI
    public GameObject MapUI;                //Node UI Element
    public Text CityNameText;               //Text of city name
    public GameObject[] NodeButtons;        //Buttons selectable at node locations
    public GameObject[] NodeInterfaces;     //Node interfaces

    //Camera
    public GameObject cam;                      //Scene camera

    //External Scripts
    DialogueHandler dhScript;               //Dialogue handler script

    //Helper Variables
    private int currentSubMenu = 0;
    private bool zoomed = false;
    private bool mapUIActive = false;

    //Controls
    private static KeyCode accessMenu = KeyCode.M;  //Button to access Menu

    void Start()
    {
        dhScript = GetComponent<SceneFlowController>().dialogueScript;
    }

    void Update(){
        WatchForMenuAccess();
    }

    void FixedUpdate(){
        ZoomControl();
    }

    //Activates menu for city locations
    public void ActivateNodeMenu(int index){
        MapUI.SetActive(true);
        CityNameText.text = mapReference.transform.GetChild(index).name;
        mapUIActive = true;
        zoomed = true;
    }

    public void DeactivateNodeMenu(){
        MapUI.SetActive(false);
        mapUIActive = false;
        zoomed = false;
    }

    void ZoomControl()
    {
        //commented out for playtesting purposes
        //if (zoomed){
        //    if (cam.GetComponent<Camera>().orthographicSize > ZOOM_IN && !MenuPanel.activeSelf)
        //        cam.GetComponent<Camera>().orthographicSize -= .5f;
        //}
        //else if (!zoomed && cam.GetComponent<Camera>().orthographicSize < NORMAL_ZOOM){
        //    cam.GetComponent<Camera>().orthographicSize += .5f;
        //}
    }

    void WatchForMenuAccess(){
        //if (!cam.GetComponent<MapCameraController>().GetInBattleState() && Input.GetKeyDown(accessMenu))
        //    if (!MenuPanel.activeSelf) { 
        //        MenuPanel.SetActive(true);
        //        SubInterfaces[currentSubMenu].SetActive(true);
        //        SubIndicators[currentSubMenu].SetActive(true);
        //        MenuParticles.SetActive(true);
        //        MapUI.SetActive(false);
        //        cam.GetComponent<Camera>().orthographicSize = NORMAL_ZOOM;
        //        UpdateItemList();
        //    }
        //    else
        //        ResetMenuStates();
    }

    //Resets Main Menu and Submenu activation states
    void ResetMenuStates(){
        MainMenuInterface.SetActive(true);
        SubInterfaces[currentSubMenu].SetActive(false);
        SubIndicators[currentSubMenu].SetActive(false);
        MenuPanel.SetActive(false);
        MenuParticles.SetActive(false);
        itemPreview.SetActive(false);
        currentItem = -1;
        MapUI.SetActive(mapUIActive);
        currentSubMenu = 0;
        if(zoomed)
            cam.GetComponent<Camera>().orthographicSize = ZOOM_IN;

    }

    /*
     * BUTTON DIRECTORY FUNCTIONS
     */

    //Access Sub Menu
    public void AccessSubMenu(int index){
        GetComponent<AudioSource>().Play();
        SubInterfaces[currentSubMenu].SetActive(false);
        SubIndicators[currentSubMenu].SetActive(false);
        SubIndicators[index].SetActive(true);
        SubInterfaces[index].SetActive(true);
        currentSubMenu = index;
    }

    public void AccessDialogue(int index)
    {
        gameObject.GetComponent<SceneFlowController>().ActivateDialogue(index);
        map.SetActive(false);
    }

    //Item Menu Controls

    public void InspectItem(int index)
    {
        if (index == currentItem)
        {
            itemPreview.SetActive(false);
            currentItem = -1;
        }
        else if(itemButtons[index].transform.GetChild(0).gameObject.activeSelf)
        {
            itemPreview.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = dhScript.GetItemStatus(index).image;
            itemPreview.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = dhScript.GetItemStatus(index).description;

            itemPreview.SetActive(true);
            currentItem = index;
        }
    }

    void UpdateItemList()
    {
        for(int i = 0; i < itemButtons.Length; i++)
        {
            if (dhScript.GetItemStatus(i).unlocked)
            {
                itemButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                itemButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = dhScript.GetItemStatus(i).image;
            }
            else
            {
                itemButtons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }

}
