using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Champion : MonoBehaviour
{
    public abstract void Attack_Normal(Champion target);

    public abstract void Attack_FixedDamage();

    public abstract void Burst();

    public abstract void TakeDamage_Normal(int damage);

    public abstract void TakeDamage_FixedDamage(int damage);

    public abstract void GetHealthByPercent(int percent);
    public abstract void SetCurrentTile(TileManager tile);
    public abstract int GetRange();
    public abstract int GetTarget();
    public abstract int GetHP();
    public abstract void SetHP();
}
