using Godot;
using System;
using System.Collections.Generic;

public partial class Forest : Area2D
{
	private PackedScene _treeScene;
	private CollisionShape2D area;
	private RandomNumberGenerator rnd;
	private int radius;
	public List<Tree> trees;
	
	public override void _Ready()
	{
		area = GetNode<CollisionShape2D>("CollisionShape2D");
		radius =(int) area.Shape.GetRect().Size.X/2;
		rnd = new RandomNumberGenerator();
		trees = [];
		
		_treeScene = ResourceLoader.Load<PackedScene>("res://Scenes/Other/Tree.tscn");
		PlaceTrees(50);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlaceTrees(int amount)
	{
		for (int i = 0; i < amount; i++)
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
					if (distance.Length() < treeRadius * 2)
					{
						cantPlace = true;
					}
				}

			} while (cantPlace);
			
			var treeSprite = tree.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			treeSprite.Frame = rnd.RandiRange(0, treeSprite.SpriteFrames.GetFrameCount("default")-1);
			var height = treeSprite.SpriteFrames.GetFrameTexture("default", treeSprite.Frame).GetHeight();
			treeSprite.Position = new Vector2(0, -height/2);
			trees.Add(tree);

		}
	}
}
