using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager instance;
    private void Awake() { instance = this; }
    public bool forcedStart = false;
    private bool clearingHero = false;

    [Header("Spawning Body Parts")]
    [SerializeField] private float distanceBetween = 0.2f;
    [SerializeField] private float SetRotation = 0;
    [SerializeField] private GameObject arrowHead = null;
    [SerializeField] private List<GameObject> heroList = new List<GameObject>();
    private List<GameObject> snakeBodies = new List<GameObject>();
    private float countUp = 0;

    [Header("Snake Movement Stats")]
    [SerializeField] private float speed = 250f;
    [SerializeField] private float turnSpeed = 2.5f;
    public enum MoveType { ARROW, MOUSE }
    private MoveType moveType;
    private Vector3 moveDir, destination;
    private Transform arrow = null;
    private bool updateArrow = false;

    ////////////////////////////////////////////////////////////////////////////
    public void AddToHeroList(List<GameObject> GOs) { heroList.Clear(); heroList.AddRange(GOs); }
    public int GetHeroListCount() { return heroList.Count; }

    ////////////////////////////////////////////////////////////////////////////
    private void Start()
    { 
        if (!forcedStart)   { SetMoveType(PlayerPrefs.GetInt("control")); }
        else                { SetMoveType(0); }

    }
    private void FixedUpdate()
    {
        if (GameManager.waveStart || forcedStart)
        {
            ManageSnakeBody();
            if (snakeBodies.Count > 0) { SnakeMovement(); }
            else if(!clearingHero)     { StartCoroutine(GameManager.instance.LoseGame()); }
        }
        else if (snakeBodies.Count > 0)
        {
            snakeBodies[0].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    private void ManageSnakeBody()
    {
        if (heroList.Count > 0) { 
            CreateBodyParts();
            if (PlayerPrefs.GetInt("ARROW") == 1) { SpawnArrowHead(); }
        }

        for (int i = 0; i < snakeBodies.Count; i++)
        {
            if (snakeBodies[i] == null)
            {
                snakeBodies.RemoveAt(i);
                if (i == 0 && PlayerPrefs.GetInt("ARROW") == 1) { SpawnArrowHead(); }
                i--;
            }
        }
    }
    private void CreateBodyParts()
    {
        //create head
        if (snakeBodies.Count == 0)
        {
            //spawn head
            GameObject _snakeHead = Instantiate(heroList[0], transform.position, transform.rotation, transform);
            _snakeHead.GetComponent<MarkerManager>().ChangeBoolean(1, true);
            
            snakeBodies.Add(_snakeHead);
            heroList.RemoveAt(0);

            if (SetRotation != 0) { snakeBodies[0].transform.GetChild(0).rotation = Quaternion.Euler(0, 0, SetRotation); }
        }

        //create the rest
        MarkerManager markerM = snakeBodies[snakeBodies.Count - 1].GetComponent<MarkerManager>();
        if (countUp == 0) { markerM.ClearMarkerList(); }

        countUp += Time.deltaTime;
        if (countUp >= distanceBetween)
        {
            GameObject _snakeBody = Instantiate(heroList[0], markerM.markerList[0].position, markerM.markerList[0].rotation, transform);

            snakeBodies.Add(_snakeBody);
            heroList.RemoveAt(0);

            if (heroList.Count == 0) { _snakeBody.GetComponent<MarkerManager>().ChangeBoolean(0, false); }
            _snakeBody.GetComponent<MarkerManager>().ClearMarkerList();

            countUp = 0;
        }

        //when final body part spawn
        if (heroList.Count == 0 && TeamManager.instance != null)
        {
            List<Member> teamMemberList = TeamManager.instance.GetTeamMember();
            for (int i = 0; i < snakeBodies.Count; i++)
            {
                snakeBodies[i].GetComponent<Hero>().SetHeroLvl(teamMemberList[i].GetHeroLvl());
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    private void SnakeMovement()
    {
        switch (moveType)
        {
            case MoveType.ARROW:
                //handle turning
                float _horizontal = Input.GetAxisRaw("Horizontal");
                if (_horizontal != 0)
                {
                    snakeBodies[0].transform.GetChild(0).Rotate(new Vector3(0, 0, -turnSpeed * 90 * _horizontal * Time.deltaTime));
                }
                break;
            case MoveType.MOUSE:
                //get direction
                if (Input.GetMouseButton(0))
                {
                    destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    destination.z = 0;
                }
                Vector2 mouseDir = (destination - snakeBodies[0].transform.GetChild(0).position).normalized; 
                
                //handle turning
                float angle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                snakeBodies[0].transform.GetChild(0).rotation = Quaternion.Slerp(snakeBodies[0].transform.GetChild(0).rotation, rotation, turnSpeed * Time.deltaTime);
                break;
        }

        //alway move head forward
        moveDir = snakeBodies[0].transform.GetChild(0).right;
        snakeBodies[0].GetComponent<Rigidbody2D>().velocity = moveDir * speed * Time.deltaTime;

        //update arrow if have to
        if (updateArrow)
        {
            arrow.position = snakeBodies[0].transform.GetChild(0).position;
            arrow.rotation = Quaternion.Euler(0, 0, snakeBodies[0].transform.GetChild(0).rotation.eulerAngles.z - 90);
        }

        //move the rest of the body
        if (snakeBodies.Count > 1)
        {
            for (int i = 1; i < snakeBodies.Count; i++)
            {
                MarkerManager markerM = snakeBodies[i - 1].GetComponent<MarkerManager>();
                snakeBodies[i].transform.position = markerM.markerList[0].position;
                snakeBodies[i].transform.rotation = markerM.markerList[0].rotation;
                markerM.markerList.RemoveAt(0);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void SnakeWallBounce(Vector3 normal)
    {
        //reflect correct angle to bounce
        moveDir = Vector3.Reflect(moveDir, normal);

        //rotate head to face direction to move
        float _angleRad = Mathf.Atan2(moveDir.y, moveDir.x);                      // get angle in radians
        float _angleDeg = (180 / Mathf.PI) * _angleRad;                           // get angle in degrees
        snakeBodies[0].transform.GetChild(0).rotation = Quaternion.Euler(0, 0, _angleDeg);      // rotate
    }
    public IEnumerator ClearSnakeBodies()
    {
        clearingHero = true;

        while (snakeBodies.Count > 0)
        {
            Destroy(snakeBodies[snakeBodies.Count - 1]);
            snakeBodies.RemoveAt(snakeBodies.Count - 1);
            yield return new WaitForSeconds(.2f);
        }
        UIManipulation.instance.CloseUI(arrow.gameObject);
        updateArrow = false;

        clearingHero = false;
    }
    public List<GameObject> GetSnakeBody() { return snakeBodies; }
    public void SetMoveType(int type) { moveType = (MoveType)type; }
    public void SpawnArrowHead()
    {
        if (arrow == null)
        {
            //spawn ARROW
            arrow = Instantiate(arrowHead, transform.position, transform.rotation, transform).transform;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
            arrow.GetComponent<SpriteRenderer>().color = snakeBodies[0].GetComponent<Hero>().GetHeroColor();
            updateArrow = true;
        }
        else
        {
            if (PlayerPrefs.GetInt("ARROW") == 0)
            {
                UIManipulation.instance.CloseUI(arrow.gameObject);
                updateArrow = false;
            }
            else
            {
                UIManipulation.instance.OpenUI(arrow.gameObject);
                updateArrow = true;

                if (snakeBodies.Count > 0) { arrow.GetComponent<SpriteRenderer>().color = snakeBodies[0].GetComponent<Hero>().GetHeroColor(); }
            }
        }
    }
}
