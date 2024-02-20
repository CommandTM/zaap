using Godot;
using System;
using ZoopAP.Scripts;

public partial class player : Sprite2D
{
	[Export] private Vector2 _gridOffset;
	[Export] private int _gridDistance;
	[ExportGroup("Elements")] 
	[Export] private Timer _inputDelay;
	[ExportSubgroup("Textures")] 
	[Export] private Texture2D _redPlayer;
	[Export] private Texture2D _yellowPlayer;
	[Export] private Texture2D _bluePlayer;
	[Export] private Texture2D _orangePlayer;
	[Export] private Texture2D _pinkPlayer;
	[Export] private Texture2D _greenPlayer;
	public bool[,] Grid = new bool[4, 4];
	public PieceType Color;
	public bool Dashing;
	private Statics _staticVars;
	
	public override void _Ready()
	{
		Grid[0, 0] = true;
		Color = PieceType.Orange;
		
		_staticVars = GetNode<Statics>("/root/Statics");
	}
	
	public override void _Process(double delta)
	{
		if (!_staticVars.Paused)
		{ 
			Tween movement;
			if (!Dashing)
			{
				if (Input.IsActionJustPressed("Cheat Player Color Red"))
				{
					Color = PieceType.Red;
				}

				if (Input.IsActionJustPressed("Cheat Player Color Yellow"))
				{
					Color = PieceType.Yellow;
				}

				if (Input.IsActionJustPressed("Cheat Player Color Blue"))
				{
					Color = PieceType.Blue;
				}

				if (Input.IsActionJustPressed("Cheat Player Color Orange"))
				{
					Color = PieceType.Orange;
				}

				if (Input.IsActionJustPressed("Cheat Player Color Pink"))
				{
					Color = PieceType.Pink;
				}

				if (Input.IsActionJustPressed("Cheat Player Color Green"))
				{
					Color = PieceType.Green;
				}
			
				if (Input.IsActionPressed("Up") && _inputDelay.TimeLeft == 0)
				{
					RotationDegrees = 0;
					if (_movePlayerOnGrid(0, 1))
					{
						//Position = GetPlayerPosition();
						movement = GetTree().CreateTween();
						movement.TweenProperty(this, "position", 
							GetTruePlayerPosition(), _inputDelay.WaitTime);
						_inputDelay.Start();
					}
				}
				if (Input.IsActionPressed("Down") && _inputDelay.TimeLeft == 0)
				{
					RotationDegrees = -180;
					if (_movePlayerOnGrid(0, -1)) 
					{
						//Position = GetPlayerPosition();
						movement = GetTree().CreateTween();
						movement.TweenProperty(this, "position", 
							GetTruePlayerPosition(), _inputDelay.WaitTime);
						_inputDelay.Start();
					}
				}

				if (Input.IsActionPressed("Right") && _inputDelay.TimeLeft == 0)
				{
					RotationDegrees = 90;
					if (_movePlayerOnGrid(1, 0))
					{
						//Position = GetPlayerPosition();
						movement = GetTree().CreateTween();
						movement.TweenProperty(this, "position", 
							GetTruePlayerPosition(), _inputDelay.WaitTime);
						_inputDelay.Start();
					}
				}
				if (Input.IsActionPressed("Left") && _inputDelay.TimeLeft == 0)
				{
					RotationDegrees = -90;
					if (_movePlayerOnGrid(-1, 0))
					{
						//Position = GetPlayerPosition();
						movement = GetTree().CreateTween();
						movement.TweenProperty(this, "position", 
							GetTruePlayerPosition(), _inputDelay.WaitTime);
						_inputDelay.Start();
					}
				}
			}
			
			if (Input.IsActionJustPressed("Dash") && !Dashing)
			{
				Dashing = true;
				Vector2 target = new Vector2();
				double endRotation = 0;

				if (90 - Math.Abs(RotationDegrees) != 0)
				{
					if (RotationDegrees >= 0)
					{
						target.Y = 16;
						endRotation = -180;
					}
					else
					{
						target.Y = 584;
						endRotation = 0;
					}

					target.X = Position.X;
				}
				else
				{
					if (RotationDegrees > 0)
					{
						target.X = 784;
						endRotation = -90;
					}
					else
					{
						target.X = 16;
						endRotation = 90;
					}

					target.Y = Position.Y;
				}

				movement = GetTree().CreateTween();
				movement.TweenProperty(this, "position", target, _inputDelay.WaitTime*1.4);
				movement.TweenProperty(this, "rotation_degrees", endRotation, 0);
				movement.TweenProperty(this, "position", GetTruePlayerPosition(), _inputDelay.WaitTime*1.4);
				movement.TweenProperty(this, "Dashing", false, 0);
			}
			
			if (!Input.IsAnythingPressed())
			{
				_inputDelay.Stop();
			}
		}

		switch (Color)
		{
			case PieceType.Red:
				Texture = _redPlayer;
				break;
			case PieceType.Yellow:
				Texture = _yellowPlayer;
				break;
			case PieceType.Blue:
				Texture = _bluePlayer;
				break;
			case PieceType.Orange:
				Texture = _orangePlayer;
				break;
			case PieceType.Pink:
				Texture = _pinkPlayer;
				break;
			case PieceType.Green:
				Texture = _greenPlayer;
				break;
		}
	}

	public Vector2 GetTruePlayerPosition()
	{
		Vector2 pos = new Vector2(0, 0);
		for (int i = 0; i < Grid.GetLength(0); i++)
		{
			for (int k = 0; k < Grid.GetLength(1); k++)
			{
				if (Grid[i, k])
				{
					pos.X = k;
					pos.Y = i;
				}
			}
		}

		pos.X *= _gridDistance;
		pos.Y *= _gridDistance;

		pos.X += _gridOffset.X;
		pos.Y += _gridOffset.Y;

		return pos;
	}

	public Vector2 GetGridPlayerPosition()
	{
		Vector2 pos = new Vector2(0, 0);
		for (int i = 0; i < Grid.GetLength(0); i++)
		{
			for (int k = 0; k < Grid.GetLength(1); k++)
			{
				if (Grid[i, k])
				{
					pos.X = k;
					pos.Y = i;
				}
			}
		}
		
		return pos;
	}

	public Row GetPlayerTarget()
	{
		switch (RotationDegrees)
		{
			case 0:
				switch (GetGridPlayerPosition().X)
				{
					case 0:
						return Row.TopOne;
					case 1:
						return Row.TopTwo;
					case 2:
						return Row.TopThree;
					case 3:
						return Row.TopFour;
				}
				break;
			case 90:
				switch (GetGridPlayerPosition().Y)
				{
					case 0:
						return Row.RightOne;
					case 1:
						return Row.RightTwo;
					case 2:
						return Row.RightThree;
					case 3:
						return Row.RightFour;
				}
				break;
			case -180:
				switch (GetGridPlayerPosition().X)
				{
					case 0:
						return Row.BottomOne;
					case 1:
						return Row.BottomTwo;
					case 2:
						return Row.BottomThree;
					case 3:
						return Row.BottomFour;
				}
				break;
			case -90:
				switch (GetGridPlayerPosition().Y)
				{
					case 0:
						return Row.LeftOne;
					case 1:
						return Row.LeftTwo;
					case 2:
						return Row.LeftThree;
					case 3:
						return Row.LeftFour;
				}
				break;
		}

		return Row.TopOne;
	}

	private bool _movePlayerOnGrid(int xChange, int yChange)
	{
		for (int i = 0; i < Grid.GetLength(0); i++)
		{
			for (int k = 0; k < Grid.GetLength(1); k++)
			{
				if (Grid[i, k])
				{
					if (i != 3 && yChange < 0)
					{
						Grid[i + -yChange, k] = true;
						Grid[i, k] = false;
						return true;
					}

					if (i != 0 && yChange > 0)
					{
						Grid[i + -yChange, k] = true;
						Grid[i, k] = false;
						return true;
					}

					if (k != 0 && xChange < 0)
					{
						Grid[i, k + xChange] = true;
						Grid[i, k] = false;
						return true;
					}
					
					if (k != 3 && xChange > 0)
					{
						Grid[i, k + xChange] = true;
						Grid[i, k] = false;
						return true;
					}
				}
			}
		}

		return false;
	}
}
