using UnityEngine.Events;

[System.Serializable]
public class MyPurchaseScriptableObjectEvent : UnityEvent<PurchaseScriptableObject> { }
//Can't use directly generic UnityEvent
