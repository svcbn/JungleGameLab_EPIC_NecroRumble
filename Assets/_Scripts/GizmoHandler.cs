using System;
using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity.Managers;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

public class GizmoHandler : MonoBehaviour
{
    private static MonoBehaviour _instance;

    //private IEnumerator _enumerator = null;

    public static void Init()
    {
        _instance = new GameObject($"[{nameof(GizmoHandler)}]").AddComponent<GizmoHandler>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;
        
        Gizmos.color = Color.magenta;
        ManagerRoot.Chunk.AllBlocks.ForEach(block =>
        {
            Gizmos.color = block.Value.chunk != null ? Color.magenta : block.Value.isFixed ? Color.cyan : Color.grey;
            Gizmos.DrawWireCube(block.Value.worldPosition, Vector3.one * ManagerRoot.Chunk.Data.BlockSize * 0.92f);
        });
    }
    #endif
}
