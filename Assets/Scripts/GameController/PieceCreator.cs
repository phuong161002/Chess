using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PieceCreator : SingletonMonobehavior<PieceCreator>
{
    [SerializeField] private GameObject[] piecesPrefab;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material blackMaterial;

    private Dictionary<string, GameObject> nameToPieceDict;

    protected override void Awake()
    {
        base.Awake();
        
        InitDictionary();
    }

    private void InitDictionary()
    {
        nameToPieceDict = new Dictionary<string, GameObject>();
        foreach (GameObject gO in piecesPrefab)
        {
            string pieceType = gO.GetComponent<Piece>().GetType().ToString();
            nameToPieceDict.Add(pieceType, gO);
        }
    }

    public Piece CreatePiece(Type type)
    {
        if (nameToPieceDict.TryGetValue(type.ToString(), out GameObject prefab))
        {
            GameObject pieceGO = Instantiate(prefab);
            return pieceGO.GetComponent<Piece>();
        }

        return null;
    }

    public Material GetTeamMaterial(TeamColor teamColor)
    {
        if (teamColor == TeamColor.WHITE)
        {
            return whiteMaterial;
        }
        else if (teamColor == TeamColor.BLACK)
        {
            return blackMaterial;
        }

        return null;
    }
}
