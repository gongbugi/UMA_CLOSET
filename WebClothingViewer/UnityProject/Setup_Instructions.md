# Unity 프로젝트 설정 가이드

## 1. Unity 프로젝트 생성

1. Unity Hub에서 새 3D 프로젝트 생성
2. 프로젝트명: `WebClothingViewer`
3. 이 폴더의 스크립트들을 Unity 프로젝트의 Assets/Scripts 폴더에 복사

## 2. 씬 설정

### GameObject 구조
```
Main Camera
├─ 위치: (0, 1, -3)
└─ 회전: (0, 0, 0)

DirectionalLight
├─ 위치: (0, 3, 0)
└─ 회전: (50, -30, 0)

ModelParent (빈 GameObject)
├─ 위치: (0, 0, 0)
└─ ClothingModel.cs 스크립트 연결

WebGLCommunicator (빈 GameObject)
├─ WebGLCommunicator.cs 스크립트 연결
├─ ClothingModel 참조 설정
├─ Main Camera 참조 설정
└─ ModelParent 참조 설정 (Camera Target)
```

## 3. 스크립트 설정

### ClothingModel.cs
- `modelParent`: ModelParent GameObject 할당
- `modelRotation`: (0, 0, 0)
- `modelScale`: (1, 1, 1)

### WebGLCommunicator.cs  
- `clothingModel`: ClothingModel 컴포넌트 할당
- `mainCamera`: Main Camera 할당
- `cameraTarget`: ModelParent Transform 할당

## 4. Resources 폴더 구조

```
Assets/Resources/
├─ Models/
│   ├─ Tshirt_Model.prefab
│   ├─ Pants_Model.prefab
│   └─ Shoes_Model.prefab
└─ Textures/
    ├─ Tshirt_001.png
    ├─ Pants_001.png
    └─ Shoes_001.png
```

## 5. WebGL 빌드 설정

### Build Settings
- Platform: WebGL
- Scenes: Main Scene 추가

### Player Settings
- Company Name: YourCompany
- Product Name: WebClothingViewer
- WebGL Template: Default
- Compression Format: Gzip
- Development Build: 체크 (디버깅용)

### 빌드 출력
- 빌드 폴더: `WebClothingViewer/WebInterface/Build/`
- 생성될 파일들:
  - WebClothingViewer.data
  - WebClothingViewer.framework.js  
  - WebClothingViewer.wasm
  - index.html (사용하지 않음)

## 6. 테스트용 3D 모델 생성

간단한 큐브나 Primitive를 사용하여 테스트:

1. Cube 생성 → Prefab으로 저장 (Tshirt_Model.prefab)
2. Capsule 생성 → Prefab으로 저장 (Pants_Model.prefab)  
3. Sphere 생성 → Prefab으로 저장 (Shoes_Model.prefab)

각 Prefab에는 MeshRenderer와 MeshCollider 컴포넌트 필요

## 7. 실행 테스트

1. Unity에서 Play 버튼으로 테스트
2. WebGL 빌드 후 웹 페이지에서 테스트
3. 브라우저 개발자 도구에서 콘솔 로그 확인