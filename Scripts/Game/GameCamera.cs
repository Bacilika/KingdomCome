using System.Collections.Generic;
using Godot;
using Scripts.Constants;

public partial class GameCamera : Camera2D
{
	private const int ZoomMaxStep = 4;
	private const int ZoomMinStep = 1;
	private readonly HashSet<string> _directionsPressed = [];
	private Vector2 _dragStartCameraPosition = Vector2.Zero;
	private Vector2 _dragStartMousePosition = Vector2.Zero;
	private bool _isDragging;
	private Vector2 _position;
	private Vector2 _zoom;
	private float _zoomStep = 3;

	private int PanStep => (int)(5 * (_zoomStep == 0 ? 0.5f : _zoomStep));

	private void MoveCamera(string direction)
	{
		switch (direction)
		{
			case "Up":
			{
				_position.Y -= PanStep;
				break;
			}
			case "Down":
			{
				_position.Y += PanStep;
				break;
			}
			case "Left":
			{
				_position.X -= PanStep;
				break;
			}
			case "Right":
			{
				_position.X += PanStep;
				break;
			}
		}
	}


	public override void _Ready()
	{
		// Set minimum size for window rescaling.
		GetTree().Root.MinSize =
			new Vector2I(ProjectSettings.GetSetting(Constants.ViewPortWidthSettingPath, 0).AsInt32(),
				ProjectSettings.GetSetting(Constants.ViewPortHeightSettingPath, 0).AsInt32());

		_zoom = Zoom;
		_position = Position;
	}

	public override void _Process(double delta)
	{
		foreach (var dir in _directionsPressed) MoveCamera(dir);
		var deltaFloat = (float)delta;
		Zoom = Zoom.Lerp(_zoom, 20 * deltaFloat);
		Position = Position.Lerp(_position, 20 * deltaFloat);
	}

	public override void _Input(InputEvent @event)
	{
		// Zooming behaviour
		if (@event.IsActionPressed(Inputs.ZoomIn) && _zoomStep > ZoomMinStep)
		{
			_zoomStep--;
			_zoom *= 2;
		}

		if (@event.IsActionPressed(Inputs.ZoomOut) && _zoomStep < ZoomMaxStep)
		{
			_zoomStep++;
			_zoom /= 2;
		}

		// Panning behaviour
		if (@event.IsActionPressed(Inputs.CameraLeft)) _directionsPressed.Add("Left");
		if (@event.IsActionPressed(Inputs.CameraRight)) _directionsPressed.Add("Right");
		if (@event.IsActionPressed(Inputs.CameraUp)) _directionsPressed.Add("Up");
		if (@event.IsActionPressed(Inputs.CameraDown)) _directionsPressed.Add("Down");

		if (@event.IsActionReleased(Inputs.CameraDown)) _directionsPressed.Remove("Down");
		if (@event.IsActionReleased(Inputs.CameraUp)) _directionsPressed.Remove("Up");
		if (@event.IsActionReleased(Inputs.CameraLeft)) _directionsPressed.Remove("Left");
		if (@event.IsActionReleased(Inputs.CameraRight)) _directionsPressed.Remove("Right");


		if (!_isDragging && @event.IsActionPressed(Inputs.CameraPan))
		{
			_dragStartMousePosition = GetViewport().GetMousePosition();
			_dragStartCameraPosition = Position;
			_isDragging = true;
		}

		if (_isDragging && @event.IsActionReleased(Inputs.CameraPan)) _isDragging = false;

		if (_isDragging)
		{
			var moveVector = GetViewport().GetMousePosition() - _dragStartMousePosition;
			_position = _dragStartCameraPosition - moveVector * (1 / Zoom.X);
			_position = _position.Floor();
		}
	}
}
