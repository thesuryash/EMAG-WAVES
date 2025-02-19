using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianFlux: MonoBehaviour

{
    private GameObject _surface; //this is the gaussian surface which will be around the charge
    private GameObject _charge; //this is the charge that is going to be inside. (maybe find a better name for it)
    private float chargeDensity = 100; //just a random quantification
    
    public GaussianFlux(GameObject charge) 
    {
        this._surface = transform.gameObject; //this directly uses the reference of the game object it is attached to
        this._charge = charge; 

      

        this._surface.AddComponent<SphereCollider>();
        this._charge.AddComponent<SphereCollider>();
    }

    private const float Epsilon0 = 8.854187817e-12f;
    
    public float OverlappingVolume()
    {
        float rCharge = this._charge.GetComponent<SphereCollider>().radius;
        float rGuassian = this._surface.GetComponent<SphereCollider>().radius;
        float d = Vector3.Distance(this._surface.transform.position, this._charge.transform.position);
        
        //have to still clarify it this is the right expression to compute it
        float volume = (Mathf.PI/(12*d)) * (rGuassian + rCharge - (d*d)) * ((d*d) + (2*d*(rCharge + rGuassian) - 3*(rGuassian - rCharge)* (rGuassian - rCharge)));

        return volume;

    }

    public float CalculateEnclosedCharge()
    {   //some logic to portion out what fraction of charge is present inside the gaussian surface then the value will go inside the currentEnclosed variable. 
        
        float currentVolumeOverlap = OverlappingVolume();
        float currentChargeEnclosed = chargeDensity * currentVolumeOverlap;
        
        return currentChargeEnclosed;

    }
    public float CalculateGaussianFlux()
    {
        float enclosedCharge = CalculateEnclosedCharge();
        float flux = enclosedCharge / Epsilon0;
        
        return flux;
    }

    private void Update() //should we add this update method? 
    {
        CalculateEnclosedCharge();
        CalculateGaussianFlux();
    }
}
