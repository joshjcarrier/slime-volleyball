using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var ball = GameObject.Find("Ball");
        var ballPosition = ball.transform.position;
        var thing = ballPosition - transform.parent.position;
        thing = Vector3.ClampMagnitude(thing, 0.10f);
        transform.position = transform.parent.position + thing;
    }
}
