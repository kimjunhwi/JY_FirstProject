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
    public Camera playerCamera;
    public GameObject playerObject;
    public GameObject TouchStartTextObject; 

    [SerializeField]
    List<Block> Block_List = new List<Block>();

    int nFloorCount = 0;
    int nBlockCount = 0;
    const int nMaxBasicBlock = 20;

    Vector3 vecOffset = new Vector3(0,0, - 10);

    float fPlusHp = 1;
    float fCurrentHp = 0;
    const float fMaxHp = 10;
    public Slider sliderTimer;

    //나중에 리스트로 추가
    public SimpleObjectPool normalBlockObjectPool;
    public SimpleObjectPool countBlockObjectPool;

    void Awake()
    {
        playerCamera = Camera.main;
        Red_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_RED));
        Blue_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_BLUE));
        Green_Button.onClick.AddListener(() => ClickButton(E_BLOCK_COLOR_TYPE.E_GREEN));
    }

    public override void Init(UIManager _uiManager)
    {
        base.Init(_uiManager);

        normalBlockObjectPool.PreloadPool();

        playerObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        GameInit();
    }

    public override void Hide()
    {
        gameObject.SetActive(false);

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
        if(isGameOver == true)
            return;

        Vector3 desiredPosition = playerObject.transform.position + vecOffset;

        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredPosition, 0.125f);
    
        sliderTimer.value -= Time.deltaTime + (float)((nFloorCount % 10) * 0.0001);
    }

     void GameInit()
    {
        nFloorCount = 0;
        nBlockCount = 0;

        fCurrentHp = fMaxHp;
        sliderTimer.maxValue = fMaxHp;
        sliderTimer.value = fMaxHp;

        isGameOver = true;

        BlockInit();

        playerObject.SetActive(true);
        TouchStartTextObject.SetActive(true);
    }
    
    void BlockInit()
    {
        BlocksClear();

        BlocksSetting();

        Block_List.Find((block) => block.GetBlockData().nFloorIndex == nFloorCount).SetBlockColor(E_BLOCK_COLOR_TYPE.E_NONE);
    }

    void BlocksClear()
    {
        while(Block_List.Count != 0)
        {
            GameObject block = Block_List[0].gameObject;
            normalBlockObjectPool.ReturnObject(block);
            Block_List.RemoveAt(0);
            block.SetActive(false);
        }

        Block_List.Clear();
    }

    void ClickButton(E_BLOCK_COLOR_TYPE _colorType)
    {
        if(Block_List.Count < 0)
            return;

        if(TouchStartTextObject.activeSelf && isGameOver)
        {
            isGameOver = false;
            TouchStartTextObject.SetActive(false);
        }


        Block checkBlock = Block_List.Find(( (list) => list.GetBlockData().nFloorIndex == nFloorCount + 1));

        switch(checkBlock.CheckBlock(_colorType))
        {
            case E_CHECK_BLOCK.E_SUCCESS:
            {
                sliderTimer.value += 1;
                
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
                            BlocksClear();
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
        
        if(isNextStairRight)
        {
            playerObject.transform.position =
                new Vector2(playerObject.transform.position.x + 1, playerObject.transform.position.y + 0.5f);
        }
        else
        {
            playerObject.transform.position =
                new Vector2(playerObject.transform.position.x - 1, playerObject.transform.position.y + 0.5f);
        }

        if(Block_List.Count != 0)
        {
            if(Vector3.Distance(playerObject.transform.position,Block_List[0].transform.position) > 5)
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
