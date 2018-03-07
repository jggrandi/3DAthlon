using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeController
{

    List<float> timeStamp = new List<float>();
    List<Vector3> positions = new List<Vector3>();
    public float interval = 0.3f;
    Transform controller;

    public ShakeController(float shakeInterval, Transform shakeController)
    {
        timeStamp = new List<float>();
        positions = new List<Vector3>();
        interval = shakeInterval;
        controller = shakeController;
    }

    /// <summary>
    /// Insert and maintain the desires time interval in the lists
    /// </summary>
    /// <returns></returns>
    public void UpdateValues()
    {
        timeStamp.Add(Time.time);
        positions.Add(controller.localPosition);

        while (timeStamp.Count > 2 && (timeStamp[timeStamp.Count-1] - timeStamp[0]) > interval)
        {
            timeStamp.RemoveAt(0);
            positions.RemoveAt(0);
        }


    }

    /// <summary>
    /// Shake intensity based on the sum of discrete acceleration values in the interval
    /// </summary>
    /// <returns></returns>
    public float GetShakeIntensity()
    {
        float intensity = 0;
        for (int i = 0; i < positions.Count - 2; i++)
        {
            float acc = Mathf.Abs((positions[i + 2] - positions[i + 1]).magnitude - (positions[i + 1] - positions[i]).magnitude);
            acc = acc / ((timeStamp[i + 2] - timeStamp[i]) * 0.5f);
            intensity += acc;
        }
        intensity /= positions.Count;
        return intensity;
    }

}


public class ShakeControl : MonoBehaviour {
    [Tooltip("The script that checks the available movements.")]
    public Checker checker;
    [Tooltip("The movement controllers.")]
    public Transform[] controllers;

    private ShakeController[] shakeControllers;

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

    public bool controlWithInspector = false;
    [Range(0,1)]
    public float normalizedScore = 0.0f;

    public GameObject performanceBar;
    Material performanceBarMaterial;
    public Color lowPerformanceColor = Color.red;
    public Color highPerformanceColor = Color.green;

    // 
    private bool climbLocked = false;

    private float NormalizedShakeIntensity(float intervalShakeIntensity)
    {
        return Mathf.Clamp ((intervalShakeIntensity - minForNormalization) / (maxForNormalization - minForNormalization), 0, 1);
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

    private void Start () {
        if (checker == null)
            checker = transform.GetComponent<Checker>();

        // allocate array
        shakeControllers = new ShakeController[controllers.Length];
        
        // initialize shakeControllers objects
        for (int i = 0; i < controllers.Length; i++)
        {
            shakeControllers[i] = new ShakeController(interval, controllers[i]);
            performanceBar.GetComponent<Renderer>().material = new Material(performanceBar.GetComponent<Renderer>().material);
        }
        if (performanceBar == null)
            performanceBar = GameObject.CreatePrimitive(PrimitiveType.Quad);
        performanceBar.transform.localScale = Vector3.one;
        // Shader.Find("Unlit/Color"));
        performanceBarMaterial = performanceBar.GetComponent<Renderer>().sharedMaterial;
        

    }

    private void LateUpdate () {


        float intervalShakeIntensity = 0;
        for (int i = 0; i < shakeControllers.Length; i++)
        {
            shakeControllers[i].UpdateValues();
            intervalShakeIntensity += shakeControllers[i].GetShakeIntensity();
        }
        intervalShakeIntensity /= shakeControllers.Length;

        float timedelay = 1 - NormalizedShakeIntensity(intervalShakeIntensity);

        if (controlWithInspector)
        {
            timedelay = normalizedScore;
        }

        if (timedelay < 0.99f)
        {
            TryToClimb(timedelay * maxLatency, 1);
        }else
            TryToClimb(maxLatency, -1);

        go.text = "Intensity: " + intervalShakeIntensity.ToString("F4") + "\n" + "Delay: " + timedelay.ToString("F4");


        performanceBarMaterial.color = lowPerformanceColor  * timedelay + highPerformanceColor * (1 - timedelay);
        performanceBar.transform.localScale = new Vector3((1 - timedelay) * 0.9f + 0.1f, 1, 1);
        performanceBar.transform.localPosition = new Vector3(timedelay * 0.45f , 0, 0);
    }




}
