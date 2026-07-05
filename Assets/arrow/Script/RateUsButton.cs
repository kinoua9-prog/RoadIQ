using UnityEngine;

public class RateUsButton : MonoBehaviour
{
    private const string packageName = "com.pleshikstudio.roadiq";

    public void OpenRateUs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            // Відкрити Google Play
            Application.OpenURL("market://details?id=" + packageName);
        }
        catch
        {
            // Якщо Google Play відсутній - відкрити браузер
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + packageName);
        }
#else
        // Для Unity Editor та ПК
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + packageName);
#endif
    }
}