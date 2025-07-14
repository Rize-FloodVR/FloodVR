using UnityEngine;

public class WaterRiseController : MonoBehaviour
{
    public float riseHeight = 1.35f; //최종 얼마나 올라갈지(y축으로 1.35까지)
    public float riseDuration = 60f; //몇 초동안?

    private float startY;
    private float endY;
    private float elapsed = 0f;

    public WaterUI waterUI; // WaterUI 스크립트 연결

    private float riseSpeed; //초당 올라가는 속도
    
    void Start()
    {
        // 현재 위치가 아직 목표 높이보다 낮을 때만 계속 올라감
        riseSpeed = riseHeight / riseDuration;
        startY = transform.position.y;
        endY = startY + riseHeight;
    }


    void Update()
    {
        //현재 위치가 아직 목표 높이보다 낮을 경우
        if (transform.position.y < endY)
        {
            //현재 위치에서 프레임당 위로 상승
            transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);

            // 수심 비율 계산
            float ratio = (transform.position.y - startY) / riseHeight;
            ratio = Mathf.Clamp01(ratio);

            // WaterUI에 전달 
            if (waterUI != null)
                waterUI.SetWaterLevel(ratio, riseHeight * ratio);

        }
    }
}
