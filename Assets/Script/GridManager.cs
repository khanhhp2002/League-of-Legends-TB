using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Space(5f), Header("Map Scale")]
    public GameObject _TilePrefabs;
    public Transform _Camera;
    // Start is called before the first frame update
    void Start()
    {
        GridGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GridGenerator()
    {
        for(int i = 8; i > 2 ; i-=2)
        {
            //var spawnedTile = Instantiate(_tile, new Vector3(j, i), Quaternion.identity);
            for(int j = 0; j < 3; j++)
            {
                var spawnedTile1 = Instantiate(_TilePrefabs, new Vector3(i, j * 2), Quaternion.identity);
                var spawnedTile2 = Instantiate(_TilePrefabs, new Vector3(-i, j * 2), Quaternion.identity);
            }
        }
        AlignCamera();
    }

    void AlignCamera()
    {
        _Camera.position = new Vector3(0f, 2f, -10f);

    }
}
