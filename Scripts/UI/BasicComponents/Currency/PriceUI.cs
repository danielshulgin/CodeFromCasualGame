using UnityEngine;

public abstract class PriceUI : MonoBehaviour
{
    public abstract void Initialize(Currency price);

    public abstract void Hide();
}
