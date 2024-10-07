using KingdomCome.Scripts.MapResources;

public partial class Tree : AbstractResource
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResourcesLeft = 20;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ResourcesLeft == 0)
		{
		}
	}
}
