using Godot;
using Scripts.Constants;

public partial class CitizenInfo : Panel
{
	private TextureRect _icon;
	public TextureRect Background;
	public Npc CitizenNpc;
	public bool focused;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("TextureRect");
		Position = new Vector2(0, 200);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetInfo(Npc npc)
	{
		CitizenNpc = npc;

		var work = GameResource.Unemployed;
		if (CitizenNpc.Work is not null) work = $" Work: {CitizenNpc.Work.GetBuildingName()}";

		var textLabel = GetNode<Label>("RichTextLabel");
		textLabel.Text = work +
						 "\nHappiness: " + GameLogistics.ConvertHappiness(npc.Happiness)
						 + "\n" + npc.GetUnhappyReason();

		_icon.Texture = npc.Sprite;
		MoveToFront();
	}

	public void OnChangeJobButtonPressed()
	{
		CitizenNpc.EmitSignal(Npc.SignalName.OnJobChange, CitizenNpc);
	}

	public void OnMouseEntered()
	{
		focused = true;
	}

	public void OnMouseExited()
	{
		focused = false;
	}
}
