using Godot;
using System;
using System.Threading;
using Godot.Collections;
using Scripts.Constants;

public partial class GameMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	
	private AbstractPlaceable _object;
	public static AbstractPlaceable SelectedPlaceable;
	public static bool ContainHouse;
	private int _money = 50000;
	public static int Citizens;
	public static int Happiness;
	public static int Food;
	public static int Stone;
	public static int WorkingCitizens; 
	private TextureRect _textureRect;
	private Label _moneyLabel;
	private Label _foodLabel;
	private Label _stoneLabel;
	private Label _citizensLabel;
	private Label _happinessLabel;
	private Dictionary<String, PackedScene> _packedScenesscenes;

	
	[Signal]
	public delegate void HousePlacedEventHandler(Node2D house);
	
	
	public override void _Ready()
	{
		var menuCanvasLayer = GetNode<CanvasLayer>("MenuCanvasLayer");
		_textureRect = menuCanvasLayer.GetNode<TextureRect>("TextureRect");
		_moneyLabel = _textureRect.GetNode<Label>("Money");
		_foodLabel = _textureRect.GetNode<Label>("Food");
		_citizensLabel = _textureRect.GetNode<Label>("Citizens");
		_stoneLabel = _textureRect.GetNode<Label>("Stone");
		_happinessLabel = _textureRect.GetNode<Label>("Happiness");
		_packedScenesscenes = new Dictionary<String, PackedScene> { { "House", ResourceLoader.Load<PackedScene>("res://Scenes/House.tscn") },
			{"FarmHouse", ResourceLoader.Load<PackedScene>("res://Scenes/FarmHouse.tscn")},
			{"StoneMine", ResourceLoader.Load<PackedScene>("res://Scenes/StoneMine.tscn")}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateMenuInfo();
		_textureRect.Size = new Vector2(GetTree().Root.Size.X,_textureRect.Size.Y);

	}
	public void OnHouseButtonPressed(String type)
	{
		Console.WriteLine(type);
		var house = _packedScenesscenes[type].Instantiate<AbstractPlaceable>();
		house.Position = GetViewport().GetMousePosition();
		var baseNode = GetParent();
		baseNode.AddChild(house);
		_object = house;
	}

	public override void _Input(InputEvent @event)
	{
		if (_object == null)
		{
			if (SelectedPlaceable is not null && @event.IsActionPressed(Inputs.LeftClick))
			{
				SelectedPlaceable.ShowBuildingInfoScreen();
			}
			
		}

		else if (@event.IsActionPressed(Inputs.LeftClick))
		{
			if (CanPlace())
			{
				_money -= _object.GetBuildingPrice();
				var placedHouse =  _object.Duplicate();
				EmitSignal(SignalName.HousePlaced, placedHouse);
			}

		}

		if (@event.IsActionPressed((Inputs.RightClick)))
		{
			_object?.QueueFree();
			_object = null;
		}
	}

	public void UpdateMenuInfo()
	{
		_moneyLabel.Text = "Money: " + _money;
		_citizensLabel.Text = "Citizens: " + Citizens;
		_happinessLabel.Text = "Happiness: " + Happiness;
		_foodLabel.Text = "Food: " + Food;
		_stoneLabel.Text = "Stone: " + Stone;
	}

	private bool CanPlace()
	{
		return ContainHouse == false && _object.GetBuildingPrice() <= _money;
	}
	
	
}
