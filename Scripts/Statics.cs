using System.Collections.Generic;
using Godot;

namespace ZoopAP.Scripts;

public partial class Statics : Node2D
{
	public bool Paused;
	public int Score;
	public int PiecesZapped;
	public List<HighScore> HighScores = new List<HighScore>();
}
