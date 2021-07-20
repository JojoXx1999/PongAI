using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class to control the ball
Controls the movement of the ball
And the behaviour of its rayCast
//*/
public class Ball_Script : MonoBehaviour
{

    //Variables which are set in the Unity Inspector
    [SerializeField]
    private float maxSpeedOfBall, startSpeedOfBall, rateOfAcceleration, minimumAngleOfBall;

    //Variables to be used by the ball
    private float currentSpeedOfBall, radius, predictedPositionOfTheBall;
    private Vector2 currentDirectionOfBall, currentDirectionOfRay, currentPositionOfRay;
    string whatRayHitPreviously;

    //Enum for the possible X directions of the ball
    enum possibleXDirectionOfBall { LEFT, RIGHT };
    possibleXDirectionOfBall currentXDirectionOfBall;
    private float currentYDirectionOfBall;

    //Access to left and right paddle scripts public funtions and/or variables
    Left_Paddle_Script leftPaddleScript;
    Right_Paddle_Script rightPaddleScript;

    //The ray will only interact with objects on layer 0 in the game
    LayerMask layerMask = 1 << 0;


    /*
    Set initial values 
    //*/
    void Start()
    {
        //Get access to the left and right paddle scripts
        leftPaddleScript = FindObjectOfType<Left_Paddle_Script>();
        rightPaddleScript = FindObjectOfType<Right_Paddle_Script>();
        radius = this.transform.localScale.x/2;
        whatRayHitPreviously = null;
        Reset();
    }

     /*
     Run constantly
     //*/
    void Update()
    {
         if (currentSpeedOfBall > 0) //If the ball is not stationery 
            if (currentSpeedOfBall < maxSpeedOfBall) //If the ball is not as it's maximum speed
                //Increase the speed of the ball
                increaseSpeed();
            else //If the ball is faster than its maximum speed
                //Set the speed of the ball to its maximum speed
                currentSpeedOfBall = maxSpeedOfBall;



        //currentPositionOfRay = this.transform.position;
        createRayCast(currentPositionOfRay, currentDirectionOfRay);

        GetComponent<Rigidbody2D>().velocity = new Vector2(currentDirectionOfBall.x * currentSpeedOfBall * Time.deltaTime, currentDirectionOfBall.y * currentSpeedOfBall * Time.deltaTime);
    }

    /*
    Increase the speed of the ball
    //*/
    private void increaseSpeed()
    {
        //add to the current speed of the ball using the rate of acceleration set in the Unity Inspector
        currentSpeedOfBall += (Time.deltaTime * rateOfAcceleration);
    }

    /*
    Create a raycast
    //*/
    private void createRayCast(Vector2 currentRayPosition, Vector2 currentRaycastDirection)
    {
        //Create a raycast with infinite length, that only sees objects on the layerMask and draws this ray
        RaycastHit2D hit = Physics2D.Raycast(currentRayPosition, currentRaycastDirection, Mathf.Infinity, layerMask);
        //Debug.DrawRay(currentRayPosition, currentRaycastDirection * 1000, Color.red);

        //Detect if the raycast hits anything
        detectRaycastHit(hit, currentRayPosition, currentRaycastDirection);
    }

    /*
    Check if the raycast has interacted with any objects
    //*/
    private void detectRaycastHit(RaycastHit2D hit, Vector2 currentRayPosition, Vector2 currentRaycastDirection)
    {
        if (hit.collider) //If the raycast hit an object
        {
           //Display the tag of this object
           Debug.Log(hit.collider.tag);

           //Set variable to the tag of the object
           whatRayHitPreviously = hit.collider.tag;
           if (hit.collider.tag == "Top_Wall" || hit.collider.tag == "Bottom_Wall") //If the raycast hit a top or bottom wall
           {
             //invert the Y direction of the ray
             currentDirectionOfRay.y = -currentDirectionOfRay.y;

             //Triganometry to account for offset of ray from ball position --------------------------------------------------------------------------------------------
             float theta = Mathf.Atan2(currentDirectionOfRay.y, currentDirectionOfRay.x);
             float xOffset = Mathf.Tan(theta) * radius;
             //---------------------------------------------------------------------------------------------------------------------------------------------------------

             if (currentDirectionOfRay.x > 0) //If the ray is pointing to the right
                //Create a new ray starting from the position it last hit, with an offset of 0.001 in the Y so it doesn't detect it's own origin, and the calculated offset in the X
                currentPositionOfRay = new Vector2 (hit.point.x - xOffset, hit.point.y + currentDirectionOfRay.y * 0.001f);
             else //If the ray is pointing to the left
                //Create a new ray starting from the position it last hit, with an offset of 0.001 in the Y so it doesn't detect it's own origin, and the calculated offset in the X
                currentPositionOfRay = new Vector2(hit.point.x + xOffset, hit.point.y + currentDirectionOfRay.y * 0.001f);
            }
            else if (hit.collider.tag == "Fake_Left_Wall" || hit.collider.tag == "Fake_Right_Wall") //If raycast went off the left or right of the screen
            {
              predictedPositionOfTheBall = hit.point.y; //set the predicted position to the Y co-ordinate that it hit
            }
              else if (hit.collider.tag == "Left_Paddle" || hit.collider.tag == "Right_Paddle") //If raycast hit a paddle
            {
                //  currentDirectionOfRay = -currentDirectionOfRay; //Invert the direction of the raycast
                if (hit.collider.tag == "Left_Paddle") { }//If raycast hit the left paddle
                 //set the current position of the ray to the point of intersection, plus a slight offset so the ray doesnt detect itself at this origin
                // currentPositionOfRay = new Vector2 (hit.point.x + 0.00001f, hit.point.y);
              // else //If raycast hit the right paddle
                 //set the current position of the ray to the point of intersection, plus a slight offset so the ray doesnt detect itself at this origin
               //  currentPositionOfRay = new Vector2(hit.point.x - 0.00001f, hit.point.y); 
             }                    
            }
        else //Ray didn't hit anything
        {
             
        }
    }

    /*
    Choose a direction for the ball
    //*/
    private void chooseBallDirection()
    {
        //Choose an X direction from the possible directions, contained within the directions enum
        currentXDirectionOfBall = (possibleXDirectionOfBall)Random.Range(0, System.Enum.GetValues(typeof(possibleXDirectionOfBall)).Length);
        currentYDirectionOfBall = (Random.Range(-100, 100));
        if (currentYDirectionOfBall >= 0 && currentYDirectionOfBall < minimumAngleOfBall)
            currentYDirectionOfBall = 50;
        else if (currentYDirectionOfBall < 0 && currentYDirectionOfBall > -minimumAngleOfBall)
            currentYDirectionOfBall = -50;

        if (currentXDirectionOfBall == possibleXDirectionOfBall.RIGHT) //If the script chose to go the right
                                                                           //Set X direction of the ball to move to the right
                currentDirectionOfBall = new Vector2(1f, currentYDirectionOfBall / 100f);
            else if (currentXDirectionOfBall == possibleXDirectionOfBall.LEFT) //If the script chose to go to the left
                                                                               //Set X direction of the ball to move to the left
                currentDirectionOfBall = new Vector2(-1f, currentYDirectionOfBall / 100f);

    }

    /*
    Update the score
    //*/
    private void UpdateScore(int whichPlayerHasScored)
    {
        //Send which player scored to the UpdateScore function in the score script
        Update_Score_Script.instance.UpdateScore(whichPlayerHasScored);
    }

    /*
    Detect if the ball has collided with another collider
    //*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Left_Paddle" || other.tag == "Right_Paddle") //If ball has hit a paddle
        {
            //Invert direction of the ball and set the raycast to the ball again
            currentDirectionOfBall.x = -currentDirectionOfBall.x;
            currentPositionOfRay = this.transform.position;
            currentDirectionOfRay = currentDirectionOfBall;

            if (other.tag == "Left_Paddle")
                currentPositionOfRay = new Vector2 (other.transform.position.x + (GameManager.widthOfPaddle/2) + 0.001f, this.transform.position.y);
            else
                currentPositionOfRay = new Vector2(other.transform.position.x - (GameManager.widthOfPaddle / 2) - 0.001f, this.transform.position.y);
        }
        else if (other.tag == "Top_Wall" || other.tag == "Bottom_Wall") //If ball hit the top or bottom wall
            //invert the Y direction of the ball
            currentDirectionOfBall.y = -currentDirectionOfBall.y; 
        else if (other.tag == "Right_Wall") //If ball went off the right hand side 
        {
            //Give a point to player 2 and reset the game
            UpdateScore(2);
            Reset();
        }
        else if (other.tag == "Left_Wall") // If ball went off the left hand side
        {
            //Give a point to player 1 and reset the game
            UpdateScore(1);
            Reset();
        }
    }

    /*
    Wait function to wait for some length of time
    //*/
    IEnumerator Wait(float duration)
    {
        //wait for this duration
        yield return new WaitForSeconds(duration); 

        //Start game, give ball a direction, and place ray back at the ball
        this.transform.position = new Vector2(0, 0);
        currentSpeedOfBall = startSpeedOfBall;
        chooseBallDirection();
        currentDirectionOfRay = currentDirectionOfBall;
        currentPositionOfRay = currentDirectionOfBall;
        currentDirectionOfRay = currentDirectionOfBall;
    }

    /*
    Reset function to reset all game objects to their initial positions
    //*/
    private void Reset()
    {
        leftPaddleScript.Reset();
        rightPaddleScript.Reset();
        this.transform.position = new Vector2(0, 0);
        currentSpeedOfBall = 0;
        currentDirectionOfBall = new Vector2(0, 0);
        currentDirectionOfRay = new Vector2(0, 0);
        currentPositionOfRay = this.transform.position;

        //Wait for 1 second
        StartCoroutine(Wait(1));
    }

    /*
    Getter function to return the predicted position of the ball
    //*/
    public float getPredictedPositionOfTheBall()
    {
        return predictedPositionOfTheBall;
    }

    /*
    Getter to return the current Y position of the ball
    //*/
    public float GetHeightPositionOfBall()
    {
        return this.transform.position.y;
    }

    /*
    Getter function to return the current direction of the ball
    //*/
    public Vector2 getDirectionOfBall()
    {
        return currentDirectionOfBall;
    }

}
