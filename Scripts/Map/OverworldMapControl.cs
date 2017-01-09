using UnityEngine;
using System.Collections;

public class OverworldMapControl : MonoBehaviour {

    public GameObject[] CityNodes;              //Stores city nodes to make activate or inactive

    void Awake()
    {
        foreach (GameObject node in CityNodes)
            node.SetActive(false);
    }


    //Activates city node
    public void OpenCity(int index)
    {
        CityNodes[index].SetActive(true);
    }

    public void CloseCity(int index)
    {
        CityNodes[index].SetActive(false);
    }

}
