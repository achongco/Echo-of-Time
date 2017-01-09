using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Controls flow of scene. Needs to be active with every scene containing dialogue
//Eventually add more functionality to control scene flow and make more general functions
public class SceneFlowController : MonoBehaviour {

    //EVENTUAL FEATURES
    /*
        -Transition into battle with screen wipe
    */
    public static int maxTime = 3;        //Maximum amount of time units before reset
    public int InGameTime = 0;            //For keeping track of time of day

    public enum SequenceType{
        NONE,           //Normal game state like exploring the overworld
        DIALOGUE,       //For executing dialogue sequence
        BATTLE,         //For executing battle sequence
        ACTIVATE_NODE,  //Activates a node
        MUSIC,          //Set music to be played
        BACKGROUND      //Set background
    }

    //For executing sequences
    [System.Serializable]
    public class Sequence
    {
        public SequenceType type;
        public string parameter;
    }

    //For executing sequences
    [System.Serializable]
    public class SequenceChain
    {
        public string name;             //Name of sequence. Not necessary for coding. Mostly for inspector view
        public bool unlocked;           //Determines if event is unlocked
        public Sequence[] sequences;    //Chain of sequences to play
    }

    //For assigning sequences to cities
    [System.Serializable]
    public class CitySequenceChain
    {
        public string cityName;             //Name of city

        //Chain of events for given time of day
        public SequenceChain[] morning;
        public SequenceChain[] afteroon;
        public SequenceChain[] night;
    }

    public CitySequenceChain[] cityEvents;      //Stores event sequences to run through for city

    //Gameobject activations to be controlled by flow controller    
    public GameObject dialogueUI;
    public GameObject combat;
    public GameObject deckUI;
    public GameObject map;
    public GameObject battleUI;
    public GameObject board;

    //External Scripts
    public DialogueHandler dialogueScript;
    AudioControlScript audioCS;
    OverworldMapControl mapScript;
    MapCameraController camScript;


    //Private variables
    private int currentSequence = 0;                //Keeps track of what sequence should be executed
    private int currentLink = -1;                   //Keeps track of what chain should be executed
    private SequenceChain[] currentChain;           //CurrentChain being examined
    private string bgName = "";                     //Background image to load when dialogue activated
    


    void Awake()
    {
        GetScripts();
        DisableUI();
        UpdateMapNodes();
    }

    void UpdateMapNodes()
    {
        switch (InGameTime)
        {
            case 0:
                for (int i = 0; i < mapScript.CityNodes.Length; i++)
                {
                    if (cityEvents[i].morning.Length > 0 && cityEvents[i].morning[0].unlocked)
                        mapScript.OpenCity(i);
                    else
                        mapScript.CloseCity(i);

                }
                break;
            case 1:
                for (int i = 0; i < mapScript.CityNodes.Length; i++)
                    if (cityEvents[i].afteroon.Length > 0 && cityEvents[i].afteroon[0].unlocked)
                        mapScript.OpenCity(i);
                    else
                        mapScript.CloseCity(i);
                break;
            case 2:
                for (int i = 0; i < mapScript.CityNodes.Length; i++)
                    if (cityEvents[i].night.Length > 0 && cityEvents[i].night[0].unlocked)
                        mapScript.OpenCity(i);
                    else
                        mapScript.CloseCity(i);
                break;
        }
    }
	
    void InterpretSequence()
    {
        if (currentSequence >= currentChain[currentLink].sequences.Length)
            return;

        string para = currentChain[currentLink].sequences[currentSequence].parameter;

        switch (currentChain[currentLink].sequences[currentSequence].type)
        {
            case SequenceType.DIALOGUE:
                map.SetActive(false);
                ActivateDialogue(int.Parse(para));
                break;
            case SequenceType.BATTLE:
                map.SetActive(false);
                camScript.ActivateBattleCam();
                board = (GameObject)Instantiate(Resources.Load("Boards/" + para));
                //combat.SetActive(true);
                //battleUI.SetActive(true);
                //battleUI.SetActive(true);
                ProjectileScript ps = GameObject.FindGameObjectWithTag("ProjectileController").GetComponent<ProjectileScript>();
                ps.Reset();
                
                /*foreach (GameObject i in GameObject.FindGameObjectsWithTag("Board"))
                    if (i.name == para)
                    {
                        for (int j = 0; j < i.transform.childCount; j++)
                            i.transform.GetChild(j).gameObject.SetActive(true);
                        UnityEditor.PrefabUtility.ResetToPrefabState(i.transform.GetChild(0));
                    }
                        */
                break;
            case SequenceType.ACTIVATE_NODE:
                mapScript.OpenCity(int.Parse(para));
                NextSequence();
                break;
            case SequenceType.MUSIC:
                audioCS.PlayMusic(para);
                NextSequence();
                break;
            case SequenceType.BACKGROUND:
                bgName = para;
                NextSequence();
                break;
        }
    }

    public void StartChain(int city)
    {
        currentSequence = 0;
        currentLink = 0;
        audioCS.PlayMusic("None");
        currentChain = null;
        switch (InGameTime)
        {
            case 0:
                if (cityEvents[city].morning.Length > 0)
                    currentChain = cityEvents[city].morning;
                break;
            case 1:
                if (cityEvents[city].afteroon.Length > 0)
                    currentChain = cityEvents[city].afteroon;
                break;
            case 2:
                if (cityEvents[city].night.Length > 0)
                    currentChain = cityEvents[city].night;
                break;
            default:
                break;
        }

        if(currentChain != null &&currentChain[currentLink].unlocked)
            InterpretSequence();
    }

    //Executes next scene sequence for current scene
    public void NextSequence(){

        currentSequence++;
        if (currentLink != -1 && currentSequence < currentChain[currentLink].sequences.Length)
            InterpretSequence();
        else{
            currentSequence = 0;
            currentLink = -1;
            map.SetActive(true);
            audioCS.PlayMusic("Overworld");
            camScript.DeactivateBattleCam();
            ProceedTime(1);
        }
    }

    //Load all scripts that will be controlled by scene flow controller
    void GetScripts(){
        audioCS = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioControlScript>();
        dialogueScript = dialogueUI.GetComponent<DialogueHandler>();
        mapScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<OverworldMapControl>();
        camScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MapCameraController>();

    }

    //Disable all UI scripts and interfaces while they are unneeded
    void DisableUI()
    {
        dialogueUI.SetActive(false);
        dialogueScript.enabled = false;
    }
    
    //Activates Dialogue
    public void ActivateDialogue(int assetIndex){
        dialogueUI.SetActive(true);
        dialogueUI.GetComponent<DialogueHandler>().LoadTextAsset(assetIndex);
        dialogueScript.enabled = true;
        dialogueScript.SetBackground(bgName);
    }

    //Proceeds time by given amount
    public void ProceedTime(int n)
    {
        InGameTime += n;
        if (InGameTime >= maxTime)
            InGameTime -= maxTime;
        UpdateMapNodes();
    }

    //Eventually going to activate sequences based on location
    public void ActivateLocationSequence(int location,int sequenceNum, int time)
    {
        switch(time)
        {
            case 0:
                cityEvents[location].morning[sequenceNum].unlocked = true;
                break;
            case 1:
                cityEvents[location].afteroon[sequenceNum].unlocked = true;
                break;
            case 2:
                cityEvents[location].night[sequenceNum].unlocked = true;
                break;
            default:
                break;
        }
    }
}
