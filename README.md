# SolidWorks 螺纹底孔小径与尺寸联动助手 (ThreadTapAssistant)

> **工业级机械螺纹标准计算与草图尺寸秒改 SolidWorks 原生插件**  
> 全球11国标准 · 纯选择防呆 · A~ZC / IT01~IT18 全公差引擎 · 三色工程分级

[![GitHub release](https://img.shields.io/github/v/release/chenyeluo/SolidWorks-ThreadTapAssistant?color=1890FF&label=%E6%9C%80%E6%96%B0%E7%89%88%E6%9C%AC)](https://github.com/chenyeluo/SolidWorks-ThreadTapAssistant/releases/latest)
[![Download](https://img.shields.io/badge/%E4%B8%80%E9%94%AE%E5%AE%89%E8%A3%85%E5%8C%85-Setup.exe-brightgreen)](https://github.com/chenyeluo/SolidWorks-ThreadTapAssistant/releases/download/v1.0.0/SolidWorks_ThreadTapAssistant_Setup.exe)

👉 **[点击直接下载单文件安装程序 (SolidWorks_ThreadTapAssistant_Setup.exe)](https://github.com/chenyeluo/SolidWorks-ThreadTapAssistant/releases/download/v1.0.0/SolidWorks_ThreadTapAssistant_Setup.exe)**

---

## 🌟 核心特性与功能升级

### 1. 纯下拉选择化（100% 拒绝手动打字）
- 彻底取消了手动输入文本框，所有规格整合为公称尺寸与粗细牙系列的清晰下拉项；
- 点击即选，避免拼写错误或输入格式不规范。

### 2. 公差等级三色工程分级（OwnerDraw 彩色指示）
遵循机械设计工程标准（GB/T 197 / ISO 965 / ISO 286）：
- 🟢 **绿色圆点【工程首选推荐】**：置顶推荐，车间最常用、钻头最齐备（如内螺纹 6H, 2B, Class A, H7 等）；
- 🟡 **黄色圆点【次选公差】**：次推荐选用公差带（如 7H, 6G, 5H, 3B, G7, F7, JS7 等）；
- 🟠 **橙色圆点【不建议/迫不得已】**：严格受控的特殊或极端公差带（如 8H, 4H, 1B, 过盈/紧配合偏差等）。

### 3. 全球标准极限配合与公差引擎（覆盖 A ~ ZC 与 IT01 ~ IT18）
内置符合 **ISO 286 / GB/T 1800.1 / GB/T 197** 的解析计算引擎：
- **28 种基本偏差代号 (A 到 ZC 全量支持)**：  
  `A, B, C, CD, D, E, EF, F, FG, G, H, J, JS, K, M, N, P, R, S, T, U, V, X, Y, Z, ZA, ZB, ZC`
- **20 种国家标准公差等级 (IT01 到 IT18 全量支持)**：  
  `IT01, IT0, IT1, IT2, IT3, IT4, IT5, IT6, IT7, IT8, IT9, IT10, IT11, IT12, IT13, IT14, IT15, IT16, IT17, IT18`
- 任意自由组合（如 H7, G6, P8, JS7 等），即时计算**上偏差 $ES$、下偏差 $EI$、理论小径范围 $D_{1\min} \sim D_{1\max}$ 及车间推荐钻头底孔尺寸**！

### 4. 覆盖 11 大国际国家标准协议
- **GB** (中国国家标准)
- **ISO** (国际标准化组织)
- **DIN** (德国工业标准)
- **ANSI Metric** (美标公制)
- **ANSI Inch** (美标英制)
- **BSI** (英国标准学会)
- **JIS** (日本工业标准)
- **KS** (韩国工业标准)
- **AS** (澳大利亚标准)
- **IS** (印度工业标准)

### 5. 覆盖三大螺纹大类体系
- **普通螺纹 (Fastening)**：公制普通粗牙/细牙 M 系列、美制统一粗牙 UNC、细牙 UNF、英制惠氏 BSW/BSF。
- **管螺纹 (Piping)**：55° 非密封圆柱管螺纹 G (BSPP/PF)、55° 密封管螺纹 Rc/Rp/R (BSPT/PT)、60° 美制锥管螺纹 NPT。
- **传动螺纹 (Transmission)**：30° 公制梯形螺纹 Tr、29° 美制爱克姆梯形螺纹 ACME、锯齿形螺纹 S/Buttress、矩形方螺纹 Sq。

---

## 🚀 安装与使用

1. 打开发行目录：`c:\DownLoad\Application_Design\SW_putin_design\ThreadTapAssistant\bin\Release\`或直接下载SolidWorks_ThreadTapAssistant_Setup.exe文件；
2. 退出 SolidWorks，运行 **`Setup.exe`**或**`SolidWorks_ThreadTapAssistant_Setup.exe`**;
3. 打开 SolidWorks，点击右侧任务窗格图标展开使用！
