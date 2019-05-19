using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Jump;

    public AudioSource moveAudio;

    private Rigidbody2D rb2d;
    private bool isServing = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.playersCanMove) return;
        var horizontal = 0f;

        if (Left == KeyCode.None || Right == KeyCode.None)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > float.Epsilon)
            {
                horizontal = 1;
            }
            else if (horizontal < -float.Epsilon)
            {
                horizontal = -1;
            }
        }
        else if (Input.GetKey(Left))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(Right))
        {
            horizontal = 1f;
        }

        var vertical = 0f;
        if (Jump == KeyCode.None)
        {
            vertical = Input.GetAxisRaw("Jump");
            if (vertical > float.Epsilon)
            {
                vertical = 1;
            }
        }
        else if (Input.GetKey(Jump))
        {
            vertical = 1f;
        }

        if (horizontal != 0 && rb2d.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            moveAudio.Play();
            rb2d.AddForce(new Vector3(horizontal, 0, 0) * 10);
        }

        if (isServing && vertical == 0f)
        {
            isServing = false;
            var ball = GameObject.Find("Ball");
            ball.transform.parent = null;
            ball.GetComponent<Collider2D>().enabled = true;
            var ballRb2D = ball.GetComponent<Rigidbody2D>();
            ballRb2D.bodyType = RigidbodyType2D.Dynamic;
        }

        if (vertical != 0 && rb2d.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            moveAudio.Play();

            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(Vector2.up * 200);


            if (GameManager.instance.playerServing == this.gameObject && GameManager.instance.playersCanServe)
            {
                GameManager.instance.playerServing = null;
                isServing = true;
            }
        }
    }
}
