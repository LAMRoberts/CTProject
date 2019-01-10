using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int currentLevel = 0;

    int GetLevel()
    {
        return currentLevel;
    }

    void Setlevel(int level)
    {
        currentLevel = level;
    }
}
