using System.Collections;
using UnityEngine;


public class GameForceAdjusterRealization : BasicForceAdjusterRealization
 {
     [SerializeField] private float forceReduceSpeed = .2f;
    
     [SerializeField] private float forceAddSpeed = .2f;
    
     [SerializeField] private float forceMaximumPointTime = 2f;
    
     [SerializeField] [Range(0f, 1f)] private float force;
    
     private bool _press = false;
    
     private bool _frozenForce;
    
     public override float Force
     {
         get => force;
        
         protected set => force = Mathf.Clamp(value, 0f, 1f);
     }
    
     
     public override void UpdateRealization()
     {
         if (!_frozenForce)
         {
             if (_press)
             {
                 Force += Time.deltaTime * forceAddSpeed;
             }
             else
             {
                 Force -= Time.deltaTime * forceReduceSpeed;
             }
         }
     }

     public override void CarouselStartTouch()
     {
         _press = true;
     }
    
     public override void CarouselEndTouch()
     {
         _press = false;
         if (force >= (1f - float.Epsilon))
         {
             StartCoroutine(StopForceInMaximumPointRoutine());
         }
     }

     IEnumerator StopForceInMaximumPointRoutine()
     {
         _frozenForce = true;
         yield return new WaitForSeconds(forceMaximumPointTime);
         _frozenForce = false;
     }

     public override void ResetGame()
     {
         force = 0f;
         _frozenForce = false;
         StopAllCoroutines();
     }
 }
