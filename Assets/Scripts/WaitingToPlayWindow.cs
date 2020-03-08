using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToPlayWindow : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        BirdControl.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
