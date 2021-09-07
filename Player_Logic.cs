using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Logic : MonoBehaviour
{
    public int Lives_Initial = 3;
    public int Lives;
    public float InvincibilityDuration = 1f;

    private int lastCheckPoint = 0;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Lives = Lives_Initial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<damageSource>()) //Check for component of damage
        {
            StartCoroutine(TakeDamage(new Vector2(collision.GetContact(0).point.x + 0.5f, collision.GetContact(0).point.y + 0.5f), 1f, collision.gameObject.GetComponent<damageSource>()));
        }
    }

    private IEnumerator TakeDamage(Vector3 location, float immunity, damageSource source)
    {
        Vector2 force = (transform.position - location).normalized * source.kb;
        rb.AddForceAtPosition(force, location);
        yield return new WaitForSeconds(immunity);
    }
}
