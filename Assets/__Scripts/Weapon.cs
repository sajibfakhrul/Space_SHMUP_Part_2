using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types
/// It also includes a "shield" type tp allow a shield power-up
/// Items marked [NI] below anr Not Implemented in the IGDPD book.
/// </summary>
public enum eWeaponType
{
    none,       //The default / no weapon
    blaster,    //A simple blaster
    spread,     //Two shots simultaneously
    phaser,     //[NI] Shots that move in waves
    missile,    //[NI] Homing missles
    laser,      //[Ni] Damage over time
    shield      //Raise shieldLevel
}
/// <summary>
/// The WeaponDefinition class allows you to set the properties
///     of a specific weapon in the Inspector. The Main class has
///     an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the power-up")]
    public string letter; //Letter to show on the power-up
    [Tooltip("Color of Power-up Cube")]
    public Color powerUpColor = Color.white; //Color of power-up
    [Tooltip("Prefab of Weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab for projectile that is fired")]

    public GameObject projectilePrefab; // Prefab for projectiles

    [Tooltip("Color of the projectile that is fired")]
    public Color projectileColor = Color.white;

    [Tooltip(" damage caused  when a single projectile hits an Enemy")]
    public float damageOnHit = 0; // Amount of damage caused
    [Tooltip(" damage caused  per second by the Laser [Not implemented]")]
    public float damagePerSec = 0;  //Damage per second (laser)
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip(" Velocity of individual Projectiles")]
    public float velocity = 50; //Speed of projectiles
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header(" Dynamic ")]
    [SerializeField]
    [Tooltip("Setting this manually while playing does not work properly")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime; // Time the Weapon will fire next
    public GameObject weaponModel;
    
    private Transform shotPointTrans;

    void Start()
    {
        //Set up PROJECTILE_ANCHOR if it has not already been done
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);

        //Call SetType() for the default _type set in the Inspector
        SetType(_type);


        //Find the fireEvent of a Hero Component in the parent  hierarchy

        Hero hero = GetComponent<Hero>();
        if (hero != null) hero.fireEvent += Fire;
    }
    public eWeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if (type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        // Get the Weapon Definition for this type from Main
        def = Main.Get_Weapon_Definition(_type);
        // Destroy any old model and then attach a model for this weapon
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0; //You can fire immediately after _type is set.
    }
    public void Fire()
    {
        //If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        //If it hasnt been enough time between shots, return
        if (Time.time < nextShotTime) return;
        
        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;
        
        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile(); //Make middle projectile
                p.vel = vel;
                p = MakeProjectile(); //Make right projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile(); // Make left projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
        }
    }
    public ProjectileHero MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;
        
       
        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return (p);
    }
}
