using Godot;
using System;

public partial class Log : RichTextLabel
{
	private double _deltaTime;
	private GameLog _gameLog;
	private double _lifetime = 0;
	public override void _Ready()
	{
		_gameLog = GetParent().GetParent<GameLog>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_lifetime += delta;
		
		if (_lifetime > 5)
		{
			_deltaTime += delta;
			if (_deltaTime >= 0.05)
			{
				_deltaTime -= 0.1;
				var mod = SelfModulate;
				mod.A8 -= 10;
				if (mod.A8 <= 0)
				{
					_gameLog.RemoveLog(this);
				}
				SelfModulate = mod;
			}
		}
		
		
		
		
	}
}
