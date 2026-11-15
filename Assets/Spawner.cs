using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private int spawntimer = 600;
    private int spawncount = 0;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Player _player;

    private List<GameObject> _enemies = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

       
    }

    // Update is called once per frame
    void Update()
    {
        spawntimer--;
        if (spawntimer <= 0 && spawncount < 3)
        {
             _enemies.Add(SpawnEnemy(3f, 3f));
            spawntimer = 600;
            spawncount++;
        }
    }
    private GameObject SpawnEnemy(float x, float y)
    {
        GameObject newEnemy = Instantiate(_enemyPrefab, new Vector2(x, y), Quaternion.identity);
        newEnemy.GetComponent<Enemy>().Initialize(_player);
        return newEnemy;
    }
    public void RespawnEnemies()
    {
        spawncount--;
        foreach (GameObject enemy in _enemies)
        {
        //    enemy.GetComponent<Enemy>().RespawnEnemy();
        }
    }
}
