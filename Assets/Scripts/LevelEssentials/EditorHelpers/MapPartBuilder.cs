using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class MapPartBuilder : MonoBehaviour
{
    public GameObject blockPrefab;

    public void CreateStartingBlock()
    {
        if (blockPrefab && !GetComponentInChildren<Block>())
        {
            GameObject firstBlock = Instantiate(blockPrefab, transform);
            firstBlock.name = "BuildingBlock " + FindObjectsOfType<Block>().Length;
            firstBlock.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = firstBlock;
        }
    }
}
#endif