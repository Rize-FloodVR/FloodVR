# 🎮 FloodVR

## 👥 브랜치 구조
```
main
└── develop
   ├── dev/soohyeon      
   ├── dev/minji       
   ├── dev/taehan       
   └── dev/seungwoo     
```

## 🔀 병합 워크플로우

### 📋 작업 프로세스
1. **개별 브랜치에서 작업**
   - 각 팀원은 자신의 브랜치(`dev/이름`)에서 작업

2. **Pull Request 생성**
   - 작업 완료 후 `develop` 브랜치로 Pull Request 생성
   - PR 제목: `[타입] 작업 내용 요약`
   - PR 설명에 주요 변경사항 포함

3. **팀장이 병합**
   - 팀장이 Pull Request 확인
   - `develop` 브랜치에 병합
   - 병합 완료 후 해당 브랜치는 최신 `develop`으로 업데이트

### 🚀 Pull Request 가이드라인

**PR 제목 예시**
```
[feat] 플레이어 전투 시스템 구현
```

**PR 설명 템플릿**
```
## 🔧 변경 사항
- 주요 변경 내용 1
- 주요 변경 내용 2
```

## 📝 커밋 메시지 가이드라인

### 🔧 기본 구조

```
[타입] 간결한 제목 (50자 이내)

선택사항: 상세 설명
- 변경 사항의 이유
- 주요 변경 내용
```

## 📚 예시

### 💡 제목 예시

```bash
feat: 플레이어 점프 시스템 구현
fix: NullReferenceException in PlayerController
refactor: 인벤토리 시스템 코드 정리
art: 캐릭터 애니메이션 추가
scene: 메인 메뉴 UI 레이아웃 수정
perf: 오브젝트 풀링으로 메모리 최적화
config: Android 빌드 설정 최적화
```

### 📝 상세 설명이 포함된 예시

```bash
feat: Enemy AI 상태머신 패턴 적용
- State 인터페이스 구현
- Idle, Chase, Attack 상태 분리  
- 상태 전환 로직 개선
- 디버그용 상태 표시 UI 추가
```

### 🏷️ 커밋 타입 분류

| 타입 | 설명 | 예시 |
|------|------|------|
| 🆕 feat | 새로운 기능 추가 | feat: 플레이어 대시 스킬 구현 |
| 🐛 fix | 버그 수정 | fix: NullReferenceException in PlayerController.Update() |
| 🔄 refactor | 코드 리팩토링 | refactor: GameManager 싱글톤 패턴 적용 |
| 🎨 art | 아트 에셋 관련 | art: 캐릭터 스프라이트 업데이트 |
| 🔊 audio | 사운드/음악 관련 | audio: BGM 트랙 추가 |
| 🎬 scene | 씬 파일 수정 | scene: 메인 메뉴 레이아웃 조정 |
| ⚡ perf | 성능 최적화 | perf: 오브젝트 풀링 시스템 도입 |
| 🎨 style | 코드 스타일 수정 | style: 코드 포맷팅 및 인덴트 정리 |
| 🧪 test | 테스트 관련 | test: PlayerController 유닛 테스트 추가 |
| 📝 docs | 문서 작성/수정 | docs: README 파일 업데이트 |
| ⚙️ config | 프로젝트 설정 변경 | config: Build Settings 플랫폼 추가 |
| 🛠️ chore | 프로젝트 관리 작업 | chore: 사용하지 않는 에셋 제거 |



## 🎯 Unity C# 네이밍 컨벤션

### 📊 Bool 변수 명명
상태나 조건을 명확히 표현
```csharp
private bool isGrounded;
private bool isJumping;
private bool canMove;
private bool hasKey;
private bool shouldRespawn;
private bool isInvincible;
```

### 📋 Collection 명명 규칙

#### List & Array
```csharp
// List - 복수형으로 명명
public List<Enemy> enemies;
public List<Transform> spawnPoints;
public List<AudioClip> backgroundMusics;

// Array - 복수형 또는 Array 접미사
public GameObject[] weaponPrefabs;
public Transform[] wayPoints;
public AudioClip[] soundEffects;
```

#### Dictionary
```csharp
// 용도에 맞는 명명
public Dictionary<string, int> itemPrices;
public Dictionary<KeyCode, Action> keyBindings;
public Dictionary<EnemyType, GameObject> enemyPrefabs;
```

#### 컴포넌트 참조
```csharp
// 컴포넌트 타입 명시
private Rigidbody2D playerRigidbody;
private SpriteRenderer spriteRenderer;
private BoxCollider2D boxCollider;
private AudioSource audioSource;
```

#### 코루틴 & 델리게이트
```csharp
// 코루틴 - 동사 + Coroutine
private IEnumerator SpawnEnemiesCoroutine()
private IEnumerator FadeOutCoroutine()

// 델리게이트/이벤트 - On + 동사
public UnityEvent OnPlayerDeath;
public Action OnItemCollected;
public System.Action<float> OnHealthChanged;
```

#### 상수 & 열거형
```csharp
// 상수 - 모두 대문자 + 언더스코어
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
```

#### 프리팹 & 에셋
```csharp
// 프리팹 참조 - Prefab 접미사
public GameObject enemyPrefab;
public GameObject bulletPrefab;
public GameObject explosionPrefab;

// 에셋 참조 - 타입 명시
public Sprite playerSprite;
public AudioClip jumpSound;
public Material playerMaterial;
```


