using Godot;
using System;

public partial class CitizenInfo : Control
{
	public Npc CitizenNpc;
	public PlaceableInfo InfoBox;


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InfoBox = GetParent().GetParent<PlaceableInfo>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetInfo(Npc npc)
	{
		CitizenNpc = npc;
		var work = "Unemployed";
		if (CitizenNpc.Work is not null)
		{
			work = CitizenNpc.Work.GetBuildingName();
		}
		
		var textLabel = GetNode<RichTextLabel>("RichTextLabel");
		textLabel.Text = "Work: " + work;


	}

	public void OnChangeJobButtonPressed()
	{
		Console.WriteLine("Changing Job");
		CitizenNpc.EmitSignal(Npc.SignalName.OnJobChange, CitizenNpc);
	}

	public void OnReturnButtonPressed()
	{
		InfoBox.HideNpcInfo();
	}
	private void OnMouseEntered()
	{
		InfoBox.Focused = true;
		
	}
	private void OnMouseExited()
	{
		InfoBox.Focused = false;
		
	}
}
