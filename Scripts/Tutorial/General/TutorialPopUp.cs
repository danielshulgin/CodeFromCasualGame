using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] private GameObject popUpPrefab;
    
    [SerializeField] private Canvas canvas;
    
    private TextMeshProUGUI _adviseText;
    
    private Button _okButton;

    private GameObject _popUpGameObject;
    
    
    public void ShowAdviseOnUI(string textId, RectTransform targetPoint, bool showOkButton = true)
    {
        ReSpawnPopUp();
        _okButton.gameObject.SetActive(showOkButton);
        SetAdviseText(textId);
        _popUpGameObject.transform.SetParent(targetPoint);
        _popUpGameObject.GetComponent<RectTransform>().transform.position = targetPoint.position;
    }
    
    public void ShowAdviseOnWorldPoint(string textId, Transform targetPoint, bool showOkButton = true)
    {
        ReSpawnPopUp();
        _okButton.gameObject.SetActive(showOkButton);
        SetAdviseText(textId);
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(targetPoint.position);  
        _popUpGameObject.transform.SetParent(canvas.transform);
        
        var canvasRect = canvas.GetComponent<RectTransform>();
        var viewportPosition= Camera.main.WorldToViewportPoint(targetPoint.transform.position);
        
        var canvasRectSizeDelta = canvasRect.sizeDelta;
        var worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRectSizeDelta.x) - (canvasRectSizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRectSizeDelta.y) - (canvasRectSizeDelta.y * 0.5f)));
        
        _popUpGameObject.GetComponent<RectTransform>()
            .anchoredPosition = worldObjectScreenPosition;
    }
    
    private void SetAdviseText(string textId)
    {
        _adviseText.text = Localization.Instance.GetTranslation(textId).Replace("\\n", "\n");
    }

    private void ReSpawnPopUp()
    {
        if (_popUpGameObject != null)
        {
            Destroy(_popUpGameObject);            
        }

        _popUpGameObject = Instantiate(popUpPrefab, canvas.transform);
        _adviseText = _popUpGameObject.GetComponentInChildren<TextMeshProUGUI>();
        _okButton = _popUpGameObject.GetComponentInChildren<Button>();
    }

    public void Hide()
    {
        if (_popUpGameObject != null)
        {
            Destroy(_popUpGameObject);            
        }
    }
}