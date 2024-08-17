using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent (typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float powerUpDropChance = 1f; //Chance to drop a powerup


    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    public Vector3 pos {

        get { 
        
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    
    
    }

    // Update is called once per frame
    void Update(){
        Move();

        // check whether this Enemy has gone off the bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown )) {
                Destroy(gameObject);
        
        }
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    
    }


     void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        
        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {    
            // Only damage this Enemy if it's on Screen
            if (bndCheck.isOnScreen)
            {
                // Get the damage amount from the Main WEAP_DICT.

                health -= Main.Get_Weapon_Definition(p.type).damageOnHit;
                if (health <= 0){
                    // Tell Main(class) that this ship was destroyed
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed= true;
                        Main.SHIP_DESTROYED(this);
                    }



                    // Destroy this Enemy
                    Destroy(this.gameObject);

                }
            }
            // Destroy the ProjectileHero regardless

            Destroy(otherGO);

        }
        else
        {
            print("Enemy hit by non-ProjectileHero : " +otherGO.name);
        }
        
        
    }
}
