using Godot;

public partial class HogInfoLabel : Label
{
    public override void _Ready()
    {
		Global.Instance.InspectionHogChanged += UpdateText;
    }

    private void UpdateText()
	{
		Hog currHog = Global.Instance.InspectionHog;
		if(currHog != null)
		{
			string hogIdStr = "Hog ID: " + currHog.Identifier + "\n";
			string sdrIdStr = "Sounder ID: " + currHog.OwnerSounder.Identifier + "\n";
			Text = hogIdStr + sdrIdStr;
		}
	}
}
