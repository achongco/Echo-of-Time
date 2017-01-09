using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileScript : MonoBehaviour {

    public GameObject[] projectiles;    //Gameobjects to run projectiles
    List<int> freeProjectiles;          //Projectiles available for use
    List<TileScript> mapTiles;          //List of tiles on current map

    //Retrieve information from cardscript to play out behavior over time
    CardScript[] cardScript;
    TileScript tileScript;
    CardScript.DamageType dmgType;
    CardScript.AOEType aoeType;

    float speed;

    //Constant variables adjust for balance
    const int NUM_OF_METEORS = 9;       //Number of meteors that fall when Landstorm cast

	// Use this for initialization
	void Start () {
        Reset();
        cardScript = new CardScript[projectiles.Length];
        GetMapTiles();
	}

    //Get all tiles on map to store in list
    void GetMapTiles()
    {
        Transform board = transform.parent.GetChild(0);
        foreach (Transform child in board)
        {
            if (child.name.Substring(0, 4) == "Tile")
                mapTiles.Add(child.gameObject.GetComponent<TileScript>());
        }
    }
   

    public void ShootProjectile(float spd, TileScript tile, GameObject card, CardScript.DamageType type, CardScript.AOEType aType)
    {
        int index = freeProjectiles[0];
        freeProjectiles.Remove(0);

        tileScript = tile;
        speed = spd;
        cardScript[index] = card.GetComponent<CardScript>();
        dmgType = type;
        aoeType = aType;
        switch (dmgType)
        {
            case (CardScript.DamageType.THREE_AWAY_DOT):
                StartCoroutine(ApplyDot(index));
                break;

            case (CardScript.DamageType.RANDOM):
                if (aoeType == CardScript.AOEType.MULTIPLE_HIT) //It's probably meteor then
                    StartCoroutine(MeteorDamage(index));
                break;

            default:
                StartCoroutine(MovingProjectile(index));
                break;
        }
        
    }

    //Reset all list
    public void Reset()
    {
        for(int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i].SetActive(false);
            StopCoroutine(MovingProjectile(i));
            StopCoroutine(ApplyDot(i));
        }
        freeProjectiles = new List<int>(new int[] { 0, 1, 2, 3, 4 });
        mapTiles = new List<TileScript>();
    }

    //Apply dot to are
    IEnumerator ApplyDot(int index)
    {
        for (int n = 0; n < 3; n++)
        {
            if (tileScript.right == null)
                break;
            tileScript = tileScript.right.GetComponent<TileScript>();
        }

        cardScript[index].hitsL = 1;
        cardScript[index].hits[0] = tileScript;
        CheckPerpendicular(index);

        for (int i = 0; i < 50; i++)
        {
            if (i % 10 == 0)
                cardScript[index].damage = 10;
            else
                cardScript[index].damage = 1;
            cardScript[index].tileDamage();
            yield return new WaitForSeconds(1f);
        }
    }

    void CheckPerpendicular(int index)
    {
        cardScript[index].hitsL = 0;
        if (tileScript.up != null)
        {
            cardScript[index].hits[cardScript[index].hitsL] = tileScript.up.GetComponent<TileScript>();
            cardScript[index].hitsL++;

            if (tileScript.up.GetComponent<TileScript>().up != null)
            {
                cardScript[index].hits[cardScript[index].hitsL] = tileScript.up.GetComponent<TileScript>().up.GetComponent<TileScript>();
                cardScript[index].hitsL++;
            }

        }
        if (tileScript.down != null)
        {
            cardScript[index].hits[cardScript[index].hitsL] = tileScript.down.GetComponent<TileScript>();
            cardScript[index].hitsL++;

            if (tileScript.down.GetComponent<TileScript>().down != null)
            {
                cardScript[index].hits[cardScript[index].hitsL] = tileScript.down.GetComponent<TileScript>().down.GetComponent<TileScript>();
                cardScript[index].hitsL++;
            }
        }

    }

    //Manages projectile that moves across screen to apply damage
    IEnumerator MovingProjectile(int index)
    {
        cardScript[index].hitsL = 1;

        while (tileScript != null)
        {
            cardScript[index].hits[0] = tileScript;
            if(cardScript[index].tileDamage() && dmgType == CardScript.DamageType.FIRST_HIT_PROJECTILE)
            {
                if(aoeType == CardScript.AOEType.PERPENDICULAR)
                {
                    CheckPerpendicular(index);
                    cardScript[index].tileDamage();
                }
                break;
            }

            if (tileScript.right != null)
                tileScript = tileScript.right.GetComponent<TileScript>();
            else
                tileScript = null;
            yield return new WaitForSeconds(speed/2f);
        }
        freeProjectiles.Add(index);
    }

    IEnumerator MeteorDamage(int index)
    {
        cardScript[index].hitsL = 1;

        for (int i = 0; i < NUM_OF_METEORS; i++)
        {
            cardScript[index].hits[0] = mapTiles[Random.Range(0, mapTiles.Count - 1)];
            cardScript[index].tileDamage();          
            yield return new WaitForSeconds(.5f);
        }

        freeProjectiles.Add(index);
    }
}
