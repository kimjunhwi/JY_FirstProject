using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;


public class Block : MonoBehaviour
{
    
    protected BlockData blockData;
    [SerializeField]
    protected SpriteRenderer backgroudnSrpite;

    public Vector2 Vec_Position {get; set;}

    public virtual void Init(BlockData _blockData, Vector2 _vecFloorPosition)
    {
        blockData = _blockData;
        Vec_Position = _vecFloorPosition;

        backgroudnSrpite = gameObject.GetComponent<SpriteRenderer>();
    }

    public virtual E_CHECK_BLOCK CheckBlock(E_BLOCK_COLOR_TYPE _Type)
    {
        Debug.LogWarning("ParentCheckBlock");
        return E_CHECK_BLOCK.E_FAILED;
    }

    public virtual void SetBlockColor(E_BLOCK_COLOR_TYPE _type = E_BLOCK_COLOR_TYPE.E_NONE)
    {
    }

    public BlockData GetBlockData()
    {
        return blockData;
    }
    [SerializeField]
    public class BlockData
    {
        //몇 층 짜리 블록 인지
        public int nFloorIndex;
        //오른쪽일 경우 전체 블록은 왼쪽아래로 이동
        //왼쪽일 경우 전체 블록은 오른쪽 아래로 이동을 판별하기 위함
        public bool isRight = false;
        public E_BLOCKTYPE E_BlockType;
        public SpriteRenderer background_blockSprite;
    }
}
