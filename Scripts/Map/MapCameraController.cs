using UnityEngine;
using System.Collections;

public class MapCameraController : MonoBehaviour {

    public GameObject player;
    Vector3 offset;
    bool inBattle;

    // Use this for initialization
    void Start(){
        offset = transform.position;
    }
    
    void LateUpdate(){
        //if (!inBattle)
            //transform.position = player.transform.position + offset;
    }

    public bool GetInBattleState(){
        return inBattle;
    }

    public void ActivateBattleCam(){
        inBattle = true;
        transform.position = offset;
    }

    public void DeactivateBattleCam(){
        inBattle = false;
    }
}
