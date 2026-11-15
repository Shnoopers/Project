using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private float _timer = 0f;
    protected int _health = 100;
    protected Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    // 
    protected virtual void Update()
    {
        if (_health <= 0)
        {
            OnDeath();
            _health = 100;
        }
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
    }
    public void Damage()
    {
        if (_timer > 0) return;
        Debug.Log("Damage");
        DamageBehavior();
        _timer = 0.5f;
    }
    protected abstract void DamageBehavior();

    protected void OnDeath()
    {
        Respawn();
    }

    protected abstract void Respawn();
}
