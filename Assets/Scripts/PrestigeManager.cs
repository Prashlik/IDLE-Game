using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;

public class PrestigeManager : MonoBehaviour
{
    public IDLEGame game;
    public Canvas prestige;

    public Text[] costText = new Text[3];
    public Image[] costBars = new Image[3];
    public Image PrestigeUpgrBar;   //statusBar

    public string[] costDest;

    public BigDouble[] costs;
    public int[] levels;

    public BigDouble asClickUpgr => 5 * BigDouble.Pow(1.5, game.data.PUlevel1);
    public BigDouble asTimeUpgr => 10 * BigDouble.Pow(1.5, game.data.PUlevel2);
    public BigDouble asGemsUpgr => 50 * BigDouble.Pow(1.5, game.data.PUlevel3);

    public Text GemsText;        //счётчик валюты
    //public Text GemsAvailableText;
    public Text GemsBoostText;   //счётчик ускорения роста

    public void StartPrestige()
    {
        costs = new BigDouble[3];
        levels = new int[3];
        costDest = new[] { "CLICK FORCE +50%", "+20% $/SEC", "x1.1 TO GEMS BOOST" };
    }

    public void Run()
    {
        var data = game.data;
        ArrayManager();
        UI();

        data.gemsAvailable = (int)(150 * Sqrt(data.count / 1e10));
        GemsText.text = "Gems: " + Methods.NotationMethod(data.gems, "F0");
        GemsBoostText.text = "x" + Methods.NotationMethod(TotalGemsBoost(), "F2") + " boost";

        if (game.GameMenuGroup.gameObject.activeSelf)
        {
        //    GemsAvailableText.text = "PRESTIGE\n+" + Methods.NotationMethod(data.gemsAvailable, "F0") + " Gems";
            Methods.BigDoubleFill(data.count, Pow((1e5 / 150), 2), PrestigeUpgrBar);
        }

        void UI()
        {
            if (!prestige.gameObject.activeSelf) return;
            for (var i = 0; i < costText.Length; i++)
            {
                costText[i].text = $"{costDest[i]}\nLevel: {Methods.NotationMethod(levels[i], "F0")}\nCost: {Methods.NotationMethod(costs[i], "F0")} Gems";
                Methods.BigDoubleFill(game.data.gems, costs[i], costBars[i]);
            }
        }
    }

    public void BuyUpgrade(int id)
    {
        var data = game.data;

        switch(id)
        {
            case 0:
                Buy(ref data.PUlevel1);
                break;
            case 1:
                Buy(ref data.PUlevel2);
                break;
            case 2:
                Buy(ref data.PUlevel3);
                break;
        }

        void Buy(ref int level)
        {
            if (data.gems < costs[id]) return;
            data.gems -= costs[id];
            level++;
        }
    }

    public void ArrayManager()
    {
        costs[0] = asClickUpgr;
        costs[1] = asTimeUpgr;
        costs[2] = asGemsUpgr;

        levels[0] = game.data.PUlevel1;
        levels[1] = game.data.PUlevel2;
        levels[2] = game.data.PUlevel3;
    }

    public void Prestige()
    {
        var data = game.data;
        if (data.gemsAvailable >= 1)
        {
            data.count = 0;
            data.ClickValue = 1;

            data.CU1Level = 0;
            data.TU1Level = 0;
            data.CU2Level = 0;
            data.TU2Level = 0;

            data.CU1Cost = 20;
            data.TU1Cost = 50;

            data.CU2Cost = 150;
            data.CU2Power = 5;
            data.TU2Cost = 250;
            data.TU2Power = 3;

            data.gems++;
        }
    }

    public BigDouble TotalGemsBoost()
    {
        var temp = game.data.gems;
        temp *= 0.05 + levels[2] * 1.1;
        return temp + 1;
    }
}