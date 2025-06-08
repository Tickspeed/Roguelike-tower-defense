// TileManager.cs
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic; // Needed for tracking occupied tiles

public class TileManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Color hoverColor;

    private Vector3Int previousCellPosition; // To track the last hovered tile
    private Color previousCellColor;         // To restore the tile's original color

    // A data structure to keep track of which tiles have towers
    private HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

    void Update()
    {
        // Convert the mouse position to a cell position on the tilemap
        Vector3Int currentCellPosition = GetMouseToCellPosition();

        // --- HOVERING LOGIC ---
        // Check if the mouse has moved to a new cell
        if (currentCellPosition != previousCellPosition)
        {
            // Reset the color of the previously hovered tile
            tilemap.SetTileFlags(previousCellPosition, TileFlags.None); // Unlock the tile for color changing
            tilemap.SetColor(previousCellPosition, previousCellColor);

            // Check if the new cell is valid before highlighting
            if (tilemap.HasTile(currentCellPosition))
            {
                // Store the new tile's original color and highlight it
                previousCellColor = tilemap.GetColor(currentCellPosition);
                tilemap.SetTileFlags(currentCellPosition, TileFlags.None);
                tilemap.SetColor(currentCellPosition, hoverColor);
            }

            // Update the previous position for the next frame
            previousCellPosition = currentCellPosition;
        }

        // --- TOWER PLACEMENT LOGIC ---
        // Check for a mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the tile is a valid building spot and not already occupied
            if (tilemap.HasTile(currentCellPosition) && !occupiedTiles.Contains(currentCellPosition))
            {
                PlaceTower(currentCellPosition);

            }
        }
    }

    private Vector3Int GetMouseToCellPosition()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Convert world position to cell position on the tilemap grid
        return tilemap.WorldToCell(mouseWorldPos);
    }

    private void PlaceTower(Vector3Int cellPosition)
    {
        // Get the selected tower prefab from our TowerPlacing manager
        GameObject towerToPlace = TowerPlacing.main.GetSelectedTower();

        if (towerToPlace != null)
        {
            // Calculate the world position for the center of the cell
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
            cellCenterPosition.z = 0;

            // Create the tower instance
            GameObject towerInstance = Instantiate(towerToPlace, cellCenterPosition, Quaternion.identity);
            towerInstance.SetActive(true);

            // Mark this tile as occupied
            occupiedTiles.Add(cellPosition);

            Debug.Log("Tower built at cell: " + cellPosition);
        }
    }
}
