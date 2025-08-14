# Web Clothing Viewer

웹에서 옷 리스트를 보여주고, Unity WebGL로 3D 옷 모델을 표시하는 독립적인 시스템입니다.

## 프로젝트 구조

```
WebClothingViewer/
├── UnityProject/           # Unity WebGL 프로젝트 (옷 3D 모델 표시용)
├── WebInterface/          # 웹 UI (옷 리스트 + WebGL 임베드)
├── Assets/               # 공통 에셋 (옷 이미지, 3D 모델 등)
├── Documentation/        # 문서
└── README.md
```

## 기능

- **웹 UI**: 옷 리스트 표시, 옷 선택 인터페이스
- **Unity WebGL**: 선택된 옷의 3D 모델 표시
- **웹-Unity 통신**: JavaScript ↔ Unity 메시지 시스템

## 기존 UMA_CLOSET 프로젝트와의 차이점

- 아바타 시스템 없음 (순수 옷 모델만 표시)
- UMA 시스템 없음 (간단한 3D 모델 시스템)
- 웹 중심의 UI/UX
- 독립적인 실행 환경