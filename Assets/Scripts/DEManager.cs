using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using System;

public class DEManager : MonoBehaviour
{
    public IDLEGame game;
    public Text eventTokensText;

    public BigDouble eventTokenBoost => (game.data.eventTokens / 100) + 1;

    public GameObject eventRewardPopUp;
    public Text eventRewardText;

    public GameObject[] events = new GameObject[2];
    public GameObject[] eventsUnlocked = new GameObject[2];
    public Text[] rewardText = new Text[2];
    public Text[] currencyText = new Text[2];
    public Text[] costText = new Text[2];
    public Text[] startText = new Text[2];

    public BigDouble[] reward = new BigDouble[2];
    public BigDouble[] currencies = new BigDouble[2];

    public BigDouble[] costs = new BigDouble[2];
    public int[] levels = new int[2];

    public bool eventActive;

    private string DayOfTheYear()
    {
        DateTime dt = DateTime.Now;
        //var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2);
        string answer;
        if (dt.DayOfYear % 2 == 1)
            answer = "Odd";
        else
            answer = "Even";
        return answer;
    }

    public string prevDayChecked;

    public void StartEvents()
    {
        eventActive = false;
        prevDayChecked = DayOfTheYear();

        if(game.data.eventActiveID != 0)
        {
            game.data.eventActiveID = 0;
            game.data.eventCooldown = new float[2];
        }
    }

    void Update()
    {
        var data = game.data;

        eventTokensText.text = $"Event Tokens: {Methods.NotationMethod(data.eventTokens, "F2")}({Methods.NotationMethod(eventTokenBoost, "F2")}x)";

        reward[0] = BigDouble.Log10(currencies[0] + 1);
        reward[1] = BigDouble.Log10(currencies[1] / 5 + 1);

        for (var i = 0; i < 2; i++)
            costs[i] = 25 * BigDouble.Pow(1.15, levels[i]);

        if(prevDayChecked != DayOfTheYear() & eventActive)
        {
            data.eventActiveID = 0;
            for (var i = 0; i < 2; i++)
                data.eventCooldown[i] = 0;
        }

        switch (DayOfTheYear())
        {
            case "Odd":
                if(game.EventGroup.gameObject.activeSelf)
                    RunEventUI(0);
                break;
            case "Even":
                if (game.EventGroup.gameObject.activeSelf)
                    RunEventUI(1);
                RunEvent(1);
                break;
        }

        if (data.eventActiveID == 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        else if (data.eventActiveID != 0 & game.data.eventCooldown[CurrentDay()] > 0)
            game.data.eventCooldown[CurrentDay()] -= Time.deltaTime;
        if (data.eventActiveID != 0 & game.data.eventCooldown[CurrentDay()] <= 0)
            CompleteEvent(CurrentDay());

        prevDayChecked = DayOfTheYear();
    }

    public int CurrentDay()
    {
        switch (DayOfTheYear())
        {
            case "Odd": return 0;
            case "Even": return 1;
        }
        return -1;
    }

    public void Click(int id)
    {
        switch (id)
        {
            case 0:
                currencies[id] += 1 + levels[id];
                break;
            case 1:
                currencies[id] += 1;
                break;
        }
    }

    public void Buy(int id)
    {
        if (currencies[id] < costs[id]) return;
        currencies[id] -= costs[id];
        levels[id]++;
    }

    public void ToggleEvent(int id)
    {
        var id2 = id - 1;
        var data = game.data;
        DateTime now = DateTime.Now;

        //Start
        if (data.eventActiveID == 0 & data.eventCooldown[id2] <= 0 & !(now.Hour == 23 & now.Minute >= 55))
        {
            data.eventActiveID = id;//////////////
            data.eventCooldown[id2] = 300;

            currencies[id2] = 0;
            levels[id2] = 0;
        }
        else if (now.Hour == 23 & now.Minute >= 55 & data.eventActiveID == 0) ;
        else if (data.eventCooldown[id2] > 0 & data.eventActiveID == 0) ;
        else   //Exit
        {
            CompleteEvent(id2);
        }
    }

    private void CompleteEvent(int id)
    {
        var data = game.data;

        data.eventTokens += reward[id];
        eventRewardText.text = $"+{Methods.NotationMethod(reward[id], "F2")} Event Tokens";

        currencies[id] = 0;
        levels[id] = 0;
        data.eventActiveID = 0;
        data.eventCooldown[id] = 7200;

        eventRewardPopUp.gameObject.SetActive(true);
    }

    public void CloseEventReward()
    {
        eventRewardPopUp.gameObject.SetActive(false);
    }

    private void RunEventUI(int id)
    {
        var data = game.data;

        for (var i = 0; i < 2; i++)
            if (i == id)
                events[id].gameObject.SetActive(true);
            else
                events[i].gameObject.SetActive(false);

        var time = TimeSpan.FromSeconds(data.eventCooldown[id]);

        if (data.eventActiveID == 0)
        {
            startText[id].text = data.eventCooldown[id] > 0 ? time.ToString(@"hh\:mm\:ss") : "Start Event";
        }
        else
            startText[id].text = $"Exit Event({time:hh\\:mm\\:ss})";

        if (data.eventActiveID != id + 1) return;
        eventsUnlocked[id].gameObject.SetActive(true);
        rewardText[id].text = $"+{Methods.NotationMethod(reward[id], "F2")} Event Tokens";
        currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} TUGRIKS";
        costText[id].text = $"Cost: {Methods.NotationMethod(costs[id], "F2")}";

    }

    private void RunEvent(int id)
    {
        switch(id)
        {
            case 1:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
        }
    }
}
