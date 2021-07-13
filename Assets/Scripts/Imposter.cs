using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imposter : Player
{
    // Start is called before the first frame update
    private Vent currentUsedVent;
    void Start()
    {
        updateRoom=GetComponent<UpdateRoom>();
    }

    void Awake() {
        imposter=true;
    }

    // Update is called once per frame
    public new void Update()
    {
        if (activePlayer()&&Input.GetKeyDown(KeyCode.Return))
        {
            if(canKill())
            {
                Player playerToKill=null;
                float nearesPlayerDistance=Mathf.Infinity;
                float distance;
                foreach (var player in Game.Instance.allPlayers)
                {
                    if(!player.isImposter()&&player.isAlive())
                    {
                        distance=Vector3.Distance(gameObject.transform.position, player.transform.position);
                        if(distance<=nearesPlayerDistance)
                        {
                            playerToKill=player;
                            nearesPlayerDistance=distance;
                        }
                    }
                }
                kill((CrewMate)playerToKill);
            }
        }
        if(activePlayer()&&Input.GetKeyDown(KeyCode.V))
        {
            if(inVent())
            {
                leaveVent();
            }
            else
            {
                Vent vent=nearestVent();
                if(vent!=null)
                {
                    hideVent(vent);
                }
            }
        }
        if(activePlayer()&&Input.GetKeyDown(KeyCode.Tab))
        {
            if(inVent())
            {
                changeVentPosition();
            }
        }
        if(activePlayer()&&Input.GetKeyDown(KeyCode.Q))
        {
            Game.Instance.startSabortageBournTrees();
            //Game.Instance.startSabortage(Game.Instance.allSabortages[0]);
        }
        base.Update();
    }
    public new void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public bool nearOtherPlayer()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isImposter()&&player.isAlive())
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.killDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /*
    Always at most one, because distance of vents should be hy
    */
    public Vent nearestVent()
    {
        foreach (var vent in Game.Instance.allVents)
        {
           if(Vector3.Distance(gameObject.transform.position, vent.transform.position)<=0.5f)
            {
                return vent;
            }
        }
        return null;
    }
    public bool canKill()
    {
        if(inVent())
        {
            return false;
        }
        bool result=Game.Instance.getKillCooldown()<=0;
        if(!result)
        {
            //Debug.Log("Kill Cooldown active");
            return result;
        }
        result=nearOtherPlayer();
        if(!result)
        {
            //Debug.Log("No Player to kill near imposter");
        }
        return result;
    }
    void kill(CrewMate crewMate)
    {
        //Debug.Log("Imposter kills CrewMate");
        crewMate.getKilledByImposter();
        Game.Instance.resetKillCooldown();
        Game.Instance.checkWinningOverPlayers();
    }
    bool inVent()
    {
        return currentUsedVent!=null;
    }

    void hideVent(Vent vent)
    {
        currentUsedVent=vent;
        gameObject.GetComponent<Renderer>().enabled=false;
        gameObject.transform.position=currentUsedVent.gameObject.transform.position;
    }

    void changeVentPosition()
    {
        currentUsedVent=currentUsedVent.matchedVent;
        gameObject.transform.position=currentUsedVent.gameObject.transform.position;
    }

    void leaveVent()
    {
        currentUsedVent=null;
        gameObject.GetComponent<Renderer>().enabled=true;
    }
    public override void goToMeeting()
    {
        goToMeetingStandard();
    }
    public override bool immobile()
    {
        return currentUsedVent!=null;
    }
    public override bool visible()
    {
        return (currentUsedVent==null)&&!deadAndInvisible;
    }
    public override void accusePublic()
    {
        if(!activePlayer())
        {
            //accursation.accurrsePublic();
        }
    }
    public override void accuse()
    {
        if(!activePlayer())
        {
            //accursation.accurrse();
        }
    }
    public override void noticePublicAccuse(int p1,int p2)
    {

    }
    public override void noticePublicDefend(int p1,int p2)
    {
        
    }

    
    public override float verdacht(int playerNumber)
    {
        return 0f;
    }
}
