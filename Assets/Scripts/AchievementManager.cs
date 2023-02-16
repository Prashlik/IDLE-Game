using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;

public class AchievementManager : MonoBehaviour
{
    public IDLEGame game;
    private static string[] AchievementStrings => new string[] { "Current $", "Total $", "Fast Hand", "Silent Thought", "Loyal" };
    private BigDouble[] AchievementsNumbers => new BigDouble[] { game.data.count, game.data.totalcount, game.data.PUlevel1, game.data.PUlevel2, game.data.PUlevel3 };

    public GameObject achievementScreen;
    public List<Achievement> achievementList = new List<Achievement>();

    public void StartAchiev()
    {
        foreach (var x in achievementScreen.GetComponentsInChildren<Achievement>())
            achievementList.Add(x);
    }

    public void RunAchievements()
    {
        var data = game.data;
        UpdateAchievement(AchievementStrings[0], AchievementsNumbers[0], "F2", ref data.AchLv1, ref achievementList[0].fill,
            ref achievementList[0].title, ref achievementList[0].progress);
        UpdateAchievement(AchievementStrings[1], AchievementsNumbers[1], "F2", ref data.AchLv2, ref achievementList[1].fill,
            ref achievementList[1].title, ref achievementList[1].progress);
        UpdateAchievement(AchievementStrings[2], AchievementsNumbers[2], "F0", ref data.AchLv3, ref achievementList[2].fill,
            ref achievementList[2].title, ref achievementList[2].progress);
        UpdateAchievement(AchievementStrings[3], AchievementsNumbers[3], "F0", ref data.AchLv4, ref achievementList[3].fill,
            ref achievementList[3].title, ref achievementList[3].progress);
        UpdateAchievement(AchievementStrings[4], AchievementsNumbers[4], "F0", ref data.AchLv5, ref achievementList[4].fill,
            ref achievementList[4].title, ref achievementList[4].progress);
    }

    private void UpdateAchievement(string name, BigDouble number, string q, ref float level, ref Image fill, ref Text title, ref Text progress)
    {
        var cap = BigDouble.Pow(10, level);

        if (game.AchievementsGroup.gameObject.activeSelf)
        {
            title.text = $"{name}\n ({level})";
            progress.text = $"{Methods.NotationMethod(number, q)} / {Methods.NotationMethod(cap, q)}";

            Methods.BigDoubleFill(number, cap, fill);
        }
        if (number < cap) return;
        BigDouble levels = 0;
        if (number / cap >= 1)
            levels = Floor(Log10(number / cap)) + 1;
        level += (float)levels;
    }
}
