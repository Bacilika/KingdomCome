using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Scripts.Constants;

public class EventGenerator
{
    public static List<EventCard> eventCards = [];
    private PackedScene _eventScene;
    private PackedScene _npcScene;
    private RandomNumberGenerator _rand = new(); 
    public EventGenerator()
    {
        //get eventcard 
        _eventScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/EventCard.tscn");
        
        //-----Create events-----
        var event1 = CreateEvent(2);
        var event2 = CreateEvent(2);
        var event3 = CreateEvent(2);
        
        event1.Ready += () => { createEvent1(event1); };
        event2.Ready += () => { createEvent2(event2); };
        event3.Ready += () => { createEvent3(event3); };
        eventCards.Add(event2);       
        eventCards.Add(event3);
        eventCards.Add(event1);
    }
    private void createEvent3(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "To help or not to help";
        event2.Description.Text =
            $"As the settlers were finishing for the day, they saw a figure moving towards them. " +
            $"It was a young boy, with hollow looking eyes and sunken cheeks. \"I got kicked out from" +
            $"my parent's place, and I need a place to stay. But I'm a good worker! Please let me stay?\"";
        event2.buttons[0].Text = "Yes, you can stay";
        event2.buttons[1].Text = "No, we don't have room...";
        
        event2.DoneButton.Pressed += () =>
        {
            event2.Gamemap.PlayGame();
            eventCards.Remove(event2);
            event2.GetParent().RemoveChild(event2);
            event2.QueueFree();
        };
        //actions
        event2.buttons[0].Pressed += () =>
        {
            _npcScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
            var npcInst = _npcScene.Instantiate<Npc>();
            event2.GetTree().Root.AddChild(npcInst);
            event2.Gamemap.SpawnFirstNpc(npcInst);
            event2.Description.Text = $"The boy smiled and happily joined the others, getting ready to help build" +
                                      $"your city. ({npcInst.CitizenName} became one of your citizens)" ;
        };
        event2.buttons[1].Pressed += () =>
        {
            event2.Description.Text = "The boy looked deflated as he listened to the refusal, and gave one final sad" +
                                      "look at your citizens before leaving, never to be seen again.";
        };
    }


    private void createEvent2(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "A little luck...";
        event2.Description.Text = $"When walking in the woods, {luckyNpc} met a traveling salesman that stood by a broken cart. " +
                                  $"He offered {luckyNpc} 5 meat in exchange for 3 wood so that he could fix his cart! " +
                                  $"Do you want to trade?";
        event2.buttons[0].Text = "Yes!";
        event2.buttons[1].Text = "No thank you";
        if (GameLogistics.Resources[RawResource.Wood] < 3)
        {
            event2.buttons[0].Disabled = true;
            event2.buttons[0].TooltipText = "Not enough wood";
        }
        
        event2.DoneButton.Pressed += () =>
        {
            event2.Gamemap.PlayGame();
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

    private void createEvent1(EventCard event1)
    {
        event1.Gamemap.PauseGame();
        event1.Title.Text = "War!";
        event1.Description.Text = "Another nation has declared war!";
        event1.buttons[0].Text = "Attack";
        event1.buttons[1].Text = "Ask for peace";
        
        event1.DoneButton.Pressed += () =>
        {
            event1.Gamemap.PlayGame();
            eventCards.Remove(event1);
            event1.GetParent().RemoveChild(event1);
            event1.QueueFree();
        };
        
        event1.buttons[0].Pressed += () =>
        {
            _eventScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/EventCard.tscn");
            Console.WriteLine("event works.");
            GameLogistics.Resources[RawResource.Wood] += 5;
            var npc = event1.Gamemap.Citizens.Last();
            event1.Description.Text = $"You won the battle, but lost {npc.CitizenName}! +5 wood " ;
            npc.OnDelete();
        };
        
        event1.buttons[1].Pressed += () =>
        {
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