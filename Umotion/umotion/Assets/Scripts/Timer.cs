using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{

    public Text timerText;

    private float time = 0.0f;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        timerText.text = time.ToString("F2");
    }
}
