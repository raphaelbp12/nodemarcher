using System;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score { get; private set; }
    Vector3 previousPosition;
    public float timeDifferenceToZeroStreak {get; private set; } = 5f;
    public float lastTimeEating {get; private set; } = 0;
    public float distanceInStreak {get; private set; } = 0;
    public int streakMultiplier {get; private set; } = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetCurrentScore()
    {
        int increment = Mathf.CeilToInt(distanceInStreak * streakMultiplier);
        int currentScore = score + increment;
        return currentScore.ToString();
    }

    public void FinishStreak()
    {
        int increment = Mathf.CeilToInt(distanceInStreak * streakMultiplier);
        score += increment;
        distanceInStreak = 0;
        streakMultiplier = 1;
    }

    public void ComputeStreak(Vector3 foodPosition)
    {
        float distance = Vector3.Distance(previousPosition, foodPosition);
        distanceInStreak += distance;
        streakMultiplier++;
        previousPosition = foodPosition;
    }

    public void ComputeEating(Vector3 foodPosition)
    {
        ComputeStreak(foodPosition);

        float timeDifference = Time.time - lastTimeEating;
        if (timeDifference > timeDifferenceToZeroStreak)
        {
            FinishStreak();
        }
        lastTimeEating = Time.time;
    }
}
