using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTeleport : MonoBehaviour
{
    private bool inSideRoom = false;

    public bool GetSideRoom()
    {
        return inSideRoom;
    }

    public void SetSideRoom(bool sr)
    {
        inSideRoom = sr;
    }
}
