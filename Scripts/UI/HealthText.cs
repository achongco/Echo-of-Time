using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthText : MonoBehaviour {
    public GameObject player;
	void Update () {
        gameObject.GetComponent<Text>().text = player.GetComponent<HealthScript>().health.ToString();
	}
}
