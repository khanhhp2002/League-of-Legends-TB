using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Sprite[] _LaneIcons;
    public static SpriteManager Instance;

    public void Awake()
    {
        Instance = this;
    }
}
