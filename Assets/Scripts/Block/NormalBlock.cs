using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class NormalBlock : Block
{
    

    [SerializeField]
    public E_BLOCK_COLOR_TYPE ColorType;

   public override void Init(BlockData _blockData, Vector2 _vecFloorPosition)
   {
       base.Init(_blockData,_vecFloorPosition);

       int nRandomIndex = Random.Range(0,9);

        ColorType = (nRandomIndex < 3 ) ? E_BLOCK_COLOR_TYPE.E_RED : 
                    (nRandomIndex < 6 ) ? E_BLOCK_COLOR_TYPE.E_BLUE :
                                          E_BLOCK_COLOR_TYPE.E_GREEN;

        //임시
        SetBlockColor(ColorType);
   }

   
    public override void SetBlockColor(E_BLOCK_COLOR_TYPE _type = E_BLOCK_COLOR_TYPE.E_NONE)
    {
        ColorType = _type;

         switch(ColorType)
        {
            case E_BLOCK_COLOR_TYPE.E_NONE:
            {
                backgroudnSrpite.color = Color.gray;
            }
            break;
            case E_BLOCK_COLOR_TYPE.E_RED:
            {
                backgroudnSrpite.color = Color.red;
            }
            break;
            case E_BLOCK_COLOR_TYPE.E_BLUE:
            {
                backgroudnSrpite.color = Color.blue;
            }
            break;
            case E_BLOCK_COLOR_TYPE.E_GREEN:
            {
                backgroudnSrpite.color = Color.green;
            }
            break;
        }
    }


   public override E_CHECK_BLOCK CheckBlock(E_BLOCK_COLOR_TYPE _Type)
   {
       if(ColorType == _Type)
       {
           return E_CHECK_BLOCK.E_SUCCESS;
       }

       return E_CHECK_BLOCK.E_FAILED;
   }
}
