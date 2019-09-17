using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adding OnTouch3D here forces us to implement the 
// OnTouch function, but also allows us to reference this
// object through the OnTouch3D class.
public class DiceGenerator : MonoBehaviour, OnTouch3D
{
    // Debouncing is a term from Electrical Engineering referring to 
    // preventing multiple presses of a button due to the physical switch
    // inside the button "bouncing".
    // In CS we use it to mean any action to prevent repeated input. 
    // Here we will simply wait a specified time before letting the button
    // be pressed again.
    // We set this to a public variable so you can easily adjust this in the
    // Unity UI.
    public float debounceTime = 0.3f;

    public float startingHeight = 1.6f;
    public GameObject originalDie;
    public Transform gameSetTransform;

    // Whether to allow user to click to generate
    internal bool enableUserClick = true;
    private BoxCollider boxCollider;
    private List<GameObject> dice;

    // Stores a counter for the current remaining wait time.
    private float remainingDebounceTime;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        dice = new List<GameObject>();
    }

    void Update()
    {
        // Time.deltaTime stores the time since the last update.
        // So all we need to do here is subtract this from the remaining
        // time at each update.
        if (remainingDebounceTime > 0)
            remainingDebounceTime -= Time.deltaTime;
    }

    public void ClearDice()
    {
        foreach (GameObject obj in dice) {
            obj.SetActive(false);
        }
        dice.Clear();
    }

    public void BeginGenerating()
    {
        remainingDebounceTime = debounceTime;
    }

    public void GenerateDieAtRandomPoint()
    {
        Bounds bounds = boxCollider.bounds;
        GenerateDie(new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)));
    }

    public void GenerateDie(Vector3 point)
    {
        GameObject newDie = GameObject.Instantiate(originalDie);
        newDie.transform.parent = gameSetTransform;
        newDie.transform.localPosition = new Vector3(point.x - transform.position.x,
                                                     startingHeight,
                                                     point.z - transform.position.z);
        //newDie.transform.localScale = new Vector3(1.f, 1.f, 1.f);
        newDie.transform.localRotation = Random.rotation;
        //newDie.GetComponent<Renderer>().enabled = true;
        newDie.SetActive(true);
        dice.Add(newDie);
        Rigidbody rigidBody = newDie.GetComponent<Rigidbody>();
        Vector3 torque;
        torque.x = Random.Range(-200, 200);
        torque.y = Random.Range(-200, 200);
        torque.z = Random.Range(-200, 200);
        rigidBody.AddTorque(torque);

        DieController dieController = newDie.GetComponent<DieController>();
        dieController.OnCreateDie();
        dieController.enabled = true;
    }

    public void OnTouch(Vector3 point)
    {
        // If a touch is found and we are not waiting,
        if (remainingDebounceTime <= 0 && enableUserClick)
        {
            GenerateDie(point);
            remainingDebounceTime = debounceTime;
        }
    }
}