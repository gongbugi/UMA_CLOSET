# Unity WebGL 빌드 파일 위치

이 폴더에는 Unity에서 WebGL로 빌드한 파일들이 위치해야 합니다.

## 필요한 파일들

Unity WebGL 빌드 시 생성되는 파일들:
- `WebClothingViewer.data`
- `WebClothingViewer.framework.js`
- `WebClothingViewer.wasm`
- (선택사항) `WebClothingViewer.symbols.json`

## 빌드 방법

1. Unity에서 File → Build Settings
2. Platform을 WebGL로 변경
3. Build 버튼 클릭
4. 이 폴더 (`WebClothingViewer/WebInterface/Build/`)를 빌드 대상으로 선택
5. 빌드 완료 후 파일들이 이 위치에 생성됩니다

## 주의사항

- Unity에서 자동 생성되는 `index.html`은 사용하지 않습니다
- 대신 상위 폴더의 `index.html`을 사용합니다
- 빌드 파일들의 이름이 `unityInterface.js`의 설정과 일치해야 합니다