using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyInfo
{
    public bool isBoss;
    public int difficulty;
    public int kills;
}

public class Profile : MonoBehaviour
{
    // player archetypes
    private float explorer = 0.0f;
    private float killer = 0.0f;
    private float achiever = 0.0f;
    private float socialiser = 0.0f;

    // explorer
    public int floorLength = 10;
    public int sideRoomCount = 2;
    public float sideRoomProbability = 50.0f;

    private int totalRooms = 0;
    private int roomsSearched = 0;
    [SerializeField]
    private float roomSearchProbability = 0.0f;

    public int numberOfSideRooms = 2;

    private int totalSideRooms = 0;
    private int sideRoomsComplete = 0;
    [SerializeField]
    private float sideRoomCompleteProbability = 0.0f;

    // killer
    public float enemyProbability = 20.0f;
    public int maxEnemiesOnFloor = 10;
    public int maxEnemyCount = 1;

    private int totalEnemiesSpawned = 0;
    private int enemiesKilled = 0;
    [SerializeField]
    private float enemyKillProbability = 0.0f;
    private int highestDifficultyEnemyKilled = 0;
    private int highestDifficultyBossKilled = 0;

    // achiever

    // socializer

    #region Explorer

    // searching rooms
    public void SearchedRoom()
    {
        roomsSearched++;

        RecalculateRoomSearchProbability();
    }

    public void AddRoomsToTotal(int numberOfRooms)
    {
        totalRooms += numberOfRooms;

        RecalculateRoomSearchProbability();
    }

    private void RecalculateRoomSearchProbability()
    {
        roomSearchProbability = (roomsSearched / totalRooms) * 100;
    }
       
    // exploring side rooms
    public void CompletedSideRoom()
    {
        sideRoomsComplete++;

        RecalculateSideRoomCompleteProbability();
    }

    public void AddSideRoomToTotal()
    {
        totalSideRooms++;
    }

    private void RecalculateSideRoomCompleteProbability()
    {
        sideRoomCompleteProbability = (sideRoomsComplete / totalSideRooms) * 100;
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
        sideRoomCompleteProbability = (sideRoomsComplete / totalSideRooms) * 100;
    }

    #endregion

    #region Achiever

    #endregion

    #region Socialiser

    #endregion
}
