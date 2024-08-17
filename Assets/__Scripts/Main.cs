using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Main : MonoBehaviour
{

    static private Main S;  // A private singleton for Main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    
    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;

    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;

    public eWeaponType[] powerUpFrequency = new eWeaponType[] {
                                            eWeaponType.blaster, eWeaponType.blaster,
                                            eWeaponType.spread, eWeaponType.shield};

    private BoundsCheck bndCheck;
    private void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        // A generic Dictionary with eWeaponType as the key

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // If spawnEnemies is false, skip to the next invoke of SpawnEnemy()

        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
            return;
        }
        // Pick a random enemy prefab to instantiate
        int ndx= Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;

        if (go.GetComponent<BoundsCheck>() != null) {

            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        
        }


        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x= Random.Range(xMin, xMax);
        pos.y= bndCheck.camHeight + enemyInset;
        go.transform.position = pos;
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }


    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene("__Scene_0");
    }
    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }


    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class
    /// </summary>
    ///<returns>The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeapnType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    static public WeaponDefinition Get_Weapon_Definition(eWeaponType wt)
    {
        //Check to make sure that the key exists in the dictionary
        // Attempting to retrieve a key that didnt exist would throw an error
        // so the following if statement is important
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //If no entry of the correct type exists in WEAP_DICT,
        // return a new WeaponDefintion with a type of WeaponType.none,
        // which means it has failed to find the right WeaponDefintion.
        return (new WeaponDefinition());
    }

    /// <summary>
    /// Called by an Enemy ship whenever it is destroyed. 
    /// It sometimes creates a PowerUp in place of the destroyed ship
    /// </summary>
    /// <param name="e">The enemy that was destroyed</param>
    static public void SHIP_DESTROYED(Enemy e)
    {
        // Potentially generate a PowerUp

        if(Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            // Spawn a PowerUp
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);

            pUp.transform.position= e.transform.position;
        }
    }

}
