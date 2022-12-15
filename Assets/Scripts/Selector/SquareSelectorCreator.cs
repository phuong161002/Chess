using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    private Board board;
    [SerializeField] private GameObject freeSquarePrefab;
    [SerializeField] private GameObject enemySquarePrefab;
    [SerializeField] private GameObject selectedSquarePrefab;
    [SerializeField] private GameObject startSquareMovePrefab;
    [SerializeField] private GameObject targetSquareMovePrefab;

    private List<GameObject> instantiatedSelectors;
    private List<GameObject> moveSquares;

    private void Awake()
    {
        instantiatedSelectors = new List<GameObject>();
        moveSquares = new List<GameObject>();
        board = GetComponent<Board>();
    }

    public void ShowSelection(Dictionary<Vector3, bool> squaresData)
    {
        foreach (Vector3 position in squaresData.Keys)
        {
            bool isFreeSquare = squaresData[position];
            // If it is free Square
            if (isFreeSquare)
            {
                GameObject freeSquare = Instantiate(freeSquarePrefab, position + Vector3.up * 0.01f,
                    Quaternion.identity, transform);
                instantiatedSelectors.Add(freeSquare);
            }
            // If it is enemy Square
            else
            {
                GameObject enemySquare = Instantiate(enemySquarePrefab, position + Vector3.up * 0.01f,
                    Quaternion.identity, transform);
                instantiatedSelectors.Add(enemySquare);
            }
        }
    }

    public void ShowSelectedSquare(Vector3 position)
    {
        GameObject selectedSquare = Instantiate(selectedSquarePrefab, position + Vector3.up * 0.01f,
            Quaternion.identity, transform);
        instantiatedSelectors.Add(selectedSquare);
    }

    public void ClearSelection()
    {
        instantiatedSelectors.ForEach(Destroy);
        instantiatedSelectors.Clear();
    }

    public void ShowMove(Move move)
    {
        GameObject startSquare = Instantiate(startSquareMovePrefab,
            board.CalculatePositionFromCoords(move.sourceCoords), Quaternion.identity, transform);
        GameObject targetSquare = Instantiate(targetSquareMovePrefab,
            board.CalculatePositionFromCoords(move.targetCoords), Quaternion.identity, transform);
        moveSquares.Add(startSquare);
        moveSquares.Add(targetSquare);
    }

    public void ClearMove()
    {
        foreach (GameObject moveSquare in moveSquares)
        {
            Destroy(moveSquare);
        }

        moveSquares.Clear();
    }

    public void ClearAll()
    {
        ClearSelection();
        ClearMove();
    }
}