using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Class to update the scores of the players and/or AIs
//*/
public class Update_Score_Script : MonoBehaviour
{

    //Create an instance so any script can easily access this one
    public static Update_Score_Script instance;

    //Create UI text for the scores
    [SerializeField]
    private Text Left_Player_Score, Right_Player_Score;

    /*
    Set initial values
    //*/
    void Start()
    {
        Reset();
        instance = this;
    }

    /*
    Update scores with the player that has scored being passed in from the ball script
    //*/
    public void UpdateScore(int whichPlayerHasScored)
    {
        if (whichPlayerHasScored == 1) //If player 1 has scored
            //Increment and display player 1's score
            Left_Player_Score.text = (int.Parse(Left_Player_Score.text) + 1).ToString();
        else if (whichPlayerHasScored == 2) //If player 2 has scored
            //Increment and display player 2's score
            Right_Player_Score.text = (int.Parse(Right_Player_Score.text) + 1).ToString();
        else if (whichPlayerHasScored == 0) //If 0 is passed in
            //Reset the score
            Reset();

    }

    /*
    Function to set both player scores to 0
    //*/
    private void Reset()
    {
        Right_Player_Score.text = Left_Player_Score.text = "0";
    }
}
