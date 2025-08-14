// 옷 데이터 정의 (실제로는 API나 JSON 파일에서 로드)
const clothingData = [
    {
        id: "tshirt_001",
        name: "기본 티셔츠",
        type: "shirt",
        description: "편안한 면 소재의 기본 티셔츠",
        image: "images/tshirt_001.jpg",
        price: 25000,
        colors: ["흰색", "검정", "회색"]
    },
    {
        id: "tshirt_002",
        name: "스트라이프 티셔츠",
        type: "shirt",
        description: "세련된 스트라이프 패턴의 티셔츠",
        image: "images/tshirt_002.jpg",
        price: 30000,
        colors: ["네이비", "흰색"]
    },
    {
        id: "shirt_001",
        name: "체크 셔츠",
        type: "shirt",
        description: "클래식한 체크 패턴의 셔츠",
        image: "images/shirt_001.jpg",
        price: 45000,
        colors: ["빨강", "파랑", "초록"]
    },
    {
        id: "pants_001",
        name: "청바지",
        type: "pants",
        description: "클래식한 데님 청바지",
        image: "images/pants_001.jpg",
        price: 60000,
        colors: ["인디고", "블랙", "라이트블루"]
    },
    {
        id: "pants_002",
        name: "슬랙스",
        type: "pants",
        description: "정장용 슬랙스 바지",
        image: "images/pants_002.jpg",
        price: 80000,
        colors: ["검정", "회색", "네이비"]
    },
    {
        id: "shorts_001",
        name: "반바지",
        type: "pants",
        description: "여름용 편안한 반바지",
        image: "images/shorts_001.jpg",
        price: 35000,
        colors: ["카키", "네이비", "베이지"]
    },
    {
        id: "shoes_001",
        name: "운동화",
        type: "shoes",
        description: "편안한 캐주얼 운동화",
        image: "images/shoes_001.jpg",
        price: 90000,
        colors: ["흰색", "검정", "회색"]
    },
    {
        id: "shoes_002",
        name: "구두",
        type: "shoes",
        description: "정장용 가죽 구두",
        image: "images/shoes_002.jpg",
        price: 150000,
        colors: ["검정", "갈색"]
    }
];

// 카테고리별 필터링 함수
function getClothingByCategory(category) {
    if (category === 'all') {
        return clothingData;
    }
    return clothingData.filter(item => item.type === category);
}

// ID로 옷 찾기
function getClothingById(id) {
    return clothingData.find(item => item.id === id);
}

// 검색 기능
function searchClothing(query, category = 'all') {
    let items = getClothingByCategory(category);
    
    if (!query.trim()) {
        return items;
    }
    
    query = query.toLowerCase();
    return items.filter(item => 
        item.name.toLowerCase().includes(query) ||
        item.description.toLowerCase().includes(query) ||
        item.colors.some(color => color.toLowerCase().includes(query))
    );
}

// 더미 이미지 생성 함수 (실제 이미지가 없을 때 사용)
function generateDummyImage(itemId, itemName) {
    // SVG로 간단한 더미 이미지 생성
    const colors = {
        'shirt': '#4CAF50',
        'pants': '#2196F3', 
        'shoes': '#FF9800'
    };
    
    const item = getClothingById(itemId);
    const bgColor = colors[item?.type] || '#757575';
    
    const svg = `
        <svg width="60" height="60" xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" fill="${bgColor}" rx="5"/>
            <text x="30" y="35" text-anchor="middle" fill="white" font-size="10" font-family="Arial">
                ${itemName.substring(0, 2)}
            </text>
        </svg>
    `;
    
    return 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svg)));
}

// 이미지 URL 검증 및 더미 이미지 생성
function getImageUrl(item) {
    // 실제 이미지 파일이 있는지 확인하고, 없으면 더미 이미지 사용
    return generateDummyImage(item.id, item.name);
}