using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    private IUserAction myActions;
    private ISceneController mySceneController;
    float btnWidth = (float)Screen.width / 6.0f;
    float btnHeight = (float)Screen.height / 6.0f;

    void Start()
    {
        mySceneController = SSDirector.getInstance().currentSceneController;
        myActions = mySceneController as IUserAction;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        //游戏正常进行
        if(mySceneController.GameOver() == 0)
        {
            if (GUI.Button(new Rect(btnWidth / 2, 250, btnWidth, btnHeight), "Priests GetOn"))
            {
                myActions.priestsGetOn();
            }
            if (GUI.Button(new Rect(btnWidth / 2 + btnWidth, 250, btnWidth, btnHeight), "Priests GetOff"))
            {
                myActions.priestsGetOff();
            }
            if (GUI.Button(new Rect(btnWidth / 2 + 2 * btnWidth, 250, btnWidth, btnHeight), "Go!"))
            {
                myActions.boatMove();
            }
            if (GUI.Button(new Rect(btnWidth / 2 + 3 * btnWidth, 250, btnWidth, btnHeight), "Devils GetOn"))
            {
                myActions.devilsGetOn();
            }
            if (GUI.Button(new Rect(btnWidth / 2 + 4 * btnWidth, 250, btnWidth, btnHeight), "Devils GetOff"))
            {
                myActions.devilsGetOff();
            }
        }
        //lose
        else if(mySceneController.GameOver() == 1)
        {
            GUI.Box(new Rect(2 * btnWidth, btnHeight, 2 * btnWidth, btnHeight), "\nYOU LOSE");
            if (GUI.Button(new Rect(2.5f * btnWidth, 2 * btnHeight, btnWidth, btnHeight), "Restart"))
            {
                mySceneController.restart();
            }
        }
        //win
        else
        {
            GUI.Box(new Rect(2 * btnWidth, btnHeight, 2 * btnWidth, btnHeight), "\nYOU WIN");
            if (GUI.Button(new Rect(2.5f * btnWidth, 2 * btnHeight, btnWidth, btnHeight), "Restart"))
            {
                mySceneController.restart();
            }
        }
        
    }
}