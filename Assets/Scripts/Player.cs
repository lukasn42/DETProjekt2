using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public string name;
    protected bool imposter;

    public bool alive = true;

    public bool deadAndInvisible=false;

    public Color color;

    public Room lastRoomBeforeMeeting;
    public UpdateRoom updateRoom;

    public void create(string name, Color color)
    {
        this.name=name;
        this.color=color;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)&&activePlayer())
        {
            Player deadBody=nearDeadBody();
            if(deadBody!=null)
            {
                deadBody.DestroyDeadBody();
                Game.Instance.startEmergencyMeeting(this);
            }
        }
    }


    public void FixedUpdate()
    {
        if(activePlayer())
        {
            foreach (var player in Game.Instance.allPlayers)
            {
                if(player.visible()&&Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    player.gameObject.GetComponent<Renderer>().enabled=true;
                }
                else
                {
                    player.gameObject.GetComponent<Renderer>().enabled=false;
                }
            }
            foreach (var task in Game.Instance.allTasks)
            {
                if(Vector3.Distance(gameObject.transform.position, task.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    task.setVisible();
                }
                else
                {
                    task.setInvisble();
                }
            }
           float distance;
            foreach (var sabTask in Game.Instance.allActiveSabortageTasks())
            {
                distance=Vector3.Distance(gameObject.transform.position, sabTask.transform.position);
                if(distance<=Game.Instance.Settings.viewDistance)
                {
                    sabTask.setActivated();
                    sabTask.setVisible();
                }
                else
                {
                    sabTask.setInvisble();
                }
            }
        
        }
        
    }

    public bool isImposter()
    {
        return imposter;
    }
    public bool isAlive()
    {
        return alive;
    }
    protected void becomeGhost()
    {
        
    }
    /*
    returns a dead-Body, that is inside the players vision, or null, if there is no deadBody inside Player vision 
    */
    public Player nearDeadBody()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isAlive()&&player.visible())
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    return player;
                }
            }
        }
        return null;
    }

    public bool activePlayer()
    {
        return gameObject==Game.Instance.swapPlayer().currentPlayer;
    }
    
    //just neaded for CrewMates, cause Imposter never have dead Bodies
    public void DestroyDeadBody()
    {
        deadAndInvisible=true;
    }

    public abstract bool immobile();

    public abstract bool visible();
}
