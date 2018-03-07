/*** DO NOT EDIT THIS SCRIPT ***/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum LimbState
{
    Stationary,
    Up,
    Down
}

public class LadderTask : MonoBehaviour
{

    [Tooltip("We expect the SteamVR [CameraRig] prefab will be used and referenced here.")]
    public GameObject ViveCameraRig;

    private GameObject LeftFoot;
    private GameObject LeftHand;
    private GameObject RightFoot;
    private GameObject RightHand;

    private float rungDistance;
    private float speed;

    private float lastLeftFoot;
    private float lastLeftHand;
    private float lastRightFoot;
    private float lastRightHand;

    private float currentLeftFoot;
    private float currentLeftHand;
    private float currentRightFoot;
    private float currentRightHand;

    private LimbState targetLeftFoot;
    private LimbState targetLeftHand;
    private LimbState targetRightFoot;
    private LimbState targetRightHand;

    private bool falling;
    private float fallVelocity;

    private bool taskCompleted;

    // Use this for initialization
    void Start()
    {

        LeftFoot = GameObject.Find("Left Foot Logic");
        LeftHand = GameObject.Find("Left Hand Logic");
        RightFoot = GameObject.Find("Right Foot Logic");
        RightHand = GameObject.Find("Right Hand Logic");

        rungDistance = 0.3f;
        speed = 0.5f;

        lastLeftFoot = currentLeftFoot = 0.0f * rungDistance;
        lastLeftHand = currentLeftHand = 4.0f * rungDistance;
        lastRightFoot = currentRightFoot = 0.0f * rungDistance;
        lastRightHand = currentRightHand = 4.0f * rungDistance;

        targetLeftFoot = LimbState.Stationary;
        targetLeftHand = LimbState.Stationary;
        targetRightFoot = LimbState.Stationary;
        targetRightHand = LimbState.Stationary;

        falling = false;
        fallVelocity = 0.0f;

        taskCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Update the limbs
        UpdateLeftFoot();
        UpdateLeftHand();
        UpdateRightFoot();
        UpdateRightHand();

        if (!taskCompleted)
        {

            // Determine if three-point control is maintained
            CheckThreePoints();

            // Simulate fall if falling
            if (falling)
            {
                HandleFalling();
            }
            else
            {
                HandleClimbing();
            }

            CheckCompletion();
        }
        // Added on 2/7/2018 : after task is complete, load the next scene in the build order.
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


    private void UpdateLeftFoot()
    {

        if (!falling)
        {
            if (targetLeftFoot == LimbState.Up)
            {
                currentLeftFoot += Time.deltaTime * speed;
                if (currentLeftFoot > lastLeftFoot + rungDistance)
                {
                    currentLeftFoot = lastLeftFoot + rungDistance;
                    lastLeftFoot = currentLeftFoot;
                    targetLeftFoot = LimbState.Stationary;
                }
            }
            else if (targetLeftFoot == LimbState.Down)
            {
                currentLeftFoot -= Time.deltaTime * speed;
                if (currentLeftFoot < lastLeftFoot - rungDistance)
                {
                    currentLeftFoot = lastLeftFoot - rungDistance;
                    lastLeftFoot = currentLeftFoot;
                    targetLeftFoot = LimbState.Stationary;
                }
            }

            Vector3 leftFootPosition = LeftFoot.transform.position;
            leftFootPosition.y = currentLeftFoot;
            LeftFoot.transform.position = leftFootPosition;
        }
    }

    private void UpdateLeftHand()
    {

        if (targetLeftHand == LimbState.Up)
        {
            currentLeftHand += Time.deltaTime * speed;
            if (currentLeftHand > lastLeftHand + rungDistance)
            {
                currentLeftHand = lastLeftHand + rungDistance;
                lastLeftHand = currentLeftHand;
                targetLeftHand = LimbState.Stationary;
            }
        }
        else if (targetLeftHand == LimbState.Down)
        {
            currentLeftHand -= Time.deltaTime * speed;
            if (currentLeftHand < lastLeftHand - rungDistance)
            {
                currentLeftHand = lastLeftHand - rungDistance;
                lastLeftHand = currentLeftHand;
                targetLeftHand = LimbState.Stationary;
            }
        }

        Vector3 leftHandPosition = LeftHand.transform.position;
        leftHandPosition.y = currentLeftHand;
        LeftHand.transform.position = leftHandPosition;
    }

    private void UpdateRightFoot()
    {

        if (targetRightFoot == LimbState.Up)
        {
            currentRightFoot += Time.deltaTime * speed;
            if (currentRightFoot > lastRightFoot + rungDistance)
            {
                currentRightFoot = lastRightFoot + rungDistance;
                lastRightFoot = currentRightFoot;
                targetRightFoot = LimbState.Stationary;
            }
        }
        else if (targetRightFoot == LimbState.Down)
        {
            currentRightFoot -= Time.deltaTime * speed;
            if (currentRightFoot < lastRightFoot - rungDistance)
            {
                currentRightFoot = lastRightFoot - rungDistance;
                lastRightFoot = currentRightFoot;
                targetRightFoot = LimbState.Stationary;
            }
        }

        Vector3 rightFootPosition = RightFoot.transform.position;
        rightFootPosition.y = currentRightFoot;
        RightFoot.transform.position = rightFootPosition;
    }

    private void UpdateRightHand()
    {

        if (targetRightHand == LimbState.Up)
        {
            currentRightHand += Time.deltaTime * speed;
            if (currentRightHand > lastRightHand + rungDistance)
            {
                currentRightHand = lastRightHand + rungDistance;
                lastRightHand = currentRightHand;
                targetRightHand = LimbState.Stationary;
            }
        }
        else if (targetRightHand == LimbState.Down)
        {
            currentRightHand -= Time.deltaTime * speed;
            if (currentRightHand < lastRightHand - rungDistance)
            {
                currentRightHand = lastRightHand - rungDistance;
                lastRightHand = currentRightHand;
                targetRightHand = LimbState.Stationary;
            }
        }

        Vector3 rightHandPosition = RightHand.transform.position;
        rightHandPosition.y = currentRightHand;
        RightHand.transform.position = rightHandPosition;
    }

    private void CheckThreePoints()
    {

        int pointsOfContact = 0;

        Debug.Log(currentLeftFoot % rungDistance);
        Debug.Log(currentLeftHand % rungDistance);
        Debug.Log(currentRightFoot % rungDistance);
        Debug.Log(currentRightHand % rungDistance);
        Debug.Log(" ");

        if ((currentLeftFoot % rungDistance) < 0.001f || (currentLeftFoot % rungDistance) > rungDistance - 0.001f)
        {
            pointsOfContact++;
        }

        if ((currentLeftHand % rungDistance) < 0.001f || (currentLeftHand % rungDistance) > rungDistance - 0.001f)
        {
            pointsOfContact++;
        }

        if ((currentRightFoot % rungDistance) < 0.001f || (currentRightFoot % rungDistance) > rungDistance - 0.001f)
        {
            pointsOfContact++;
        }

        if ((currentRightHand % rungDistance) < 0.001f || (currentRightHand % rungDistance) > rungDistance - 0.001f)
        {
            pointsOfContact++;
        }

        if (pointsOfContact < 3)
        {
            falling = true;
        }

        if (currentLeftFoot > (currentLeftHand - (rungDistance * 2.0f)) || currentLeftFoot > (currentRightHand - (rungDistance * 2.0f)))
        {
            falling = true;
        }

        if (currentRightFoot > (currentLeftHand - (rungDistance * 2.0f)) || currentRightFoot > (currentRightHand - (rungDistance * 2.0f)))
        {
            falling = true;
        }

        if (currentLeftHand > (currentLeftFoot + (rungDistance * 6.0f)) || currentLeftHand > (currentRightFoot + (rungDistance * 6.0f)))
        {
            falling = true;
        }

        if (currentRightHand > (currentLeftFoot + (rungDistance * 6.0f)) || currentRightHand > (currentRightFoot + (rungDistance * 6.0f)))
        {
            falling = true;
        }
    }

    private void HandleFalling()
    {
        fallVelocity += 9.8f * Time.deltaTime;
        Vector3 fallPosition = ViveCameraRig.transform.position;
        fallPosition.y -= fallVelocity * Time.deltaTime;
        if (fallPosition.y < 0.0f)
        {
            fallPosition.y = 0.0f;
            fallVelocity = 0.0f;
            falling = false;

            float fallDistance = 0.0f;
            if (currentLeftFoot < currentRightFoot)
            {
                fallDistance = currentLeftFoot;
            }
            else
            {
                fallDistance = currentRightFoot;
            }

            lastLeftFoot -= fallDistance;
            lastLeftHand -= fallDistance;
            lastRightFoot -= fallDistance;
            lastRightHand -= fallDistance;
            currentLeftFoot -= fallDistance;
            currentLeftHand -= fallDistance;
            currentRightFoot -= fallDistance;
            currentRightHand -= fallDistance;

        }
        ViveCameraRig.transform.position = fallPosition;
    }

    private void HandleClimbing()
    {
        Vector3 climbPosition = ViveCameraRig.transform.position;
        if (currentLeftFoot < currentRightFoot)
        {
            climbPosition.y = currentLeftFoot;
        }
        else
        {
            climbPosition.y = currentRightFoot;
        }
        ViveCameraRig.transform.position = climbPosition;
    }

    private void CheckCompletion()
    {

        if (ViveCameraRig.transform.position.y >= (rungDistance * 10.0f) - 0.001f)
        {
            Vector3 completionPosition = ViveCameraRig.transform.position;
            completionPosition.z += 1.5f;
            ViveCameraRig.transform.position = completionPosition;
            taskCompleted = true;

            lastLeftFoot = currentLeftFoot = 0.0f;
            lastLeftHand = currentLeftHand = 0.0f;
            lastRightFoot = currentRightFoot = 0.0f;
            lastRightHand = currentRightHand = 0.0f;
        }

    }

    public void LeftFootUp()
    {
        targetLeftFoot = LimbState.Up;
    }

    public void LeftFootDown()
    {
        targetLeftFoot = LimbState.Down;
    }

    public void LeftHandUp()
    {
        targetLeftHand = LimbState.Up;
    }

    public void LeftHandDown()
    {
        targetLeftHand = LimbState.Down;
    }

    public void RightFootUp()
    {
        targetRightFoot = LimbState.Up;
    }

    public void RightFootDown()
    {
        targetRightFoot = LimbState.Down;
    }

    public void RightHandUp()
    {
        targetRightHand = LimbState.Up;
    }

    public void RightHandDown()
    {
        targetRightHand = LimbState.Down;
    }

}
