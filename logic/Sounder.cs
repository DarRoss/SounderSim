using System.Collections.Generic;
using Godot;

/**
 * A herd of hogs.
 */
public partial class Sounder : Node2D
{
    // Time between sounder announcements.
    [Export]
    private float communicationInterval = 0.1f;
    // All hogs in the sounder should travel at this speed.
    [Export]
    private float sounderDesiredSpeed = 100;
    [Signal]
    public delegate void AnnounceSounderInfoEventHandler(S2hInfo packet);

    // node whose children belong to the hog class
    private Node2D hogChildrenNode;
    // timer for sounder announcements
    private Timer intermittentTimer = new();
    private S2hInfo sounderInfo = new();

    // dictionary containing information of all hogs in the sounder
    private readonly Dictionary<string, H2sInfo> hogInfoDict = new();
    // sum of all hog global positions in the sounder
    private Vector2 birdseyePositionSum = Vector2.Zero;
    // sum of all polar coordinate directions in the sounder
    private float polarDirectionSum = 0;

    public override void _Ready()
    {
        hogChildrenNode = GetNode<Node2D>("HogChildren");
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
        sounderInfo.DesiredSpeed = sounderDesiredSpeed;

        AnnounceUpdatedSounderInfo();
        SetupIntermittentTimer();
    }

    private void SetupIntermittentTimer()
    {
        AddChild(intermittentTimer);
        // attach intermittent calculations to timer
        intermittentTimer.Timeout += AnnounceUpdatedSounderInfo;
        intermittentTimer.WaitTime = communicationInterval;
        intermittentTimer.Start();
    }

    /**
     * Add a hog to this sounder.
     */
    private bool AddHogChild(Hog hogChild)
    {
        bool success = hogInfoDict.TryAdd(hogChild.Name, hogChild.HogInfoPacket);
        // add the hog to the info dictionary
        if(success)
        {
            ConnectChildSignals(hogChild);
            birdseyePositionSum += hogInfoDict[hogChild.Name].BirdseyePosition;
            polarDirectionSum += hogInfoDict[hogChild.Name].PolarDirection;
        }
        return success;
    }

    /**
     * Remove a hog from this sounder.
     */
    private bool RemoveHogChild(Hog hogChild)
    {
        bool success = hogInfoDict.ContainsKey(hogChild.Name);
        // only remove if the hog is in the dictionary
        if(success)
        {
            DisconnectChildSignals(hogChild);
            polarDirectionSum -= hogInfoDict[hogChild.Name].PolarDirection;
            birdseyePositionSum -= hogInfoDict[hogChild.Name].BirdseyePosition;
            hogInfoDict.Remove(hogChild.Name);
        }
        return success;
    }

    private void OnReceiveHogInfo(H2sInfo newInfo)
    {
        // get the hog's node from our current list of hogs
        Node child = hogChildrenNode.FindChild(newInfo.HogName, false);
        if(child != null && child is Hog hogChild)
        {
            if(newInfo.IsAlive)
            {
                if(hogInfoDict.TryGetValue(newInfo.HogName, out H2sInfo oldInfo))
                {
                    // subtract the old direction from the sum and add the new one
                    polarDirectionSum += newInfo.PolarDirection - oldInfo.PolarDirection;
                    // subtract the old position from the sum and add the new one
                    birdseyePositionSum += newInfo.BirdseyePosition - oldInfo.BirdseyePosition;

                    // replace dictionary entry with new packet
                    hogInfoDict[oldInfo.HogName] = newInfo;
                }

            }
            else
            {
                if(RemoveHogChild(hogChild))
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
        }
    }

    /**
     * Connect all necessary signals belonging to a hog child.
     */
    private void ConnectChildSignals(Hog hogChild)
    {
        // hog to sounder (H2S)
        hogChild.AnnounceHogInfo += OnReceiveHogInfo;
        // sounder to hog (S2H)
        AnnounceSounderInfo += hogChild.OnReceiveSounderInfo;
    }

    /**
     * Disconnect all necessary signals belonging to a hog child.
     */
    private void DisconnectChildSignals(Hog hogChild)
    {
        // hog to sounder (H2S)
        hogChild.AnnounceHogInfo -= OnReceiveHogInfo;
        // sounder to hog (S2H)
        AnnounceSounderInfo -= hogChild.OnReceiveSounderInfo;
    }

    /**
     * Emit signals informing all hog children of sounder information.
     */
    private void AnnounceUpdatedSounderInfo()
    {
        sounderInfo.AveragePolarDirection = polarDirectionSum / hogInfoDict.Count;
        sounderInfo.AverageBirdseyePosition = birdseyePositionSum / hogInfoDict.Count;
        EmitSignal(SignalName.AnnounceSounderInfo, sounderInfo);
    }
}