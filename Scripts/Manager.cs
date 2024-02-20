using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using ZoopAP.Scripts;

public partial class Manager : Node2D
{
	[ExportGroup("Textures")] 
	[Export] private Texture2D _red;
	[Export] private Texture2D _yellow;
	[Export] private Texture2D _blue;
	[Export] private Texture2D _orange;
	[Export] private Texture2D _pink;
	[Export] private Texture2D _green;
	[ExportGroup("Elements")] 
	[Export] private player _player;
	[Export] private Timer _gameTime;
	[Export] private RichTextLabel _scoreDisplay;
	[Export] private RichTextLabel _comboDisplay;
	[Export] private RichTextLabel _zappedDisplay;
	[Export] private RichTextLabel _levelDisplay;
	[Export] private RichTextLabel _clearsDisplay;
	[ExportSubgroup("Rows")] 
	[Export] private VBoxContainer _topOne;
	[Export] private VBoxContainer _topTwo;
	[Export] private VBoxContainer _topThree;
	[Export] private VBoxContainer _topFour;
	[Export] private HBoxContainer _rightOne;
	[Export] private HBoxContainer _rightTwo;
	[Export] private HBoxContainer _rightThree;
	[Export] private HBoxContainer _rightFour;
	[Export] private VBoxContainer _bottomOne;
	[Export] private VBoxContainer _bottomTwo;
	[Export] private VBoxContainer _bottomThree;
	[Export] private VBoxContainer _bottomFour;
	[Export] private HBoxContainer _leftOne;
	[Export] private HBoxContainer _leftTwo;
	[Export] private HBoxContainer _leftThree;
	[Export] private HBoxContainer _leftFour;

	private List<List<Piece>> _top;
	private List<List<Piece>> _right;
	private List<List<Piece>> _bottom;
	private List<List<Piece>> _left;

	private Dictionary<Row, List<Piece>> _rowToList;
	
	private Dictionary<List<Piece>, Row> _listToRow;

	private Dictionary<List<Piece>, BoxContainer> _listToBox;

	private Dictionary<PieceType, Texture2D> _typeToTexture;

	private List<PieceType> _possibleSpawns;
	private List<PieceType> _excludedSpawns;

	private Statics _staticVars;
	private int _combo;
	
	private int _level;
	private int _clearsUntilLevelUp;
	private int _clearsThreshold;

	private bool _gameOver;

	public List<Piece> nextTarget;
	
	public override void _Ready()
	{
		_top = new List<List<Piece>>();
		_right = new List<List<Piece>>();
		_bottom = new List<List<Piece>>();
		_left = new List<List<Piece>>();

		for (int i = 0; i < 4; i++)
		{
			_top.Add(new List<Piece>());
			_right.Add(new List<Piece>());
			_bottom.Add(new List<Piece>());
			_left.Add(new List<Piece>());
		}

		_possibleSpawns = new List<PieceType>()
		{
			PieceType.Red,
			PieceType.Yellow,
			PieceType.Blue,
			PieceType.Orange,
			PieceType.Pink,
			PieceType.Green
		};

		_excludedSpawns = new List<PieceType>();

		for (int i = 0; i < 2; i++)
		{
			int index = GD.RandRange(0, _possibleSpawns.Count - 1);
			_excludedSpawns.Add(_possibleSpawns[index]);
			_possibleSpawns.RemoveAt(index);
		}

		_player.Color = _possibleSpawns[GD.RandRange(0, _possibleSpawns.Count - 1)];
		
		_rowToList= new Dictionary<Row, List<Piece>>()
		{
			{ Row.TopOne, _top[0] },
			{ Row.TopTwo, _top[1] },
			{ Row.TopThree, _top[2] },
			{ Row.TopFour, _top[3] },
			{ Row.RightOne, _right[0] },
			{ Row.RightTwo, _right[1] },
			{ Row.RightThree, _right[2] },
			{ Row.RightFour, _right[3] },
			{ Row.BottomOne, _bottom[0] },
			{ Row.BottomTwo, _bottom[1] },
			{ Row.BottomThree, _bottom[2] },
			{ Row.BottomFour, _bottom[3] },
			{ Row.LeftOne, _left[0] },
			{ Row.LeftTwo, _left[1] },
			{ Row.LeftThree, _left[2] },
			{ Row.LeftFour, _left[3] }
		};
		
		_listToRow= new Dictionary<List<Piece>, Row>()
		{
			{ _top[0], Row.TopOne },
			{ _top[1], Row.TopTwo },
			{ _top[2], Row.TopThree },
			{ _top[3], Row.TopFour },
			{ _right[0], Row.RightOne },
			{ _right[1], Row.RightTwo },
			{ _right[2], Row.RightThree },
			{ _right[3], Row.RightFour },
			{ _bottom[0], Row.BottomOne },
			{ _bottom[1], Row.BottomTwo },
			{ _bottom[2], Row.BottomThree },
			{ _bottom[3], Row.BottomFour },
			{ _left[0], Row.LeftOne },
			{ _left[1], Row.LeftTwo },
			{ _left[2], Row.LeftThree },
			{ _left[3], Row.LeftFour }
		};

		_listToBox = new Dictionary<List<Piece>, BoxContainer>()
		{
			{_top[0], _topOne},
			{_top[1], _topTwo},
			{_top[2], _topThree},
			{_top[3], _topFour},
			{_right[0], _rightOne},
			{_right[1], _rightTwo},
			{_right[2], _rightThree},
			{_right[3], _rightFour},
			{_bottom[0], _bottomOne},
			{_bottom[1], _bottomTwo},
			{_bottom[2], _bottomThree},
			{_bottom[3], _bottomFour},
			{_left[0], _leftOne},
			{_left[1], _leftTwo},
			{_left[2], _leftThree},
			{_left[3], _leftFour}
		};

		_typeToTexture = new Dictionary<PieceType, Texture2D>()
		{
			{ PieceType.Red, _red },
			{ PieceType.Yellow, _yellow },
			{ PieceType.Blue, _blue },
			{ PieceType.Orange, _orange },
			{ PieceType.Pink, _pink },
			{ PieceType.Green, _green }
		};

		nextTarget = _rowToList[(Row)GD.RandRange(0, 15)];

		_staticVars = GetNode<Statics>("/root/Statics");
		_staticVars.Score = 0;
		_combo = 0;

		_level = 1;
		_clearsThreshold = 60;
		_clearsUntilLevelUp = 60;
	}
	
	public override void _Process(double delta)
	{
		if (_clearsUntilLevelUp <= 0)
		{
			_levelUp();
		}
		
		_scoreDisplay.Text = "Score: " + _staticVars.Score;
		_comboDisplay.Text = "Combo: " + _combo;
		_zappedDisplay.Text = "Zapped: " + _staticVars.PiecesZapped;
		_clearsDisplay.Text = _clearsUntilLevelUp.ToString();
		_levelDisplay.Text = _level.ToString();
		
		if (Input.IsActionJustPressed("Manual Tick"))
		{
			GD.Print("Tick!");
			SpawnPiece(nextTarget);
			nextTarget = _rowToList[(Row)GD.RandRange(0, 15)];
		}

		if (Input.IsActionJustPressed("Pause Ticks"))
		{
			_gameTime.Paused = !_gameTime.Paused;
		}

		if (Input.IsActionJustPressed("Pause") && !_gameOver)
		{
			_staticVars.Paused = !_staticVars.Paused;
			GD.Print("Pause!");
		}
		
		if (Input.IsActionJustPressed("Dash") && !_player.Dashing)
		{
			List<Piece> target = _rowToList[_player.GetPlayerTarget()];

			int queuedZaps = 0;
			for (int i = 0; i < target.Count; i++)
			{
				if (_player.Color == target[i].Type)
				{
					queuedZaps++;
				}
				else
				{
					(_player.Color, target[i].Type) = (target[i].Type, _player.Color);
					target[i].Sprite.Texture = _typeToTexture[target[i].Type];
					_staticVars.Score += (_combo * 10);
					_combo = 0;
					break;
				}
			}

			for (int i = 0; i < queuedZaps; i++)
			{
				ZapPiece(target);
				_combo++;
			}
		}
	}

	public void ZapPiece(List<Piece> row)
	{
		PieceType type = row[0].Type;
		
		switch (type)
		{
			default:
				row[0].Sprite.QueueFree();
				row.RemoveAt(0);
				_staticVars.Score += 10;
				_staticVars.PiecesZapped++;
				_clearsUntilLevelUp--;
				break;
		}
		
	}

	public void SpawnPiece(List<Piece> target)
	{
		if ((target.Count == 5 && (int)_listToRow[target] < 8) || 
			(target.Count == 7 && (int)_listToRow[target] > 7))
		{
			_gameOver = true;
			_staticVars.Paused = true;
			GD.Print("You Lose!");
		}
		
		bool rigSpawn = false;
		PieceType rigCheck = PieceType.Red;
		if (target.Count >= 3)
		{
			rigSpawn = true;
			rigCheck = target[0].Type;
			foreach (Piece piece in target)
			{
				if (piece.Type != rigCheck)
				{
					rigSpawn = false;
				}
			}
		}

		Piece toSpawn;
		if (rigSpawn)
		{
			toSpawn = new Piece(rigCheck);
		}
		else
		{
			toSpawn = new Piece(_possibleSpawns[GD.RandRange(0, _possibleSpawns.Count - 1)]);
		}

		TextureRect sprite = new TextureRect();

		sprite.Texture = _typeToTexture[toSpawn.Type];
		sprite.Name = "Piece";

		toSpawn.Sprite = sprite;
		
		if (_listToBox[target].RotationDegrees > 0)
		{
			sprite.FlipH = true;
			sprite.FlipV = true;
		}
		
		target.Add(toSpawn);
		_listToBox[target].AddChild(sprite);
	}

	private void _levelUp()
	{
		if (_level == 9)
		{
			return;
		}

		_level++;
		_clearsThreshold += 20;
		_clearsUntilLevelUp = _clearsThreshold;
		
		if (_level == 4 || _level == 7)
		{
			int index = GD.RandRange(0, _excludedSpawns.Count - 1);
			_possibleSpawns.Add(_excludedSpawns[index]);
			_excludedSpawns.RemoveAt(index);
		}

		_gameTime.WaitTime = 2 - (0.21875 * _level);
	}
	
	private void _onTick()
	{
		if (!_staticVars.Paused)
		{
			GD.Print("Tick!");
			SpawnPiece(nextTarget);
			nextTarget = _rowToList[(Row)GD.RandRange(0, 15)];
		}
	}
}
