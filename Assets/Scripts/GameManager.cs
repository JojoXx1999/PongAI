using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Main class used to manage all game objects within the scene
Main purpose is to instantiate and place these objects
NOT to edit their values
//*/
public class GameManager : MonoBehaviour
{      

    //Game Objects and script variables, serialized so they can be edited in the Unity Inspector
    [SerializeField]
    private Left_Paddle_Script leftPaddle;

    [SerializeField]
    private Right_Paddle_Script rightPaddle;

    [SerializeField]
    private Ball_Script ball;

    [SerializeField]
    private GameObject centreLine, backgroundVideo, videoPlayer, leftPlayerScore, rightPlayerScore,
            topWall, bottomWall, rightWall, leftWall, fakeBottom, fakeTop, fakeLeft, fakeRight;

    //Static variables mostly used to position objects within the scene, values will not chance, hence static. Public variables are accessible from other scripts
    static public float screenSizeX; 
    static public float screenSizeY;
    static public Vector2 topRight;
    static public Vector2 bottomLeft;
    static public float widthOfPaddle, radiusOfBall;

    /*
    Standard Start function
    Creates clones of the game objects and scripts and places them within the scene
    Also calculates screen size and prpotions which can be acessed from other scripts
    //*/
    private void Start()
    {

        //Create instances of the required scripts for the game to run
        leftPaddle = Instantiate(leftPaddle) as Left_Paddle_Script;
        rightPaddle = Instantiate(rightPaddle) as Right_Paddle_Script;
        ball = Instantiate(ball) as Ball_Script;

        //Create instabces of the game objects to be included within the game scene
        centreLine = Instantiate(centreLine);
        backgroundVideo = Instantiate(backgroundVideo);
        videoPlayer = Instantiate(videoPlayer);
        topWall = Instantiate(topWall);
        bottomWall = Instantiate(bottomWall);
        leftWall = Instantiate(leftWall);
        rightWall = Instantiate(rightWall);

        fakeBottom = Instantiate(fakeBottom);
        fakeTop = Instantiate(fakeTop);
        fakeLeft = Instantiate(fakeLeft);
        fakeRight = Instantiate(fakeRight);

        //Calculate static variables of screen size and key points which can be used for placing objects, can be accessed from other external scripts
        screenSizeX = Screen.width;
        screenSizeY = Screen.height;
        topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));

        //Get dimensions of the paddles and the ball, so calculations can be performed correctly, accounting for their sizes, due to the game object origins
        widthOfPaddle = leftPaddle.transform.localScale.x;
        radiusOfBall = ball.transform.localScale.x/2;

        //Seperate functions used to place the game objects in their correct positions on-screen
        PlaceObjectsInScene();
    }

    /*
    Function using the variables from the Start function
    Places game objects in the correct positions on screen in relation to the current screen size
    //*/
    private void PlaceObjectsInScene()
    {
        //Position the paddles at each side of the screen
        leftPaddle.transform.position = new Vector2(bottomLeft.x + (widthOfPaddle + 1), 0);
        rightPaddle.transform.position = new Vector2(topRight.x - (widthOfPaddle + 1), 0);

        //Position the ball and centre line in the centre of the screen - also scale the centre line so it is always the same size relative to the screen
        ball.transform.position = new Vector2(0, 0);
        centreLine.transform.localScale = new Vector2(centreLine.transform.localScale.x, screenSizeY);
        centreLine.transform.position = new Vector3(0, 0, 1);

        //Position the animated background at the back of the scene and scale it to be tje size of the entire screen
        backgroundVideo.transform.localScale = new Vector2(topRight.x*2, topRight.y*2);
        backgroundVideo.transform.position = new Vector3(0, 0, 2);

        //Position the UI Text showing the player scores at each side of the centre line
        leftPlayerScore.transform.position = new Vector2(centreLine.transform.position.x + (leftPlayerScore.transform.localScale.x * 1.5f), topRight.y - leftPlayerScore.transform.localScale.y);
        rightPlayerScore.transform.position = new Vector2(centreLine.transform.position.x - (rightPlayerScore.transform.localScale.x * 1.5f), topRight.y - rightPlayerScore.transform.localScale.y);

        //Position and scale the 4 external walls just outside the view of the suer's camera, these will be used for bouncing the ball, aswell as well as for RayCast bouncing/detection
        topWall.transform.localScale = bottomWall.transform.localScale = new Vector2(topRight.x*2, topRight.y/10);
        topWall.transform.position = new Vector2(0, topRight.y + (topWall.transform.localScale.y/2));
        bottomWall.transform.position = new Vector2(0, bottomLeft.y - (topWall.transform.localScale.y / 2));
        leftWall.transform.localScale = rightWall.transform.localScale = new Vector2(topRight.y/10, topRight.y*2);
        leftWall.transform.position = new Vector2(bottomLeft.x - (leftWall.transform.localScale.x/2), 0);
        rightWall.transform.position = new Vector2(topRight.x + (rightWall.transform.localScale.x / 2), 0);

        fakeBottom.transform.localScale = bottomWall.transform.localScale;
        fakeBottom.transform.position = new Vector2 (bottomWall.transform.position.x, bottomWall.transform.position.y + radiusOfBall);

        fakeTop.transform.localScale = bottomWall.transform.localScale;
        fakeTop.transform.position = new Vector2(topWall.transform.position.x, topWall.transform.position.y - radiusOfBall);

        fakeLeft.transform.localScale = new Vector2(leftPaddle.transform.localScale.x, leftWall.transform.localScale.y);
        fakeLeft.transform.position = new Vector2(leftPaddle.transform.position.x, leftWall.transform.position.y);

        fakeRight.transform.localScale = new Vector2(rightPaddle.transform.localScale.x, rightWall.transform.localScale.y);
        fakeRight.transform.position = new Vector2(rightPaddle.transform.position.x, rightWall.transform.position.y);
    }
    
}
