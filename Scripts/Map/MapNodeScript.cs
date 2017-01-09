using UnityEngine;
using System.Collections;

public class MapNodeScript : MonoBehaviour {

    public int nodeIndex;                   //Sets city index specific to city node

    private GameObject mapPlayer;           //Map player gameobject which moves across map
    private OverworldUIControl oiScript;    //Access overworld ui script to determine if Menu Panels active
    public LocationTracker locationTracker;
    public SceneFlowController s;


    void Awake()
    {

        oiScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<OverworldUIControl>();
    }

    void OnMouseDown()
    {

        s.StartChain(nodeIndex);
    }
}
