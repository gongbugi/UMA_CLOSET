# Web Clothing Viewer 구현 가이드

## 프로젝트 개요

웹 UI에서 옷 리스트를 표시하고, Unity WebGL로 선택된 옷의 3D 모델을 표시하는 독립적인 시스템입니다.

## 기술 스택

### 웹 프론트엔드
- **HTML5**: 기본 구조
- **CSS3**: 스타일링 및 반응형 디자인
- **Vanilla JavaScript**: Unity 통신 및 UI 로직
- **No Framework**: 가벼운 구현을 위해 프레임워크 사용 안함

### Unity WebGL
- **Unity 2022.3 LTS** (권장)
- **WebGL Build Platform**
- **Custom Scripts**: 웹 통신 및 3D 모델 관리

## 주요 기능

### 1. 웹 UI 기능
- 옷 리스트 표시 (카테고리별 필터링)
- 검색 기능
- 반응형 디자인
- Unity WebGL 임베드

### 2. Unity WebGL 기능
- 3D 옷 모델 표시
- 카메라 컨트롤 (회전, 줌)
- 웹과의 실시간 통신
- 텍스처 동적 변경

### 3. 웹-Unity 통신
- JavaScript → Unity: 옷 선택, 카메라 조작
- Unity → JavaScript: 상태 변경 알림

## 파일 구조 설명

```
WebClothingViewer/
├─ UnityProject/                    # Unity 프로젝트 파일들
│   ├─ Scripts/
│   │   ├─ ClothingModel.cs        # 3D 옷 모델 관리
│   │   └─ WebGLCommunicator.cs    # 웹 통신 관리
│   ├─ Setup_Instructions.md       # Unity 설정 가이드
│   └─ [Unity Assets]
│
├─ WebInterface/                    # 웹 인터페이스
│   ├─ index.html                  # 메인 페이지
│   ├─ styles.css                  # 스타일시트
│   ├─ js/
│   │   ├─ clothingData.js         # 옷 데이터 정의
│   │   ├─ unityInterface.js       # Unity 통신 관리
│   │   └─ app.js                  # 메인 앱 로직
│   ├─ Build/                      # Unity WebGL 빌드 파일들
│   └─ images/                     # 옷 썸네일 이미지들
│
├─ Assets/                         # 공통 에셋 (3D 모델, 텍스처)
├─ Documentation/                  # 문서들
└─ README.md
```

## 구현 단계

### 1단계: Unity 프로젝트 설정
1. Unity에서 새 3D 프로젝트 생성
2. 제공된 스크립트들을 Assets/Scripts에 복사
3. 씬 설정 (카메라, 라이트, GameObject 구조)
4. Resources 폴더에 테스트용 3D 모델 배치

### 2단계: WebGL 빌드
1. Build Settings에서 WebGL 플랫폼 선택
2. Player Settings 구성
3. `WebInterface/Build/` 폴더에 빌드

### 3단계: 웹 페이지 테스트
1. 로컬 웹 서버 실행 (http-server, Live Server 등)
2. `WebInterface/index.html` 열기
3. Unity WebGL 로딩 확인
4. 옷 선택 및 3D 표시 테스트

## 주요 클래스 설명

### ClothingModel.cs
- 3D 옷 모델의 로딩, 표시, 텍스처 적용 관리
- Resources 폴더에서 동적으로 에셋 로드
- 웹에서 호출 가능한 공개 메서드 제공

### WebGLCommunicator.cs  
- 웹과 Unity 간 통신 브리지 역할
- JavaScript에서 직접 호출 가능한 메서드들
- Unity → JavaScript 메시지 전송 기능

### ClothingViewerApp (JavaScript)
- 웹 UI의 메인 애플리케이션 로직
- 옷 리스트 렌더링, 검색, 카테고리 필터링
- Unity 인터페이스와의 연동

### UnityInterface (JavaScript)
- Unity WebGL 초기화 및 통신 관리
- 메시지 큐 시스템으로 안정적인 통신 보장
- 로딩 상태 관리

## 확장 가능성

### 데이터 소스 확장
- JSON 파일 → REST API 연동
- 실시간 옷 데이터 업데이트
- 사용자별 맞춤 옷 리스트

### 3D 기능 확장
- 더 정교한 3D 모델 (FBX, GLTF)
- 애니메이션 지원
- 물리 시뮬레이션 (천 재질 등)

### UI/UX 개선
- 더 많은 카메라 컨트롤 옵션
- AR/VR 지원
- 소셜 공유 기능

## 성능 최적화

### Unity WebGL
- LOD (Level of Detail) 시스템 적용
- 텍스처 압축 최적화
- 불필요한 컴포넌트 제거

### 웹 인터페이스
- 이미지 최적화 (WebP, 압축)
- 지연 로딩 (Lazy Loading)
- 캐싱 전략

## 배포 가이드

### 로컬 테스트
```bash
# 간단한 HTTP 서버 실행
cd WebClothingViewer/WebInterface
python -m http.server 8000
# 또는
npx http-server
```

### 프로덕션 배포
- CDN을 통한 에셋 제공
- Gzip 압축 활용
- HTTPS 필수 (Unity WebGL 요구사항)

## 트러블슈팅

### 일반적인 문제들
1. **Unity 로딩 실패**: 빌드 파일 경로 및 이름 확인
2. **통신 오류**: 브라우저 콘솔에서 JavaScript 에러 확인
3. **3D 모델 표시 안됨**: Resources 폴더 구조 및 파일명 확인
4. **카메라 조작 안됨**: Unity 캔버스 이벤트 리스너 확인

### 디버깅 팁
- Unity 에디터에서 먼저 테스트
- 브라우저 개발자 도구 활용
- Unity Development Build로 빌드하여 디버그 정보 확인