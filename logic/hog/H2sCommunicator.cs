using Godot;

/*
 * H2sCommunicator: Send hog signals to and receive signals from the Sounder class.
 */
public partial class H2sCommunicator : Node
{
    [Signal]
    public delegate void AnnounceHogInfoEventHandler(H2sInfo packet);

    public S2hInfo SounderInfo{get; private set;}

    /*
     * Signal Receive Functions
     */
    public void OnReceiveSounderInfo(S2hInfo packet)
    {
        SounderInfo = packet;
    }

    /*
     * Signal Announcement Functions 
     */
    public void AnnounceHogInfoToSounder(H2sInfo packet)
    {
        EmitSignal(SignalName.AnnounceHogInfo, packet);
    }
}