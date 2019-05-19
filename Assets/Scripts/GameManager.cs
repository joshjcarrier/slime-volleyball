using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public bool playersCanMove;
    [HideInInspector] public bool playersCanServe;
    public AudioClip scoreSfx;
    public AudioClip backgroundMusic;

    [HideInInspector] public GameObject playerServing;

    private Text bannerText;
    private Text player1Text;
    private Text player2Text;
    private Text timerText;

    private int player1Score;
    private int player2Score;
    private float timeRemaining;

    [HideInInspector] public bool Shaking;
    private float ShakeDecay;
    private float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        bannerText = GameObject.Find("Banner").GetComponent<Text>();
        player1Text = GameObject.Find("Points Player1").GetComponent<Text>();
        player2Text = GameObject.Find("Points Player2").GetComponent<Text>();
        timerText = GameObject.Find("Timer").GetComponent<Text>();

        playersCanMove = false;
        bannerText.text = "A      left    J\nD     right    L\nSPACE  jump    /\n\nlet go jump to serve";
        timerText.text = "PRESS ENTER";
        player1Text.text = "";
        player2Text.text = "";
    }

    public void Reset()
    {
        playersCanMove = true;
        player1Score = 0;
        player2Score = 0;
        timeRemaining = 140; // 2 min 20 sec is about as long as the theme music
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(backgroundMusic);
        Reset(GameObject.Find("Player1"), true);
    }

    public void Reset(GameObject player, bool right)
    {
        timerText.GetComponent<ParticleSystem>().Stop();
        var ball = GameObject.Find("Ball");
        var ballScript = ball.GetComponent<Ball>();
        playerServing = player;

        player1Text.text = player1Score.ToString().PadLeft(2, '0');
        player2Text.text = player2Score.ToString().PadLeft(2, '0');

        ballScript.PrepareToServe(player, right);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        playersCanServe = false;
        yield return new WaitForSeconds(3);

        if (playersCanMove)
        {
            bannerText.text = "Ready?";
            yield return new WaitForSeconds(1);
            if (playersCanMove)
            {
                playersCanServe = true;
                bannerText.text = "Slime Volleyball";
                timerText.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    IEnumerator EndGame()
    {
        playersCanMove = false;

        var ball = GameObject.Find("Ball");
        var ballScript = ball.GetComponent<Ball>();
        player1Text.text = player1Score.ToString().PadLeft(2, '0');
        player2Text.text = player2Score.ToString().PadLeft(2, '0');
        ballScript.PrepareToServe(GameObject.Find("Player1"), true);
        timerText.text = "TIME UP";

        if (player1Score > player2Score)
        {
            bannerText.text = "RED wins!";
        }
        else if (player1Score < player2Score)
        {
            bannerText.text = "BLUE wins!";
        }
        else
        {
            bannerText.text = "TIE!";
        }
        yield return new WaitForSeconds(10);
        bannerText.text = "A      left    J\nD     right    L\nSPACE  jump    /\n\njump to serve";
        timerText.text = "PRESS ENTER";
        player1Text.text = "";
        player2Text.text = "";
    }

    public void ScorePlayer1()
    {
        DoShake();
        GetComponent<AudioSource>().PlayOneShot(scoreSfx);
        player1Score++;
        GameObject.Find("Points Player1").GetComponent<Animator>().SetTrigger("PointScored");
        bannerText.text = "RED point!";
        Reset(GameObject.Find("Player1"), true);
    }

    public void ScorePlayer2()
    {
        DoShake();
        GetComponent<AudioSource>().PlayOneShot(scoreSfx);
        player2Score++;
        GameObject.Find("Points Player2").GetComponent<Animator>().SetTrigger("PointScored");
        bannerText.text = "BLUE point!";
        Reset(GameObject.Find("Player2"), false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (ShakeIntensity > 0)
        {
            transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
            transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

            ShakeIntensity -= ShakeDecay;
        }
        else if (Shaking)
        {
            Shaking = false;
        }

        if (!playersCanMove)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                Reset();
            }
            else
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.Backspace))
        {
            StartCoroutine(EndGame());
            return;
        }


        timeRemaining = Mathf.Max(timeRemaining - Time.deltaTime, 0f);
        if (timeRemaining >= 60)
        {
            timerText.text = Mathf.Floor(timeRemaining / 60).ToString().PadLeft(2, '0') + ":" + ((int)timeRemaining % 60).ToString().PadLeft(2, '0');
        }
        else
        {
            timerText.text = timeRemaining.ToString("0.00");
        }

        if (timeRemaining == 0)
        {
            StartCoroutine(EndGame());

        }


    }
    public void DoShake()
    {
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;

        ShakeIntensity = 0.3f;
        ShakeDecay = 0.005f;
        Shaking = true;
    }
}
