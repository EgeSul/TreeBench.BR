# TreeBench v4.0.0 - Enterprise Full-Stack Data Structures Performance Lab

![License](https://img.shields.io/badge/License-MIT-yellow.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)
![Vue.js](https://img.shields.io/badge/Vue.js-3.x-brightgreen.svg)
![SignalR](https://img.shields.io/badge/SignalR-Realtime-blueviolet.svg)
![xUnit](https://img.shields.io/badge/Testing-xUnit%20%2F%20Moq-informational.svg)
![Docker](https://img.shields.io/badge/Docker-Compose-blue.svg)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red.svg)
![Architecture](https://img.shields.io/badge/Architecture-Enterprise%20Microservices-orange.svg)

---

TreeBench is an advanced, enterprise-grade benchmarking laboratory designed to analyze, profile, and contrast self-balancing binary search trees, multi-way indexes, and spatial data structures (AVL Tree, Red-Black Tree, Splay Tree, B+ Tree, and Quadtree).

Transitioning from a console-based profiler to a **Fully Dockerized Full-Stack Application with Real-Time Capabilities**, TreeBench now features a robust ASP.NET Core 10 Web API backend empowered by **SignalR WebSockets** for live progress tracking, an isolated SQL Server 2022 container, a comprehensive **xUnit & Moq automated testing suite**, and a highly responsive cyberpunk-themed Vue 3 (Vite) Single Page Application (SPA).

The architecture streams 100,000 live records from the SQL Server directly into volatile C# memory via low-level Dapper pipelines, pushing telemetry and execution progress in real-time to a rich graphical dashboard powered by ApexCharts. If the database engine is empty or offline, a robust **In-Memory Fallback Mechanism** seamlessly initializes a mock production dataset to ensure zero telemetry distortion.

---

## 🛠️ Tech Stack & Packages Used

### Backend & API (.NET 10)
* **ASP.NET Core Web API**: Main RESTful framework.
* **SignalR**: Real-time bidirectional communication hub for live telemetry streaming.
* **Dapper**: High-performance Micro-ORM for raw SQL execution.
* **Microsoft.Data.SqlClient**: Modern, cross-platform SQL Server driver (Linux/Docker compatible).
* **Serilog**: Structured diagnostic logging.
* **Swashbuckle.AspNetCore (Swagger)**: API documentation and endpoint testing.

### Testing Suite
* **xUnit**: Industry-standard unit testing framework for .NET.
* **Moq**: Popular mocking library for isolated component validation.

### Frontend & UI (Vue 3)
* **Vue 3 (Composition API)**: Core frontend framework.
* **Vite**: Next-generation frontend tooling and bundler.
* **Microsoft SignalR Client (`@microsoft/signalr`)**: Real-time websocket subscription layer.
* **ApexCharts (vue3-apexcharts)**: Interactive and responsive telemetry visualization.
* **FontAwesome**: Scalable vector icons.

### DevOps & Infrastructure
* **Docker**: Containerization of the ecosystem.
* **Docker Compose**: Multi-container orchestration (`db`, `api`, `ui`).
* **Nginx (Alpine)**: Lightweight, high-performance web server serving the Vue SPA.

---

## 📂 Enterprise Directory Structure

Below is the updated layout of the solution, exhibiting a clean Separation of Concerns (SoC) across the full-stack dockerized environment:

```text
📂 TreeBench/
│
├── 🐳 docker-compose.yml            # Multi-container orchestration (SQL, API, Nginx)
├── 📂 TreeBench.API/                # ASP.NET Core Web API Presentation Layer
│   ├── 🐳 Dockerfile                # .NET 10 Build & Runtime definitions
│   ├── Controllers/                 # RESTful Endpoints (BenchmarkController)
│   ├── Hubs/                        # Real-time SignalR Hubs (BenchmarkHub.cs)
│   └── Program.cs                   # IoC Registry, CORS, SignalR & Serilog Bootstrapper
│
├── 📂 TreeBench.UI/                 # Enterprise Vue 3 Frontend (Vite)
│   ├── 🐳 Dockerfile                # Node.js Build -> Nginx Alpine Serve
│   ├── 📂 src/
│   │   ├── App.vue                  # Main Dashboard (SignalR Client, Dark/Light Mode, i18n)
│   │   └── main.js                  # Vue Application & ApexCharts Initialization
│   └── vite.config.js               # Vite Proxy Configuration
│
├── 📂 TreeBench.DB/                 # Database Assets
│   └── T-SQL/TreeBenchDB.sql        # SQL Script for table initialization
│
├── 📂 TreeBench.Tests/              # Automated Unit Testing Suite
│   └── UnitTest1.cs                 # xUnit & Moq validation pipelines
│
└── 📂 TreeBench.BS/                 # Business Logic & Infrastructure
    ├── 📂 Interfaces/               # Domain abstraction contracts (IBalancedTree.cs)
    ├── 📂 Models/                   # Abstract Templates & Concrete Tree Implementations
    └── 📂 Services/                 # Telemetry Profiler & Dapper Data Ingestion
```

🏗️ Docker Architecture & Real-Time Data Flow

To maintain a production-grade ecosystem, TreeBench implements strict modularity. The entire system is spun up using Docker Compose, creating an isolated internal bridge network (treebench-network).


```

graph TD
    subgraph Browser ["Client Browser"]
        User((User))
    end

    subgraph DockerNetwork ["Docker Bridge Network (treebench-network)"]
        subgraph UIContainer ["UI Container (Port 5173)"]
            Nginx[Nginx Alpine Server]
            Vue[Vue 3 Static Build + SignalR Client]
        end

        subgraph APIContainer ["API Container (Port 5174:5174)"]
            NET[.NET 10 Kestrel Server]
            Hub[SignalR BenchmarkHub]
            Dapper[Dapper ORM]
        end

        subgraph DBContainer ["DB Container (Port 1433)"]
            SQL[(MS SQL Server 2022)]
        end
    end

    User -->|http://localhost:5173| Nginx
    Nginx --> Vue
    Vue -->|HTTP POST /api/Benchmark/run| NET
    Vue -.->|WebSocket /benchmarkHub| Hub
    NET -->|SQL Queries| Dapper
    Dapper -->|TCP/IP| SQL
    Hub -.->|Live Progress Broadcast| Vue

    
```

🌳 Models Tree Update (Template Method Refactoring)

In version 2.0+, the tree architecture underwent a massive refactoring process. Instead of individual trees handling their own edge cases, execution timers, and validation logic, the system enforces the Template Method Design Pattern via an abstract BaseBalancedTree class.

This guarantees that all performance metrics (Stopwatch operations) and null-reference safety checks are uniformly executed before reaching the specific algorithmic behaviors (InsertInternal, SearchInternal, DeleteInternal) of the concrete tree implementations.

Algorithmic Execution Pipeline
    
```
graph TD
    subgraph ClientRequest ["Client / Profiler Service"]
        Req[Initiate Tree Operation <br> Insert / Search / Delete]
    end

    subgraph BaseClass ["BaseBalancedTree.cs (Abstract Engine)"]
        TimerStart((Start High-Res <br> Stopwatch))
        Validation{Edge Case & <br> Null Pointer Check}
        TimerStop((Stop Stopwatch & <br> Capture Telemetry))
    end

    subgraph ConcreteTrees ["Concrete Algorithmic Implementations"]
        AVL[AvlTree <br> Execute Internal Logic]
        RBT[RedBlackTree <br> Execute Internal Logic]
        Splay[SplayTree <br> Execute Internal Logic]
        BPlus[BPlusTree <br> Execute Internal Logic]
        Quad[QuadTree <br> Execute Internal Logic]
    end

    Req --> TimerStart
    TimerStart --> Validation
    
    Validation -->|Pass| AVL
    Validation -->|Pass| RBT
    Validation -->|Pass| Splay
    Validation -->|Pass| BPlus
    Validation -->|Pass| Quad
    
    Validation -->|Fail: Empty Tree| TimerStop

    AVL --> TimerStop
    RBT --> TimerStop
    Splay --> TimerStop
    BPlus --> TimerStop
    Quad --> TimerStop

    TimerStop --> Result[Return Standardized <br> BenchmarkResultModel]
        
```

🚀 Installation & Getting Started (Docker Mode)

Forget manual dependency setups. You only need Docker Desktop installed on your machine.
1. Fire up the Ecosystem

Open a terminal in the root directory (where docker-compose.yml is located) and run:

    docker compose up -d --build

This command will pull the SQL Server, build the .NET 10 API, compile the Vue 3 application, and wire them all together in an isolated network.
2. Seed the Database

Connect to the Dockerized SQL Server via SQL Server Management Studio (SSMS) or Azure Data Studio:

    Server Name: localhost,1433

    Authentication: SQL Server Authentication

    Login: sa

    Password: TreeBench_StrongPass123!

Open a New Query and run the provided SQL script to seed 100,000 records:

SQL

    CREATE DATABASE TreeBenchDB;
    GO
    USE TreeBenchDB;
    GO
    CREATE TABLE TestNumbers (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Value INT NOT NULL,
        DataType VARCHAR(20) NOT NULL
    );
    GO
    SET NOCOUNT ON;
    DECLARE @i INT = 1;
    WHILE @i <= 100000
    BEGIN
        INSERT INTO TestNumbers (Value, DataType) VALUES (CAST(RAND() * 1000000 AS INT), 'Production');
        SET @i = @i + 1;
    END;
    GO

(Note: If you skip this step, the API's Fault Tolerance mechanism will auto-generate an in-memory dataset without crashing!)
3. Launch the Dashboard

Open your browser and navigate to:

👉 http://localhost:5173

Click the "Run Benchmark" button to watch the real-time SignalR progress bar stream telemetry and render the comprehensive ApexCharts performance analytics.
🗺️ Development Roadmap

    [x] v1.0.0 - AVL & Red-Black Tree benchmarking with advanced memory profiling.

    [x] v1.5.0 - .NET Dependency Injection & Dapper micro-ORM integration.

    [x] v2.0.0 - Abstract Template Engine refactoring, Fallback architecture, Serilog structure.

    [x] v2.5.0 - ASP.NET Web API integration, Vue 3 SPA frontend with ApexCharts, CORS tunneling.

    [x] v3.0.0 - Complete Full-Stack Dockerization (Nginx, .NET 10, SQL Server 2022) with custom network mapping.

    [x] v4.0.0 - SignalR real-time telemetry streaming, xUnit & Moq automated testing suite, and advanced robust error handling.

📄 License & Architecture

This architecture is completely open-source and released under the MIT License. Designed and engineered for high-performance enterprise benchmarking analysis.