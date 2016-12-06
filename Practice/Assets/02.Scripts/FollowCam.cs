using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    public Transform targetTr;          // 추적할 타깃 게임오브젝트의 Transform 변수
    public float dist = 8.0f;          // 카메라와의 일정 거리
    public float height = 3.0f;         // 카메라의 높이 설정
    public float dampTrace = 10.0f;     // 부드러운 추적을 위한 변수
    // 카메라 자신의 Transform 변수
    private Transform tr;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update 함수 호출 이후 한 번씩 호출되는 함수인 LateUpdate 사용
    // 추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate 함수 사용
	void LateUpdate () {
        // 카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고
        // height 변수만큼 위로 올림
        // Vector3.Lerp(시작위치, 종료위치, float 시간) : 두 벡터(시작, 종료)를 선형 보간 시키는 함수이다(부드럽게 이동)
        tr.position = Vector3.Lerp(tr.position, targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), Time.deltaTime * dampTrace);
        tr.LookAt(targetTr.position);
	}
}
