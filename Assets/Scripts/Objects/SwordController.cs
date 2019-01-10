using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SwordState
{
    idle = 0,
    charging = 1,
    attacking = 2,
    resetting = 3
}

public class SwordController : MonoBehaviour
{
    private SwordTrail tr;

    public Material unchargedColour;
    public Material chargedColour;
    public Material swordMaterial;

    public Transform swordIdleTransform;
    public Transform swordStartTransform;
    public Transform swordEndTransform;

    public float chargingStep = 0.5f;
    public float attackingStep = 1.0f;
    public float resettingStep = 1.0f;

    public float chargingRotation = 0.5f;
    public float attackingRotation = 1.0f;
    public float resettingRotation = 1.0f;

    private SwordState state = SwordState.idle;

    private void Start()
    {
        tr = GetComponentInChildren<SwordTrail>();        
    }

    public void UpdateColour(float percent)
    {
        swordMaterial.color = Color.Lerp(unchargedColour.color, chargedColour.color, percent);
    }

    public bool UpdateSword(bool attacking, bool charging)
    {
        tr.Trail(false);

        switch (state)
        {
            case SwordState.idle:
                {
                    if (attacking && charging)
                    {
                        state = SwordState.charging;

                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            case SwordState.charging:
                {
                    transform.position = Vector3.MoveTowards(transform.position, swordStartTransform.position, chargingStep);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, swordStartTransform.rotation, chargingStep * chargingRotation);

                    if (attacking && !charging)
                    {
                        state = SwordState.attacking;
                    }

                    return true;
                }
            case SwordState.attacking:
                {
                    transform.position = Vector3.MoveTowards(transform.position, swordEndTransform.position, attackingStep);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, swordEndTransform.rotation, attackingStep * attackingRotation);

                    if (transform.position == swordEndTransform.position)
                    {
                        state = SwordState.resetting;
                    }

                    tr.Trail(true);

                    return true;
                }
            case SwordState.resetting:
                {
                    transform.position = Vector3.MoveTowards(transform.position, swordIdleTransform.position, resettingStep);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, swordIdleTransform.rotation, resettingStep * resettingRotation);

                    if (transform.position == swordIdleTransform.position)
                    {
                        state = SwordState.idle;
                    }

                    return true;
                }
        }

        return false;
    }
}
