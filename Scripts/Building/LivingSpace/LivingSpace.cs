using Godot;
using KingdomCome.Scripts.Building;
using KingdomCome.Scripts.Other;
using Scripts.Constants;

public abstract partial class LivingSpace : AbstractPlaceable
{
    protected int Growth = 5; // 1/_growth% chance to increase habitants by 1 each tick. 
    [Signal]
    public delegate void OnCreateNpcEventHandler(LivingSpace house);

    public string HouseholdName = NameGenerator.GenerateLastName();

    protected virtual void DoAction() { }

    protected override void Tick()
    {
        if (Inhabitants < Upgrades[Upgrade.MaxInhabitants][Level])
        {
            if (Rnd.RandiRange(0, Growth) == 0)
            {
                Inhabitants++;
                PlayAnimation();
                EmitSignal(SignalName.OnCreateNpc, this);
            }
        }

        DoAction();
        UpdateInfo();

    }

    public void MoveIntoHouse(Npc npc)
    {
        People.Add(npc);
        var npcPortrait = InfoBox.CitizenPortrait.Instantiate<CitizenPortraitButton>();
        npcPortrait.npc = npc;
        InfoBox.PortraitContainer.AddChild(npcPortrait);
        GetNode<AnimatedSprite2D>("HouseSprite").SetAnimation("AnimationLevel" + Level);
        GetNode<AnimatedSprite2D>("HouseSprite").Play();
        npc.OnMoveIn();
        npc.CitizenName += $" {HouseholdName}";
        npc.EmitSignal(GameMap.SignalName.SendLog, $"{npc.CitizenName} moved into house!");
    }
    public void UpdateInfo()
    {
        if (People.Count == 0)
        {
            InfoBox.UpdateInfo("Empty House");
        }
        else
        {
            InfoBox.UpdateInfo($"The {HouseholdName}'s",
                $"Inhabitants: {Inhabitants} / {Upgrades[Upgrade.MaxInhabitants][Level]}");
        }

    }
}