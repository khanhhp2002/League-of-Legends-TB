using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissFortune : Champion
{
    public int Atk;
    public int Health;
    private int health;
    public int Def;
    public int Mana;
    public TileManager _Tile;
    public int target;
    public int range;
    public void Awake()
    {
        health = Health;
    }
    public override void Attack_FixedDamage()
    {

    }

    public override void Attack_Normal(Champion target)
    {
        target.TakeDamage_Normal(this.Atk);
    }

    public override void Burst()
    {

    }

    public override void TakeDamage_FixedDamage(int damage)
    {

    }

    public override void TakeDamage_Normal(int damage)
    {
        this.Health -= damage*(100-Def)/100;
        SetHP();
    }

    public override void GetHealthByPercent(int percent)
    {
        throw new System.NotImplementedException();
    }

    public override void SetCurrentTile(TileManager tile)
    {
        this._Tile = tile;
    }

    public override void SetHP()
    {
        _Tile._HP.text = Health.ToString();
    }
    public override int GetRange()
    {
        return this.range;
    }

    public override int GetTarget()
    {
        return this.target;
    }
    public override int GetHP()
    {
        return this.Health;
    }
}
