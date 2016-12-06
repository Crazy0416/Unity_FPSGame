using UnityEngine;
using UnityEngine.UI;   // UI 항목에 접근하기 위해 반드시 추가
using System.Collections;

// 클래스에 System.Serializable 이라는 어트리뷰트(Attribute)를 명시해야
// Inspector 뷰에 노출됨
[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}


public class PlayerCtrl : MonoBehaviour {
    private float h = 0.0f;
    private float v = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수 할당한 후 사용
    private Transform tr;
    // 이동속도 변수
    public float moveSpeed = 10.0f;

    // 회전 속도 변수
    public float rotSpeed = 100.0f;

    // 인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;
    // 아래에 있는 3D모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    // Player의 생명 변수
    public int hp = 100;
    // Player의 생명 초깃값
    private int initHp;
    // Player의 Health bar 이미지
    public Image imgHpbar;

    // 델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // 게임 매니저에 접근하기 위한 변수
    private GameMgr gameMgr;

	// Use this for initialization
	void Start () {
        // 생명 초깃값 설정
        initHp = hp;

        tr = GetComponent<Transform>();             // 이 스크립트가 포함된 게임 오브젝트가 갖고 있는 여러 컴포넌트중 Transform 컴포넌트를 추출해서 tr변수에 저장하라(앞에 GameObject 생략)

        // 자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();

        // Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        _animation.clip = anim.idle;
        _animation.Play();

        // GameMgr 스크립트 할당
        gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
    }
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");            // InputManager의 "Horizantal"에 미리 설정한 값으로 키보드의 A,D 또는 화살표키 Left,Right를 눌렀을 때 -1부터 +1 까지의 값을 반환한다.
        v = Input.GetAxis("Vertical");              // InputManager의 "Vertical"에 미리 설정한 값으로 키보드의 W,S 또는 화살표키 Up,Down를 눌렀을 때 -1부터 +1 까지의 값을 반환한다.

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준좌표)           // Time.deltaTime 을 집어 넣어야 매 초마다 10 유닛만큼 이동한다 아니면 프레임마다 이동(프레임이 달라질 경우 속도가 달라진다)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        // Vector3.up을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        if (v >= 0.1f)
        {
            // 전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            // 후진 애니메이션
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            // 왼쪽 애니메이션
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            // 오른쪽 애니메이션
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else
            _animation.CrossFade(anim.idle.name, 0.3f);
    }

    void OnTriggerEnter(Collider coll)
    {
        // 충돌한 Collider가 몬스터의 PUNCH이면 Player의 HP 차감
        if (coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;
            // Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpbar.fillAmount = (float)hp / (float)initHp;
            Debug.Log("Player hp : " + this.hp);

            // Player의 hp가 0이면 사망
            if(hp <= 0)
            {
                PlayerDie();
                // 게임 매니저의 isGameOver 변숫값을 변경해 몬스터의 출현을 중지시킴
                gameMgr.isGameOver = true;
            }
        }
    }

    // Player의 사망 처리 루틴
    void PlayerDie()
    {
        Debug.Log("Player Die!!!!!!");
        /*
        // MONSTER라는 Tag를 가진 모든 게임오브젝트를 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        // 모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        */

        // 이벤트 발생시킴
        OnPlayerDie();

        // 게임 메니저의 isGameOver 변수값을 변경해 몬스터 출현을 중지시킴
        // gameMgr.isGameOver = true;

        // GameMgr의 싱글턴 인스턴스를 접근해 isGameOver 변수값을 변경
        GameMgr.instance.isGameOver = true;

    }
}
