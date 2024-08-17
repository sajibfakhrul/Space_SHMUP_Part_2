using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]

public class Enemy_4 : Enemy
{

    [Header(" Enemy_4 Inscribed Fields")]

    public float duration = 4;
    private Vector3 p0, p1; //The two points to interpolate
    private float timeStart; //birth time for this Enemy_4


    private EnemyShield[] allshields;
    private EnemyShield thisShield;
    // Start is called before the first frame update
    void Start()
    {
        allshields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();


        p0 = p1 = pos;

        InitMovement();

    }

    void InitMovement()
    {
        p0 = p1; //Set p0 to the old p1
        //Assign a new on screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        if (p0.x * p1.x > 0 && p0.y * p1.y > 0)
        {
            if (Mathf.Abs(p0.x) > Mathf.Abs(p0.y))
            {
                p1.x *= -1;

            }
            else
            {
                p1.y *= -1;

            }
        }

        //Reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        //This completely overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = u -  0.15f*Mathf.Sin(u*2*Mathf.PI); // Easing : Sine -0.15
        pos = (1 - u) * p0 + u * p1; //Simple linear interpolation

    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            Destroy(otherGO);
            // Only damage this Enemy if it's on Screen
            if (bndCheck.isOnScreen)
            {
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                {
                    hitGO = coll.contacts[0].otherCollider.gameObject;
                }
                float dmg = Main.Get_Weapon_Definition(p.type).damageOnHit;

                bool shieldFound = false;
                foreach (EnemyShield es in allshields)
                {
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;

                    }
                }

                if (!shieldFound) thisShield.TakeDamage(dmg);


                if (thisShield.isActive) return;


                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
