using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class VasylAnimation : MonoBehaviour
{
    [SerializeField] private Animator elkAnimator;

    [Inject] private PlayerVasylSkins _playerElkSkins;

    
    public void HandleStartTouch()
    {
        elkAnimator.Play(_playerElkSkins.LastSelectedVasylSkin.spinCarouselAnimationName);
    }
    
    public void HandleEndTouch()
    {
        elkAnimator.Play(_playerElkSkins.LastSelectedVasylSkin.idleAnimationName);
    }
    
    public void HandleStartFly()
    {
        elkAnimator.Play(_playerElkSkins.LastSelectedVasylSkin.idleAnimationName);
    }
}
