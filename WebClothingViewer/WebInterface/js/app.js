// 메인 애플리케이션 로직
class ClothingViewerApp {
    constructor() {
        this.currentCategory = 'all';
        this.currentSearchQuery = '';
        this.selectedClothingId = null;
        
        this.init();
    }

    // 앱 초기화
    async init() {
        console.log("Clothing Viewer App 초기화 시작");
        
        // 이벤트 리스너 설정
        this.setupEventListeners();
        
        // 초기 옷 리스트 렌더링
        this.renderClothingList();
        
        // Unity WebGL 초기화
        await unityInterface.initializeUnity();
        
        console.log("Clothing Viewer App 초기화 완료");
    }

    // 이벤트 리스너 설정
    setupEventListeners() {
        // 카테고리 탭 버튼들
        document.querySelectorAll('.tab-button').forEach(button => {
            button.addEventListener('click', (e) => {
                const category = e.target.dataset.category;
                this.selectCategory(category);
            });
        });

        // 검색 입력
        const searchInput = document.getElementById('searchInput');
        searchInput.addEventListener('input', (e) => {
            this.currentSearchQuery = e.target.value;
            this.renderClothingList();
        });

        // 뷰어 컨트롤 버튼들
        document.getElementById('resetViewBtn').addEventListener('click', () => {
            this.resetView();
        });

        document.getElementById('hideModelBtn').addEventListener('click', () => {
            unityInterface.hideModel();
        });

        document.getElementById('showModelBtn').addEventListener('click', () => {
            unityInterface.showModel();
        });

        // Unity 캔버스에서 마우스 이벤트 (카메라 컨트롤)
        this.setupCameraControls();
    }

    // 카메라 컨트롤 설정
    setupCameraControls() {
        const canvas = document.getElementById('unity-canvas');
        let isMouseDown = false;
        let lastMouseX = 0;
        let lastMouseY = 0;

        canvas.addEventListener('mousedown', (e) => {
            isMouseDown = true;
            lastMouseX = e.clientX;
            lastMouseY = e.clientY;
        });

        canvas.addEventListener('mousemove', (e) => {
            if (!isMouseDown) return;

            const deltaX = e.clientX - lastMouseX;
            const deltaY = e.clientY - lastMouseY;

            // 카메라 회전 (감도 조절)
            unityInterface.rotateCamera(deltaX * 0.5, -deltaY * 0.5);

            lastMouseX = e.clientX;
            lastMouseY = e.clientY;
        });

        canvas.addEventListener('mouseup', () => {
            isMouseDown = false;
        });

        canvas.addEventListener('mouseleave', () => {
            isMouseDown = false;
        });

        // 마우스 휠로 줌
        canvas.addEventListener('wheel', (e) => {
            e.preventDefault();
            const delta = e.deltaY > 0 ? 0.5 : -0.5;
            unityInterface.zoomCamera(delta);
        });
    }

    // 카테고리 선택
    selectCategory(category) {
        this.currentCategory = category;
        
        // 탭 버튼 스타일 업데이트
        document.querySelectorAll('.tab-button').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-category="${category}"]`).classList.add('active');
        
        // 옷 리스트 재렌더링
        this.renderClothingList();
    }

    // 옷 리스트 렌더링
    renderClothingList() {
        const container = document.getElementById('clothesList');
        const items = searchClothing(this.currentSearchQuery, this.currentCategory);
        
        container.innerHTML = '';
        
        if (items.length === 0) {
            container.innerHTML = '<div class="no-items">검색 결과가 없습니다.</div>';
            return;
        }

        items.forEach(item => {
            const itemElement = this.createClothingItemElement(item);
            container.appendChild(itemElement);
        });
    }

    // 옷 아이템 요소 생성
    createClothingItemElement(item) {
        const div = document.createElement('div');
        div.className = 'clothing-item';
        div.dataset.clothingId = item.id;
        
        // 선택된 아이템인지 확인
        if (this.selectedClothingId === item.id) {
            div.classList.add('selected');
        }

        div.innerHTML = `
            <img src="${getImageUrl(item)}" alt="${item.name}" 
                 onerror="this.src='${getImageUrl(item)}'">
            <div class="clothing-info">
                <h3>${item.name}</h3>
                <p>${item.description}</p>
                <p class="price">₩${item.price.toLocaleString()}</p>
                <p class="colors">색상: ${item.colors.join(', ')}</p>
            </div>
        `;

        // 클릭 이벤트
        div.addEventListener('click', () => {
            this.selectClothing(item.id);
        });

        return div;
    }

    // 옷 선택
    selectClothing(clothingId) {
        console.log(`옷 선택: ${clothingId}`);
        
        this.selectedClothingId = clothingId;
        
        // UI 업데이트
        this.updateSelectedClothing(clothingId);
        
        // Unity에 옷 변경 요청
        unityInterface.changeClothing(clothingId);
    }

    // 선택된 옷 UI 업데이트
    updateSelectedClothing(clothingId) {
        // 모든 아이템에서 selected 클래스 제거
        document.querySelectorAll('.clothing-item').forEach(item => {
            item.classList.remove('selected');
        });

        // 선택된 아이템에 selected 클래스 추가
        const selectedItem = document.querySelector(`[data-clothing-id="${clothingId}"]`);
        if (selectedItem) {
            selectedItem.classList.add('selected');
        }
    }

    // 뷰 리셋
    resetView() {
        console.log("뷰 리셋");
        // Unity 카메라를 기본 위치로 리셋하는 기능은 Unity 스크립트에서 구현 필요
        unityInterface.sendToUnity("WebGLCommunicator", "ResetCamera");
    }

    // 현재 선택된 옷 정보 가져오기
    getCurrentSelection() {
        if (this.selectedClothingId) {
            return getClothingById(this.selectedClothingId);
        }
        return null;
    }
}

// 전역 함수들 (Unity에서 호출 가능)
function updateSelectedClothing(clothingId) {
    if (window.clothingApp) {
        window.clothingApp.updateSelectedClothing(clothingId);
    }
}

// DOM 로드 완료 후 앱 시작
document.addEventListener('DOMContentLoaded', () => {
    console.log("DOM 로드 완료, 앱 시작");
    window.clothingApp = new ClothingViewerApp();
});

// 페이지 언로드 시 정리
window.addEventListener('beforeunload', () => {
    console.log("페이지 언로드");
    // 필요한 정리 작업
});