using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaradayLawVariables 
{
    public float Flux;
    public FaradayLawVariables()
    {
        this.Flux = 0;
    }

    public float getFlux()
    {
        return this.Flux;
    }

    public void setFlux(float value)
    { 
        this.Flux = value;
    } 
}
