using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f; 
    private static Color blinkColor= Color.red;
    public bool ignoreOnCollisionEnter=false;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;

     void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        // Get materials and colors for this GameObject and its children

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showingColor && Time.time > blinkCompleteTime) RevertColors();
    }


     void OnCollisionEnter(Collision coll)
    {

        if ((ignoreOnCollisionEnter)) return;
       
        // Check for collisions with ProjectileHero

        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p != null) {
            if (bndCheck != null && !bndCheck.isOnScreen) {

                return; // Don't show damage if this is off screen
            
            }

            SetColors();
        
        }
        
    }


    public void SetColors()
    {
        foreach (Material m in materials) {

            m.color = blinkColor;
        
        }

        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
        
    }


    void RevertColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingColor = false;
    }

}
