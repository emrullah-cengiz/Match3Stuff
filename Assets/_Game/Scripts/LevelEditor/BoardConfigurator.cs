using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BoardConfigurator : MonoBehaviour
{
    public SpriteRenderer tileSpriteRenderer;
    public Transform boardTopArea;

    public BoardSettings boardSettings;

    public void ConfigureBoard()
    {
        float tileWidth = tileSpriteRenderer.sprite.bounds.size.x;

        tileSpriteRenderer.size = new Vector2(boardSettings.HorizontalTileCount * tileWidth,
                                              boardSettings.VerticalTileCount * tileWidth);

        boardTopArea.transform.position = new Vector3(0, (float)boardSettings.VerticalTileCount / 2 * tileWidth);

        boardSettings.TileWidth = tileWidth;
    }

}

[CustomEditor(typeof(BoardConfigurator))]
public class CustomInspectorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardConfigurator boardGenerator = (BoardConfigurator)target;

        if (GUILayout.Button("Configure Board"))
            boardGenerator.ConfigureBoard();
    }
}

