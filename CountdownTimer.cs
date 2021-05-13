using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class CountdownTimer : MonoBehaviour
{
    public float currentTime = 0f;
    public float startingTime; //this is the set time to be connected 
    public string inputTime;
    public GameObject inputField;


    public bool timerstart; //initialize time when called
    public AudioClip soundToPlay;
    public AudioSource audio;
    public float Volume;
    public bool alreadyPlayed = false;
    public bool paused = false;
    public bool soundAlarm = true;


    [SerializeField] Text countDownText; //SerializeFiled is private however it shows up as a variable in inspector tab

    private string input;
    
    public void StoreTimer()
    {
        input = inputField.GetComponent<Text>().text;
        print(input);

    }

    public void setTimer()
    {
        //startingTime = float.Parse(input);
        float starttimeFloat = float.Parse(input);
        currentTime = starttimeFloat;
        print("StarttimeFloat" + starttimeFloat + "StartingTime" + startingTime);

    }

    void Start() 
    {
        timerstart = false;
        audio = GetComponent<AudioSource>();
        currentTime = startingTime;
       
    }
    public void TimerStartFunction()
    {
        timerstart = true;
        paused = false;
        soundAlarm = true;
    }
    
    public void PauseFunction()
    {
        paused = true;
    }
    
    public void cancelAlarm()
    {
        soundAlarm = false;
        paused = true; 
        countDownText.text = "0";
        currentTime = 0;
    }
 


    void Update()
    {   if (timerstart == true && paused == false)
        {
            currentTime -= 1 * Time.deltaTime; //lowers timer 
            countDownText.text = currentTime.ToString("0"); //converts timeText to string 
        }



        if (currentTime <= 0 && soundAlarm == true)
        {
            currentTime = 0;

            if (alreadyPlayed == false)
            {
                audio.PlayOneShot(soundToPlay, Volume);
                alreadyPlayed = true;
                StartCoroutine(ExampleCoroutine());
               
            }
            //events here 

        }
    
    }
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("After Waiting 5 Seconds");
        SceneManager.LoadScene("ChessGame");
    }

}

