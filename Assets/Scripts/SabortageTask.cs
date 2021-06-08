using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabortageTask : Task
{
    // Start is called before the first frame update
    private Sabortage mySabortage;
    public bool solved=false;
    void Start()
    {
        sabortageTask=true;
    }
    public void createSabortageTask(int taskNum, float timeToSolve, Sabortage sabortage) {
        this.taskNum = taskNum;
        this.timeToSolve=timeToSolve;
        this.solvingVisible=true;
        mySabortage=sabortage;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void setActivated()
    {
        if(!visibleAktivated&&mySabortage.active)
        {
            visibleAktivated=true;
            currentColor= Color.red;
        }
    }
    public void finishSolving()
    {
        solved=true;
        mySabortage.maybeDeactivated();
        endSolving();
        setDeactivated();
        setVisible();
    }
}