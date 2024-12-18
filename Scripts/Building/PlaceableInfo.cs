using Godot;
using KingdomCome.Scripts.Building;

public partial class PlaceableInfo : Panel
{
	
	[Signal]
	public delegate void OnDeleteEventHandler();

	[Signal]
	public delegate void OnMoveEventHandler();

	[Signal]
	public delegate void OnUpgradeEventHandler();
	
	[Signal]
	public delegate void OnTurnOffBuildingEventHandler();
	
	private Label _buildingName;
	private Control _houseInfo;
	private Label _textLabel;
	private RichTextLabel _upgradesInfo;
	public PackedScene CitizenPortrait;

	public bool Focused;
	public HFlowContainer PortraitContainer;

	public Button DeleteButton;
	public Button UpgradeButton;

	public override void _Process(double delta)
	{
	}

	public override void _Ready()
	{
		_buildingName = GetNode<Label>("HouseInfo/BuildingName");
		_textLabel = GetNode<Label>("HouseInfo/BuildingInfo");
		PortraitContainer = GetNode<HFlowContainer>("HouseInfo/CitizenPortraitContainer");
		CitizenPortrait = ResourceLoader.Load<PackedScene>("res://Scenes/Building/CitizenPortraitButton.tscn");
		_houseInfo = GetNode<Control>("HouseInfo");
		_upgradesInfo = GetNode<RichTextLabel>("UpgradesDescription");
		DeleteButton = GetNode<Button>("HouseInfo/ButtonContainer/DeleteButton");
		UpgradeButton = GetNode<Button>("HouseInfo/ButtonContainer/UpgradeButton");
	}
	
	

	public void OnTurnOffButtonPressed()
	{
		EmitSignal(SignalName.OnTurnOffBuilding); //emitted to Production
		Button turnoffbutton = GetNode<Button>("HouseInfo/TurnOffButton");
		if (turnoffbutton.IsPressed())
		{
			turnoffbutton.Text = "Turn On";
		}
		else
		{
			turnoffbutton.Text = "Turn Off";
		}
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

	public void UpdateInfo(string buildingName, string text = "", string upgrades = "")
	{
		_buildingName.Text = buildingName;
		_textLabel.Text = text;
		_upgradesInfo.Text = upgrades;
		foreach (var child in PortraitContainer.GetChildren())
		{
			var portrait = child as CitizenPortraitButton;
			if (portrait.npc.CurrentBuilding == null)
			{
				portrait.SelfModulate = new Color("#696969");
				portrait.TooltipText = "Not in building";
				continue;
			}
			if (portrait.npc.CurrentBuilding.CurrentPeople.Contains(portrait.npc))
			{
				portrait.SelfModulate = new Color("#696969");
				portrait.TooltipText = "Not in building";
			}
			else
			{
				portrait.SelfModulate = new Color("#fff");
				portrait.TooltipText = "";
			}
		}
	}
}
