using Godot;

namespace ZoopAP.Scripts;

public class Piece
{
    public PieceType Type;
    public TextureRect Sprite;

    public Piece(PieceType inputType)
    {
        Type = inputType;
    }
}