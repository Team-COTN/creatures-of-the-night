
using System;
using UnityEngine;
using System.Collections;

public class TeleportableHazard : MonoBehaviour
{
    public int hazardStrength;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IPlayerTeleportable>(out IPlayerTeleportable playerTeleportable))
        {
            playerTeleportable.PlayerTakeTeleportDamage(hazardStrength);
        }
    }
}
