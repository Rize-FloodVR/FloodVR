using UnityEngine;

public class WaterRiseController : MonoBehaviour
{
    public float riseHeight = 1.35f; //최종 얼마나 올라갈지(y축으로 1.35까지)
    public float riseDuration = 60f; //몇 초동안?

    private float riseSpeed; //초당 올라가는 속도
        void Start()
    {
        // 현재 위치가 아직 목표 높이보다 낮을 때만 계속 올라감
        riseSpeed = riseHeight / riseDuration;
    }


    void Update()
    {
        //현재 위치가 아직 목표 높이보다 낮을 경우
        if (transform.position.y < riseHeight)
        {
            //현재 위치에서 프레임당 위로 상승
            transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);
        }
    }
}
