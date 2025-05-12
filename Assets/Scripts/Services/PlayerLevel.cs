using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    public static int playerLevel;
    public static int totalXp;              // amount of xp that the player has earned in total
    public static int xpForNextLevel;       // amount of xp that the player needs in total to reach the next level
    public static int xpDelta;              // the difference in xp between current level n and n+1
    public static int gainedXp = 0;


    public const int correctAnswerXp = 15;
    public const int falseAnswerXp = 5;
    public const int dailyTaskXp = 45;
    public const int bronceAchievementXp = 100;
    public const int silverAchievementXp = 300;
    public const int goldAchievementXp = 700;



    // loads level data from player prefs and initializes data if no data exists yet
    public static void FetchData()
    {
        //print("Fetching data from level system");

        if (PlayerPrefs.GetInt("PlayerLevel", -1) == -1)
        {
            //print("Setting up Level System");

            PlayerPrefs.SetInt("PlayerLevel", 1);
            PlayerPrefs.SetInt("TotalXp", 0);
            PlayerPrefs.SetInt("XpForNextLevel", 100);
            PlayerPrefs.SetInt("XpDelta", 100);
            PlayerPrefs.Save();
        }

        playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        totalXp = PlayerPrefs.GetInt("TotalXp");
        xpForNextLevel = PlayerPrefs.GetInt("XpForNextLevel");
        xpDelta = PlayerPrefs.GetInt("XpDelta");
        //print($"Received Data: Level {playerLevel}, {totalXp} XP, {xpForNextLevel} needed, {xpDelta} delta");
    }


    // adds xp to current player stats, triggers popup if player reaches new level
    // call this method after a player exits a game session
    public static void AddPlayerXp(GameObject newLevelPopup = null, TextMeshProUGUI popupMessage = null)
    {
        //print($"Adding {gainedXp} to player");
        totalXp += gainedXp;
        bool reachedNewLevel = false;

        while (totalXp >= xpForNextLevel )
        {
            playerLevel++;
            //print("New Level: " + playerLevel);
            reachedNewLevel = true;

            if (playerLevel > 1 && playerLevel <= 4)
            {
                xpDelta = (int)Math.Floor(xpDelta * 1.5f);
            }

            if (playerLevel > 4 && playerLevel <= 9)
            {
                xpDelta = (int)Math.Floor(xpDelta * 1.2f);
            }

            if (playerLevel > 9 && playerLevel <= 35)
            {
                xpDelta = (int)Math.Floor(xpDelta * 1.025f);
            }

            xpForNextLevel += xpDelta;
        }

        Save(playerLevel, xpForNextLevel, totalXp, xpDelta);
        gainedXp = 0;

        if (reachedNewLevel)
        {
            if (newLevelPopup != null)
            {
                //print("Trigger Popup");
                newLevelPopup.gameObject.SetActive(true);
                if (popupMessage != null)
                {
                    popupMessage.text = $"Du hast ein neues Level erreicht:\n\n" +
                        $"Level {playerLevel}";
                }
            }
        }
    }


    // adds up gained xp during a game session
    public static void GainXp(int newXp)
    {
        //print("Gained XP: " + newXp);
        gainedXp += newXp;
    }


    public static void Save(int playerLevel, int xpForNextLevel, int currentXp, int xpDelta)
    {
        //print($"Saving data: Level {playerLevel}, {currentXp} XP, {xpForNextLevel} needed, {xpDelta} delta");
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("XpForNextLevel", xpForNextLevel);
        PlayerPrefs.SetInt("TotalXp", currentXp);
        PlayerPrefs.SetInt("XpDelta", xpDelta);
        PlayerPrefs.Save();
    }
}
