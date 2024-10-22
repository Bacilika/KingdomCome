using System;
using System.Collections.Generic;
using Godot;

namespace Scripts.Constants;

public class EventGenerator
{
    public static List<EventCard> eventCards = [];
    private PackedScene _eventScene;
    public EventGenerator()
    {
        //get eventcard 
        _eventScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/EventCard.tscn");
        
        //-----Event 1-----
        var event1 = CreateEvent(2);
        
        event1.Title.Text = "War!";
        event1.Description.Text = "Another nation has declared war!";
        event1.buttons[0].Text = "Attack";
        event1.buttons[1].Text = "Ask for peace";
        
        //actions
        event1.buttons[0].Pressed += () =>
        {
            Console.WriteLine("event works.");
            event1.Description.Text = "You won the battle, but lost a citizen! +5 wood, -1 npc";
        };
        event1.buttons[1].Pressed += () =>
        {
            Console.WriteLine("event 2 works.");
            event1.Description.Text = "They didn't listen and killed 5 of your people";
        };
        eventCards.Add(event1);
    }

    public EventCard CreateEvent(int choices)
    {
        var event1 = _eventScene.Instantiate<EventCard>();
        event1.SetFields();
        event1.AddButtons(choices);
        return event1;
    }
}