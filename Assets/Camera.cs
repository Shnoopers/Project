using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private float _smoothSpeed = 0.7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(_player.position.x, _player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position,targetPos, _smoothSpeed * Time.deltaTime);
    }
}
