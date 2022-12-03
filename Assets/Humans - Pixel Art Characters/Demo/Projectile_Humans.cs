using UnityEngine;
using System.Collections;

public class Projectile_Humans : MonoBehaviour
{
    [SerializeField] private string _target, _target_2;
    [SerializeField] float speed = 1.0f;

    private Rigidbody2D body2d;

    // Use this for initialization
    void Start()
    {
        body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body2d.velocity = new Vector2(speed * transform.localScale.x, body2d.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_target) || collision.CompareTag(_target_2)){
            collision.gameObject.GetComponent<HeroKnight>()?.Hurt((-(int)transform.localScale.x));
            collision.gameObject.GetComponent<Priest>()?.Hurt((-(int)transform.localScale.x));
            collision.gameObject.GetComponent<Crossbowman>()?.Hurt();
            collision.gameObject.GetComponent<EnemyKnight>()?.Hurt();
            Destroy(gameObject);
        }
            

    }
}
