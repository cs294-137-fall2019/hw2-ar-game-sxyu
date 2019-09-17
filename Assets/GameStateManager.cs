using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{

    public Text messageText;
    public Text score0Text;
    public Text score1Text;
    public Text turnScoreText;
    public GameObject messageTextPanel;
    public GameObject endTurnButton;
    public Text endTurnButtonText;
    public DiceGenerator diceGenerator;
    public int goal = 100;

    internal int turnScore, player;
    internal int[] score = new int[2];
    internal string[] names = { "Human", "AI" };

    public void ShowMessage(string text)
    {
        messageTextPanel.SetActive(true);
        messageText.text = text;
    }

    public void Reset()
    {
        player = 0;
        score[0] = score[1] = 0;
        turnScore = 0;
        diceGenerator.enableUserClick = (player == 0);
        diceGenerator.ClearDice();
    }
    public void UpdateScoreText()
    {
        score0Text.text = names[0] + ": " + score[0].ToString();
        score1Text.text = names[1] + ": " + score[1].ToString();
        if (player == 0)
            turnScoreText.text = names[0] + "'s turn: " + turnScore.ToString();
        else
            turnScoreText.text = names[1] + "'s turn: " + turnScore.ToString();
        endTurnButtonText.text = "End turn";
    }

    public bool CheckForWin()
    {
        for (int i = 0; i < 2; ++i) {
            if (score[i] >= goal)
            {
                UpdateScoreText();
                ShowMessage(names[i] + " wins! Resetting...");
                Reset();
                UpdateScoreText();
                return true;
            }
        }
        return false;
    }

    public bool AddDieRoll(int v)
    {
        if (v == 1)
        {
            turnScore = 1;
            EndTurn();
            return false;
        }
        else
        {
            turnScore += v;
            UpdateScoreText();
            return !CheckForWin();
        }
    }

    int bacon()
    {
        return 10 - Mathf.Min(score[1 - player] % 10, score[1 - player] / 10 % 10); 
    }

    IEnumerator RunAI()
    {
        int i;
        int numToRoll = 4;
        if (score[1] + bacon() >= goal)
        {
            numToRoll = 0;
        }
        float r = Random.Range(0.0f, 1.0f);
        if (r < 0.1f)
        {
            numToRoll = 5;
        }
        else if (r < 0.2f)
        {
            numToRoll = 3;
        }
        else if (r < 0.25f)
        {
            numToRoll = 1;
        }
        else if (r < 0.3f)
        {
            numToRoll = 6;
        }
        yield return new WaitForSecondsRealtime(2);
        for (i = 0; i < numToRoll; ++i)
        {
            if (player == 1)
            {
                diceGenerator.GenerateDieAtRandomPoint();
                yield return new WaitForSecondsRealtime(2);
            }
            else break;
        }
        if (player == 1 && i == numToRoll) EndTurn();
    }

    public void EndTurn()
    {
        if (turnScore == 0)
        {
            turnScore = bacon();
            ShowMessage(name[player] + " used free bacon\nGot " + turnScore + " points");
        }
        string message = "It's " + names[1 - player] + "'s turn";
        if (turnScore == 1)
        {
            ShowMessage("Rolled one :(\n" + message);
        } else
        {
            ShowMessage(message);
        }
        score[player] += turnScore;
        if (!CheckForWin())
        {
            player = 1 - player;
            turnScore = 0;

            diceGenerator.ClearDice();
            diceGenerator.enableUserClick = (player == 0);
            if (!diceGenerator.enableUserClick)
            {
                StartCoroutine(RunAI());
            }
            UpdateScoreText();
            endTurnButton.SetActive(player == 0);
            endTurnButtonText.text = "Take free " + bacon().ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
               
    }
}
