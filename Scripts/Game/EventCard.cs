using Godot;
using System;
using System.Collections.Generic;

public partial class EventCard : Control
{
	public Label Title;
	public Label Description;
	public Button DoneButton;
	public HBoxContainer ButtonContainer;
	public List<Button> buttons = [];
	

	[Signal]
	public delegate void OnEventDoneEventHandler(EventCard card);

	public override void _Ready()
	{
		//SetAnchorsPreset(LayoutPreset.Center);
	}

	public void SetFields()
	{
		Title = GetNode<Label>("Panel/VBoxContainer/Title");
		Description = GetNode<Label>("Panel/VBoxContainer/PromptText");
		ButtonContainer = GetNode<HBoxContainer>("Panel/HBoxContainer");
		DoneButton = GetNode<Button>("Panel/DoneContainer/Button");
		DoneButton.Pressed += () =>
		{
			EmitSignal(SignalName.OnEventDone, this);
		};
	}

	public void OnEventCompleted()
	{
		GetParent().RemoveChild(this);
		QueueFree();
	}

	public void AddButtons(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			var button = new Button();
			ButtonContainer.AddChild(button);
			button.Pressed += () =>
			{
				OnEventChoiceSelected();
			};
			buttons.Add(button);
		}
	}
	public void OnEventChoiceSelected()
	{
		DoneButton.Visible = true;
		ButtonContainer.Visible = false;
	}
}
