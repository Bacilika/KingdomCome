using System.Collections.Generic;
using Godot;
using KingdomCome.Scripts.MapResources;

public partial class Forest : Area2D
{
	private PackedScene _treeScene;
	private CollisionShape2D area;
	private int radius;
	private RandomNumberGenerator rnd;
	public List<AbstractResource> trees;
	private RandomNumberGenerator _rnd;
	private List<AbstractResource> _assignedTrees = [];
	private List<AbstractResource> consumedTrees = [];
	private double time;
	public override void _Ready()
	{
		_rnd = new RandomNumberGenerator();
		area = GetNode<CollisionShape2D>("CollisionShape2D");
		radius = (int)area.Shape.GetRect().Size.X / 2;
		rnd = new RandomNumberGenerator();
		trees = [];

		_treeScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Tree.tscn");
		PlaceTrees(50);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += delta;
		if (time >= 10) //every 10 seconds
		{
			time -= 10;
			for (int i = consumedTrees.Count-1; i >= 0; i--)
			{
				var tree = consumedTrees[i];
				RemoveChild(tree);
				consumedTrees.Remove(tree);
				tree.QueueFree();
			}
		}
		foreach (var tree in _assignedTrees)
		{
			foreach (var npc in tree.AssignedNpcs)
			{
				if (npc.Position.DistanceTo(tree.GlobalPosition) < 10)
				{
					npc._move = false;
					if (npc.AtWorkTimer.IsStopped())
					{
						npc.AtWorkTimer.Start();
					}
				}
			}
		}
	}

	public void PlaceTrees(int amount)
	{
		for (var i = 0; i < amount; i++)
		{
			var tree = _treeScene.Instantiate<Tree>();
			AddChild(tree);
			var treeRadius = (tree.GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size / 2)[0];
			bool cantPlace;
			do
			{
				cantPlace = false;
				var x = rnd.RandiRange(-radius, radius);
				var y = rnd.RandiRange(-radius, radius);
				tree.Position = new Vector2(x, y);
				foreach (var placedTree in trees)
				{
					var distance = tree.Position - placedTree.Position;
					if (distance.Length() < treeRadius * 2) cantPlace = true;
				}
			} while (cantPlace);

			var treeSprite = tree.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			treeSprite.Frame = rnd.RandiRange(0, treeSprite.SpriteFrames.GetFrameCount("default") - 1);
			var height = treeSprite.SpriteFrames.GetFrameTexture("default", treeSprite.Frame).GetHeight();
			treeSprite.Position = new Vector2(0, -height / 2);
			trees.Add(tree);
		}
	}

	public AbstractResource GetClosest(Npc npc)
	{
		
		trees.Sort((x, y) => x.GlobalPosition.DistanceTo(npc.Position).CompareTo(y.GlobalPosition.DistanceTo(npc.Position)));
		var tree = trees[_rnd.RandiRange(0, 10)];
		_assignedTrees.Add(tree);
		tree.AssignedNpcs.Add(npc);
		return tree;
	}

	public AbstractResource GetClosestTo(Vector2 position)
	{
		AbstractResource closest = trees[0];
		foreach (var tree in trees)
		{
			if (tree.GlobalPosition.DistanceTo(position) < closest.GlobalPosition.DistanceTo(position))
			{
				closest = tree;
			}
		}
		return closest;
	}

	public void RemoveResource(AbstractResource abstractResource)
	{
		if (abstractResource.ResourcesLeft > 0)
		{
			abstractResource.ResourcesLeft--;
			if (abstractResource.ResourcesLeft == 0)
			{
				foreach (var npc in abstractResource.AssignedNpcs)
				{
					npc.SetDestination(npc.Work.Position);
				}

				trees.Remove(abstractResource);
				abstractResource.AssignedNpcs.Clear();
				_assignedTrees.Remove(abstractResource);
				abstractResource.Visible = false;
				consumedTrees.Add(abstractResource);
			}
		}
	}
}
