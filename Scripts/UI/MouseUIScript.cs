using UnityEngine;
using System.Collections;

public class MouseUIScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Cursor.visible = false;

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = Input.mousePosition;
        pos.z = transform.position.z - Camera.main.transform.position.z;
        transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
}
