using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class UpDownController
{
    public float interval = 0.5f;
    Vector3 startPos;
    Transform controller;
    List<float> timeStamp = new List<float>();
    List<float> magnitude = new List<float>();

    public UpDownController(Transform shakeController, float scoreTimeInterval)
    {
        controller = shakeController;
        interval = scoreTimeInterval;
    }

    public void PressButton()
    {
        startPos = controller.position;
    }

    public void HoldButton()
    {

        timeStamp.Add(Time.time);
        magnitude.Add((controller.position - startPos).y);
        startPos = controller.position;

    }

    public void ManuallyAddEntry(float time, float value)
    {
        timeStamp.Add(time);
        magnitude.Add(value);
    }

    /*public void ReleaseButton()
    {

        timeStamp.Add(Time.time);
        magnitude.Add((controller.position - startPos).y);
    }*/

    public float GetIntensity()
    {
        while (timeStamp.Count > 0 && (Time.time - timeStamp[0]) > interval)
        {
            timeStamp.RemoveAt(0);
            magnitude.RemoveAt(0);
        }

        float intensity = 0;
        for (int i = 0; i < magnitude.Count; i++)
        {
            intensity += magnitude[i];
        }
        return intensity;
    }
}

public class UpDownControl : MonoBehaviour {
    [Tooltip("The script that checks the available movements.")]
    public Checker checker;
    [Tooltip("The movement controllers.")]
    public Hand[] controllers;

    private UpDownController[] upDownControllers;

    [Tooltip("Max shaking score that yields no control latency.")]
    public float maxForNormalization = 1;
    [Tooltip("Min shaking score to trigger a movement with the maximum control latency.")]
    public float minForNormalization = 0.5f;
    [Tooltip("Max control latency in seconds, used when shaking == minForNormalization.")]
    public float maxLatency = 1;
    [Tooltip("Interval of time to take the controllers movement in consideration.")]
    public float interval = 0.3f;
    [Tooltip("Just for debugging.")]
    public TextMesh go;
    [Tooltip("Whether boundedScore should be used instad of the controller movement (for debugging).")]
    public bool controlWithInspector = false;
    [Tooltip("For debugging, negative value climbs up, positive value climbs down.")]
    [Range(-1, 1)]
    public float boundedScore = 0.0f;

    public GameObject performanceBar;
    Material performanceBarMaterial;
    public Color lowPerformanceColor = Color.red;
    public Color highPerformanceColor = Color.green;

    [Tooltip("Boost the climbing at startup, controllers must be connected for it to work.")]
    public bool useStartupBoost = false;
    [Tooltip("Negative value climbs up, positive value climbs down.")]
    public float startupBoost = -1;


    // 
    private bool climbLocked = false;

    private float NormilizedShakeIntensity(float intervalShakeIntensity)
    {

        return Mathf.Clamp((Mathf.Abs(intervalShakeIntensity) - minForNormalization) / (maxForNormalization - minForNormalization), 0, 1);
    }

    private void TryToClimb(float latencyTime, int step)
    {
        // do nothing if climbing or if control is locked
        if (checker.LimbMoving || climbLocked)
            return;

        StartCoroutine(DeferredClimb(latencyTime, step));
    }

    /// <summary>
    /// Defer the climb gesture by postponeTime seconds
    /// </summary>
    /// <param name="latencyTime"></param>
    /// <returns></returns>
    IEnumerator DeferredClimb(float latencyTime, int step)
    {
        step = step > 0 ? 1 : -1;
        string direction = step > 0 ? "UP" : "DOWN";
        climbLocked = true;
        yield return new WaitForSeconds(latencyTime);
        climbLocked = false;
        // optimal order : feet first then hands at the same level
        if (checker.CheckNextDistance(Lfoot: step))
            checker.LeftFoot(direction);
        else if (checker.CheckNextDistance(Rfoot: step))
            checker.RightFoot(direction);
        else if (checker.CheckNextDistance(Lhand: step) && checker.HcurrentPos[0] < checker.HcurrentPos[1])
            checker.LeftHand(direction);
        else if (checker.CheckNextDistance(Rhand: step))
            checker.RightHand(direction);
        else if (checker.CheckNextDistance(Lhand: step))
            checker.LeftHand(direction);

    }

    void Start () {
        if (checker == null)
            checker = transform.GetComponent<Checker>();

        // allocate array
        upDownControllers = new UpDownController[controllers.Length];

        // initialize shakeControllers objects
        for (int i = 0; i < controllers.Length; i++)
        {
            upDownControllers[i] = new UpDownController(controllers[i].transform, interval);
            performanceBar.GetComponent<Renderer>().material = new Material(performanceBar.GetComponent<Renderer>().material);
            if (useStartupBoost)
            {
                upDownControllers[i].ManuallyAddEntry(Time.time, startupBoost);
            }
        }
        if (performanceBar == null)
            performanceBar = GameObject.CreatePrimitive(PrimitiveType.Quad);
        performanceBar.transform.localScale = Vector3.one;
        // Shader.Find("Unlit/Color"));
        performanceBarMaterial = performanceBar.GetComponent<Renderer>().sharedMaterial;

    }

    void LateUpdate () {
        float intervalShakeIntensity = 0;
        for (int i = 0; i < upDownControllers.Length; i++)
        {
            if (controllers[i].controller == null)
                break;
            if(controllers[i].controller.GetHairTriggerDown())
                upDownControllers[i].PressButton();
            if(controllers[i].controller.GetHairTrigger())
                upDownControllers[i].HoldButton();
            intervalShakeIntensity += upDownControllers[i].GetIntensity();
        }

        intervalShakeIntensity /= upDownControllers.Length;

        if (controlWithInspector)
        {
            intervalShakeIntensity = boundedScore;
        }

        float normIntensity = NormilizedShakeIntensity(intervalShakeIntensity);
        float timedelay = 1 - normIntensity;
        if (normIntensity > 0.001f ) {
            

            if (intervalShakeIntensity < 0)
            {
                TryToClimb(timedelay * maxLatency, 1);
            }
            else
                TryToClimb(maxLatency, -1);
        }

        go.text = "Intensity: " + intervalShakeIntensity.ToString("F4") + "\n" + "Delay: " + ((timedelay + 1) * 0.5f).ToString("F4");

        performanceBarMaterial.color = lowPerformanceColor * timedelay + highPerformanceColor * (1 - timedelay);
        //performanceBar.transform.localScale = new Vector3((1 - timedelay) * 0.9f + 0.1f, 1, 1);
        performanceBar.transform.localScale = new Vector3((1-timedelay) * 0.5f, 1, 1);
        performanceBar.transform.localPosition = new Vector3(((1 - timedelay) * 0.25f) * ((intervalShakeIntensity < 0) ? -1 : 1), 0, 0);
    }
}
