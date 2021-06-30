using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyTouchEvent : UnityEvent<Touch> { }
//Can't use directly generic UnityEvent