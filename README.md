🎮 FloodVR


👥 브랜치 구조
main
├── develop
├── soohyeon    
├── minji   
├── taehan       
└── seungwoo     

📝 커밋 메시지 가이드라인
🔧 기본 구조
[타입] 간결한 제목 (50자 이내)

선택사항: 상세 설명
- 변경 사항의 이유
- 주요 변경 내용
🏷️ 커밋 타입 분류
타입설명예시🆕 feat새로운 기능 추가feat: 플레이어 대시 스킬 구현🐛 fix버그 수정fix: NullReferenceException in PlayerController.Update()🔄 refactor코드 리팩토링refactor: GameManager 싱글톤 패턴 적용🎨 art아트 에셋 관련art: 캐릭터 스프라이트 업데이트🔊 audio사운드/음악 관련audio: BGM 트랙 추가🎬 scene씬 파일 수정scene: 메인 메뉴 레이아웃 조정⚡ perf성능 최적화perf: 오브젝트 풀링 시스템 도입🎨 style코드 스타일 수정style: 코드 포맷팅 및 인덴트 정리🧪 test테스트 관련test: PlayerController 유닛 테스트 추가📝 docs문서 작성/수정docs: README 파일 업데이트⚙️ config프로젝트 설정 변경config: Build Settings 플랫폼 추가🛠️ chore프로젝트 관리 작업chore: 사용하지 않는 에셋 제거

🎯 Unity C# 네이밍 컨벤션
📊 Bool 변수 명명

상태나 조건을 명확히 표현

csharpprivate bool isGrounded;
private bool isJumping;
private bool canMove;
private bool hasKey;
private bool shouldRespawn;
private bool isInvincible;
📋 Collection 명명 규칙
List & Array
csharp// List - 복수형으로 명명
public List<Enemy> enemies;
public List<Transform> spawnPoints;
public List<AudioClip> backgroundMusics;

// Array - 복수형 또는 Array 접미사
public GameObject[] weaponPrefabs;
public Transform[] wayPoints;
public AudioClip[] soundEffects;
Dictionary
csharp// 용도에 맞는 명명
public Dictionary<string, int> itemPrices;
public Dictionary<KeyCode, Action> keyBindings;
public Dictionary<EnemyType, GameObject> enemyPrefabs;
🎯 Unity 특화 명명 규칙
컴포넌트 참조
csharp// 컴포넌트 타입 명시
private Rigidbody2D playerRigidbody;
private SpriteRenderer spriteRenderer;
private BoxCollider2D boxCollider;
private AudioSource audioSource;
코루틴 & 델리게이트
csharp// 코루틴 - 동사 + Coroutine
private IEnumerator SpawnEnemiesCoroutine()
private IEnumerator FadeOutCoroutine()

// 델리게이트/이벤트 - On + 동사
public UnityEvent OnPlayerDeath;
public Action OnItemCollected;
public System.Action<float> OnHealthChanged;
상수 & 열거형
csharp// 상수 - 모두 대문자 + 언더스코어
public const float GRAVITY_SCALE = 9.8f;
public const int MAX_HEALTH = 100;
public const string PLAYER_TAG = "Player";

// 열거형 - PascalCase
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
프리팹 & 에셋
csharp// 프리팹 참조 - Prefab 접미사
public GameObject enemyPrefab;
public GameObject bulletPrefab;
public GameObject explosionPrefab;

// 에셋 참조 - 타입 명시
public Sprite playerSprite;
public AudioClip jumpSound;
public Material playerMaterial;

💡 커밋 가이드라인
🔗 복합적인 변경사항
여러 타입의 작업이 포함된 경우 주요 작업을 기준으로 타입을 정하고 상세 설명에 포함
bashfeat: 플레이어 스킬 시스템 구현

- 스킬 매니저 스크립트 추가
- UI 스킬 트리 구현  
- 스킬 이펙트 파티클 적용
- 관련 애니메이션 및 사운드 추가
🧩 Unity 특화 커밋 예시
bash# 프리팹 관련
feat: 적 캐릭터 프리팹 생성
refactor: 플레이어 프리팹 구조 개선
fix: UI 프리팹 참조 오류 수정

# 성능 최적화
perf: 오브젝트 풀링으로 메모리 최적화
perf: 드로우 콜 배칭 개선
perf: 코루틴을 async/await로 변경
