using Godot;
using System;

public partial class PlaceableInfo : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnButtonDownPressed(){
				GD.Print("button down");

		
	}
	
	public void OnDeleteButtonPressed()
	{
		Console.WriteLine("Delete");
		GD.Print("Delete");
		//GetParent().QueueFree();
	}
}
