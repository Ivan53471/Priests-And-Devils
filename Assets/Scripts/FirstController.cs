using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene Controller
/// Usage: host on a gameobject in the scene   
/// responsiablities:
///   acted as a scene manager for scheduling actors.log something ...
///   interact with the director and players
/// </summary>
public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    // 对每种物体新建类记录当前状态
    public class PriestsStatus
    {
        public bool onBoatLeft, onBoatRight;
        public bool onBankLeft, onBankRight;
        public PriestsStatus()
        {
            this.onBoatLeft = false;
            this.onBoatRight = false;
            this.onBankLeft = true;
            this.onBankRight = false;
        }
    }
    public class DevilsStatus
    {
        public bool onBoatLeft, onBoatRight;
        public bool onBankLeft, onBankRight;
        public DevilsStatus()
        {
            this.onBoatLeft = false;
            this.onBoatRight = false;
            this.onBankLeft = true;
            this.onBankRight = false;
        }
    }
    public class BoatBehaviour
    {
        public bool isMoving;
        public bool atLeftSide;
        public bool leftPosEmpty, rightPosEmpty;
        public BoatBehaviour() 
        {
            this.isMoving = false;
            this.atLeftSide = true;
            this.leftPosEmpty = true;
            this.rightPosEmpty = true;
        }
    }

    // 记录物体样式、位置等等
    public List<GameObject> Priests, Devils;
    public GameObject boat, bankLeft, bankRight;

    // 物体状态
    public List<PriestsStatus> p_status;
    public List<DevilsStatus> d_status;
    public BoatBehaviour myBoatBehaviour;

    // 记录某个位置有多少个priest（devil）
    public int leftBankPriests, rightBankPriests;
    public int leftBankDevils, rightBankDevils;
    public int boatPriest, boatDevil;

    // the first scripts
    void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.setFPS(60);
        director.currentSceneController = this;
        director.currentSceneController.LoadResources();
    }

    // loading resources for the first scence
    public void LoadResources()
    {
        // priests
        Priests = new List<GameObject>();
        p_status = new List<PriestsStatus>();
        for (int i = 0; i < 3; i++)
        {
            GameObject priests = Instantiate<GameObject>(
                                Resources.Load<GameObject>("Prefabs/Priests"),
                                Vector3.zero, Quaternion.identity);
            priests.name = "Priest " + (i + 1);
            //priests.tag = "Priest";
            Priests.Add(priests);
        }

        //devils
        Devils = new List<GameObject>();
        d_status = new List<DevilsStatus>();
        for (int i = 0; i < 3; i++)
        {
            GameObject devils = Instantiate<GameObject>(
                                Resources.Load<GameObject>("Prefabs/Devils"),
                                Vector3.zero, Quaternion.identity);
            devils.name = "Devil " + (i + 1);
            //devils.tag = "Devil";
            Devils.Add(devils);
        }

        //boat
        boat = Instantiate<GameObject>(
               Resources.Load<GameObject>("Prefabs/Boat"),
               Vector3.zero, Quaternion.identity);
        boat.name = "Boat";

        //bank
        bankLeft = Instantiate<GameObject>(
               Resources.Load<GameObject>("Prefabs/Shore"),
               Vector3.zero, Quaternion.identity);
        bankLeft.name = "BankLeft";

        bankRight = Instantiate<GameObject>(
               Resources.Load<GameObject>("Prefabs/Shore"),
               Vector3.zero, Quaternion.identity);
        bankRight.name = "BankRight";

        // 初始化
        restart();
    }

    public void boatMove()
    {
        // 船在动或者船上没人时不能开船
        if(!myBoatBehaviour.isMoving && (boatPriest + boatDevil > 0))
        {
            myBoatBehaviour.isMoving = true;
        }
    }
    public void onBoatMoving()
    {
        if(myBoatBehaviour.isMoving) 
        {
            // 找出船上有谁
            Vector3 moveDir; //船移动幅度
            int i1 = -1;
            int i2 = -1;
            for (int i = 0; i < 3; i++)
            {
                if (p_status[i].onBoatLeft || p_status[i].onBoatRight)
                {
                    i1 = i;
                    break;
                }
                if (d_status[i].onBoatLeft || d_status[i].onBoatRight)
                {
                    i1 = i + 3;
                    break;
                }
            }
            for (int i = 0;i < 3;i++)
            {
                //第一个选了priest
                if (i == i1)
                {                   
                    if (d_status[i].onBoatLeft || d_status[i].onBoatRight)
                    {
                        i2 = i + 3;
                        break;
                    }
                }
                //第一个选了devil
                else if (i == i1 - 3)
                {
                    if (p_status[i].onBoatLeft || p_status[i].onBoatRight)
                    {
                        i2 = i;
                        break;
                    }
                }
                else
                {
                    if (p_status[i].onBoatLeft || p_status[i].onBoatRight)
                    {
                        i2 = i;
                        break;
                    }
                    if (d_status[i].onBoatLeft || d_status[i].onBoatRight)
                    {
                        i2 = i + 3;
                        break;
                    }
                }
            }
            // 判断向左向右走
            // 向左
            if (!myBoatBehaviour.atLeftSide)
            {
                moveDir = new Vector3(-0.1f, 0, 0);
                boat.transform.Translate(moveDir);
                // 判断第一个位置
                if (i1 != -1)
                {
                    if(i1 < 3)
                        Priests[i1].transform.Translate(moveDir);
                    else
                        Devils[i1 - 3].transform.Translate(moveDir);
                }
                // 判断第二个位置
                if (i2 != -1)
                {
                    if (i2 < 3)
                        Priests[i2].transform.Translate(moveDir);
                    else
                        Devils[i2 - 3].transform.Translate(moveDir);
                }
                //到达对岸
                if (boat.transform.position.x == -2.4f)
                {
                    myBoatBehaviour.isMoving = false;
                    myBoatBehaviour.atLeftSide = true;
                }
                    
            }
            // 向右
            else
            {
                moveDir = new Vector3(0.1f, 0, 0);
                boat.transform.Translate(moveDir);
                // 判断第一个位置
                if (i1 != -1)
                {
                    if (i1 < 3)
                        Priests[i1].transform.Translate(moveDir);
                    else
                        Devils[i1 - 3].transform.Translate(moveDir);
                }
                // 判断第二个位置
                if (i2 != -1)
                {
                    if (i2 < 3)
                        Priests[i2].transform.Translate(moveDir);
                    else
                        Devils[i2 - 3].transform.Translate(moveDir);
                }
                //到达对岸
                if (boat.transform.position.x == 1.7f)
                {
                    myBoatBehaviour.isMoving = false;
                    myBoatBehaviour.atLeftSide = false;
                }
            }
        }
    }
    public void priestsGetOn()
    {
        // 如果船正在动，那么不能响应用户事件
        if (myBoatBehaviour.isMoving)
            return;
        // 船在左边
        if (myBoatBehaviour.atLeftSide)
        {
            for (int i = 0; i < Priests.Count; i++)
            {
                // 左侧岸上有牧师
                if (p_status[i].onBankLeft)
                {
                    // 上船位置
                    if (myBoatBehaviour.leftPosEmpty)
                    {
                        p_status[i].onBankLeft = false;
                        p_status[i].onBoatLeft = true;
                        p_status[i].onBoatRight = false;
                        myBoatBehaviour.leftPosEmpty = false;
                        Priests[i].transform.position = new Vector3(-3.3f, 1.5f, 0);
                        boatPriest++;
                        leftBankPriests--;
                        break;
                    }
                    if (myBoatBehaviour.rightPosEmpty)
                    {
                        p_status[i].onBankLeft = false;
                        p_status[i].onBoatRight = true;
                        p_status[i].onBoatLeft = false;
                        myBoatBehaviour.rightPosEmpty = false;
                        Priests[i].transform.position = new Vector3(-1.8f, 1.5f, 0);
                        boatPriest++; 
                        leftBankPriests--;
                        break;
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < Priests.Count; i++)
            {
                //右侧岸上有牧师
                if (p_status[i].onBankRight)
                {
                    if (myBoatBehaviour.leftPosEmpty)
                    {
                        p_status[i].onBankRight = false;
                        p_status[i].onBoatLeft = true;
                        p_status[i].onBoatRight = false;
                        myBoatBehaviour.leftPosEmpty = false;
                        Priests[i].transform.position = new Vector3(1, 1.5f, 0);
                        boatPriest++;
                        rightBankPriests--;
                        break;
                    }
                    if (myBoatBehaviour.rightPosEmpty)
                    {
                        p_status[i].onBankRight = false;
                        p_status[i].onBoatRight = true;
                        p_status[i].onBoatLeft = false;
                        myBoatBehaviour.rightPosEmpty = false;
                        Priests[i].transform.position = new Vector3(2.5f, 1.5f, 0);
                        boatPriest++; 
                        rightBankPriests--;
                        break;
                    }
                }
            }
        }
    }
    public void priestsGetOff()
    {
        if (myBoatBehaviour.isMoving)
            return;
        // 船在左边
        if (myBoatBehaviour.atLeftSide)
        {
            for (int i = 0; i < Priests.Count; i++)
            {
                //船左侧有priest
                if (p_status[i].onBoatLeft)
                {
                    p_status[i].onBoatLeft = false;
                    p_status[i].onBankLeft = true;
                    p_status[i].onBankRight = false;
                    myBoatBehaviour.leftPosEmpty = true;
                    Priests[i].transform.position = new Vector3((-8.6f + i * 1.3f), 3, 0);
                    boatPriest--;
                    leftBankPriests++;
                    break;
                }
                //船右侧有priest
                if (p_status[i].onBoatRight)
                {
                    p_status[i].onBoatRight = false;
                    p_status[i].onBankLeft = true;
                    p_status[i].onBankRight = false;
                    myBoatBehaviour.rightPosEmpty = true;
                    Priests[i].transform.position = new Vector3((-8.6f + i * 1.3f), 3, 0);
                    boatPriest--;
                    leftBankPriests++;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < Priests.Count; i++)
            {
                //船左侧有priest
                if (p_status[i].onBoatLeft)
                {
                    p_status[i].onBoatLeft = false;
                    p_status[i].onBankLeft = false;
                    p_status[i].onBankRight = true;
                    myBoatBehaviour.leftPosEmpty = true;
                    Priests[i].transform.position = new Vector3((6 + i * 1.3f), 3, 0);
                    boatPriest--;
                    rightBankPriests++;
                    break;
                }
                //船右侧有priest
                if (p_status[i].onBoatRight)
                {
                    p_status[i].onBoatRight = false;
                    p_status[i].onBankLeft = false;
                    p_status[i].onBankRight = true;
                    myBoatBehaviour.rightPosEmpty = true;
                    Priests[i].transform.position = new Vector3((6 + i * 1.3f), 3, 0);
                    boatPriest--;  
                    rightBankPriests++;
                    break;
                }
            }
        }
    }
    public void devilsGetOn()
    {
        if (myBoatBehaviour.isMoving)
            return;
        // 船在左边
        if (myBoatBehaviour.atLeftSide)
        {
            for (int i = 0; i < Devils.Count; i++)
            {
                //左侧岸上有devils
                if (d_status[i].onBankLeft)
                {
                    // 上船位置
                    if (myBoatBehaviour.leftPosEmpty)
                    {
                        d_status[i].onBankLeft = false;
                        d_status[i].onBoatLeft = true;
                        d_status[i].onBoatRight = false;
                        myBoatBehaviour.leftPosEmpty = false;
                        Devils[i].transform.position = new Vector3(-3.3f, 1.5f, 0);
                        boatDevil++;
                        leftBankDevils--;
                        break;
                    }
                    if (myBoatBehaviour.rightPosEmpty)
                    {
                        d_status[i].onBankLeft = false;
                        d_status[i].onBoatRight = true;
                        d_status[i].onBoatLeft = false;
                        myBoatBehaviour.rightPosEmpty = false;
                        Devils[i].transform.position = new Vector3(-1.8f, 1.5f, 0);
                        boatDevil++; 
                        leftBankDevils--;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < Devils.Count; i++)
            {
                //右侧岸上有devils
                if (d_status[i].onBankRight)
                {
                    if (myBoatBehaviour.leftPosEmpty)
                    {
                        d_status[i].onBankRight = false;
                        d_status[i].onBoatLeft = true;
                        d_status[i].onBoatRight = false;
                        myBoatBehaviour.leftPosEmpty = false;
                        Devils[i].transform.position = new Vector3(1, 1.5f, 0);
                        boatDevil++;
                        rightBankDevils--;
                        break;
                    }
                    if (myBoatBehaviour.rightPosEmpty)
                    {
                        d_status[i].onBankRight = false;
                        d_status[i].onBoatRight = true;
                        d_status[i].onBoatLeft = false;
                        myBoatBehaviour.rightPosEmpty = false;
                        Devils[i].transform.position = new Vector3(2.5f, 1.5f, 0);
                        boatDevil++;
                        rightBankDevils--;
                        break;
                    }
                }
            }
        }
    }
    public void devilsGetOff()
    {
        if (myBoatBehaviour.isMoving)
            return;
        // 船在左边
        if (myBoatBehaviour.atLeftSide)
        {
            for (int i = 0; i < Devils.Count; i++)
            {
                //船左侧有devils
                if (d_status[i].onBoatLeft)
                {
                    d_status[i].onBoatLeft = false;
                    d_status[i].onBankLeft = true;
                    d_status[i].onBankRight = false;
                    myBoatBehaviour.leftPosEmpty = true;
                    Devils[i].transform.position = new Vector3((-12.9f + i * 1.3f), 3, 0);
                    boatDevil--;
                    leftBankDevils++;
                    break;
                }
                //船右侧有devils
                if (d_status[i].onBoatRight)
                {
                    d_status[i].onBoatRight = false;
                    d_status[i].onBankLeft = true;
                    d_status[i].onBankRight = false;
                    myBoatBehaviour.rightPosEmpty = true;
                    Devils[i].transform.position = new Vector3((-12.9f + i * 1.3f), 3, 0);
                    boatDevil--;
                    leftBankDevils++;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < Devils.Count; i++)
            {
                //船左侧有devils
                if (d_status[i].onBoatLeft)
                {
                    d_status[i].onBoatLeft = false;
                    d_status[i].onBankLeft = false;
                    d_status[i].onBankRight = true;    
                    myBoatBehaviour.leftPosEmpty = true;
                    Devils[i].transform.position = new Vector3((10.3f + i * 1.3f), 3, 0);
                    boatDevil--;
                    rightBankDevils++;
                    break;
                }
                //船右侧有devils
                if (d_status[i].onBoatRight)
                {
                    d_status[i].onBoatRight = false;
                    d_status[i].onBankLeft = false;
                    d_status[i].onBankRight = true;
                    myBoatBehaviour.rightPosEmpty = true;
                    Devils[i].transform.position = new Vector3((10.3f + i * 1.3f), 3, 0);
                    boatDevil--;
                    rightBankDevils++;
                    break;
                }
            }
        }
    }
    public int GameOver()
    {
        // 船靠岸分情况讨论
        if(myBoatBehaviour.atLeftSide)
        {
            if ((leftBankPriests + boatPriest > 0
                    && leftBankDevils + boatDevil > leftBankPriests + boatPriest)
                    || (rightBankDevils > rightBankPriests && rightBankPriests > 0))
            {
                return 1;
            }
        }
        else
        {
            if ((rightBankPriests + boatPriest > 0
                && rightBankDevils + boatDevil > rightBankPriests + boatPriest)
                || (leftBankDevils > leftBankPriests && leftBankPriests > 0))
            {
                return 1;
            }
            if (rightBankDevils + boatDevil == 3 && rightBankPriests + boatPriest == 3)
            {
                return 2;
            }
        }
        return 0;
    }
    public void restart()
    {
        // priests
        p_status = new List<PriestsStatus>();
        for (int i = 0; i < 3; i++)
        {
            p_status.Add(new PriestsStatus());
        }
        Priests[0].transform.position = new Vector3(-8.6f, 3, 0);
        Priests[1].transform.position = new Vector3(-7.3f, 3, 0);
        Priests[2].transform.position = new Vector3(-6, 3, 0);

        //devils
        d_status = new List<DevilsStatus>();
        for (int i = 0; i < 3; i++)
        {
            d_status.Add(new DevilsStatus());
        }
        Devils[0].transform.position = new Vector3(-12.9f, 3, 0);
        Devils[1].transform.position = new Vector3(-11.6f, 3, 0);
        Devils[2].transform.position = new Vector3(-10.3f, 3, 0);

        //boat
        myBoatBehaviour = new BoatBehaviour();
        boat.transform.position = new Vector3(-2.4f, 0.5f, 0);

        //bank
        bankLeft.transform.position = new Vector3(-8.5f, 1.5f, 0);
        bankRight.transform.position = new Vector3(8.5f, 1.5f, 0);

        // 初始化
        leftBankDevils = 3;
        leftBankPriests = 3;
        rightBankDevils = 0;
        rightBankPriests = 0;
        boatPriest = 0;
        boatDevil = 0;
    }
    // Use this for initialization
    void Start()
    {
        //give advice first
    }

    // Update is called once per frame
    void Update()
    {
        //give advice first
        //用于船和人的移动
        onBoatMoving();
    }

}
