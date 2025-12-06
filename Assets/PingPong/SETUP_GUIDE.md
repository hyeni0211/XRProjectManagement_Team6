# 탁구 게임 씬 구성 가이드

## 폴더 구조
```
Assets/PingPong/
├── Scripts/
│   ├── Manager/
│   │   └── PingPongGameManager.lua   ✅ 생성됨
│   └── Objects/
│       ├── PingPongRacket.lua        ✅ 생성됨
│       ├── PingPongBall.lua          ✅ 생성됨
│       └── BallLauncher.lua          ✅ 생성됨
├── Prefabs/                          📁 여기에 프리팹 저장
├── Scenes/                           📁 여기에 씬 저장
└── SETUP_GUIDE.md                    📄 이 파일
```

---

## 1. 씬 구성 순서

### Step 1: 새 씬 생성
1. `Assets/PingPong/Scenes/` 폴더에 새 씬 생성
2. 씬 이름: `PingPongGame`

### Step 2: 게임 매니저 설정
1. 빈 GameObject 생성 → 이름: `PingPongGameManager`
2. 컴포넌트 추가:
   - `VivenLuaBehaviour`
   - Script에 `PingPongGameManager.lua` 연결

---

## 2. 탁구채 (PingPongRacket) 설정

### 필수 컴포넌트
```
PingPongRacket (GameObject)
├── VObject
├── VivenGrabbableModule
├── VivenRigidbodyControlModule
├── VivenGrabbableRigidView
├── VivenLuaBehaviour → PingPongRacket.lua
├── Rigidbody
├── Collider (BoxCollider 또는 MeshCollider)
└── 3D 모델 (탁구채 모양)
```

### 설정 값
| 컴포넌트 | 속성 | 값 |
|----------|------|-----|
| VivenGrabbableModule | grabType | Velocity |
| VivenGrabbableModule | parentToHandOnGrab | true |
| VivenRigidbodyControlModule | physicsType | Physics |
| Rigidbody | Mass | 0.5 |
| Rigidbody | Collision Detection | Continuous Dynamic |

### Inspector 주입
| 변수 | 값 |
|------|-----|
| gameManagerName | "PingPongGameManager" |

### 프리팹 저장
→ `Assets/PingPong/Prefabs/PingPongRacket.prefab`

---

## 3. 탁구공 (PingPongBall) 설정

### 필수 컴포넌트
```
PingPongBall (GameObject)
├── VivenLuaBehaviour → PingPongBall.lua
├── Rigidbody
├── SphereCollider
└── 3D 모델 (Pack_FREE_Balls 에셋 사용 가능)
```

### 설정 값
| 컴포넌트 | 속성 | 값 |
|----------|------|-----|
| Rigidbody | Mass | 0.1 |
| Rigidbody | Drag | 0.1 |
| Rigidbody | Use Gravity | true |
| Rigidbody | Collision Detection | Continuous |
| SphereCollider | Radius | 0.02 (탁구공 크기) |

### Physics Material (선택)
- Bounciness: 0.8 (탄성)
- Friction: 0.2

### 프리팹 저장
→ `Assets/PingPong/Prefabs/PingPongBall.prefab`

---

## 4. 공 발사 기계 (BallLauncher) 설정

### 필수 컴포넌트
```
BallLauncher (GameObject)
├── VivenLuaBehaviour → BallLauncher.lua
├── 3D 모델 (기계 모양)
└── LaunchPoint (빈 자식 오브젝트 - 공 발사 위치)
```

### 하위 구조
```
BallLauncher
├── Model (기계 3D 모델)
└── LaunchPoint (Transform) ← 공이 생성되는 위치
```

### Inspector 주입
| 변수 | 값 |
|------|-----|
| BallPrefab | PingPongBall 프리팹 |
| LaunchPoint | LaunchPoint Transform |
| PlayerTarget | 플레이어 위치 또는 XR Origin |
| gameManagerName | "PingPongGameManager" |

---

## 5. 게임 매니저 Inspector 주입

| 변수 | 값 |
|------|-----|
| BallLauncherObject | BallLauncher 오브젝트 |
| ScoreUIObject | (선택) 점수 UI |

---

## 6. 씬 계층 구조 예시

```
PingPongGame (Scene)
├── XR Origin (또는 Viven Player)
├── PingPongGameManager
│   └── BallLauncher
│       ├── Model
│       └── LaunchPoint
├── PingPongRacket (플레이어 근처에 배치)
├── Environment
│   ├── Floor
│   ├── Walls
│   └── Table (선택)
└── UI
    └── ScoreCanvas (선택)
```

---

## 7. 테스트 방법

1. Play 모드 진입
2. 탁구채를 잡는다
3. 게임 시작 (스크립트에서 `StartGame()` 호출)
4. 공이 날아오면 탁구채로 친다
5. 점수가 올라가는지 확인

### 게임 시작 테스트 코드
게임 매니저의 `start()` 함수에 임시로 추가:
```lua
function start()
    SetDifficulty(1)
    StartGame()  -- 자동 시작 (테스트용)
end
```

---

## 8. 난이도별 설정

| 난이도 | 공 속도 | 발사 간격 | 기계 속도 |
|--------|---------|-----------|-----------|
| 1 (쉬움) | 3 | 3초 | 1 |
| 2 (보통) | 5 | 2초 | 2 |
| 3 (어려움) | 8 | 1초 | 3 |

---

## 9. 사용 가능한 에셋

### 탁구공
- `Assets/Pack_FREE_Balls/` 폴더의 공 모델 사용 가능

### 기타
- 탁구채, 기계 모델은 직접 제작하거나 에셋스토어에서 다운로드

---

## 10. 주의사항

1. **VObject는 탁구채에만** - 공과 기계는 네트워크 동기화 불필요
2. **Collider 크기** - 너무 작으면 충돌 감지 안 됨
3. **Rigidbody Collision Detection** - 빠른 물체는 Continuous 필수
4. **LaunchPoint 방향** - Z축이 발사 방향 (forward)

---

## 11. 확장 아이디어

- [ ] 점수 UI 추가
- [ ] 난이도 선택 UI
- [ ] 공을 놓치면 점수 감소
- [ ] 연속 히트 콤보 시스템
- [ ] 사운드 효과 (공 치는 소리)
- [ ] 파티클 효과 (공 맞을 때)
