using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using UnityEngine.UI;

public class inGameScene : Scene
{
    bool isGameOver;

    public Button Red_Button;
    public Button Blue_Button;
    public Button Green_Button;

    //나중에 수정 
    public Button retryButton;

    [SerializeField]
    List<Block> Block_List = new List<Block>();

    int nFloorCount = 0;
    int nBlockCount = 0;
    const int nMaxBasicBlock = 20;

    //나중에 리스트로 추가
    public SimpleObjectPool normalBlockObjectPool;
    public SimpleObjectPool countBlockObjectPool;

    void Awake()
    {
        Red_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_RED));
        Blue_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_BLUE));
        Green_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_GREEN));
    }

    public override void Init(UIManager _uiManager)
    {
        base.Init(_uiManager);

        normalBlockObjectPool.PreloadPool();

        gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        GameInit();
    }

    public override void Hide()
    {
        base.Hide();
    }

    void Update()
    {
#if UNITY_EDITOR

    if(Input.GetKeyDown(KeyCode.LeftArrow))
    {
        ClickButton(E_BLOCK_COLOR_TYPE.E_RED);
    }

    if(Input.GetKeyDown(KeyCode.DownArrow))
    {
        ClickButton(E_BLOCK_COLOR_TYPE.E_BLUE);
    }

    if(Input.GetKeyDown(KeyCode.RightArrow))
    {
        ClickButton(E_BLOCK_COLOR_TYPE.E_GREEN);
    }

#endif
    }

     void GameInit()
    {
        nFloorCount = 0;
        nBlockCount = 0;

        Block_List.Clear();

        isGameOver = false;

        BlockInit();
    }

    void BlockInit()
    {
        while(Block_List.Count != 0)
        {
            normalBlockObjectPool.ReturnObject(Block_List[0].gameObject);
            Block_List.RemoveAt(0);
        }

        BlocksSetting();

        Block_List.Find((block) => block.GetBlockData().nFloorIndex == nFloorCount).SetBlockColor(E_BLOCK_COLOR_TYPE.E_NONE);
    }

    void ClickButton(E_BLOCK_COLOR_TYPE _colorType)
    {
        if(Block_List.Count < 0 || isGameOver)
            return;

        Block checkBlock = Block_List.Find(( (list) => list.GetBlockData().nFloorIndex == nFloorCount + 1));

        switch(checkBlock.CheckBlock(_colorType))
        {
            case E_CHECK_BLOCK.E_SUCCESS:
            {
                checkBlock.SetBlockColor(E_BLOCK_COLOR_TYPE.E_NONE);
                NextStair();
            }
            break;
            case E_CHECK_BLOCK.E_FAILED:
            {
#if CHEAT
                checkBlock.SetBlockColor(E_BLOCK_COLOR_TYPE.E_NONE);
                NextStair();
#endif
                isGameOver = true;

                CWindowResultPopup.CWindowResultData data = new CWindowResultPopup.CWindowResultData();

                //임시
                data.strTitle   = "Result";
                data.strOk      = "Lobby";
                data.strCancle  = "Restart";
                data.strScoreValue = nFloorCount.ToString();

                GameManager.Instance.Window_ResultPopup(data, (result) =>
                {
                    switch(result)
                    {
                        case (int)CWindowResultPopup.E_WINDOW_RESULT.E_LOBBY:
                        {
                            uiManager.ShowScene(this,E_GAME_SCENE.E_LOBBY);
                        }
                        break;
                        case (int)CWindowResultPopup.E_WINDOW_RESULT.E_RESTART:
                        {
                            GameInit();
                        }
                        break;
                    }
                });
            }
            break;
        }
    }

    public void BlocksSetting()
    {
        while(Block_List.Count <= nMaxBasicBlock)
        {
            GameObject blockObject = null;
            Vector2 vec_BlockPosition = Vector2.one;
            Block.BlockData blockData = new Block.BlockData();

            blockData.nFloorIndex = nBlockCount;
            blockData.E_BlockType = Get_BLOCKTYPE(blockData.nFloorIndex);

            switch(blockData.E_BlockType)
            {
                case E_BLOCKTYPE.E_NONE:
                {
                    blockObject = normalBlockObjectPool.GetObject();
                }
                break;
                case E_BLOCKTYPE.E_COUNT:
                {
                    blockObject = countBlockObjectPool.GetObject();
                }
                break;
                default:
                {
                    Debug.LogWarning("맞는 블록이 없습니다!!");
                }
                break;
            }

            if(nBlockCount == 0)
            {
                vec_BlockPosition = new Vector2(0,-0.25f);
            }
            else
            {
                Vector2 vec_BeforeBlockPosition = Block_List[Block_List.Count -1].transform.position;

                //0 일 경우 오른쪽 생성 1일 경우 왼쪽
                if(Random.Range(0, 2) == 0)
                {
                    blockData.isRight = true;
                    vec_BlockPosition = new Vector2(vec_BeforeBlockPosition.x + 1, vec_BeforeBlockPosition.y + 0.5f);
                }
                else
                {
                    blockData.isRight = false;
                    vec_BlockPosition = new Vector2(vec_BeforeBlockPosition.x - 1, vec_BeforeBlockPosition.y + 0.5f);
                }
            }

            blockObject.transform.position = vec_BlockPosition;

            Block block = blockObject.GetComponent<Block>();

            block.Init(blockData,vec_BlockPosition);

            Block_List.Add(block);

            nBlockCount++;
        }
    }


    //
    void NextStair()
    {
        bool isNextStairRight = Block_List.Find((block) => block.GetBlockData().nFloorIndex == nFloorCount + 1).GetBlockData().isRight;
        
        for(int nIndex = 0; nIndex < Block_List.Count; nIndex++)
        {
            if(isNextStairRight)
            {
                Block_List[nIndex].transform.position = 
                    new Vector2(Block_List[nIndex].transform.position.x - 1, Block_List[nIndex].transform.position.y - 0.5f);
            }
            else
            {
                Block_List[nIndex].transform.position = 
                    new Vector2(Block_List[nIndex].transform.position.x + 1, Block_List[nIndex].transform.position.y - 0.5f);
            }
        }

        if(Block_List.Count != 0)
        {
            if(Block_List[0].transform.position.y <- 4.8f)
            {
                GameObject obj = Block_List[0].gameObject;

                Block_List.RemoveAt(0);
                normalBlockObjectPool.ReturnObject(obj);
                BlocksSetting();
            }
        }

        nFloorCount++;
    }


    //미정
    E_BLOCKTYPE Get_BLOCKTYPE(int _nFloor)
    {
        return E_BLOCKTYPE.E_NONE;
    }
}
