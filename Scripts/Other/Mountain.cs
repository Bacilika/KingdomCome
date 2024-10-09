using Godot;
using System;
using System.Collections.Generic;

public partial class Mountain : Area2D
{
	private PackedScene _treeScene;
	private CollisionShape2D area;
	private int radius;
	private RandomNumberGenerator rnd;

	public override void _Ready()
	{
		area = GetNode<CollisionShape2D>("CollisionShape2D");
		radius = (int)area.Shape.GetRect().Size.X / 2;
		rnd = new RandomNumberGenerator();

		_treeScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Tree.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
