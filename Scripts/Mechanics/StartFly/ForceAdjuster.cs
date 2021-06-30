using UnityEngine;

public class ForceAdjuster : MonoBehaviour
{
    [SerializeField] private BasicForceAdjusterRealization forceAdjusterRealization;
    
    public float Force => forceAdjusterRealization.Force;

    
    private void Start()
    {
        GameState.OnResetGame += HandleResetGame;
    }

    private void OnDestroy()
    {
        GameState.OnResetGame -= HandleResetGame;
    }
    
    private void Update()
    {
        forceAdjusterRealization.UpdateRealization();
    }

    public void CarouselStartTouch()
    {
        forceAdjusterRealization.CarouselStartTouch();
    }
    
    public void CarouselEndTouch()
    {
        forceAdjusterRealization.CarouselEndTouch();
    }

    public void HandleResetGame()
    {
        forceAdjusterRealization.ResetGame();
    }

    public void BindForceAdjusterRealization(BasicForceAdjusterRealization forceAdjusterRealization)
    {
        this.forceAdjusterRealization = forceAdjusterRealization;
    }
}
