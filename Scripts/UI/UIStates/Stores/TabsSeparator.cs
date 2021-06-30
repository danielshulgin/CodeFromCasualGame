using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TabsSeparator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    
    [SerializeField] private Image leftImage;
    
    [SerializeField] private Image rightImage;

    private Color _textColor;
    
    
    public void Initialize(string text)
    {
        textMesh.text = text;
        _textColor = textMesh.color;
    }

    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            leftImage.color = Constants.NormalColor;
            rightImage.color = Constants.NormalColor;
            textMesh.color = _textColor;
        }
        else
        {
            leftImage.color = Constants.DisabledColor;
            rightImage.color = Constants.DisabledColor;
            textMesh.color = Constants.DisabledColor;
        }
    }
}
