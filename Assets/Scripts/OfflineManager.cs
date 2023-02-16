using System;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;

public class OfflineManager : MonoBehaviour
{
    public IDLEGame game;

    public GameObject offlinePopUp;
    public Text timeAwayText;
    public Text gainsText;

    public void LoadOfflineProduction()
    {
        if (game.data.offlineProgressCheck)
        {
            // Offline time management
            var tempOfflineTime = Convert.ToInt64(PlayerPrefs.GetString("OfflineTime"));
            var oldTime = DateTime.FromBinary(tempOfflineTime);
            var currentTime = DateTime.Now;
            var difference = currentTime.Subtract(oldTime);
            var rawTime = (float)difference.TotalSeconds;
            var offlineTime = rawTime / 10;

            offlinePopUp.gameObject.SetActive(true);
            TimeSpan timer = TimeSpan.FromSeconds(rawTime);
            timeAwayText.text = $"You were away for\n<color=#52E6FB>{timer:dd\\:hh\\:mm\\:ss}</color>";
            
            BigDouble coinsGains = game.TotalCoinsPerSec() * offlineTime;
            game.data.count += coinsGains;
            gainsText.text = $"You earned:\n+{Methods.NotationMethod(coinsGains, "F2")} $";
        }
    }

    public void CloseOffline()
    {
        offlinePopUp.gameObject.SetActive(false);
    }
}
