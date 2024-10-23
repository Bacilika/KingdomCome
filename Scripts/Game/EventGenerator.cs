using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Scripts.Constants;

public class EventGenerator
{
    public static List<EventCard> eventCards = [];
    private PackedScene _eventScene;
    private RandomNumberGenerator _rand = new(); 
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
        
        event1.DoneButton.Pressed += () =>
        {
            eventCards.Remove(event1);
            event1.GetParent().RemoveChild(event1);
            event1.QueueFree();
        };
        //actions
        event1.buttons[0].Pressed += () =>
        {
            Console.WriteLine("event works.");
            GameLogistics.Resources[RawResource.Wood] += 5;
            var npc = event1.Gamemap.Citizens.Last();
            
            event1.Description.Text = $"You won the battle, but lost {npc.CitizenName}! +5 wood " ;
            npc.OnDelete();
        };
        event1.buttons[1].Pressed += () =>
        {
            Console.WriteLine("event 2 works.");
            int amount = _rand.RandiRange(1, Math.Max(event1.Gamemap.Citizens.Count-2, 1));
            List<Npc> sublist = event1.Gamemap.Citizens.GetRange(event1.Gamemap.Citizens.Count-amount-1, event1.Gamemap.Citizens.Count-1);
            String names = ""; 
            foreach (var npc in sublist)
            {
                names += $" {npc.CitizenName},";
                npc.OnDelete();
            }
            names = names.Remove(names.Length - 1);
            event1.Description.Text = $"They didn't listen and killed {amount} of your people. Their names were {names} ";
            //event1.Gamemap.Citizens.RemoveRange(event1.Gamemap.Citizens.Count-amount, event1.Gamemap.Citizens.Count);
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

    public EventCard getEvent()
    {
        return eventCards[0];
    }
}