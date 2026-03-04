using UnityEngine;

public interface IPlayerShootable
{
    void TakeShotDamage(int damageAmount);
    void EnterDamage(Vector2 hazardPosition); 

}
