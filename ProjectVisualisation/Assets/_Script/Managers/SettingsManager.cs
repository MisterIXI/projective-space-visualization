using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static EnvironmentDefaults EnvironmentDefaults => Instance._environmentDefaults;
    [field: SerializeField] private EnvironmentDefaults _environmentDefaults { get; set; }
    public static SettingsManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}