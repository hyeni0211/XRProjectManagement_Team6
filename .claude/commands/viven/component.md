# Viven 컴포넌트 가이드

Viven SDK 컴포넌트 추가 및 설정 가이드입니다.

## 컴포넌트 종류별 필수 조합

### 1. 기본 네트워크 오브젝트
```
VObject
├── VivenTransformView (Transform 동기화)
└── Collider (선택)
```

### 2. 잡기 가능 오브젝트 (Grabbable)
```
VObject
├── VivenGrabbableModule
├── VivenRigidbodyControlModule
├── VivenGrabbableRigidView
├── Rigidbody
└── Collider
```

### 3. 앉기 가능 오브젝트 (Sittable)
```
VObject
├── VivenSittable
└── Collider (Trigger)
```

### 4. 탑승 가능 오브젝트 (Ridable)
```
VObject
├── VivenRidableModule
├── VivenCustomAnimationModule
└── Collider
```

### 5. 손 포즈/제스처 감지
```
VivenPoseOrGestureInteraction
├── VivenHandPose (ScriptableObject)
└── detectHandType: Both/Left/Right
```

## VObject 설정

```
displayName: "오브젝트 이름"
objectId: (자동 생성 GUID)
contentType: Prepared (맵과 함께 로드)
objectSyncType: Continuous (지속적 동기화)
```

## VivenGrabbableModule 설정

```
grabType: Velocity (물리 적용) / Kinematic (물리 무시)
parentToHandOnGrab: true
throwForce: 5.0
holdTimeThreshold: 1.0
```

## VivenRigidbodyControlModule 설정

```
physicsType: Physics / Kinematic
originMass: 1.0
originDrag: 0.1
originAngularDrag: 0.05
automaticCenterOfMass: true
```

## VivenPoseOrGestureInteraction 이벤트

```lua
-- Lua에서 접근
local poseDetector = object:GetComponent("VivenPoseOrGestureInteraction")

function onEnable()
    poseDetector.onPoseOrGesturePerformed:AddListener(onPoseDetected)
    poseDetector.onPoseOrGestureEnded:AddListener(onPoseEnded)
end

function onDisable()
    poseDetector.onPoseOrGesturePerformed:RemoveListener(onPoseDetected)
    poseDetector.onPoseOrGestureEnded:RemoveListener(onPoseEnded)
end

function onPoseDetected()
    Debug.Log("포즈 감지됨")
end

function onPoseEnded()
    Debug.Log("포즈 종료됨")
end
```

## 컴포넌트 추가 순서

1. **VObject** 먼저 추가 (네트워크 기반)
2. **Collider** 추가 (상호작용 영역 정의)
3. **기능별 모듈** 추가 (Grabbable, Sittable 등)
4. **View 컴포넌트** 추가 (네트워크 동기화)
5. **Lua 스크립트** 연결

## 주의사항

- VObject의 objectId는 수정하지 말 것 (자동 생성)
- Rigidbody는 직접 접근하지 말고 VivenRigidbodyControlModule 사용
- 네트워크 동기화가 필요하면 반드시 View 컴포넌트 추가

사용자가 원하는 기능에 맞는 컴포넌트 조합을 안내해주세요.
