using UnityEngine;

public class ForceUI : MonoBehaviour
{
    [SerializeField] private Transform sliderTransform;
    
    [SerializeField] private ForceAdjuster forceAdjuster;

    
    private void Update()
    {
        sliderTransform.localScale = new Vector3(forceAdjuster.Force, 1f, 1f);
    }
}
