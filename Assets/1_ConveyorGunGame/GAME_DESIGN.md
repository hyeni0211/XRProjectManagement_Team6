# Conveyor Gun Game - 게임 설계 문서

## 개요
컨베이어 벨트에서 끝없이 오는 타겟들을 총으로 쏴서 맞추는 VR 슈팅 게임

## 게임 플로우
1. 플레이어가 총(Gun_Cosmic_Retro_Blaster_1)을 잡음
2. 컨베이어 벨트(PropSpawnPoint)에서 타겟 오브젝트가 스폰됨
3. 타겟이 컨베이어 벨트를 따라 이동
4. 플레이어가 총으로 타겟을 쏴서 맞춤
5. 맞춘 타겟은 파괴되고 점수 획득
6. 맞추지 못한 타겟은 바구니(끝)에서 파괴됨 (점수 감소 또는 HP 감소)

## 씬 오브젝트 구조

### 핵심 오브젝트
| 오브젝트 이름 | 역할 | Lua 스크립트 |
|-------------|------|-------------|
| Gun_Cosmic_Retro_Blaster_1 | 총 (잡기 가능) | ShootingGun.lua |
| Shoot Start Point | 총알 발사 위치 | - |
| PropSpawnPoint | 타겟 스폰 위치 | - |
| ConveyorBelt | 컨베이어 벨트 | ConveyorBelt.lua |
| GameCanvas | 게임 UI (점수) | UIManager.lua |
| Canvas_left | 가이드 UI | - |

### 관리자 오브젝트
| 오브젝트 이름 | 역할 | Lua 스크립트 |
|-------------|------|-------------|
| GameManager | 게임 전체 관리 | ConveyorGameManager.lua |
| SpawnManager | 타겟 풀링/스폰 관리 | TargetSpawnManager.lua |

## 풀링 시스템 (VIVEN SDK 특수)

### 왜 풀링이 필요한가?
- **VIVEN SDK에서는 동적 VObject Instantiate가 불가능**
- 씬에 미리 배치된 오브젝트를 재사용해야 함
- SetActive() 대신 MeshRenderer/Collider 토글 방식 사용

### 풀링 구조
```
TargetPool (부모 오브젝트)
├── Target_0 (비활성화 상태)
├── Target_1 (비활성화 상태)
├── Target_2 (비활성화 상태)
└── ... (필요한 만큼)
```

### 풀링 동작
1. **비활성화**: MeshRenderer/Collider 끄기 + HIDE_POSITION(-9999)으로 이동
2. **활성화**: MeshRenderer/Collider 켜기 + 스폰 위치로 이동
3. **상태 동기화**: `VivenGrabbableModule.FlushInteractableCollider()` 호출

## 스크립트 목록

### 매니저 스크립트
| 파일 | 설명 |
|------|------|
| `Manager/ConveyorGameManager.lua` | 게임 상태, 점수, 타이머 관리 |
| `Manager/TargetSpawnManager.lua` | 타겟 풀링 및 스폰 관리 |

### 오브젝트 스크립트
| 파일 | 설명 |
|------|------|
| `Objects/ShootingGun.lua` | 총 발사 로직 |
| `Objects/Bullet.lua` | 총알 동작 및 충돌 |
| `Objects/Target.lua` | 타겟 피격 처리 |
| `Objects/ConveyorBelt.lua` | 컨베이어 벨트 물리 |

### UI 스크립트
| 파일 | 설명 |
|------|------|
| `UI/GameUIManager.lua` | 점수 UI 업데이트 |
| `UI/GuideUI.lua` | 가이드 텍스트 관리 |

## 주입 변수 설정

### ConveyorGameManager.lua
```lua
SpawnManagerObject   -- 스폰 매니저
ScoreTextObject      -- 점수 텍스트 (TMP)
GameUIPanel          -- 게임 UI 패널
StartUIPanel         -- 시작 UI 패널
StartButtonObject    -- 시작 버튼
```

### TargetSpawnManager.lua
```lua
SpawnPoint          -- PropSpawnPoint
TargetPool          -- 타겟 풀 부모 오브젝트
GameManagerObject   -- 게임 매니저
```

### ShootingGun.lua
```lua
ShootPoint          -- Shoot Start Point
BulletPool          -- 총알 풀 부모 오브젝트
GameManagerObject   -- 게임 매니저
ShootSound          -- 발사 사운드 (AudioSource)
```

### Target.lua
```lua
SpawnManagerObject  -- 스폰 매니저 (풀 반환용)
GameManagerObject   -- 게임 매니저 (점수용)
```

### Bullet.lua
```lua
SpawnManagerObject  -- 스폰 매니저 (타겟 히트 시)
GameManagerObject   -- 게임 매니저 (점수용)
```

## 개발 순서

### Phase 1: 기본 구조
- [x] 게임 설계 문서 작성
- [x] TargetSpawnManager.lua (풀링 시스템)
- [x] Target.lua (타겟 스크립트)

### Phase 2: 총 시스템
- [x] ShootingGun.lua (총 발사)
- [x] Bullet.lua (총알)
- [ ] BulletPool 설정 (씬에서 수동 설정 필요)

### Phase 3: 게임 로직
- [x] ConveyorGameManager.lua 업데이트
- [x] 점수 시스템 연동
- [x] 게임 시작/종료 로직

### Phase 4: UI
- [x] GameUIManager.lua
- [ ] GuideUI.lua (선택적)
- [x] 점수/타이머 표시

### Phase 5: 씬 Setup
- [ ] ConveyorGameSetup.cs 업데이트
- [ ] TargetPool 오브젝트 생성 (씬에 Target 프리팹 배치)
- [ ] BulletPool 오브젝트 생성 (씬에 Bullet 프리팹 배치)
- [ ] 컴포넌트 연결

## 참고 코드

### 풀링 핵심 패턴 (RecycleDunk 참고)
```lua
-- 비활성화: MeshRenderer/Collider 끄기
local HIDE_POSITION = Vector3(0, -9999, 0)

function SetPoolObjectVisible(poolIndex, visible)
    -- MeshRenderer 토글
    for _, mr in ipairs(meshRenderers[poolIndex]) do
        mr.enabled = visible
    end
    -- Collider 토글
    for _, col in ipairs(colliders[poolIndex]) do
        col.enabled = visible
    end
    -- 숨김 위치 이동
    if not visible then
        obj.transform.position = HIDE_POSITION
    end
    -- VIVEN SDK 상태 동기화
    grabbable:FlushInteractableCollider()
end
```

## 사운드 효과
- `GunShooting.wav` - 총 발사 사운드
- `GoalAchive.wav` - 타겟 명중 사운드
- `GunGrab&Drop.wav` - 총 잡기/놓기 사운드

## 씬 설정 가이드

### 1. TargetPool 설정
```
TargetPool (빈 오브젝트)
├── Target_0 (타겟 프리팹 복사)
│   ├── VivenLuaBehaviour (Target.lua)
│   ├── Collider (Is Trigger 권장)
│   └── MeshRenderer
├── Target_1
├── Target_2
└── ... (10~20개 권장)
```

### 2. BulletPool 설정
```
BulletPool (빈 오브젝트)
├── Bullet_0 (총알 프리팹 복사)
│   ├── VivenLuaBehaviour (Bullet.lua)
│   ├── Rigidbody (Use Gravity: Off)
│   ├── Collider (Is Trigger 권장)
│   └── MeshRenderer
├── Bullet_1
├── Bullet_2
└── ... (20~30개 권장)
```

### 3. 매니저 오브젝트 설정
```
GameManager (빈 오브젝트)
└── VivenLuaBehaviour (ConveyorGameManager.lua)
    - SpawnManagerObject: SpawnManager
    - ScoreTextObject: 점수 TMP 오브젝트
    - TimerTextObject: 타이머 TMP 오브젝트
    - StartUIPanel: 시작 UI 패널
    - GameUIPanel: 게임 중 UI 패널
    - GameOverUIPanel: 게임 오버 UI 패널
    - StartButtonObject: 시작 버튼
    - RestartButtonObject: 재시작 버튼

SpawnManager (빈 오브젝트)
└── VivenLuaBehaviour (TargetSpawnManager.lua)
    - SpawnPoint: PropSpawnPoint
    - TargetPool: TargetPool 오브젝트
    - GameManagerObject: GameManager
```

### 4. Gun 설정
```
Gun_Cosmic_Retro_Blaster_1
├── VivenGrabbableModule (필수)
├── VivenRigidbodyControlModule
├── VivenLuaBehaviour (ShootingGun.lua)
│   - ShootPoint: Shoot Start Point
│   - BulletPool: BulletPool 오브젝트
│   - GameManagerObject: GameManager
│   - ShootSoundObject: 발사 사운드 AudioSource
└── Shoot Start Point (자식 오브젝트, 총구 위치)
```

### 5. 태그 설정
- Target 오브젝트: Tag = "Target"
- Bullet 오브젝트: Tag = "Bullet"

### 6. 레이어 설정 (선택적)
- Target: "Target" 레이어
- Bullet: "Bullet" 레이어
- 충돌 매트릭스에서 Bullet-Target만 충돌 활성화

## 완료된 스크립트 목록

| 파일 경로 | 설명 | 상태 |
|----------|------|------|
| `Scripts/Manager/ConveyorGameManager.lua` | 게임 상태, 점수, 타이머 | ✅ 완료 |
| `Scripts/Manager/TargetSpawnManager.lua` | 타겟 풀링 및 스폰 | ✅ 완료 |
| `Scripts/Objects/ShootingGun.lua` | 총 잡기, 발사 | ✅ 완료 |
| `Scripts/Objects/Bullet.lua` | 총알 이동, 충돌 | ✅ 완료 |
| `Scripts/Objects/Target.lua` | 타겟 피격 처리 | ✅ 완료 |
| `Scripts/UI/GameUIManager.lua` | UI 업데이트 | ✅ 완료 |
