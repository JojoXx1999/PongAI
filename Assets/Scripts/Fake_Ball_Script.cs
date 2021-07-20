using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake_Ball_Script : MonoBehaviour
{
    [SerializeField]
    private float maxSpeedOfBall, startSpeedOfBall, rateOfAcceleration;

    private float currentSpeedOfBall, radius, predictedPositionOfTheBall;
    private Vector2 currentDirectionOfBall, currentDirectionOfRay, currentPositionOfRay;
    enum possibleXDirectionOfBall { LEFT, RIGHT };
    possibleXDirectionOfBall currentXDirectionOfBall;

    Left_Paddle_Script leftPaddleScript;
    Right_Paddle_Script rightPaddleScript;
    Ball_Script ball_Script;

    LayerMask layerMask = 1 << 0;

    string whatRayHitPreviously;

    void Start()
    {
        leftPaddleScript = FindObjectOfType<Left_Paddle_Script>();
        rightPaddleScript = FindObjectOfType<Right_Paddle_Script>();
        ball_Script = FindObjectOfType<Ball_Script>();
        whatRayHitPreviously = null;
        Reset();
    }


    void Update()
    {
        if (currentSpeedOfBall > 0)
            if (currentSpeedOfBall < maxSpeedOfBall)
                increaseSpeed();
            else
                currentSpeedOfBall = maxSpeedOfBall;


        currentDirectionOfBall = ball_Script.getDirectionOfBall();
        //currentPositionOfRay = this.transform.position;
        createRayCast(currentPositionOfRay, currentDirectionOfRay);

        GetComponent<Rigidbody2D>().velocity = new Vector2(currentDirectionOfBall.x * currentSpeedOfBall * Time.deltaTime, currentDirectionOfBall.y * currentSpeedOfBall * Time.deltaTime);
    }

    private void increaseSpeed()
    {
        currentSpeedOfBall += (Time.deltaTime * rateOfAcceleration);
    }


    private void createRayCast(Vector2 currentRayPosition, Vector2 currentRaycastDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(currentRayPosition, currentRaycastDirection, Mathf.Infinity, layerMask);
        //Debug.DrawRay(currentRayPosition, currentRaycastDirection * 1000, Color.red);

        detectRaycastHit(hit, currentRayPosition, currentRaycastDirection);
    }

    private void detectRaycastHit(RaycastHit2D hit, Vector2 currentRayPosition, Vector2 currentRaycastDirection)
    {
        if (hit.collider)
        {

            Debug.Log(hit.collider.tag);
            whatRayHitPreviously = hit.collider.tag;
            if (hit.collider.tag == "Top_Wall" || hit.collider.tag == "Bottom_Wall")
            {
                currentDirectionOfRay.y = -currentDirectionOfRay.y;

                //hit.collider.gameObject.layer = LayerMask.NameToLayer("Wall");
                float theta = Mathf.Atan2(currentDirectionOfRay.x, currentDirectionOfRay.y);
                float xOffset = Mathf.Tan(theta) * radius;

                if (currentDirectionOfRay.x > 0)
                    currentPositionOfRay = new Vector2(hit.point.x - xOffset, hit.point.y + currentDirectionOfRay.y * 0.001f);
                else
                    currentPositionOfRay = new Vector2(hit.point.x + xOffset, hit.point.y + currentDirectionOfRay.y * 0.001f);

            }
            else if (hit.collider.tag == "Left_Wall" || hit.collider.tag == "Right_Wall")
            {
                predictedPositionOfTheBall = hit.point.y;
            }
            else if (hit.collider.tag == "Left_Paddle" || hit.collider.tag == "Right_Paddle")
            {
                currentDirectionOfRay = -currentDirectionOfRay;


                //hit.collider.gameObject.layer = LayerMask.NameToLayer("Wall");



                if (hit.collider.tag == "Left_Paddle")
                    currentPositionOfRay = new Vector2(hit.point.x + 0.00001f, hit.point.y);
                else
                    currentPositionOfRay = new Vector2(hit.point.x - 0.00001f, hit.point.y);
            }

        }
        else
        {
            //Ray didn't hit anything
        }
    }


    private void chooseBallDirection()
    {
        currentXDirectionOfBall = (possibleXDirectionOfBall)Random.Range(0, System.Enum.GetValues(typeof(possibleXDirectionOfBall)).Length);

        if (currentXDirectionOfBall == possibleXDirectionOfBall.RIGHT)
            currentDirectionOfBall = new Vector2(1f, 1f);
        else if (currentXDirectionOfBall == possibleXDirectionOfBall.LEFT)
            currentDirectionOfBall = new Vector2(-1f, 1f);

    }

    private void UpdateScore(int whichPlayerHasScored)
    {
        Update_Score_Script.instance.UpdateScore(whichPlayerHasScored);

    }

    public float GetHeightPositionOfBall()
    {
        return this.transform.position.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Left_Paddle" || other.tag == "Right_Paddle")
        {
            currentDirectionOfBall.x = -currentDirectionOfBall.x;
            currentPositionOfRay = currentDirectionOfBall;
            currentDirectionOfRay = currentDirectionOfBall;
        }
        else if (other.tag == "Top_Wall" || other.tag == "Bottom_Wall")
            currentDirectionOfBall.y = -currentDirectionOfBall.y;
        else if (other.tag == "Right_Wall")
        {
            UpdateScore(2);
            Reset();
        }
        else if (other.tag == "Left_Wall")
        {
            UpdateScore(1);
            Reset();
        }
    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        this.transform.position = new Vector2(0, 0);
        currentSpeedOfBall = startSpeedOfBall;
        chooseBallDirection();
        currentDirectionOfRay = currentDirectionOfBall;
        currentPositionOfRay = currentDirectionOfBall;
        currentDirectionOfRay = currentDirectionOfBall;
    }

    private void Reset()
    {
        leftPaddleScript.Reset();
        rightPaddleScript.Reset();
        this.transform.position = new Vector2(0, 0);
        currentSpeedOfBall = 0;
        currentDirectionOfBall = new Vector2(0, 0);
        StartCoroutine(Wait(1));
    }

    public float getPredictedPositionOfTheBall()
    {
        return predictedPositionOfTheBall;
    }

}
