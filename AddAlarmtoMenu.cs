using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class AddAlarmtoMenu : MonoBehaviour
{
    /// <summary> -------------------------------------------------------------------------------------------------------
    /// will these values be saved to a database? so the alarms are retained when the scene is reloaded?
    /// 
    /// experimental features in the pipeline unsure if necessary:
    /// the creation of alarms lines 166-170, no way to delete them would require a lot of work.
    /// the selection of date MTWTF has no function and probably wont.
    /// no way to add notes atm, not really a issue for now
    /// </summary>-------------------------------------------------------------------------------------------------------
    //buttons these are written seperatly as a single function would consume too much work.
    public GameObject btn1;
    public GameObject btn2;
    public GameObject btn3;
    public GameObject btn4;
    public GameObject btn5;
    public GameObject btn6;
    public GameObject btn7;
    public GameObject btn8;
    public GameObject btn9;
    public GameObject btn10;
    public GameObject btn11;
    public GameObject btn12;
    public GameObject AMbtn;
    public GameObject PMbtn;
    public GameObject SaveButton;
    //end buttons

    public GameObject addAlarmTable; //table storing alarms
    //end game objects


    private int k; //for itterations

    string timeNumb; //time as a number before appending to string then to date-time.
    List<double> Alarms = new List<double>(); //alamrs all in format of double note( if a value is rounded i.e. 4.00->4 double doesnot retain these values they must be concatinated as a string.)
    List<double> AlarmsGreater = new List<double>(); //alarms greater than current time 
    List<string> AlarmsfromFile = new List<string>(); //alarms from file.
    public string TIMESTATE; //for time AM or PM
    public string TIMESELECTEDHOUR; // hour in 12 HR
    public string TIMESELECTED24HOUR; //hour in 24 hour format
    public string TIMESELECTEDMIN; //min
    private string TIMESELECTED; //combined time 12 HR
    private string TIMESELECTED24; //combined time 24 HR 

    private double closestHour; //converts double to time 24HR.
    private double closestMin; //for coversion of double to time.
    private double closestTotal;//closest combined time
    private string closestTotalString; //closest combined type of string


    public GameObject inputField; //might be a useless function
    public GameObject AlarmTable;
    public Transform entryContainer;
    public Transform entryTemplate;
    int i = 0;



    //section for time handlers
    private System.DateTime time; //current time
    private System.DateTime Alarmtime; //unsued i think
    private System.DateTime AlarmTimeClosest; //closest alarm

    //private System.DateTime dateTimeString;

    //bool statements for system handlers

    bool firstClick = false; //used to determine if first click is true 

    bool timestateClickAM = false; //used to toggle between AM and PM Panel Color

    bool secondClick = false; //used to see if the clock should be on the second part

    bool startcountdown = false; //starts countdown for timer to ring!



    //string removing value





    // Update is called once per frame
    // Start is called before the first frame update
    void Start()
    { }
    void OnValidate()
    {
        if (new FileInfo("alarms.txt").Length != 0) //if file isn't empty.
        {

            readtoClock(); //call read times to clock
            Update();
        }
    }
    void Update()
    {
        time = System.DateTime.Now;






        //function to determine the alarm to ring first
        if (startcountdown == true)
        {

            if (AlarmTimeClosest.Hour == time.Hour && AlarmTimeClosest.Minute == time.Minute) //comparison to time and minute using date time function
            {
                print("ALARM RANG!");//sound should be played here

                Alarms.Remove(AlarmsGreater[0]);




                writeTofile(Alarms);
                print("hello!");
                //ExampleCoroutine();

                SceneManager.LoadScene("ChessGame");
                startcountdown = false; //after the alarm rings

               
            }
            //print(time.Hour + time.Minute);

        }

    }
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("After Waiting 1 Seconds");
        
    }
    public void addToMenu() //function adds selected alarms to menu screen THIS IS DOOKIE I DON'T LIKE THIS WHY DID I WRITE THIS ???
    {

        //print("addtoMenuCalled!");
        //input = inputField.GetComponent<Text>().text;
        entryContainer = transform.Find("AlarmEntryContainer");
        entryTemplate = entryContainer.Find("AlarmEnrtyTemplate");

        //entryTemplate.gameObject.SetActive(false);
        float templateHeight = 130f; //distance between alarm segments
        double timeDouble; //variable for time in double format
        Transform entryTransform = Instantiate(entryTemplate, entryContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * k);

        //string concatination



        if (secondClick == false) //if the user has not selected a minuet we will append 00
        {
            TIMESELECTED = TIMESELECTEDHOUR + "00"; //used in menu
            TIMESELECTED24 = TIMESELECTED24HOUR + "00"; //used for backend comparison
            timeNumb = TIMESELECTED24.ToString(); //stores time24 to string timenumb
            timeNumb = timeNumb.Replace(":", "."); //replaces : with . i.e. 2:40->2.4 
            //print(timeNumb);
            k++; //tells itteration for alarm

            //getClosestAlarm();//called inside savebutton

            timeDouble = Convert.ToDouble(timeNumb); //converts string to int
            Alarms.Add(timeDouble);  //appended to alarms array
        }
        else //the user has specified the minute
        {
            TIMESELECTED = TIMESELECTEDHOUR + TIMESELECTEDMIN;
            TIMESELECTED24 = TIMESELECTED24HOUR + TIMESELECTEDMIN;
            timeNumb = TIMESELECTED24.ToString();
            //converts string to double value
            timeNumb = timeNumb.Replace(":", ".");
            timeDouble = Convert.ToDouble(timeNumb);
            //print(timeDouble);

            Alarms.Add(timeDouble);
            k++;
        }
        entryTransform.Find("NoteText").GetComponent<TMP_Text>().text = "Note"; //finds components and creates copies 
        entryTransform.Find("12HRLabelText").GetComponent<TMP_Text>().text = TIMESTATE;
        entryTransform.Find("AlarmTImeText").GetComponent<TMP_Text>().text = TIMESELECTED;
        entryTransform.Find("SMTWTFSText").GetComponent<TMP_Text>().text = "S M T W T F S";

        entryTemplate.gameObject.SetActive(true);//sets the template as visible
        i++;
    }

    public void getClosestAlarm() //returns closest alarm 
    {
        //completed
        Update(); //returns current time
        double currentTime; //value to hold current time


        currentTime = time.Minute; //simple to subtract time from the list 
        currentTime = time.Hour + (currentTime / 100); //new operation due to not converting directly 4:30->4.3 etc.
        //print(currentTime);
        for (i = 0; i < Alarms.Count; i++) //for all alarms in array
        {

            if (Alarms[i] >= currentTime) //if the alarm set is greater than current time append to array
            {
                AlarmsGreater.Add(Alarms[i]); //add the current alarms greater

            }
        }
        //
        AlarmsGreater.Sort(); //sorts alarms by closest to time
        //convert Alarms[0] to date time using string conversion from . to :

        if (AlarmsGreater.Count > 0) //if there exists an alarm greater
        {
            closestTotal = AlarmsGreater[0]; //sets value
            closestTotalString = closestTotal.ToString(); //converts double to string
            closestTotalString = closestTotalString.Replace(".", ":"); //replaces . with : 
            closestTotalString = concatvalue(closestTotalString);// calls concat value for closest string sets return value = to closest total string
            //print(closestTotalString); //debug 
            AlarmTimeClosest = DateTime.Parse(closestTotalString); //parses to date-time 
            print("The closest alarm is set for: " + AlarmTimeClosest); //debug log for closest time
            //pass this to update
            startcount(); //begins alarm countdown
        }
        else //if greater alarms don't exist same functionality except the array selects from alarms.
        {
            Alarms.Sort(); //sort the alarms
            closestTotal = Alarms[0]; //sets value
            closestTotalString = closestTotal.ToString(); //converts double to string
            closestTotalString = closestTotalString.Replace(".", ":");
            closestTotalString = concatvalue(closestTotalString);
            //print(closestTotalString);
            AlarmTimeClosest = DateTime.Parse(closestTotalString);
            print("The closest alarm is set for: " + AlarmTimeClosest);
            //pass this to update
            startcount();

        }
    }
    public string concatvalue(string closestTotalString)
    {
        int colonIndex; //position of colon in the string, used to determine whether or not a 0 should be appended, as trailing 0's don't function when using doubles.

        if (closestTotalString.Contains(":"))
        {
            colonIndex = closestTotalString.IndexOf(':'); //finds position of :

            if (closestTotalString.Substring(colonIndex).Length == 2) //if closesttotalstring from : to endofline is 2 then
            {
                //length is 2 because of :0 therefore for a proper conversion of time another 0 must be appended!
                closestTotalString = closestTotalString + "0";
                return closestTotalString;
            }

        }
        else
        {
            closestTotalString = closestTotalString + ":00";
            return closestTotalString;

        }
        return closestTotalString;
        //print("value concatinated for Double!");

    }

    public void startcount()
    {
        startcountdown = true; //starts countdown i odn't know why i wanted it in a seperate function don't ask :^) 
    }



    public void writeTofile(List<double> list) //write to file based on array<double format>
    {
        TextWriter tw = new StreamWriter("Alarms.txt");
        foreach (double d in list)
        {
            tw.WriteLine(d); //writes float value apx no need for am pm 24hr format
        }
        tw.Close();

    }
    /*
    public string determineTimestate(double value) //unused ment for UI section IGNORE-----
    {
        if (value > 12)
        {
            return "PM";
        }
        else
        {
            return "AM";
        }
    }
    private string changetimeformat(string numberS) //unused ment for UI Section IGNORE -----
    {
        double numberd;
        numberd = Convert.ToDouble(numberS);
        if (numberd > 12)
        {
            numberd = numberd - 12;
            numberS = Convert.ToString(numberd);
            return numberS;
        }
        else
        {
            return numberS;
        }
    }
    */
    /*public void writetoClock()
    {

        //input = inputField.GetComponent<Text>().text;
        entryContainer = transform.Find("AlarmEntryContainer");
        entryTemplate = entryContainer.Find("AlarmEnrtyTemplate");

        //entryTemplate.gameObject.SetActive(false);
        float templateHeight = 130f; //distance between alarm segments
        double timeDouble; //variable for time in double format
        Transform entryTransform = Instantiate(entryTemplate, entryContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);

        //string concatination




        //Alarms.Add(Convert.ToDouble(AlarmsfromFile[i])); //adds alarms to current list of alarms

        TIMESTATE = determineTimestate(Alarms[i]); //determines time state on functioncall
            TIMESELECTED = changetimeformat(AlarmsfromFile[i]); //used to print time 12hr time still in decimal though 12.3 w/e
        
        
        entryTransform.Find("NoteText").GetComponent<TMP_Text>().text = "Note"; //finds components and creates copies 
        entryTransform.Find("12HRLabelText").GetComponent<TMP_Text>().text = "Text";
        entryTransform.Find("AlarmTImeText").GetComponent<TMP_Text>().text = "Text";
        entryTransform.Find("SMTWTFSText").GetComponent<TMP_Text>().text = "S M T W T F S";

        entryTemplate.gameObject.SetActive(true);//sets the template as visible
        i++;

    } //testing atm this is what was ment to append values to UI not functioning IGNORE!
    */




    private void readtoClock() //done
    {
        //int i = 0;

        string line; //string line
        StreamReader sr = new StreamReader("Alarms.txt"); //reads strings from alarms.txt
        line = sr.ReadLine(); //read fist line




        //not a static method

        while (line != null) //while string != null
        {
            //print(line); //line holds value of double.
            //AlarmsfromFile[i] = line;
            Alarms.Add(Convert.ToDouble(line)); //adds alarms to current list of alarms
            line = sr.ReadLine();



        }
        getClosestAlarm(); //start 
        sr.Close();
    }



    public void returnButtonCheck() //checks to see if an alarm was set, if it was it will print the menu upon return click, else false
    {
        firstClick = false;
        secondClick = false;
        originalbuttonValues();
        if (i > 0)
        {
            AlarmTable.SetActive(true);
        }
        else
        {
            AlarmTable.SetActive(false);
        }
    }

    public void PM() //still not functioning as intended but w/e works well enough :D 
    {
        TIMESTATE = "PM";
        timestateClickAM = false;

        if (timestateClickAM == false)
        {
            AMbtn.GetComponentInChildren<Image>().color = new Color32(106, 106, 105, 255);

        }
        else
        {
            AMbtn.GetComponentInChildren<Image>().color = new Color32(90, 168, 90, 255);
        }
    }
    public void AM()
    {
        TIMESTATE = "AM";
        timestateClickAM = true; //this will tell the time system to add a 12 to the count.
        if (timestateClickAM == true)
        {
            PMbtn.GetComponentInChildren<Image>().color = new Color32(106, 106, 105, 255);

        }
        else
        {
            PMbtn.GetComponentInChildren<Image>().color = new Color32(90, 168, 90, 255);
        }
    }

    //button section
    public void btn1Press() //note all buttons function more or less same there are 12 
    {

        if (firstClick == false) //this is kindof backwards but it functions all this does is say if the user has NOT selected an hour 
        {

            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "13:"; //this is for time comparison
            }
            else
            {
                TIMESELECTED24HOUR = "1:";
            }
            TIMESELECTEDHOUR = "1:";
            changebuttonValues();
            firstClick = true;


        }
        else //this is kindof backwards but it functions all this does is say if the user has selected an hour 
        {
            TIMESELECTEDMIN = "55";
            secondClick = true;
        }

    }
    public void btn2Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "14:";
            }
            else
            {
                TIMESELECTED24HOUR = "2:";
            }
            TIMESELECTEDHOUR = "2:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "50";
            secondClick = true;

        }
    }
    public void btn3Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "15:";
            }
            else
            {
                TIMESELECTED24HOUR = "1:";
            }
            TIMESELECTEDHOUR = "3:";
            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "45";
            secondClick = true;
        }
    }
    public void btn4Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "16:";
            }
            else
            {
                TIMESELECTED24HOUR = "4:";
            }
            TIMESELECTEDHOUR = "4:";
            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "40";
            secondClick = true;
        }
    }
    public void btn5Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "17:";
            }
            else
            {

                TIMESELECTED24HOUR = "5:";
            }
            TIMESELECTEDHOUR = "5:";
            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "35";
            secondClick = true;
        }
    }
    public void btn6Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "18:";
            }
            else
            {

                TIMESELECTED24HOUR = "6:";
            }
            TIMESELECTEDHOUR = "6:";
            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "30";
            secondClick = true;
        }
    }
    public void btn7Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "19:";
            }
            else
            {

                TIMESELECTED24HOUR = "7:";
            }
            TIMESELECTEDHOUR = "7:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "25";
            secondClick = true;
        }
    }
    public void btn8Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "20:";
            }
            else
            {

                TIMESELECTED24HOUR = "8:";
            }
            TIMESELECTEDHOUR = "8:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "20";
            secondClick = true;
        }
    }
    public void btn9Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "21:";
            }
            else
            {

                TIMESELECTED24HOUR = " 9:";
            }
            TIMESELECTEDHOUR = "9:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "15";
            secondClick = true;
        }
    }
    public void btn10Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "22:";
            }
            else
            {

                TIMESELECTED24HOUR = "10:";
            }
            TIMESELECTEDHOUR = "10:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "10";
            secondClick = true;
        }
    }
    public void btn11Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "23:";
            }
            else
            {

                TIMESELECTED24HOUR = "11:";
            }
            TIMESELECTEDHOUR = "11:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "05";
            secondClick = true;
        }
    }
    public void btn12Press()
    {

        if (firstClick == false)
        {
            if (timestateClickAM == false)
            {
                TIMESELECTED24HOUR = "00:";
            }
            else
            {

                TIMESELECTED24HOUR = "12:";
            }
            TIMESELECTEDHOUR = "12:";

            changebuttonValues();
            firstClick = true;
        }
        else
        {
            TIMESELECTEDMIN = "00";
            secondClick = true;
        }
    }
    public void SaveButtonPress()
    {
        //print(i);
        if (firstClick == true)
        {
            addToMenu();
            originalbuttonValues();
            firstClick = false;
            secondClick = false;
        }
        else
            firstClick = false;
        if (i > 1)
        {
            Alarms.Sort();
            getClosestAlarm();
            writeTofile(Alarms);
            //print(TIMESELECTED24);
        }
        else
        {
            getClosestAlarm();
            writeTofile(Alarms);
        }
        //updates time when an alarm is saved
        startcount();
        OnValidate();
        //print values here upon save

    }
    public void changebuttonValues()
    {

        //change all button values
        btn1.GetComponentInChildren<TMP_Text>().text = "55";
        btn2.GetComponentInChildren<TMP_Text>().text = "50";
        btn3.GetComponentInChildren<TMP_Text>().text = "45";
        btn4.GetComponentInChildren<TMP_Text>().text = "40";
        btn5.GetComponentInChildren<TMP_Text>().text = "35";
        btn6.GetComponentInChildren<TMP_Text>().text = "30";
        btn7.GetComponentInChildren<TMP_Text>().text = "25";
        btn8.GetComponentInChildren<TMP_Text>().text = "20";
        btn9.GetComponentInChildren<TMP_Text>().text = "15";
        btn10.GetComponentInChildren<TMP_Text>().text = "10";
        btn11.GetComponentInChildren<TMP_Text>().text = "05";
        btn12.GetComponentInChildren<TMP_Text>().text = "00";


    }
    public void originalbuttonValues()
    {
        //change all button values to origial
        btn1.GetComponentInChildren<TMP_Text>().text = "1";
        btn2.GetComponentInChildren<TMP_Text>().text = "2";
        btn3.GetComponentInChildren<TMP_Text>().text = "3";
        btn4.GetComponentInChildren<TMP_Text>().text = "4";
        btn5.GetComponentInChildren<TMP_Text>().text = "5";
        btn6.GetComponentInChildren<TMP_Text>().text = "6";
        btn7.GetComponentInChildren<TMP_Text>().text = "7";
        btn8.GetComponentInChildren<TMP_Text>().text = "8";
        btn9.GetComponentInChildren<TMP_Text>().text = "9";
        btn10.GetComponentInChildren<TMP_Text>().text = "10";
        btn11.GetComponentInChildren<TMP_Text>().text = "11";
        btn12.GetComponentInChildren<TMP_Text>().text = "12";
    }

}

