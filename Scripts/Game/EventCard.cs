using Godot;
using System;
using System.Collections.Generic;
using Scripts.Constants;

public partial class EventCard : Control
{
	public Label Title;
	public Label Description;
	public Button DoneButton;
	public HBoxContainer ButtonContainer;
	public List<Button> buttons = [];
	public GameMap Gamemap;
	

	[Signal]
	public delegate void OnEventDoneEventHandler(EventCard card);

	public override void _Ready()
	{
		Gamemap = GetParent().GetParent().GetParent<GameMap>();
	}

	public void SetFields()
	{
		Title = GetNode<Label>("Panel/VBoxContainer/Title");
		Description = GetNode<Label>("Panel/VBoxContainer/PromptText");
		ButtonContainer = GetNode<HBoxContainer>("Panel/HBoxContainer");
		DoneButton = GetNode<Button>("Panel/DoneContainer/Button");

	}

	public void AddButtons(int amount)
	{
		
		for (int i = 0; i < amount; i++)
		{
			var button = new Button();
			ButtonContainer.AddChild(button);
			button.AddThemeFontSizeOverride("font_size",22);
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
