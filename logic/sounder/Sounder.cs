using System.Collections.Generic;
using Godot;

/**
 * A herd of hogs.
 */
public partial class Sounder : Node
{
    [Export]
    private float recalculationInterval = 0.1f;
    // All hogs in the sounder should travel at this speed.
    [Export]
    private float sounderDesiredSpeed = 15;

    [Export]
    public Vector2 StartPosition{get; private set;}

    // node whose children belong to the hog class
    private Node hogChildrenNode;
    // timer for sounder announcements
    private Timer intermittentTimer = new();

    // dictionary containing information of all hogs in the sounder
    private readonly Dictionary<string, Hog> hogDict = new();

    /**
     * public sounder variables
     */
    // average birdseye position of all hogs in the sounder
    public Vector2 AverageBirdseyePosition{get; private set;}
    // average polar coordinate direction of all hogs in the sounder. -pi inclusive to +pi uninclusive
    public float AveragePolarDirection{get; private set;}
    // speed that the sounder wants all hogs to travel at
    public float DesiredSpeed{get; private set;}

    public override void _Ready()
    {
        hogChildrenNode = GetNode("HogChildren");
        foreach(Node child in hogChildrenNode.GetChildren())
        {
            if (child is Hog hogChild)
            {
                AddHogChild(hogChild);
            }
            else
            {
                GD.PushError("Sounder contains child which is not the 'Hog' class");
            }
        }
        // update packet information
        DesiredSpeed = sounderDesiredSpeed;

        UpdateAverages();
        SetupIntermittentTimer();
    }

    private void SetupIntermittentTimer()
    {
        AddChild(intermittentTimer);
        // attach intermittent calculations to timer
        intermittentTimer.Timeout += UpdateAverages;
        intermittentTimer.WaitTime = recalculationInterval;
        intermittentTimer.Start();
    }

    /**
     * Add a hog to this sounder.
     */
    private bool AddHogChild(Hog hogChild)
    {
        bool success = hogDict.TryAdd(hogChild.Name, hogChild);
        return success;
    }

    /**
     * Remove a hog from this sounder.
     */
    private bool RemoveHogFromDict(string hogChildName)
    {
        bool success = hogDict.ContainsKey(hogChildName);
        // only remove if the hog is in the dictionary
        if(success)
        {
            success = hogDict.Remove(hogChildName);
        }
        return success;
    }

    public void OnReceiveHogDeath(string hogName)
    {
        if(RemoveHogFromDict(hogName))
        {
            // check if there are no more hogs in the sounder
            if(hogChildrenNode.GetChildCount() <= 0)
            {
                // disband the sounder
                QueueFree();
                GD.Print("Sounder '" + Name + "' has been disbanded");
            }
            else
            {
                // there are still more hogs in the sounder
            }
        }
        else
        {
            // we tried to remove a hog that didn't exist
        }
    }

    private void UpdateAverages()
    {
        // sum of all hog global positions in the sounder
        Vector2 birdseyePositionSum = Vector2.Zero;
        // sums for calculating circular mean
        float sinSum = 0;
        float cosSum = 0;
        float sinAngle;
        float cosAngle;

        foreach(KeyValuePair<string, Hog> entry in hogDict)
        {
            birdseyePositionSum += entry.Value.BirdseyePosition;
            (sinAngle, cosAngle) = Mathf.SinCos(entry.Value.PolarDirection);
            sinSum += sinAngle;
            cosSum += cosAngle;
        }
        AveragePolarDirection = Mathf.Atan2(sinSum, cosSum);
        AverageBirdseyePosition = birdseyePositionSum / hogDict.Count;
    }
}