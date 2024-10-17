using Godot;

public partial class PlaceableInfo : Panel
{
	[Signal]
	public delegate void OnChooseWareEventHandler();

	[Signal]
	public delegate void OnDeleteEventHandler();

	[Signal]
	public delegate void OnMoveEventHandler();

	[Signal]
	public delegate void OnUpgradeEventHandler();

	private Label _buildingName;
	private Control _houseInfo;
	private RichTextLabel _textLabel;
	public PackedScene CitizenPortrait;

	public bool Focused;
	public HFlowContainer PortraitContainer;

	public override void _Process(double delta)
	{
	}

	public override void _Ready()
	{
		_buildingName = GetNode<Label>("HouseInfo/BuildingName");
		_textLabel = GetNode<RichTextLabel>("HouseInfo/RichTextLabel");
		PortraitContainer = GetNode<HFlowContainer>("HouseInfo/CitizenPortraitContainer");
		CitizenPortrait = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenPortraitButton.tscn");
		_houseInfo = GetNode<Control>("HouseInfo");
	}

	public void OnButtonDownPressed()
	{
		GD.Print("button down");
	}

	public void OnChooseWareButtonPressed()
	{
		EmitSignal(SignalName.OnChooseWare);
	}

	public void OnDeleteButtonPressed()
	{
		EmitSignal(SignalName.OnDelete); //emitted to AbstractPlaceable
	}

	public void OnUpgradeButtonPressed()
	{
		EmitSignal(SignalName.OnUpgrade); //emitted to AbstractPlaceable
	}

	public void OnMoveButtonPressed()
	{
		EmitSignal(SignalName.OnMove); //emitted to AbstractPlaceable
	}

	public void ShowNpcInfo(Npc npc)
	{
		_houseInfo.Visible = false;
		npc.ShowInfo();
	}

	public void HideNpcInfo()
	{
		_houseInfo.Visible = true;
	}

	private void OnMouseEntered()
	{
		Focused = true;
	}

	private void OnMouseExited()
	{
		Focused = false;
	}

	public void UpdateInfo(string buildingName, string text ="")
	{
		_buildingName.Text = buildingName;
		_textLabel.Text = text;
	}
}
