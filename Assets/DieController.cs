using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieController : MonoBehaviour
{
    public GameStateManager gameStateManager;
    public float movementThresh = 1e-3f;
    public float startingHeight = 1.6f;
    internal int rollingPlayer;

    private List<Vector3> directions;
    private bool isStopped;
    private Vector3 prevLocation;

    // Start is called before the first frame update
    void Start()
    {
        directions = new List<Vector3>();
        directions.Add(Vector3.forward);
        directions.Add(Vector3.down);
        directions.Add(Vector3.left);
        directions.Add(Vector3.right);
        directions.Add(Vector3.up);
        directions.Add(Vector3.back);
    }

    public void OnCreateDie()
    {
        isStopped = false;
        prevLocation = transform.position;
        rollingPlayer = gameStateManager.player;
    }

    void OnDieStop()
    {
        int num = GetNumber();
        if (rollingPlayer == gameStateManager.player)
            gameStateManager.AddDieRoll(num);
    }

    public int GetNumber()
    {
        // here I would assert lookup is not empty, epsilon is positive and larger than smallest possible float etc
        // Transform reference up to object space
        Vector3 referenceObjectSpace = transform.InverseTransformDirection(Vector3.up);

        // Find smallest difference to object space direction
        float best = float.MaxValue;
        int bestFaceValue = -1;
        for (int i = 0; i < directions.Count; ++i)
        {
            float a = Vector3.Angle(referenceObjectSpace, directions[i]);
            if (a < best)
            {
                best = a;
                bestFaceValue = i+1;
            }
        }
        return bestFaceValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped) return;
        float dist = Vector3.Distance(transform.position, prevLocation);
        isStopped = dist < movementThresh;
        if (isStopped)
        {
            OnDieStop();
        }
        else
        {
            prevLocation = transform.position;
        }
    }
}
