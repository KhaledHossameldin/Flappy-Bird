using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdControl : MonoBehaviour
{
    private const float JUMP_AMOUNT = 100f;

    private Rigidbody2D birdRigidbody2D;

    public static BirdControl Instance;

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private State state;

    public static BirdControl GetInstance()
    {
        return Instance;
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        Instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null)
                    {
                        OnStartedPlaying(this, EventArgs.Empty);
                    }
                }
                break;

            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y*0.15f);
                break;

            case State.Dead:

                break;
        }
    }

    private void Jump()
    {
        if (birdRigidbody2D.bodyType != RigidbodyType2D.Static)
        {
            birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
            SoundManager.PlaySound(SoundManager.Sound.BirdJump);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null)
        {
            OnDied(this, EventArgs.Empty);
        }
    }
}
