using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public List<Collider2D> ignoreColliders;
    public AudioSource hitAudio;
    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ignoreColliders.ForEach(x => Physics2D.IgnoreCollision(GetComponent<Collider2D>(), x));

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public void PrepareToServe(GameObject player, bool right)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(player.transform);


        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        transform.position = player.transform.position + new Vector3(0.5f * (right ? 1f : -1f), -0.1f);

        GetComponent<TrailRenderer>().colorGradient = new Gradient()
        {
            colorKeys = new[] {
                new GradientColorKey(right ? Color.red : Color.blue, 0f)
              }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (rb2d.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            foreach (var foo in GameObject.FindGameObjectsWithTag("Player1Point"))
            {
                if (rb2d.IsTouching(foo.GetComponent<Collider2D>()))
                {
                    GameManager.instance.ScorePlayer1();

                    GetComponent<Animator>().SetTrigger("BallGrow");
                }
            }
            foreach (var foo in GameObject.FindGameObjectsWithTag("Player2Point"))
            {
                if (rb2d.IsTouching(foo.GetComponent<Collider2D>()))
                {
                    GameManager.instance.ScorePlayer2();

                    GetComponent<Animator>().SetTrigger("BallGrow");
                }
            }
        }
        else if (rb2d.IsTouchingLayers(LayerMask.GetMask("Air")))
        {
            foreach (var foo in GameObject.FindGameObjectsWithTag("Player1Point"))
            {
                if (rb2d.IsTouching(foo.GetComponent<Collider2D>()))
                {
                    GameManager.instance.ScorePlayer1();
                    GetComponent<Animator>().SetTrigger("BallGrow");
                }
            }
            foreach (var foo in GameObject.FindGameObjectsWithTag("Player2Point"))
            {
                if (rb2d.IsTouching(foo.GetComponent<Collider2D>()))
                {
                    GameManager.instance.ScorePlayer2();
                    GetComponent<Animator>().SetTrigger("BallGrow");
                }
            }
        }
        else if (rb2d.IsTouchingLayers(LayerMask.GetMask("Player1")))
        {
            hitAudio.Play();
            rb2d.velocity = new Vector2(Mathf.Abs(rb2d.velocity.x), Mathf.Abs(rb2d.velocity.y) + 0.1f);
            GetComponent<TrailRenderer>().colorGradient = new Gradient()
            {
                colorKeys = new[] {
                new GradientColorKey(Color.red, 0f)
              },
                alphaKeys = new[]{
                new GradientAlphaKey(0.8f, 0f)
              }
            };
        }
        else if (rb2d.IsTouchingLayers(LayerMask.GetMask("Player2")))
        {
            hitAudio.Play();
            rb2d.velocity = new Vector2(-Mathf.Abs(rb2d.velocity.x), Mathf.Abs(rb2d.velocity.y) + 0.1f);
            GetComponent<TrailRenderer>().colorGradient = new Gradient()
            {
                colorKeys = new[] {
                new GradientColorKey(Color.blue, 0f)
              },
                alphaKeys = new[]{
                new GradientAlphaKey(0.8f, 0f)
              }
            };
        }
    }
}
