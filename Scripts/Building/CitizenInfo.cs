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
	private Button _unemploy;
	private bool _unemployedAdded;

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
		if (!GameMap.TutorialMode)
		{
			//GetNode<Button>("HBoxContainer/ChangeJob").ToggleMode = false;
		}
		if (_unemploy is null)
		{
			_unemploy = new Button();
			_unemploy.Pressed += () =>
			{
				npc.GetJob(null);
			};
			_unemploy.Text = "Unemploy";
			GetNode<HBoxContainer>("HBoxContainer").AddChild(_unemploy);
		}

		if (npc.Work != null)
			_unemploy.Visible = true;

		else if(_unemployedAdded)
			_unemploy.Visible = false;
		
		
		
		CitizenNpc = npc;
		Job.Text = CitizenNpc.Work is not null ? $"Works at {CitizenNpc.Work.GetBuildingName()}" : NpcStatuses.Unemployed;
		Job.Text += CitizenNpc.Home is null ? "\nHomeless" : "";
		//Job.Text += $"\n At Work: {npc.AtWork}";
		Name.Text = CitizenNpc.CitizenName;
		Happiness.Text = $"Happiness: {GameLogistics.ConvertHappiness(npc.Happiness)}" + "\n"+ npc.GetUnhappyReason();
		
		_icon.Texture = npc.Sprite;
		if (npc.Work == null)
		{
			GetNode<Button>("HBoxContainer/ChangeJob").Text = "Give Job";
		}
		else
		{
			GetNode<Button>("HBoxContainer/ChangeJob").Text = "Change Job";
		}
		MoveToFront();
	}

	public void OnChangeJobButtonPressed()
	{
		Visible = false;
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
