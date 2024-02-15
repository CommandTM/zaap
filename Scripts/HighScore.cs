using System.Xml;

namespace ZoopAP.Scripts;

public class HighScore
{
    public string Name;
    public int Score;

    public HighScore(int setScore, string setName)
    {
        Name = setName;
        Score = setScore;
    }
}