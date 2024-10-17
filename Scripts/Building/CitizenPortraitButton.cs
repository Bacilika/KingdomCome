using Godot;

public partial class CitizenPortraitButton : Control
{
	public PlaceableInfo InfoBox;
	public Npc npc;

	public override void _Ready()
	{
		InfoBox = GetParent().GetParent().GetParent<PlaceableInfo>();
	}

	public void OnPortraitPressed()
	{
		InfoBox.Visible = false;
		npc.ShowInfo();
	}

	private void OnMouseEntered()
	{
		InfoBox.Focused = true;
		GrabFocus();
	}

	private void OnMouseExited()
	{
		ReleaseFocus();
		InfoBox.Focused = false;
	}
}
