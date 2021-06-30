using UnityEngine;

public class Constants : MonoBehaviour
{
    [SerializeField] private static Color normalColor = new Color(1f,1f,1f,1f);
     
    [SerializeField] private static Color disabledColor = new Color(.5f,.5f,.5f,1f);

    [HideInInspector] public const string FirstBloodAchievementId = "CgkItrLVhpAWEAIQAw";
    
    [HideInInspector] public const string WoundedAchievementId = "CgkItrLVhpAWEAIQBA";
    
    [HideInInspector] public const string SickAchievementId = "CgkItrLVhpAWEAIQBQ";
    
    [HideInInspector] public const string PatientAchievementId = "CgkItrLVhpAWEAIQBg";
    
    [HideInInspector] public const string DisabledAchievementId = "CgkItrLVhpAWEAIQBw";

    public static Color NormalColor => normalColor;
    
    public static Color DisabledColor => disabledColor;
    
    public static Constants Instace { get; private set; }

    private void Awake()
    {
        if (Instace == null)
        {
            Instace = this;
        }
        else
        {
            Debug.Log("Multiple Constants instances");
        }
    }
}
