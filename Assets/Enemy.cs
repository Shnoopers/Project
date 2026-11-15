using UnityEngine;

public class Enemy : Entity
{
        private float _attackTimer = 0f;
       [SerializeField] private Player _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Initialize(Player player)
    {
        _player = player;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
         if (_attackTimer > 0f)
        {
            _attackTimer -= Time.deltaTime;
        
        }
    }
    void FixedUpdate()
    {
        if (_player.transform.position.x < transform.position.x)
        {
            rb.linearVelocityX -= 0.2f;
        }
        else if (_player.transform.position.x > transform.position.x)
        {
            rb.linearVelocityX += 0.2f;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_attackTimer <= 0f && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Damage();
            _attackTimer = 2f;
        }
    }
    protected override void Respawn()
    {
       transform.position = new Vector2(0, 0);
       _health = 100;
    }
    public void RespawnEnemy()
    {
        Respawn();
    }
    protected override void DamageBehavior()
    {
        _health -= 10;
        rb.linearVelocity = new Vector2(-3 * rb.linearVelocityX, 0);
        Debug.Log(_health);
    }
}
