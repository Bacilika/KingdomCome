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
    private PackedScene _npcScene1;
    private PackedScene _npcScene2;
    private RandomNumberGenerator _rand = new(); 
    
    //Event variables
    private bool _helpedSalesman = false;
    private bool _helpedBoy = false;
    private Npc _boyNpc;
    private Npc _sibling1Npc;
    private Npc _sibling2Npc;
    
    public EventGenerator()
    {
        //get eventcard 
        _eventScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/EventCard.tscn");
        
        //-----Create events-----
        var event0 = CreateEvent(1);
        var event1 = CreateEvent(2);
        var event2 = CreateEvent(2);
        var event3 = CreateEvent(2);
        var event4 = CreateEvent(3);
        var event5 = CreateEvent(2);
        
        event0.Ready += () => { createEvent0(event0); };
        event1.Ready += () => { createEvent1(event1); };
        event2.Ready += () => { createEvent2(event2); };
        event3.Ready += () => { createEvent3(event3); };
        event4.Ready += () =>
        {
            if (_helpedBoy)
            {
                createEvent4Boy(event4);
            }
            else
            {
                createEvent4NoBoy(event4);
    
            }
        };
        event5.Ready += () =>
        {
            if (_helpedBoy)
            {
                createEvent5Boy(event5);
            }
            else
            {
                createEvent5NoBoy(event5);
            }

        };
        eventCards.Add(event0);       
        eventCards.Add(event3);       
        eventCards.Add(event2);
        eventCards.Add(event4);
        eventCards.Add(event5);
        eventCards.Add(event1);
    }
    
    private void createEvent4Boy(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "A thief among us";
        event2.Description.Text =
            $"For the past few days, some of your wood and stone had mysteriously gone missing. One night, {luckyNpc} " +
            $"sees {_boyNpc.CitizenName} grabbing some of your food and walking towards the forest. {luckyNpc} follows him and watches as he" +
            $"hands over the food to an unknown man. As {_boyNpc.CitizenName} starts walking back towards your village, {luckyNpc} confronts him. " +
            $"\n\"What are you doing?\" The boy looks scared and starts crying, but after some time you get an explaination. Apparently his father" +
            $"demands that he gives food to them, otherwise he will leave his siblings to starve, and that is why he has been stealing. ";
        event2.buttons[0].Text = "Stealing is never okay. Please leave.";
        event2.buttons[1].Text = "You can stay, but don't speak to your family again.";
        event2.buttons[2].Text = "Bring your siblings here!";
        
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
            GameLogistics.Resources[RawResource.Wood] += 5;
            GameLogistics.ProcessedResources[ProcessedResource.Plank] += 5;
            event2.Description.Text = $"{_boyNpc.CitizenName} looked horrified as {luckyNpc} gathered his belongings and forced him out the door. " +
                                      $"This boy was, however, not of your concern anymore.";
        };
        event2.buttons[1].Pressed += () =>
        {
            foreach (var npc in event2.Gamemap.Citizens)
            {
                npc.SetMoodReason("Event", "Feeling scammed...", -2, 120);
            }
            event2.Description.Text = $"{_boyNpc.CitizenName} looked unhappy, but promised never to see his family or to steal again. " +
                                      $"Although you wanted to believe him, it was hard to picture him so easily leaving his siblings" +
                                      $"without a second thought. (-2 mood, feeling scammed)";
        };
        
        event2.buttons[2].Pressed += () =>
        {
            _npcScene1 = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
            _sibling1Npc = _npcScene1.Instantiate<Npc>();
            event2.GetTree().Root.AddChild(_sibling1Npc);
            event2.Gamemap.SpawnFirstNpc(_sibling1Npc);
            _npcScene2 = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
            _sibling2Npc = _npcScene2.Instantiate<Npc>();
            event2.GetTree().Root.AddChild(_sibling2Npc);
            event2.Gamemap.SpawnFirstNpc(_sibling2Npc);
            event2.Description.Text =
                $"{_boyNpc.CitizenName} looked startled, but soon started smiling in joy. \"Thank you!\" he almost screamed. " +
                $"\n After a few days of preparation, {_boyNpc.CitizenName}'s two siblings, {_sibling1Npc.CitizenName} and" +
                $"{_sibling2Npc.CitizenName} were quietly brought from their" +
                $"parents' house, without the parents noticing, to your village. Although {luckyNpc} felt certain they had made the right desition, " +
                $"there was an uncertainty of the parents reaction if they ever were to find out... (+2 citizens).";
        };
    }
    
    
        private void createEvent4NoBoy(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "Something in the ditch";
        event2.Description.Text =$"As {luckyNpc} was walking along the outskirts of the village, they saw something curled up in a blanket in" +
                                 $"the ditch. After looking more closely, it looked like a human body curled up in those blankets, with what looked " +
                                 $"like a plank next to it.";
        event2.buttons[0].Text = "Try to shake the human awake";
        event2.buttons[1].Text = "Take the plank and leave.";
        event2.buttons[2].Text = "Leave without doing anything";
        
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
            foreach (var npc in event2.Gamemap.Citizens)
            {
                npc.SetMoodReason("Event", "Might have done a bad choice...", -2, 120);
            }
            event2.Description.Text = $"{luckyNpc} tried shake the body awake, but soon discovered that it was not moving. " +
                                      $"When unwrapping the blankets, {luckyNpc} saw that it was the boy from earlier, dead." +
                                      $"{luckyNpc} left the area, wondering if turning him away had really been the right choice." +
                                      $"(-2 mood)" ;
        };
        event2.buttons[1].Pressed += () =>
        {
            GameLogistics.ProcessedResources[ProcessedResource.Plank] += 1;
            event2.Description.Text = $"{luckyNpc} took the plank while carefully avoiding the blankets, and quickly left. " +
                                      $"Whatever was under there, it was none of their business. ";
        };
        
        event2.buttons[1].Pressed += () =>
        {
            event2.Description.Text = $"{luckyNpc} quickly left the area without looking back. Whatever was under that blanket, it was none of their business." ;
        };
    }
    
        
    private void createEvent5NoBoy(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "Second time's the charm";
        event2.Description.Text =
            $"The villagers had been noticing that food had been mysteriously dissapearing the past few days. One night, {luckyNpc}" +
            $"saw a small girl grabbing some of your food and running away. {luckyNpc} run after her, and quickly caught her." +
            $"\n\"Why do you steal or food?\" {luckyNpc} asked." +
            $"\n\"I'm sorry!\" she squealed. \"My family is forcing me to. If I don't, they will kick me out, just like they did to my brother!";
        event2.buttons[0].Text = "Not our problem. Leave, and don't come back!";
        event2.buttons[1].Text = "You can stay here.";
        
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
            GameLogistics.Resources[RawResource.Wood] += 5;
            GameLogistics.ProcessedResources[ProcessedResource.Plank] += 5;
            event2.Description.Text = $"Just as {_boyNpc.CitizenName} has said, you found a small storage of both wood and " +
                                      $"planks not far from the city! (+5 Planks, +5 Wood)" ;
        };
        event2.buttons[1].Pressed += () =>
        {
            event2.Description.Text = $"The boy looked sad as he accepted your refusal, but said nothing more.";
        };
    }

        
    
    private void createEvent5Boy(EventCard event2)
    {
        event2.Gamemap.PauseGame();
        var luckyNpc = event2.Gamemap.Citizens.Last().CitizenName;
        event2.Title.Text = "A proposition";
        event2.Description.Text =
            $"{_boyNpc.CitizenName} has settled in nicely with the village. He is working just as hard " +
            $"as anyone else, and is kind and gentle. One night, he pulls {luckyNpc} aside and tells him: " +
            $"I didn't want to tell you this when I just got here, but now I feel like part of your family. " +
            $"I know of a good place that keeps planks and wood not long from here. Do you want me to show you?";
        event2.buttons[0].Text = "Yes, place";
        event2.buttons[1].Text = "No, I still don't know if I can trust you.";
        
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
            GameLogistics.Resources[RawResource.Wood] += 5;
            GameLogistics.ProcessedResources[ProcessedResource.Plank] += 5;
            event2.Description.Text = $"Just as {_boyNpc.CitizenName} has said, you found a small storage of both wood and " +
                                      $"planks not far from the city! (+5 Planks, +5 Wood)" ;
        };
        event2.buttons[1].Pressed += () =>
        {
            event2.Description.Text = $"The boy looked sad as he accepted your refusal, but said nothing more.";
        };
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
            _helpedBoy = true;
            _npcScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
            _boyNpc = _npcScene.Instantiate<Npc>();
            event2.GetTree().Root.AddChild(_boyNpc);
            event2.Gamemap.SpawnFirstNpc(_boyNpc);
            event2.Description.Text = $"The boy smiled and happily joined the others, getting ready to help build" +
                                      $"your city. ({_boyNpc.CitizenName} became one of your citizens)" ;
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
        event2.Title.Text = "A little bit of kindness for a stranger?";
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
            _helpedSalesman = true;
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
    
    private void createEvent0(EventCard event0)
    {
        event0.Gamemap.PauseGame();
        var firstNpc = event0.Gamemap.Citizens.Last().CitizenName;
        var lastNpc = event0.Gamemap.Citizens.First().CitizenName;
        event0.Title.Text = "Growing a family";
        event0.Description.Text =$"As {firstNpc} and {lastNpc} finished up the work for the day, {lastNpc} suddenly" +
                                 $"got a terrible stomach ache. After some panic, they realised she's about to give birth!";
        event0.buttons[0].Text = "Oh my!";
        
        event0.DoneButton.Pressed += () =>
        {
            event0.Gamemap.PlayGame();
            eventCards.Remove(event0);
            event0.GetParent().RemoveChild(event0);
            event0.QueueFree();
        };
        //actions
        event0.buttons[0].Pressed += () =>
        {
            _npcScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/NPC.tscn");
            _boyNpc = _npcScene.Instantiate<Npc>();
            event0.GetTree().Root.AddChild(_boyNpc);
            event0.Gamemap.SpawnFirstNpc(_boyNpc);
            event0.Gamemap.ChildIsBorn = true;
            event0.Description.Text = $"The birth went well! {firstNpc} and {lastNpc} are ready to welcome {_boyNpc.CitizenName}";
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