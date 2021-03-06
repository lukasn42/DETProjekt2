using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Voting : MonoBehaviour
{
    int t, n, p;
    public bool[][] accusesPublic, defendsPublic;
    int[] accuses;
    bool[] wantsSkip;
    bool votingActive;
    public void Awake() {
        t = Game.Instance.Settings.votingTime;
        n = Game.Instance.Settings.numberPlayers;
        p = Game.Instance.allPlayers.FindIndex(x => x.Equals(Game.Instance.gameObject.GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>()));

        accuses = new int[n];
        accusesPublic = new bool[n][];
        defendsPublic = new bool[n][];
        wantsSkip = new bool[n];

        for (int i = 0; i<n; i++) {
            accuses[i] = -1;
            accusesPublic[i] = new bool[n];
            defendsPublic[i] = new bool[n];
        }

        Game.Instance.GUI.setStandardGui(false);

        AsyncOperation op = SceneManager.LoadSceneAsync("SelectionGUI", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));

        StartCoroutine(Countdown(op));
    }

    private IEnumerator Countdown(AsyncOperation op) {
        while (!op.isDone) {
            yield return new WaitForEndOfFrame();
        }
        Game.Instance.GetComponent<swapPlayer>().currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = false;
        GameObject.Find("Canvas/timeRemaining/textTimeRemaining").GetComponent<TextMeshProUGUI>().SetText(t.ToString());
        for (int i=n; i<10; i++) {
            GameObject.Destroy(GameObject.Find("Canvas/Players/Player" + i));
        }
        GameObject.Find("Canvas/Players/Player" + p + "/textName").GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline | FontStyles.Bold;
        deactivatePlayerButtons(p);//deaktiviert alle Buttons zum aktiven Spieler
        for(int i=0;i<Game.Instance.allPlayers.Count;i++)
        {
            TextMeshProUGUI text=GameObject.Find("Canvas/Players/Player" + i + "/textName").GetComponent<TextMeshProUGUI>();
            text.color=Game.Instance.allPlayers[i].color;
            GameObject.Find("Canvas/Players/Player" + i + "/textName/RawImage").SetActive(Game.Instance.GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>() is Imposter && Game.Instance.allPlayers[i] is Imposter);
            if(!Game.Instance.allPlayers[i].isAlive())
            {
                //Destroy(GameObject.Find("Canvas/Players/Player" + i));
                
                text.fontStyle = FontStyles.Strikethrough | FontStyles.Bold;
                deactivatePlayerButtons(i);
                GameObject.Find("Canvas/Players/Player" + i + "/Votings").SetActive(false);
                GameObject.Find("Canvas/Players/Player" + i + "/Votings/VotingsDefensive").SetActive(false);
                GameObject.Find("Canvas/Players/Player" + i + "/Votings/VotingsAttacking").SetActive(false);
            }
        }
        votingActive = true;
        yield return new WaitForSeconds(1);
        foreach(Player player in Game.Instance.allLivingPlayers())
        {
            player.accusePublic();
        }
        int votingTimeStart=t;
        while (t > 0) {
            t--;
            if(t==1)
            {
                foreach(Player player in Game.Instance.allLivingPlayers())
                {
                    player.accuse();
                }
            }
            GameObject.Find("Canvas/timeRemaining/textTimeRemaining").GetComponent<TextMeshProUGUI>().SetText(t.ToString());
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        votingActive = false;
        GameObject.Find("Canvas/Players/Player" + p + "/Buttons").SetActive(true);
        int result = results();
        t = 6;
        while (t > 0) {
            t--;
            GameObject.Find("Canvas/timeRemaining/textTimeRemaining").GetComponent<TextMeshProUGUI>().SetText(t.ToString());
            yield return new WaitForSeconds(1);
        }
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("SelectionGUI"));
        StartCoroutine(Game.Instance.meetingResult(result));
        yield return new WaitForSeconds(5);
        Destroy(this);
        yield return null;
    }

    private void deactivatePlayerButtons(int playerNr)
    {
        GameObject.Find("Canvas/Players/Player" + playerNr + "/Buttons").SetActive(false);
        GameObject.Find("Canvas/Players/Player" + playerNr + "/Buttons/buttonAccusePublic").SetActive(false);
        GameObject.Find("Canvas/Players/Player" + playerNr + "/Buttons/buttonDefendPublic").SetActive(false);
    }
    private int results() {
        int[] accusedBy = new int[n];
        int iMax = -1;
        bool multipleMax = true;

        for (int i=0; i<n; i++) {
            if (accuses[i] != -1) {
                accusedBy[accuses[i]]++;
            }
        }

        for (int i=0; i<n; i++) {
            GameObject obj = GameObject.Find("Canvas/Players/Player" + i + "/Buttons/buttonAccuse/textAccuse");
            if (obj != null) {
                obj.GetComponent<TextMeshProUGUI>().SetText("Accused by " + accusedBy[i]);
            }

            if (accusedBy[i] > 0) {
                if (iMax == -1) {
                    iMax = i;
                    multipleMax = false;
                } else if (accusedBy[i] >= accusedBy[iMax]) {
                    if (accusedBy[i] > accusedBy[iMax]) {
                        multipleMax = false;
                        iMax = i;
                    } else {
                        multipleMax = true;
                    }      
                }
            }
        }

        if (!multipleMax) {
            int skipWantedBy = 0;
            foreach (bool b in wantsSkip) {
                if (b) {
                    skipWantedBy++;
                }
            }
            if (skipWantedBy < accusedBy[iMax]) {
                foreach(Player player in Game.Instance.allLivingPlayers())
                {
                    player.feedbackMeeting(iMax);
                }
                return iMax;
            }
        }
        return -1;
    }

    public void accuse(int p1, int p2) {
        if (!votingActive) return;

        if (p1 == -1) {
            p1 = p;
        }
        if(p1==p)
        {
            if (wantsSkip[p1]) {
                skip(p1);
            } else if (accuses[p1] != -1) {
                GameObject.Find("Canvas/Players/Player" + accuses[p1] + "/Buttons/buttonAccuse").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            if (accuses[p1] == p2) {
                accuses[p1] = -1;
            } else if (p1 == p) {
                accuses[p1] = p2;
                GameObject.Find("Canvas/Players/Player" + p2 + "/Buttons/buttonAccuse").GetComponent<Image>().color = (Color.red + Color.yellow) / 2;
            }
        }
        else
        {
            accuses[p1]=p2;
        }
    }

    public void accusePublic(int p1, int p2) {
        if (!votingActive) return;

        if (p1 == -1) {
            p1 = p;
        }

        if (defendsPublic[p1][p2]) {
            defendPublic(p1, p2);
        }

        accusesPublic[p1][p2] = !accusesPublic[p1][p2];

        if (p1 == p) {
            Color newColor;
            
            if (accusesPublic[p1][p2]) {
                newColor = Color.red;
            } else {
                newColor = new Color(0.5f, 0.5f, 0.5f);
            }
            
            GameObject.Find("Canvas/Players/Player" + p2 + "/Buttons/buttonAccusePublic").GetComponent<Image>().color = newColor;
        }

        if (accusesPublic[p1][p2]) {
            GameObject newEntry = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/publicVotingEntry.prefab", typeof(GameObject)) as GameObject, new Vector3(), new Quaternion());
            newEntry.name = p1.ToString();
            newEntry.GetComponent<TextMeshProUGUI>().SetText(p1.ToString());
            newEntry.transform.SetParent(GameObject.Find("Canvas/Players/Player" + p2 + "/Votings/VotingsAttacking").transform);
        } else {
            GameObject.Destroy(GameObject.Find("Canvas/Players/Player" + p2 + "/Votings/VotingsAttacking/" + p1));
        }
    }

    public void defendPublic(int p1, int p2) {
        if (!votingActive) return;
        
        if (p1 == -1) {
            p1 = p;
        }

        if (accusesPublic[p1][p2]) {
            accusePublic(p1, p2);
        }

        defendsPublic[p1][p2] = !defendsPublic[p1][p2];

        if (p1 == p) {
            Color newColor;
            
            if (defendsPublic[p1][p2]) {
                newColor = Color.green;
            } else {
                newColor = new Color(0.5f, 0.5f, 0.5f);
            }
            
            GameObject.Find("Canvas/Players/Player" + p2 + "/Buttons/buttonDefendPublic").GetComponent<Image>().color = newColor;
        }

        if (defendsPublic[p1][p2]) {
            GameObject newEntry = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/publicVotingEntry.prefab", typeof(GameObject)) as GameObject, new Vector3(), new Quaternion());
            newEntry.name = p1.ToString();
            newEntry.GetComponent<TextMeshProUGUI>().SetText(p1.ToString());
            newEntry.transform.SetParent(GameObject.Find("Canvas/Players/Player" + p2 + "/Votings/VotingsDefensive").transform);
        } else {
            GameObject.Destroy(GameObject.Find("Canvas/Players/Player" + p2 + "/Votings/VotingsDefensive/" + p1));
        }
    }
    public bool hasAccusatedPublic(int initiator, int reciver)
    {
        return accusesPublic[initiator][reciver];
    }
    public bool hasDefendsPublic(int initiator, int reciver)
    {
        return defendsPublic[initiator][reciver];
    }
    public void skip(int p1) {
        if (!votingActive) return;
        if (p1 == -1) {
            p1 = p;
        }
        if(p1==p)
        {
            if (accuses[p1] != -1) {
                accuse(p1, accuses[p1]);
            }

            wantsSkip[p1] = !wantsSkip[p1];

            if (wantsSkip[p1]) {
                GameObject.Find("Canvas/skip").GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
            } else {
                GameObject.Find("Canvas/skip").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        else
        {
            wantsSkip[p1]=true;
        }
        
    }
}