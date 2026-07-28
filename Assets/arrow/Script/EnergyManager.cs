using System;
using TMPro;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;

    [Header("Settings")]
    public int maxEnergy = 5;
    public int restoreMinutes = 5;

    [Header("UI")]
    public GameObject energyRoot;
    public GameObject[] energyIcons;
    public TMP_Text energyText;
    public TMP_Text timerText;
    public TMP_Text noEnergyTimerText;
    public GameObject noEnergyPanel;

    private int currentEnergy;

    private const string EnergyKey = "Energy";
    private const string LastEnergyTimeKey = "LastEnergyTime";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadEnergy();
        RestoreEnergyByTime();
        UpdateUI();

        if (noEnergyPanel != null)
            noEnergyPanel.SetActive(false);

        Debug.Log(
            "ENERGY START | Energy: " + currentEnergy + "/" + maxEnergy +
            " | DisableEnergy: " + PurchaseState.DisableEnergy
        );
    }

    private void Update()
    {
        if (PurchaseState.DisableEnergy)
        {
            UpdateUI();
            return;
        }

        RestoreEnergyByTime();
        UpdateTimer();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool TryUseEnergy()
    {
        if (PurchaseState.DisableEnergy)
        {
            Debug.Log("ENERGY: Unlimited energy enabled.");
            return true;
        }

        RestoreEnergyByTime();

        if (currentEnergy <= 0)
        {
            ShowNoEnergyPanel();
            Debug.Log("ENERGY: Not enough energy.");
            return false;
        }

        currentEnergy--;

        SaveEnergy();
        UpdateUI();

        Debug.Log(
            "ENERGY USED | Current: " +
            currentEnergy + "/" + maxEnergy
        );

        return true;
    }

    private void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        if (!PlayerPrefs.HasKey(LastEnergyTimeKey))
        {
            PlayerPrefs.SetString(
                LastEnergyTimeKey,
                DateTime.Now.ToString("O")
            );

            PlayerPrefs.Save();
        }
    }

    private void SaveEnergy()
    {
        PlayerPrefs.SetInt(EnergyKey, currentEnergy);

        PlayerPrefs.SetString(
            LastEnergyTimeKey,
            DateTime.Now.ToString("O")
        );

        PlayerPrefs.Save();
    }

    private void RestoreEnergyByTime()
    {
        if (PurchaseState.DisableEnergy)
            return;

        if (currentEnergy >= maxEnergy)
            return;

        string savedTime = PlayerPrefs.GetString(
            LastEnergyTimeKey,
            DateTime.Now.ToString("O")
        );

        if (!DateTime.TryParse(savedTime, out DateTime lastTime))
            lastTime = DateTime.Now;

        TimeSpan timePassed = DateTime.Now - lastTime;

        int energyToAdd =
            (int)(timePassed.TotalMinutes / restoreMinutes);

        if (energyToAdd <= 0)
            return;

        currentEnergy += energyToAdd;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        DateTime newLastTime =
            lastTime.AddMinutes(energyToAdd * restoreMinutes);

        if (currentEnergy >= maxEnergy)
            newLastTime = DateTime.Now;

        PlayerPrefs.SetInt(EnergyKey, currentEnergy);

        PlayerPrefs.SetString(
            LastEnergyTimeKey,
            newLastTime.ToString("O")
        );

        PlayerPrefs.Save();

        UpdateUI();

        Debug.Log(
            "ENERGY RESTORED | Added: " + energyToAdd +
            " | Current: " + currentEnergy + "/" + maxEnergy
        );
    }

    private void UpdateUI()
    {
        bool disableEnergy = PurchaseState.DisableEnergy;

        if (energyRoot != null)
            energyRoot.SetActive(!disableEnergy);

        if (disableEnergy)
        {
            if (timerText != null)
                timerText.text = "";

            if (noEnergyTimerText != null)
                noEnergyTimerText.text = "";

            if (noEnergyPanel != null)
                noEnergyPanel.SetActive(false);

            return;
        }

        if (energyText != null)
            energyText.text = currentEnergy + "/" + maxEnergy;

        if (energyIcons != null)
        {
            for (int i = 0; i < energyIcons.Length; i++)
            {
                if (energyIcons[i] != null)
                    energyIcons[i].SetActive(i < currentEnergy);
            }
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        string timeText = "";

        if (!PurchaseState.DisableEnergy && currentEnergy < maxEnergy)
        {
            string savedTime = PlayerPrefs.GetString(
                LastEnergyTimeKey,
                DateTime.Now.ToString("O")
            );

            if (!DateTime.TryParse(savedTime, out DateTime lastTime))
                lastTime = DateTime.Now;

            DateTime nextRestoreTime =
                lastTime.AddMinutes(restoreMinutes);

            TimeSpan remaining =
                nextRestoreTime - DateTime.Now;

            if (remaining.TotalSeconds < 0)
                remaining = TimeSpan.Zero;

            int minutes = Mathf.Max(0, (int)remaining.TotalMinutes);
            int seconds = Mathf.Max(0, remaining.Seconds);

            timeText = string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
        }

        if (timerText != null)
            timerText.text = timeText;

        if (noEnergyTimerText != null)
            noEnergyTimerText.text = timeText;
    }

    public void ShowNoEnergyPanel()
    {
        if (PurchaseState.DisableEnergy)
            return;

        UpdateTimer();

        if (noEnergyPanel != null)
            noEnergyPanel.SetActive(true);
    }

    public void CloseNoEnergyPanel()
    {
        if (noEnergyPanel != null)
            noEnergyPanel.SetActive(false);
    }

    public void AddEnergy(int amount)
    {
        if (PurchaseState.DisableEnergy)
            return;

        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        SaveEnergy();
        UpdateUI();

        Debug.Log(
            "ENERGY ADDED | Current: " +
            currentEnergy + "/" + maxEnergy
        );
    }

    public void RefreshUI()
    {
        LoadEnergy();
        RestoreEnergyByTime();
        UpdateUI();
    }

    public int GetCurrentEnergy()
    {
        RestoreEnergyByTime();
        return currentEnergy;
    }

    public void ResetEnergyForTesting()
    {
        PurchaseState.DisableEnergy = false;

        currentEnergy = maxEnergy;

        PlayerPrefs.SetInt(EnergyKey, currentEnergy);
        PlayerPrefs.SetString(
            LastEnergyTimeKey,
            DateTime.Now.ToString("O")
        );

        PlayerPrefs.Save();
        UpdateUI();

        Debug.Log("ENERGY TEST RESET: 5/5");
    }
}