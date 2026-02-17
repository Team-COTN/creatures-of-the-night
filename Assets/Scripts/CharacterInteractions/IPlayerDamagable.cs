using UnityEngine;

public interface IPlayerDamagable
{
    void PlayerTakeDamage(int damageAmount);
    void EnterDamage(Vector2 hazardPosition); 

}
