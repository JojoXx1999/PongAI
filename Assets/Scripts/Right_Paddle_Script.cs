using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class to control the right player (or AI) paddle
Controls the movement of the paddle
//*/
public class Right_Paddle_Script : MonoBehaviour
{

    //Variable to determine the speed the paddle can move at, set in the Unity Inspector
    [SerializeField]
    private float playerMoveSpeed;

    //Variable to decide if the paddle is to be controlled by a player or the AI, aslo set in the Unity Inspector
    [SerializeField]
    private bool isControlledByUser;

    //Variables for the AI to use
    private float paddleHeight, heightOfBall, heightOfPaddle;

    //Variables for the player to use
    private string userInput;

    //Enumumerator used for telling the script which direction the paddle is currently moving in
    private enum directionOfPaddle { NONE, UP, DOWN };
    directionOfPaddle currentDirectionOfPaddle;

    //Give this paddle access to the public functions/variables within the ball script
    Ball_Script ballScript;

    /*
    Set initial values to variables
    //*/
    void Start()
    {
        paddleHeight = this.transform.localScale.y;
        playerMoveSpeed *= Time.deltaTime;
        currentDirectionOfPaddle = directionOfPaddle.NONE;
        ballScript = FindObjectOfType<Ball_Script>();
    }

    /*
    Function to run constantly
    //*/
    void Update()
    {
        //Depending on the boolean set in the Unity Inspector choose which function to run
        if (isControlledByUser) //If boolean if set to true
            PlayerControls();
        else
            FollowBallPosition();
    }

    /*
    Function to move the paddle towards the current Y position of the ball
    //*/
    private void FollowBallPosition()
    {
        //Get the current Y value of the ball's position from the "GetHeightPositionOfBall" function in the ball script and the current Y position of the paddle
        heightOfBall = ballScript.GetHeightPositionOfBall();
        heightOfPaddle = this.transform.position.y;

        if (myApproximation(heightOfPaddle, heightOfBall, 0.05f)) //If paddle is where the ball will end up, use approximation for floating point uncertanty, last value is the allowance for inaccuracy
        {
            //Stop the paddle moving and set enum direction to NONE
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            currentDirectionOfPaddle = directionOfPaddle.NONE;
        }                                                                                                                                                             
        else if (heightOfPaddle > heightOfBall) //If the paddle is below the predicted position                                                            
        {
            //Move the paddle up and set enum direction to UP
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -playerMoveSpeed);
            currentDirectionOfPaddle = directionOfPaddle.UP;
        }                                                                                                                                                             
        else if (heightOfPaddle < heightOfBall) //If the paddle is above the predicted position                                                                                                                                
        {
            //Move the paddle down and set enum direction to DOWN
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, playerMoveSpeed);
            currentDirectionOfPaddle = directionOfPaddle.DOWN;
        }

        //Go to the function to stop the paddle from ever going off the screen
        StopPaddleAtScreenEdge();
    }

    /*
    Function to move the paddle
    This is the player control of the paddle
    //*/
    private void PlayerControls()
    {
        if (Input.GetKey(KeyCode.W)) //If player presses the W key   
        {
            //Move the paddle up and set enum direction to UP
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, playerMoveSpeed);
            currentDirectionOfPaddle = directionOfPaddle.UP;
        }
        else if (Input.GetKey(KeyCode.S)) //If the player presses the S key   
        {
            //Move the paddle down and set enum direction to DOWN
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -playerMoveSpeed);
            currentDirectionOfPaddle = directionOfPaddle.DOWN;
        }
        else //If the user doesn't press the W or S keys 
        {
            //Stop the paddle moving and set enum direction to NONE 
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            currentDirectionOfPaddle = directionOfPaddle.NONE;
        }

        //Go to the function to stop the paddle from ever going off the screen
        StopPaddleAtScreenEdge();
    }

    /*
    Stop the paddle going off the top or bottom of the screen
    //*/
    private void StopPaddleAtScreenEdge()
    {

        if (transform.position.y < GameManager.bottomLeft.y + (paddleHeight / 2) && GetComponent<Rigidbody2D>().velocity.y < 0) //If paddle is at the bottom of the screen and is trying to move down  
        {
            //Stop the paddle moving and set enum direction to NONE
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            currentDirectionOfPaddle = directionOfPaddle.NONE;
        }
        else if (transform.position.y > GameManager.topRight.y - (paddleHeight / 2) && GetComponent<Rigidbody2D>().velocity.y > 0) //If paddle is at the top of the screen and is trying to move up 
        {
            //Stop the paddle moving and set enum direction to NONE
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            currentDirectionOfPaddle = directionOfPaddle.NONE;
        }
    }

    /*
    Reset the paddle back to its position at the start of the game
    //*/
    public void Reset()
    {
        this.transform.position = new Vector2 (this.transform.position.x, 0);
    }

    /*
    Hand-made approximate function for floating point uncertanties
    //*/
    private bool myApproximation(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }
}
