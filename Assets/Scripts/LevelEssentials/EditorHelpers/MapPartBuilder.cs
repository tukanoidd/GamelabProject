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

            Block checkBlock = firstBlock.GetComponent<Block>(); 
            if (checkBlock) checkBlock.mapPartBuilderParent = this;
            else
            {
                Block[] blocks = firstBlock.GetComponentsInChildren<Block>();
                foreach (Block block in blocks)
                {
                    block.mapPartBuilderParent = this;
                }
            }

            Selection.activeGameObject = firstBlock;
        }
    }
}
#endif