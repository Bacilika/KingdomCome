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
        
        
        //-----Event 2-----
        var event2 = CreateEvent(2);

        event2.Ready += () => { createEvent2(event2); };
        eventCards.Add(event2);
        eventCards.Add(event1);
        
    }



    private void createEvent2(EventCard event2)
    {
        
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "A little luck...";
        event2.Description.Text = $"When walking in the woods, {luckyNpc} met a traveling salesman that stood by a broken cart. " +
                                  $"He offered {luckyNpc} 5 meat in exchange for 3 wood so that he could fix his cart! " +
                                  $"Do you want to trade?";
        event2.buttons[0].Text = "Yes!";
        event2.buttons[1].Text = "No thank you";
        
        event2.DoneButton.Pressed += () =>
        {
            eventCards.Remove(event2);
            event2.GetParent().RemoveChild(event2);
            event2.QueueFree();
        };
        //actions
        event2.buttons[0].Pressed += () =>
        {
            GameLogistics.FoodResource[Food.Meat] += 5;
            GameLogistics.Resources[RawResource.Wood] -= 3;
            event2.Description.Text = $"The salesman smiled as he happily trades the meat for the wood, and thanks {luckyNpc} profusely! He promises to come" +
                                      "back and trade more in the future. (+5 meat, -3 wood)" ;
        };
        event2.buttons[1].Pressed += () =>
        {
            event2.Description.Text = $"The salesman looked sad as {luckyNpc} politely said no and continued walking forward, not looking back." ;
        };
    }

    public EventCard CreateEvent(int choices)
    {
        var newEvent = _eventScene.Instantiate<EventCard>();
        newEvent.SetFields();
        newEvent.AddButtons(choices);
        return newEvent;
    }

    public EventCard getEvent()
    {
        return eventCards[0];
    }
}