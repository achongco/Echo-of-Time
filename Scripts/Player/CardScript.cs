using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardScript : MonoBehaviour {

    public enum DamageType
    {
        FIRST_HIT_PROJECTILE,
        LINEAR,
        PINPOINT,
        SINGLEHEAL,         
        BACKSTAB,           //Attacks backrow
        EVERYTHING,         
        THREE_AWAY,         //Targets 3 spaces away
        THREE_AWAY_DOT,     //Targets 3 spaces away to apply DoT
        FLAME_THROWER,      
        RANDOM,             //Targets random square
        RANDOM_UNIT,        //Targets random unit not including player
        TRAP,               //Leaves trap in random square
        NEAREST_ENEMY,      //Targets nearest enemy
        ALL_ENEMY,          //Targets all enemies
        HIGHEST_HP,         //Targets enemy with the highest hp
        HIGHEST_HP_HEAL,    //Targets enemy with highest hp and heals self
        MULTI_HIT           //Specific to vulcan effect
    }

    public enum AOEType
    {
        NONE,               //No AoE
        BEHIND,             //Hits singular space behind unit hit
        PERPENDICULAR,      //Hits up and down from unit hit
        COLUMN_BEHIND,      //Hits column behind unit hit
        CROSS,              //Hits in a "+" shape from initial hit
        MULTIPLE_HIT        //Specific to meteor
    }
    
    public Sprite sprite;
    public DamageType damageType;
    public AOEType aoeType;
    

    public string cardName;
    public string description;
    public string playerAnimation;
    public string hitAnimation;
    public int manaCost;
    public int damage;
    public int range;
    public int speed;
    public int health;

    GameObject hitAnim;
    GameObject character;
    CharacterScript characterScript;
    GameObject tile;
    TileScript tileScript;
    public TileScript[] hits;
    public int hitsL;
    int rangeCount;
    bool hit;
    bool heal;

    public void parseCard(CardScript card)
    {

        heal = false;
        hits = new TileScript[18];
        rangeCount = 0;
        hitsL = 0;
        hit = false;
        hitAnim = GameObject.FindGameObjectWithTag("CardHitAnimation");
        character = GameObject.FindGameObjectWithTag("Player");
        characterScript = character.GetComponent<CharacterScript>();
        tile = characterScript.tile;
        tileScript = tile.GetComponent<TileScript>();
        Debug.Log("Parsing " + card.name);
        Debug.Log("Starting Damage ");
        calcDamage();
        if(damageType != DamageType.FLAME_THROWER)
        {
            if (speed <= 1)
                checkAOEType();
            tileDamage();
        }

        Debug.Log("DONE Damage ");
    }

    //Deal the damage here
    public bool tileDamage()
    {
        bool somethingHit = false;
        if (heal)
        {
            character.GetComponent<HealthScript>().heal(damage);
            heal = false;
        }
        else
        {
            for (int i = 0; i < hitsL; i++)
            {
                Debug.Log(hits[i].transform.position);
                (Instantiate(hitAnim, hits[i].transform.position, Quaternion.identity) as GameObject).GetComponent<CardHitAnimation>().go(hitAnimation);
                if (hits[i].occupiedBy != null && hits[i].occupiedBy.ToString().Substring(0,6) != "Player")
                {
                    HealthScript enemyHealth = hits[i].occupiedBy.GetComponent<HealthScript>();
                    enemyHealth.takeDamage(damage);
                    somethingHit = true;
                }
            }
        }
        return somethingHit;
    }

    //Determine what gets hit
    void calcDamage()
    {
        switch (damageType)
        {
            case (DamageType.TRAP): ApplyDamageTrap();
                break;
            case (DamageType.RANDOM): ApplyDamageRandom();
                break;
            case (DamageType.THREE_AWAY): ApplyDamageThreeAway();
                break;
            case (DamageType.FLAME_THROWER): FlameThrowerDamage();
                break;
            case (DamageType.THREE_AWAY_DOT): ApplyDamageThreeAwayDot();
                break;
            case (DamageType.FIRST_HIT_PROJECTILE): ApplyDamageFirstHitProj();
                break;
            case (DamageType.LINEAR): ApplyDamageLinear();
                break;
            case (DamageType.PINPOINT): ApplyDamagePinpoint();
                break;
            case (DamageType.SINGLEHEAL): heal = true;
                break;
            case (DamageType.EVERYTHING):
                break;
            case (DamageType.BACKSTAB): ApplyDamageBackstab();
                break;
            case (DamageType.NEAREST_ENEMY): ApplyDamageNearEnemy();
                break;
            case (DamageType.ALL_ENEMY): ApplyDamageAllEnemy();
                break;
            case (DamageType.HIGHEST_HP): ApplyDamageHighHP();
                break;
            case (DamageType.HIGHEST_HP_HEAL): ApplyDamageHighHP();
                character.GetComponent<HealthScript>().heal(damage);
                break;
            default:
                break;
        }
    }

    //Determine what gets hit by AOE
    void checkAOEType()
    {
        int hitsLT = hitsL;
        switch (aoeType)
        {
            case (AOEType.NONE):
                break;
            case (AOEType.BEHIND):
                for (int i = 0; i < hitsLT; i++) {
                    if (hits[i].right != null)
                    {
                        hits[hitsLT] = hits[i].right.GetComponent<TileScript>();
                        hitsL++;
                    }
                }
                break;
            case (AOEType.PERPENDICULAR):
                for (int i = 0; i < hitsLT; i++)
                {
                    if (hits[i].up != null)
                    {
                        hits[hitsL] = hits[i].up.GetComponent<TileScript>();
                        hitsL++;
                    }
                    if (hits[i].down != null)
                    {
                        hits[hitsL] = hits[i].down.GetComponent<TileScript>();
                        hitsL++;
                    }
                }
                break;
            case (AOEType.COLUMN_BEHIND):
                if(hitsL > 0 && hits[0].right != null)
                {
                    hits[hitsL] = hits[0].right.GetComponent<TileScript>();
                    hitsL++;
                    if(hits[1].up != null)
                    {
                        hits[hitsL] = hits[1].up.GetComponent<TileScript>();
                        hitsL++;
                    }
                    if(hits[1].down != null)
                    {
                        hits[hitsL] = hits[1].down.GetComponent<TileScript>();
                        hitsL++;
                    }

                }
                break;
            case (AOEType.CROSS):
                for (int i = 0; i < hitsLT; i++)
                {
                    if (hits[i].up != null)
                    {
                        hits[hitsL] = hits[i].up.GetComponent<TileScript>();
                        hitsL++;
                    }
                    if (hits[i].down != null)
                    {
                        hits[hitsL] = hits[i].down.GetComponent<TileScript>();
                        hitsL++;
                    }
                    if (hits[i].left != null)
                    {
                        hits[hitsL] = hits[i].left.GetComponent<TileScript>();
                        hitsL++;
                    }
                    if (hits[i].right != null)
                    {
                        hits[hitsL] = hits[i].right.GetComponent<TileScript>();
                        hitsL++;
                    }
                }
                break;
        }
    }



    /*
     *  Functions for applying damage types
     */

    //Apply effect of Trap type Damage
    void ApplyDamageTrap()
    {
        List<TileScript> tileList = new List<TileScript>();

        for (int i = 3; i < 6; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                if (tileScript.transform.parent.GetChild(i + n * 6).gameObject.GetComponent<TileScript>().occupiedBy == null)
                {
                    tileList.Add(tileScript.transform.parent.GetChild(i + n * 6).gameObject.GetComponent<TileScript>());
                }
            }
        }

        tileList[Random.Range(0, tileList.Count - 1)].SetAlliedTrap(damage);
    }

    //Apply effect of Random type Damage
    void ApplyDamageRandom()
    {
        if (aoeType == AOEType.MULTIPLE_HIT)
        {
            ProjectileScript ps = GameObject.FindGameObjectWithTag("ProjectileController").GetComponent<ProjectileScript>();
            ps.ShootProjectile((float)speed, tileScript, this.gameObject, damageType, aoeType);
        }
        else
        {
            List<TileScript> enemyList = new List<TileScript>();

            enemyList = GetEnemyOccupiedTiles();

            if (enemyList.Count > 0)
            {
                hitsL = 1;
                hits[0] = enemyList[Random.Range(0, enemyList.Count - 1)];
            }

        }
    }

    //Apply effect of Three Away type Damage
    void ApplyDamageThreeAway()
    {
                 if(tileScript != null)
                {
                    for (int n = 0; n < 3; n++){
                        if (tileScript.right == null)
                            break;
                        tileScript = tileScript.right.GetComponent<TileScript>();
                    }

                    hitsL = 1;
                    hits[0] = tileScript;
                }
    }

    //Apply effect for Flame Thrower
    void FlameThrowerDamage()
    {
        if (tileScript != null)
        {
            int originalDamage = damage;
            hitsL = 1;

            if (tileScript.right != null)
            {
                hits[0] = tileScript.right.GetComponent<TileScript>();
                tileDamage();

                for (int i = 2; i < 4; i++)
                {
                    if (hits[0].right != null)
                    {
                        hits[0] = hits[0].right.GetComponent<TileScript>();
                        for (int n = 0; n < i; n++)
                            tileDamage();
                    }
                }
            }
        }
    }

    //Apply effect for Three Away Dot type Damage
    void ApplyDamageThreeAwayDot()
    {
        if (tileScript != null)
        {
            if (tileScript.occupiedBy.ToString() != "Player")
                hits[hitsL] = tileScript;
            ProjectileScript ps = GameObject.FindGameObjectWithTag("ProjectileController").GetComponent<ProjectileScript>();
            ps.ShootProjectile((float)speed, tileScript, this.gameObject, damageType, aoeType);
        }
    }

    //Apply effect for First Hit Projectile type Damage
   void ApplyDamageFirstHitProj()
    {
        if (speed > 1)
        {
            if (tileScript != null)
            {
                if (tileScript.occupiedBy.ToString() != "Player")
                    hits[hitsL] = tileScript;
                ProjectileScript ps = GameObject.FindGameObjectWithTag("ProjectileController").GetComponent<ProjectileScript>();
                ps.ShootProjectile((float)speed, tileScript, this.gameObject, damageType, aoeType);
            }

        }
        else
        {
            while (rangeCount < range && !hit && tileScript != null){
                if (tileScript == null || tileScript.right == null)
                    tileScript = null;
                else{
                    rangeCount++;
                    tileScript = tileScript.right.GetComponent<TileScript>();
                    if (tileScript.occupiedBy != null || rangeCount == range){
                        hit = true;
                        hits[hitsL] = tileScript;
                        hitsL++;
                    }
                }
            }
        }
    }

    //Apply effect for Linear type Damage
    void ApplyDamageLinear()
    {
        while (rangeCount < range && tileScript != null)
        {
            if (tileScript == null || tileScript.right == null)
                tileScript = null;
            else
            {
                tileScript = tileScript.right.GetComponent<TileScript>();
                hits[hitsL] = tileScript;
                hitsL++;
                rangeCount++;
            }
        }
    }

    //Apply effect for Pinpoint type Damage
    void ApplyDamagePinpoint()
    {
        while (rangeCount < range && tileScript != null)
        {
            if (tileScript == null || tileScript.right == null)
                tileScript = null;
            else
            {
                tileScript = tileScript.right.GetComponent<TileScript>();
                rangeCount++;
            }
        }
        if (tileScript != null)
        {
            hits[hitsL] = tileScript;
            hitsL++;
        }
    }

    //Apply effect for Backstab type Damage
    void ApplyDamageBackstab()
    {
        while (rangeCount < range && tileScript != null)
        {
            if (tileScript.right == null)
            {
                hits[hitsL] = tileScript;
                hitsL++;

                return;
            }
            else
            {
                tileScript = tileScript.right.GetComponent<TileScript>();
                rangeCount++;
            }
        }
    }

    //Apply effect for Nearest Enemy type Damage
    void ApplyDamageNearEnemy()
    {
        for (int i = 3; i < 6; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                if (tileScript.transform.parent.GetChild(i + n * 6).gameObject.GetComponent<TileScript>().occupiedBy != null)
                {
                    if (tileScript.transform.parent.GetChild(i + n * 6).GetComponent<TileScript>().occupiedBy.tag == "Enemy")
                    {
                        hits[hitsL] = tileScript.transform.parent.GetChild(i + n * 6).GetComponent<TileScript>();
                        hitsL++;
                        return;
                    }
                }
            }
        }
    }

    //Apply effect for All Enemy type Damage
    void ApplyDamageAllEnemy()
    {
        List<TileScript> enemyList = GetEnemyOccupiedTiles();
        for(int i = 0; i < enemyList.Count; i++)
        {
            hits[hitsL] = enemyList[i];
            hitsL++;
        }
    }

    //Apply effect for Highest HP type Damage
    void ApplyDamageHighHP()
    {
        List<TileScript> enemyList = GetEnemyOccupiedTiles();
        TileScript target;

        if(enemyList.Count > 0)
        {
            target = enemyList[0];
            for(int i = 0; i < enemyList.Count; i++)
            {
                if (target.occupiedBy.GetComponent<HealthScript>().health < enemyList[i].occupiedBy.GetComponent<HealthScript>().health)
                    target = enemyList[i];
            }

            hits[0] = target;
            hitsL = 1;
        }


    }
    
    /*
     *  Helper Functions
     */

    //Returns a list of tiles occupied by an enemy
    List<TileScript> GetEnemyOccupiedTiles()
    {
        List<TileScript> enemies = new List<TileScript>();

        for (int i = 0; i < 18; i++)
        {
            if (tileScript.transform.parent.GetChild(i).GetComponent<TileScript>().occupiedBy != null
                && tileScript.transform.parent.GetChild(i).GetComponent<TileScript>().occupiedBy.tag == "Enemy")
            {
                enemies.Add(tileScript.transform.parent.GetChild(i).GetComponent<TileScript>());
            }

        }

        return enemies;
    }


}
