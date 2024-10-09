using Godot;
using Scripts.Constants;

public partial class CitizenInfo : Panel
{
	private TextureRect _icon;
	public TextureRect Background;
	public Label Name;
	public Label Job;
	public Label Happiness;
	public Npc CitizenNpc;
	public bool focused;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("VBoxContainer/TextureRect");
		Position = new Vector2(0, 200);
		Name = GetNode<Label>("VBoxContainer/Name");
		Job = GetNode<Label>("VBoxContainer/Job");
		Happiness = GetNode<Label>("VBoxContainer/Happiness");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetInfo(Npc npc)
	{
		CitizenNpc = npc;
		
		Job.Text = CitizenNpc.Work is not null ? $"Works at {CitizenNpc.Work.GetBuildingName()}" : GameResource.Unemployed;

		Name.Text = CitizenNpc.CitizenName;
		Happiness.Text = $"Happiness: {GameLogistics.ConvertHappiness(npc.Happiness)}/n" + npc.GetUnhappyReason();
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
