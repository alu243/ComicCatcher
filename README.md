# ComicCatcher

ComicCatcher 是一個基於 .NET (C#) 開發的漫畫追蹤、閱讀與下載解決方案。它支援多個漫畫來源，提供 Web 介面進行線上閱讀，並具備自動追蹤新章節的功能。

## 🌟 功能特色 (Features)

*   **多來源支援**: 支援 DM5 (動漫屋)、Seemh、Xindm 等漫畫網站。
*   **Web 閱讀介面**: 透過 `ComicApi` 提供網頁版介面，可直接瀏覽與閱讀漫畫。
*   **自動追蹤更新**: 內建 Quartz.NET 排程系統，定時檢查收藏漫畫的更新狀態。
*   **桌面客戶端**: 提供 Windows Forms (`ComicCatcher`) 應用程式，方便本機管理與下載。
*   **資料持久化**: 使用 SQLite 輕量級資料庫儲存收藏與下載紀錄。
*   **Docker 支援**: 支援容器化部署，方便安裝於 NAS 或伺服器。

## 📂 專案結構 (Project Structure)

本方案包含以下主要專案：

| 專案名稱 | 類型 | 描述 |
|:--- |:--- |:--- |
| **ComicApi** | ASP.NET Core API | 核心後端服務，包含 Web 介面與背景排程 (Quartz)。 |
| **ComicCatcherLib** | Class Library | 核心邏輯庫，包含爬蟲實作 (Scrapers)、資料庫存取 (DAO) 與模型定義。 |
| **ComicCatcher** | Windows Forms | Windows 桌面版應用程式，提供圖形化操作介面。 |
| **ComicArchiver** | Console App | 漫畫封裝與壓縮工具。 |
| **GameRenamer** | Utility | 檔案重新命名輔助工具。 |

## 🚀 快速開始 (Getting Started)

### 使用 Docker 部署 (推薦)

專案內附有 `Dockerfile` 與建置腳本，可快速部署 API 服務。

1. **建置映像檔**
   ```bash
   # Windows 使用者可直接執行
   BuildDocker.bat
   
   # 或手動執行
   docker build . -t comic
   ```

2. **啟動容器**
   ```bash
   docker run -d -p 8080:80 --name comic-catcher comic
   ```
   啟動後即可透過瀏覽器訪問 `http://localhost:8080`。

### 本地開發 (Local Development)

**前置需求:**
*   Visual Studio 2022 (或更新版本)
*   .NET 6.0 SDK (或該專案指定的 .NET 版本)

**步驟:**
1. 使用 Visual Studio 開啟 `ComicCatcher.sln`。
2. 還原 NuGet 套件。
3. 若要執行 Web 版，將 `ComicApi` 設為起始專案並執行。
4. 若要執行桌面版，將 `ComicCatcher` 設為起始專案並執行。

## ⚙️ 配置 (Configuration)

主要設定位於 `appsettings.json` (Web) 或 `app.config` (Desktop)。

*   **排程設定**: 預設每小時檢查一次更新 (可在 `Program.cs` 中調整 Quartz 設定)。
*   **資料庫**: 系統會自動在執行目錄建立 SQLite 資料庫檔案 (`.db`)。

## 🛠️ 技術棧 (Tech Stack)

*   **Framework**: .NET / ASP.NET Core
*   **Database**: SQLite
*   **Scheduling**: Quartz.NET
*   **UI**: Razor Pages / Windows Forms
*   **Deployment**: Docker

## 📝 授權 (License)

[MIT License](LICENSE) (如有特定授權請自行修改)
