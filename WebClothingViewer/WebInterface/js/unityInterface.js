// Unity WebGL 인터페이스 관리
class UnityInterface {
    constructor() {
        this.unityInstance = null;
        this.isUnityReady = false;
        this.messageQueue = [];
    }

    // Unity WebGL 초기화
    async initializeUnity() {
        try {
            console.log("Unity WebGL 초기화 시작...");
            
            // Unity 로더 설정
            const config = {
                dataUrl: "Build/WebClothingViewer.data",
                frameworkUrl: "Build/WebClothingViewer.framework.js",
                codeUrl: "Build/WebClothingViewer.wasm",
                streamingAssetsUrl: "StreamingAssets",
                companyName: "YourCompany",
                productName: "WebClothingViewer",
                productVersion: "1.0",
            };

            // Unity 인스턴스 생성
            this.unityInstance = await createUnityInstance(
                document.querySelector("#unity-canvas"), 
                config, 
                (progress) => {
                    this.updateLoadingProgress(progress * 100);
                }
            );

            // 로딩 완료 처리
            this.onUnityLoaded();
            
        } catch (error) {
            console.error("Unity 초기화 실패:", error);
            this.showError("Unity WebGL 로딩에 실패했습니다.");
        }
    }

    // Unity 로딩 완료 처리
    onUnityLoaded() {
        console.log("Unity WebGL 로딩 완료");
        
        // 로딩 화면 숨기기
        document.getElementById('unity-loading').style.display = 'none';
        document.getElementById('unity-canvas').style.display = 'block';
        
        // Unity 준비 완료 대기
        this.waitForUnityReady();
    }

    // Unity 준비 완료 대기
    waitForUnityReady() {
        const checkReady = () => {
            if (this.isUnityReady) {
                this.processMessageQueue();
                return;
            }
            setTimeout(checkReady, 100);
        };
        checkReady();
    }

    // 로딩 진행률 업데이트
    updateLoadingProgress(progress) {
        console.log(`로딩 진행률: ${progress.toFixed(1)}%`);
        
        const loadingText = document.querySelector('#unity-loading p');
        if (loadingText) {
            loadingText.textContent = `Unity WebGL 로딩 중... ${progress.toFixed(0)}%`;
        }
    }

    // 에러 표시
    showError(message) {
        const loadingDiv = document.getElementById('unity-loading');
        loadingDiv.innerHTML = `
            <div style="color: #ff4444;">
                <h3>오류 발생</h3>
                <p>${message}</p>
                <button onclick="location.reload()">새로고침</button>
            </div>
        `;
    }

    // Unity에 메시지 전송
    sendToUnity(gameObjectName, methodName, parameter = "") {
        if (!this.unityInstance) {
            console.warn("Unity 인스턴스가 없습니다. 메시지를 큐에 추가합니다.");
            this.messageQueue.push({ gameObjectName, methodName, parameter });
            return;
        }

        if (!this.isUnityReady) {
            console.warn("Unity가 준비되지 않았습니다. 메시지를 큐에 추가합니다.");
            this.messageQueue.push({ gameObjectName, methodName, parameter });
            return;
        }

        try {
            this.unityInstance.SendMessage(gameObjectName, methodName, parameter);
            console.log(`Unity 메시지 전송: ${gameObjectName}.${methodName}(${parameter})`);
        } catch (error) {
            console.error("Unity 메시지 전송 실패:", error);
        }
    }

    // 큐에 있는 메시지들 처리
    processMessageQueue() {
        console.log(`큐에 있는 메시지 ${this.messageQueue.length}개 처리 중...`);
        
        while (this.messageQueue.length > 0) {
            const message = this.messageQueue.shift();
            this.sendToUnity(message.gameObjectName, message.methodName, message.parameter);
        }
    }

    // 옷 변경 요청
    changeClothing(clothingId) {
        console.log(`옷 변경 요청: ${clothingId}`);
        this.sendToUnity("WebGLCommunicator", "ChangeClothingFromWeb", clothingId);
    }

    // 모델 숨기기
    hideModel() {
        this.sendToUnity("WebGLCommunicator", "HideModelFromWeb");
    }

    // 모델 보이기
    showModel() {
        this.sendToUnity("WebGLCommunicator", "ShowModelFromWeb");
    }

    // 현재 옷 정보 요청
    getCurrentClothingInfo() {
        this.sendToUnity("WebGLCommunicator", "GetCurrentClothingInfo");
    }

    // 카메라 회전
    rotateCamera(x, y) {
        this.sendToUnity("WebGLCommunicator", "RotateCamera", `${x},${y}`);
    }

    // 카메라 줌
    zoomCamera(delta) {
        this.sendToUnity("WebGLCommunicator", "ZoomCamera", delta.toString());
    }
}

// Unity에서 호출할 수 있는 전역 함수들
window.SendMessageToWeb = function(message) {
    console.log("Unity에서 메시지 수신:", message);
    
    if (message === "unityReady") {
        unityInterface.isUnityReady = true;
        console.log("Unity 준비 완료!");
        
        // UI 업데이트
        document.getElementById('currentClothingInfo').textContent = "Unity 준비 완료 - 옷을 선택해주세요.";
        
    } else if (message.startsWith("clothingChanged:")) {
        const clothingId = message.split(":")[1];
        onClothingChanged(clothingId);
        
    } else if (message.startsWith("currentClothingInfo:")) {
        const info = message.split(":")[1];
        document.getElementById('currentClothingInfo').textContent = info;
        
    } else if (message.startsWith("error:")) {
        const error = message.split(":")[1];
        console.error("Unity 에러:", error);
        
    } else if (message === "modelHidden") {
        console.log("모델이 숨겨졌습니다.");
        
    } else if (message === "modelShown") {
        console.log("모델이 표시되었습니다.");
    }
};

// 옷 변경 완료 처리
function onClothingChanged(clothingId) {
    console.log(`옷 변경 완료: ${clothingId}`);
    
    // 선택된 옷 정보 업데이트
    const clothing = getClothingById(clothingId);
    if (clothing) {
        document.getElementById('currentClothingInfo').textContent = 
            `현재 선택: ${clothing.name} (${clothing.type})`;
    }
    
    // UI에서 선택된 아이템 하이라이트
    updateSelectedClothing(clothingId);
}

// Unity 인터페이스 인스턴스 생성
const unityInterface = new UnityInterface();