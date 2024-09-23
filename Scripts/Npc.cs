using Godot;
using System;

public partial class Npc : RigidBody2D
{
	private AbstractPlaceable _home;
	private NavigationAgent2D _navigation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("Npc Ready");
		_home = GetNode<AbstractPlaceable>("House");
		_navigation = GetNode<NavigationAgent2D>("NavigationAgent2D");
		Position = _home.GlobalPosition;
		_navigation.TargetPosition = GetNode<StoneMine>("res://Scenes/StoneMine.tscn").GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void move()
	{
		
	}
}
