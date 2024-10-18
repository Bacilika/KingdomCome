using System;
using System.Collections.Generic;
using Godot;

namespace Scripts.Constants;

public class EventGenerator
{
    public static List<EventCard> eventCards = new List<EventCard>();
    


    public EventGenerator()
    {
        //variables 
        var eventScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/EventCard.tscn");
        var eventCard = eventScene.Instantiate<EventCard>();
        var title = eventCard.GetNode<Label>("Panel/VBoxContainer/Title");
        var description = eventCard.GetNode<Label>("Panel/VBoxContainer/PromptText");
        var button1 = eventCard.GetNode<Button>("Panel/HBoxContainer/Button");
        var button2 = eventCard.GetNode<Button>("Panel/HBoxContainer/Button2");
        
        //Event 1
        title.Text = "War!";
        description.Text = "Another nation has declared war!";
        button1.Text = "Attack";
        button2.Text = "Ask for peace";
        
        //create ok button
        Button buttonOk = new Button(); 
        buttonOk.Text = "Ok";
        buttonOk.Visible = false;
        eventCard.GetNode<HBoxContainer>("Panel/HBoxContainer").AddChild(buttonOk);
        
        //actions
        button1.Pressed += () =>
        {
            buttonOk.Visible = true;
            Console.WriteLine("event works.");
            button1.Visible = false;
            button2.Visible = false;
            description.Text = "You won the battle, but lost a citizen! +5 wood, -1 npc";
            
        };
        button2.Pressed += () =>
        {
            buttonOk.Visible = true;
            Console.WriteLine("event 2 works.");
            button1.Visible = false;
            button2.Visible = false;
            description.Text = "They didn't listen and killed 5 of your people";
        };

        buttonOk.Pressed += () =>
        {
            eventCard.Visible = false;
            eventCard.EmitSignal(EventCard.SignalName.OnEventDone, eventCard);
        };
    }
    
}