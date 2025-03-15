using UnityEngine;

public interface IDamageable
{
    int Health { set; get; }
    bool Targetable { set; get; }
    void OnHit(int damage, Vector3 knockback, GameObject hit);
    void RemoveCharacter();
}
