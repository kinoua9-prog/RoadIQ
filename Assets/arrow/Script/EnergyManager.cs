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

    public bool TryUseEnergy()
    {
        if (PurchaseState.DisableEnergy)
            return true;

        RestoreEnergyByTime();

        if (currentEnergy <= 0)
        {
            ShowNoEnergyPanel();
            return false;
        }

        currentEnergy--;

        SaveEnergy();
        UpdateUI();

        return true;
    }

    private void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);

        if (!PlayerPrefs.HasKey(LastEnergyTimeKey))
        {
            PlayerPrefs.SetString(LastEnergyTimeKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    private void SaveEnergy()
    {
        PlayerPrefs.SetInt(EnergyKey, currentEnergy);
        PlayerPrefs.SetString(LastEnergyTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void RestoreEnergyByTime()
    {
        if (currentEnergy >= maxEnergy)
            return;

        string savedTime = PlayerPrefs.GetString(LastEnergyTimeKey, DateTime.Now.ToString());

        if (!DateTime.TryParse(savedTime, out DateTime lastTime))
            lastTime = DateTime.Now;

        TimeSpan timePassed = DateTime.Now - lastTime;
        int energyToAdd = (int)(timePassed.TotalMinutes / restoreMinutes);

        if (energyToAdd > 0)
        {
            currentEnergy += energyToAdd;

            if (currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;

            DateTime newLastTime = lastTime.AddMinutes(energyToAdd * restoreMinutes);

            PlayerPrefs.SetInt(EnergyKey, currentEnergy);
            PlayerPrefs.SetString(LastEnergyTimeKey, newLastTime.ToString());
            PlayerPrefs.Save();

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (energyRoot != null)
            energyRoot.SetActive(!PurchaseState.DisableEnergy);

        if (PurchaseState.DisableEnergy)
        {
            if (timerText != null)
                timerText.text = "";

            if (noEnergyPanel != null)
                noEnergyPanel.SetActive(false);

            return;
        }

        if (energyText != null)
            energyText.text = currentEnergy + "/" + maxEnergy;

        for (int i = 0; i < energyIcons.Length; i++)
        {
            if (energyIcons[i] != null)
                energyIcons[i].SetActive(i < currentEnergy);
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        string timeText = "";

        if (!PurchaseState.DisableEnergy && currentEnergy < maxEnergy)
        {
            string savedTime = PlayerPrefs.GetString(LastEnergyTimeKey, DateTime.Now.ToString());

            if (!DateTime.TryParse(savedTime, out DateTime lastTime))
                lastTime = DateTime.Now;

            DateTime nextRestoreTime = lastTime.AddMinutes(restoreMinutes);
            TimeSpan remaining = nextRestoreTime - DateTime.Now;

            if (remaining.TotalSeconds < 0)
                remaining = TimeSpan.Zero;

            timeText = string.Format("{0:00}:{1:00}", remaining.Minutes, remaining.Seconds);
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

        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;

        SaveEnergy();
        UpdateUI();
    }
    public void RefreshUI()
    {
        UpdateUI();
    }
}