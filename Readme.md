# <img src="./images/main.png" alt="ISECLOTH 로고" width="50">이세옷 (이세계옷장)

<div align="center">
  <img src="./images/1.png" alt="ISECLOTH 메인 화면" width="800">
</div>

## Contents

- 📖 Introduction
- ✨ Features
- 🖼️ Preview
- 🛠️ Tech Stack
- 🏗️ Structure
- 📄 License

## 📖 Introduction
이세옷은 사용자의 퍼스널 체형을 기반으로 한 3D 가상 피팅 서비스입니다. 자신의 신체 사이즈에 맞춰 생성된 아바타를 통해, 실제 옷을 입어보지 않고도 다양한 옷을 가상으로 피팅해볼 수 있는 새로운 경험을 제공합니다.

## ✨ Features

### • 퍼스널 체형 기반 아바타 생성
1. **전신 사진 업로드**: 한 장의 전신 사진 입력  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
2. **SMPL 모델 추정**: AI 기반으로 정확한 체형 분석  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
3. **UMA 파라미터 변환**: 체형과 맞는 3D 아바타 생성  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
4. **실시간 렌더링**: 즉시 확인 가능한 나만의 아바타
<br>

### • 3D 의류 생성 및 가상 피팅
1. **의류 사진 분석**: 앞/뒤 사진으로 의류의 모든 디테일 캡처  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
2. **자동 배경 제거**: U-2-Net 기반 정확한 의류 분리  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
3. **텍스처 생성**: Cloth2Tex로 현실적인 의류 텍스처 구현  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️
4. **실시간 피팅**: 아바타에 즉시 적용되는 가상 착용 경험
<br>

### • 인터랙티브 경험
- **다양한 배경**: 여러 환경에서 의상 확인
- **동적 모션**: 실제 움직임을 시뮬레이션한 아바타 애니메이션
- **360도 뷰**: 모든 각도에서 의상 확인 가능


## 🖼️ Preview

<div align="center">

### MY CLOSET 화면  
<img src="./images/2.png" alt="MY CLOSET 화면" width="700">

### 기본 옷장 화면
<img src="./images/3.png" alt="기본 옷장 화면" width="700">

### 아바타 피팅 데모
<img src="./images/4.png" alt="다른 배경에서 상하의를 입고 뛰는 사진" width="700">

</div>

## 🛠️ Tech Stack

### 🖥️ Frontend
<div>
  <img src="https://img.shields.io/badge/React-61DAFB?style=for-the-badge&logo=react&logoColor=black">
  <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black">
</div>

### ⚙️ Backend
<div>
  <img src="https://img.shields.io/badge/Node.js-339933?style=for-the-badge&logo=node.js&logoColor=white">
  <img src="https://img.shields.io/badge/Express-000000?style=for-the-badge&logo=express&logoColor=white">
  <img src="https://img.shields.io/badge/FastAPI-009688?style=for-the-badge&logo=fastapi&logoColor=white">
</div>

### 🗄️ Database
<div>
  <img src="https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white">
</div>

### 🤖 AI/ML
<div>
  <img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white">
  <img src="https://img.shields.io/badge/PyTorch-EE4C2C?style=for-the-badge&logo=pytorch&logoColor=white">
</div>

### 🎮 3D Graphics
<div>
  <img src="https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white">
  <img src="https://img.shields.io/badge/Blender-F5792A?style=for-the-badge&logo=blender&logoColor=white">
  <img src="https://img.shields.io/badge/Autodesk Maya-37A5CC?style=for-the-badge&logo=autodesk&logoColor=white">
</div>

### 🤝 Collaboration
<div>
  <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white">
  <img src="https://img.shields.io/badge/Notion-000000?style=for-the-badge&logo=notion&logoColor=white">
</div>

### 🚀 Deployment
<div>
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white">
  <img src="https://img.shields.io/badge/AWS-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white">
</div>

## 🏗️ Structure

### 🔧 프로젝트 아키텍처

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│    Frontend     │    │     Backend      │    │    AI Server    │
│     (React)     │◄──►│ (Node.js+Express)│◄──►│    (FastAPI)    │
│                 │    │                  │    │                 │
│ • My Page       │    │ • Outfit API     │    │ • SMPLify-X     │
│ • My Closet     │    │ • Mannequin API  │    │ • U-2-Net       │
│ • Fiting Room   │    └──────────────────┘    │ • Cloth2Tex     │
└─────────────────┘              │             │ • PyMAF-X       │
                                 │             └─────────────────┘
                                 │           
                  ┌──────────────────────────────┐
         ┌─────────────────┐            ┌─────────────────┐              
         │      Unity      │            │    MongoDB      │              
         │     (WebGL)     │            │                 │
         │                 │            │ • User Data     │               
         │ • OutfitLoader  │            │ • Avatar Data   │              
         │ • Avatar Loader │            │ • Cloth Data    │              
         └─────────────────┘            └─────────────────┘
```

### 📊 데이터 플로우


#### 아바타 생성
```
User Input (전신사진) 
            ↓
AI Processing (SMPL 추정)
            ↓
UMA DNA Converter (SMPL 베타값 ─► UMA DNA 파라미터)
            ↓  
Avatar Generation (UMA 파라미터 적용)
```

#### 옷 텍스처 생성
```
Cloth Input (옷 앞/뒤 사진)
                ↓
Image Processing (rembg[배경제거], Landmark detection[특징점 검출], U-2-Net[마스킹])
                ↓
Texture Generation (Cloth2Tex)
                ↓
3D Virtual Fitting (템플릿 메쉬에 텍스처 적용)
```

## 📄 License

이 프로젝트는 여러 오픈소스 AI 모델을 사용하며, 각각 다른 라이선스가 적용됩니다.

⚠️ **중요**: 일부 AI 모델(특히 SMPLify-X)은 **비상업적 연구 목적으로만** 사용 가능합니다.

상세한 라이선스 정보는 [LICENSE](./LICENSE) 파일을 참조하세요.

**상업적 사용 시 주의사항:**
- SMPLify-X: 별도 상업 라이선스 필요 (sales@meshcapade.com)
- 기타 제한 사항은 LICENSE 파일 참조

본 프로젝트의 오리지널 코드는 **MIT License** 하에 배포됩니다.