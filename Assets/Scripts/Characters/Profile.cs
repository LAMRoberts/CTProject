using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyInfo
{
    public bool isBoss;
    public float difficulty;
    public int kills;

    public EnemyInfo(bool boss, float hardness, int playerKills)
    {
        isBoss = boss;
        difficulty = hardness;
        kills = playerKills;
    }
}

public class Profile : MonoBehaviour
{
    // player archetypes
    //private float explorer = 0.0f;
    //private float killer = 0.0f;
    //private float achiever = 0.0f;
    //private float socialiser = 0.0f;


    // explorer
        // main rooms
    public float floorLength = 10.0f;                       // how many room on next floor
    public float sideRoomCount = 2.0f;                      // how many side rooms on next floor

    private float totalSearchableRooms = 0.0f;              // number of searchable rooms generated
    private float roomsSearched = 0.0f;                     // number of rooms searched
    private float roomSearchProbability = 0.0f;             // probability of player to search room

        // side rooms
    private int totalSideRooms = 0;                         // number of side rooms generated
    private int sideRoomsExplored = 0;                      // number of side rooms entered


    // killer
    public float enemyProbability = 20.0f;                  // how likely an enemy is to be spawned in each room on the next floor
    public int maxEnemiesOnFloor = 10;                      // maximum limit to spawned enemies on the next floor
    public int maxEnemyCount = 1;                           // maximum number of enemies to spawn in one room

    private int totalEnemiesSpawned = 0;                    // total enemies spawned
    private int enemiesKilled = 0;                          // total enemies killed
    private float enemyKillProbability = 0.0f;              // probability that the player will kill an enemy
    private float highestDifficultyEnemyKilled = 0.0f;      // highest level enemy player has killed

    private float highestDifficultyBossKilled = 0.0f;       // highest level boss player has killed


    // achiever
    private int sideRoomsComplete = 0;                      // number of side rooms completed
    private float sideRoomCompleteProbability = 0.0f;       // probability that the player will complete any side room
    private float sideRoomAverageLikelihood = 0.0f;         // the average chest likelihood of side rooms that player has entered


    // socializer 


    #region Explorer

    // generation
    public void AddRoomsToTotal(int numberOfRooms)
    {
        totalSearchableRooms += numberOfRooms;

        RecalculateRoomSearchProbability();
    }

    public void AddSideRoomToTotal()
    {
        totalSideRooms = totalSideRooms + 1;

        RecalculateSideRoomCompleteProbability();
    }

    // play
    public void SearchedRoom()
    {
        roomsSearched++;

        RecalculateRoomSearchProbability();
    }

    public void ExploredSideRoom()
    {

    }

    public void CompletedSideRoom()
    {
        sideRoomsComplete = sideRoomsComplete + 1;

        RecalculateSideRoomCompleteProbability();
    }

    // regeneration
    private void RecalculateRoomSearchProbability()
    {
        roomSearchProbability = (roomsSearched / totalSearchableRooms) * 100.0f;
    }

    private void RecalculateSideRoomCompleteProbability()
    {
        sideRoomCompleteProbability = (sideRoomsComplete / totalSideRooms) * 100;
    }

    public void SetExplorer()
    {
        if (sideRoomCompleteProbability > 75.0f)
        {
            floorLength = floorLength + 5;

            sideRoomCount = sideRoomCount + 2;
        }
        else if (sideRoomCompleteProbability < 75.0f && sideRoomCompleteProbability > 50.0f)
        {
            floorLength = floorLength + 3;

            sideRoomCount = sideRoomCount + 1;
        }
        //else if (sideRoomCompleteProbability < 50.0f && sideRoomCompleteProbability > 25.0f)
        //{
        //    floorLength = floorLength * 1.1f;

        //    sideRoomCount = sideRoomCount * 1.1f;
        //}
        else
        {
            if (floorLength > 8.0f)
            {
                floorLength = floorLength * 0.8f;
            }

            if (sideRoomCount > 2.0f)
            {
                sideRoomCount = sideRoomCount * 0.8f;
            }
        }
    }

    #endregion

    #region Killer

    // killing enemies and bosses
    public void KilledEnemy(EnemyInfo info)
    {
        if (!info.isBoss)
        {
            enemiesKilled++;

            RecalculateEnemyKillProbability();

            if (info.difficulty > highestDifficultyEnemyKilled)
            {
                highestDifficultyEnemyKilled = info.difficulty;
            }
        }
        else
        {
            if (info.difficulty > highestDifficultyBossKilled)
            {
                highestDifficultyBossKilled = info.difficulty;
            }
        }
    }

    public void AddEnemiesToTotal(int numberOfEnemies)
    {
        totalEnemiesSpawned += numberOfEnemies;

        RecalculateEnemyKillProbability();
    }

    private void RecalculateEnemyKillProbability()
    {
        sideRoomCompleteProbability = (sideRoomsComplete / totalSideRooms) * 100.0f;
    }

    #endregion

    #region Achiever

    #endregion

    #region Socialiser

    #endregion
}
