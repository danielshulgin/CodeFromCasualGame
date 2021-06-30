using UnityEngine;

public abstract class BasicForceAdjusterRealization : MonoBehaviour
{
    public abstract float Force { get; protected set; }

    
    public abstract void UpdateRealization();
    
    public abstract void CarouselStartTouch();

    public abstract void CarouselEndTouch();
    
    public abstract void ResetGame();
}
 