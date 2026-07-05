using System;
using UnityEngine;

public class SupportButton : MonoBehaviour
{
    public void OpenSupportEmail()
    {
        string email = "kinoua9@gmail.com";
        string subject = "Road IQ Support";
        string body =
            "Hello!\n\n" +
            "I need help with Road IQ.\n\n" +
            "Please describe your problem here:\n";

        Application.OpenURL($"mailto:{email}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}");
    }
}