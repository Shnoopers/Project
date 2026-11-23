using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class Player : Entity
{
    
    [SerializeField] private Volume _damageVolume;
    const float DAMAGE_EFFECT_DURATION = 0.5f;
    const float DAMAGE_EFFECT_INTENSITY = 0.33f;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Image _attackBar;
    private float _cooldown = 0f;
    private const float MAX_COOLDOWN = 0.5f;
    private Vector2 _moveInput;
    private Vector2 _playerPos;
    private Vignette _vignette;
    private Coroutine _damageEffectCoroutine;
    const float BLINK_INTERVAL = 0.05f;
    private SpriteRenderer _spriteRenderer;
    private const float INVINCIBILITY_DURATION = 0.5f;

    public bool IsInvincible { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerPos = transform.position;
        _damageVolume.profile.TryGet<Vignette>(out _vignette);
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    protected override void Update()
    {


        base.Update();
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pause.PauseGame();
        }
        if (_cooldown > 0f)
        {
            _attackBar.fillAmount = Mathf.Clamp01(_cooldown / MAX_COOLDOWN); 
            _cooldown -= Time.deltaTime;
        }
        else
        {
            _attackBar.fillAmount = 0f;
        }
        transform.Translate(_moveInput * Time.deltaTime * 20);
        if (Keyboard.current.wKey.wasPressedThisFrame && IsGrounded())
        {
            Debug.Log("jump");
            rb.linearVelocityY = _jumpForce;
        }
        if (Keyboard.current.aKey.isPressed)
        {if (IsGrounded() == true || _cooldown <= 0f)
        {
            rb.linearVelocityX = -4;
        }
        }
        if (Keyboard.current.dKey.isPressed)
        {if (IsGrounded() == true || _cooldown <= 0f)
        {
            rb.linearVelocityX = 4;
        }
        }
        if ((Keyboard.current.aKey.isPressed && Keyboard.current.dKey.isPressed) || (Keyboard.current.aKey.isPressed == false && Keyboard.current.dKey.isPressed == false))
        {
            if (IsGrounded() == true)
            {
            rb.linearVelocityX = 0;
            }
        }
        if (Keyboard.current.spaceKey.isPressed && _cooldown <= 0f)
        {
            _cooldown = MAX_COOLDOWN;
            Attack();
        }



    }

    protected override void Respawn()
    {
        transform.position = _playerPos;
        rb.linearVelocity = Vector2.zero;
        
    }

    private void Attack()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -UnityEngine.Camera.main.transform.position.z));
        Vector2 aimDirection = (mouseWorldPosition - transform.position).normalized;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 2f, _enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            Vector2 enemyDirection = (enemy.bounds.center - transform.position).normalized;
            if (Vector2.Angle(aimDirection, enemyDirection) <= 30f)
            {
                enemy.GetComponent<Enemy>().Damage();
            }
        }  
            
        Collider2D[] maps = Physics2D.OverlapBoxAll(transform.position, new Vector2(2.5f, 1.4f), 0f, _groundLayer);
        foreach (Collider2D map in maps)
        {
            if (map != null && !IsGrounded())
            {      
                Vector2 groundDirection = ((Vector2)transform.position - map.ClosestPoint(transform.position)).normalized ;
                if (Vector2.Angle(aimDirection, groundDirection) <= 30f)
                {
                    continue;
                }
                
                float horizontalDirection = transform.position.x - map.bounds.center.x;
                Vector2 direction = new Vector2(horizontalDirection, 15f).normalized;
                rb.AddForce(direction * 8f, ForceMode2D.Impulse);
                rb.linearVelocityY += 8f;
                break;
            }          
        }
        
        Debug.Log(mouseWorldPosition);
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector2(2.5f, 1.4f));
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(_groundCheck.position, new Vector2(0.999f, 0.01f), CapsuleDirection2D.Horizontal, 0f, _groundLayer) || Physics2D.OverlapCapsule(_groundCheck.position, new Vector2(0.999f, 0.01f), CapsuleDirection2D.Horizontal, 0f, _enemyLayer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnDeath();
    }
     protected override void DamageBehavior()
    {
        if (IsInvincible) return;
        _health -= 10;
        rb.linearVelocity = new Vector2(0, 10);
        Debug.Log(_health);
        if (_damageEffectCoroutine != null)
        {
            StopCoroutine(_damageEffectCoroutine);
        }
        _damageEffectCoroutine = StartCoroutine(DamageEffect());
        if (!IsInvincible) StartCoroutine(BlinkEffect());
    }
    private IEnumerator DamageEffect()
    {
        _vignette.intensity.value = DAMAGE_EFFECT_INTENSITY; 
        float elapsedTime = 0f;
        while (elapsedTime < DAMAGE_EFFECT_DURATION)
        {
            elapsedTime += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(DAMAGE_EFFECT_INTENSITY, 0f, Mathf.Clamp01(elapsedTime / DAMAGE_EFFECT_DURATION));
            yield return null;
        }
    }
    private IEnumerator BlinkEffect()
    {
        IsInvincible = true;
        float elapsedTime = 0f;
        Color originalColor = _spriteRenderer.color;
        while (elapsedTime < INVINCIBILITY_DURATION)
        {
            float alpha = Mathf.PingPong(elapsedTime/BLINK_INTERVAL, 1f);
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.5f, 1f, alpha));
            yield return new WaitForSeconds(BLINK_INTERVAL);
            elapsedTime += BLINK_INTERVAL;
        }
        _spriteRenderer.enabled = true;
        IsInvincible = false;
    } 


}