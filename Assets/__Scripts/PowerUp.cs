using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{
    [Header("Inspector")]
    //This is an unusual but handy use of Vector2s. x holds a min value
    // and y a max value for a Random.Range() that will be called later
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range()  call")]
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 10; //Seconds the PowerUp exists
    public float fadeTime = 4; //Seconds until it will fade

    [Header("Dynamically")]
    public eWeaponType type; // The type of the PowerUp
    public GameObject cube; //Reference to the Cube child
    public TextMesh letter; // Reference to the TextMesh
    public Vector3 rotPerSecond; //Euler rotation speed of PowerCube
    public float birthTime; // the Time.time this was instantiated

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;
    private eWeaponType _type;

    void Awake()
    {
        //Find the Cube reference
        cube = transform.GetChild(0).gameObject;
        //Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        //Set a random velocity
        Vector3 vel = Random.onUnitSphere; //Get Random XYZ velocity
        //Random.onUnitSphere gives you a vector point that is somewhere on
        // the surface of the sphere with a radius of 1m around the origin
        vel.z = 0; //Flatten the vel to the XY plane
        vel.Normalize(); //Normalize a Vector3 makes it length 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //Set the rotation of this GameObject to R:[0, 0, 0]
        transform.rotation = Quaternion.identity;
        //Quaternion.identity is equal to no rotation

        // Randomize rotPerSecond for the PowerCube  using rotMinMax x and y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
    }
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //Fade out the PowerUp over time
        //Given the default values, a PowerUp will exist for 10 seconds
        // and then fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        //If u >= 1, Destroy this powerup
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        //  if (u > 0) decrease the opacity (i.e alpha value)  of the PowerCube and letter
        if (u > 0)
        {
            Color c = cubeMat.color;
            c.a = 1f - u;  // set the alpha of PowerCube to 1-u
            cubeMat.color = c;
            //Fade the Letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f); // set the alpha of the letter to (1-(u/2)
            letter.color = c;
        }
        if (!bndCheck.isOnScreen)
        {
            //If the powerup has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }

    public eWeaponType Type
    {
        get { return _type; }
        set { SetType(value); }
    }
    public void SetType(eWeaponType wt)
    {
        //Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.Get_Weapon_Definition(wt);
        //Set the color of the cube child
        cubeMat.color = def.powerUpColor;
        //letter.color = def.color; //We could colorize the letter too
        letter.text = def.letter; //Set the letter that is shown
        _type = wt;  //Finally actually set the type
    }

    /// <summary>
    /// //This function is called by the Hero class when a Powerup is collected
    /// </summary>
    /// <param name="target"> The GameObject absorbing this PowerUp</param>
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
