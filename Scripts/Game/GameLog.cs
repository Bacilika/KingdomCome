using Godot;
using System;
using System.Collections.Generic;

public partial class GameLog : ScrollContainer
{
	public List<Log> Logs = [];
	private PackedScene _logScene;
	private VBoxContainer _container;
	public override void _Ready()
	{
		_logScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/Log.tscn");
		_container = GetNode<VBoxContainer>("VBoxContainer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CreateLog(string text)
	{
		var label = _logScene.Instantiate<Log>();
		label.Text = text;
		_container.AddChild(label);
		_container.MoveChild(label,0);
		Logs.Add(label);
	}

	public void RemoveLog(Log log)
	{
		Logs.Remove(log);
		_container.RemoveChild(log);
		log.QueueFree();
	}
}
