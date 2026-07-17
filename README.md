# TreeBench v2.0 - Advanced Data Structures Performance Lab

![License](https://img.shields.io/badge/License-MIT-yellow.svg)
![.NET Core](https://img.shields.io/badge/.NET%20Core-6.0%2B-purple.svg)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red.svg)
![Architecture](https://img.shields.io/badge/Architecture-Enterprise%20Layered-orange.svg)

---

TreeBench is an advanced, enterprise-grade benchmarking laboratory designed to analyze, profile, and contrast self-balancing binary search trees, multi-way indexes, and spatial data structures (**AVL Tree, Red-Black Tree, Splay Tree, B+ Tree, and Quadtree**). 

The architecture streams **100,000 live records from a Microsoft SQL Server** directly into volatile C# memory via low-level Dapper pipelines. If the database engine is offline, a robust **In-Memory Fallback Mechanism** seamlessly initializes a mock production dataset to ensure zero telemetry distortion during execution.

---

## 📂 Project Directory Structure

Below is the enterprise layout of the solution, exhibiting a clean Separation of Concerns (SoC) and Abstract Template patterns:

```text
📂 TreeBench/
│
├── 📂 TreeBench.DB/
│   └── 📂 T-SQL/
│       └── TreeBenchDB.sql          # SQL Script for database initialization
│
└── 📂 TreeBench.BS/
    ├── 📂 Interfaces/
    │   └── IBalancedTree.cs         # Domain abstraction contract (The Blueprint)
    │
    ├── 📂 Models/
    │   ├── BaseBalancedTree.cs      # Abstract Template Class for structural standardization
    │   ├── AvlTree.cs               # AVL Tree implementation (Height-Balanced)
    │   ├── RedBlackTree.cs          # Red-Black Tree implementation (Color-Balanced)
    │   ├── SplayTree.cs             # Splay Tree implementation (Locality Optimized)
    │   ├── BPlusTree.cs             # B+ Tree implementation (Multi-way Database Index)
    │   └── QuadTree.cs              # Quadtree implementation (Spatial 2D Coordinate Index)
    │
    ├── 📂 Services/
    │   ├── BenchmarkService.cs      # Telemetry Profiler Engine with Deletion Stress Testing
    │   └── DataGenerator.cs         # Micro-ORM Dapper Data Ingestion Streamer
    │
    ├── TreeBench.BS.csproj          # .NET Project Configuration File with NuGet Manifests
    └── Program.cs                   # IoC Container Registry, App Bootstrapper & Serilog Configuration
```

## 🏗️ Architectural Principles Applied

The project strictly follows SOLID design principles, combining Inversion of Control (IoC) and Template Method Patterns to isolate runtime pipelines.

```mermaid

graph TD

    subgraph Presentation & Framework Layer
        A[Program.cs - Bootstrapper]
        S[Serilog - Logging Pipeline]
        IoC[Microsoft DI Container]
    end
    
    subgraph Application Service Layer
        B[BenchmarkService.cs - Profiler]
        C[DataGenerator.cs - Dapper ORM]
    end
    
    subgraph Domain Abstraction Layer
        D[IBalancedTree.cs - Interface]
        Base[BaseBalancedTree.cs - Abstract Template]
    end
    
    subgraph Infrastructure/Model Layer
        E[AvlTree.cs]
        F[RedBlackTree.cs]
        G[SplayTree.cs]
        I[BPlusTree.cs]
        J[QuadTree.cs]
    end
    
    subgraph Data Source & Resilience Layer
        H[(SQL Server - TreeBenchDB)]
        FB[In-Memory Fallback Dataset]
    end

    A -->|Configures| S
    A -->|Builds| IoC
    IoC -->|Injects| B
    IoC -->|Injects| C
    C -->|Try/Catch Connect| H
    H -->|On Failure / Catch| FB
    B -->|Evaluates Performance| D
    D -->|Standardizes| Base
    Base -->|Inheritance & Contract Enforcement| E
    Base -->|Inheritance & Contract Enforcement| F
    Base -->|Inheritance & Contract Enforcement| G
    Base -->|Inheritance & Contract Enforcement| I
    Base -->|Inheritance & Contract Enforcement| J

```

* **Interface Segregation & Template Abstraction (BaseBalancedTree):** Enforces a rigid template method for search and deletion hooks, ensuring global edge-case checks (e.g., empty tree detection) are executed uniformly across all topologies before invoking concrete algorithmic engines (SearchInternal / DeleteInternal).
* **Dependency Injection & Loose Coupling:** All operational dependencies are registered inside an asynchronous service collection and resolved via Microsoft.Extensions.DependencyInjection, avoiding hardcoded allocations.
* **Graceful Degradation / Resilience:** Features an automatic runtime check during veritabanı ingestion. If a connection fault occurs, the pipeline catches the anomaly and populates a 100,000 record fallback dataset dynamically without delaying or prompting the main execution thread.

---

## 🔬 Monitored Metrics & Low-Level Profiling

The lab captures real-time telemetry backed by structural validation parameters:

* **Insert Duration:** Tracks the exact CPU clock cycles taken to construct and structure 100,000 entries using System.Diagnostics.Stopwatch.
* **Search Telemetry:** Runs 10,000 randomized lookups to compute index navigation speed.
* **Deletion Stress Testing:** Executes 5,000 concrete sequential removals to evaluate balancing and restructuring penalties.
* **Total Rotations & Structural Mutations:** Tracks balancing operations, color changes, page splits, and quadrant divisions.
* **Structured Logging & Telemetry Reporting:** Managed by Serilog; records telemetry indicators simultaneously to the console with precise formatting and a persistent filesystem sink (logs/treebench_perf.txt).

---

## 💻 Technical Implementations & Tree Mechanics
### 📊 Technical Flowcharts & Execution Vectors (Click to Expand)

### 1. AVL Tree (`AvlTree.cs`)

* Strict height-balancing regime where height differences cannot exceed 1.
* Employs reactive single and double rotations (**Left-Right / Right-Left Double Rotations**) immediately during recursive unwinding.


<details>
<summary><b>📐 1. AVL Tree (AvlTree.cs) - Balancing Logic</b></summary>

```mermaid

graph TD
    A[Start: InsertRec / Node, Key] --> B[Standard BST Insertion]
    B --> C[Update node.Height = 1 + Max Height]
    C --> D[Calculate balance = Getbalance node]
    D --> E{Evaluate balance Coefficient}

    

    E -->|balance > 1 AND key < node.Left.Key| F[Left-Left: Return RightRotate node]
    E -->|balance < -1 AND key > node.Right.Key| G[Right-Right: Return LeftRotate node]
    E -->|balance > 1 AND key > node.Left.Key| H[Left-Right: node.Left = LeftRotate -> Return RightRotate]
    E -->|balance < -1 AND key < node.Right.Key| I[Right-Left: node.Right = RightRotate -> Return LeftRotate]
    E -->|Else: Tree is Balanced| J[Return node]
    
    F --> K[Increment rotationsCount]
    G --> K
    H --> K
    I --> K
    K --> L[Return Balanced Node Topology]
    J --> L

```

</details>

### 2. Red-Black Tree (`RedBlackTree.cs`)

* Node-based structural color balancing mapping pointer properties to `Color.Red` and `Color.Black`.
* Implements a persistent **Sentinel Node (`TNULL`)** architecture to minimize memory reference errors.
* Leverages iterative pointer tracing up to the **Uncle** and **Grandparent** nodes inside an iterative repair loop (`FixInsert`).


<details>
<summary><b>📐 2. RBT Tree (RedBlackTree.cs) - Balancing Logic</b></summary>
    

```mermaid

graph TD
    A[Start: FixInsert / Node k] --> B{k.Parent.Color == Color.Red}
    B -->|False| C[Force root.Color = Color.Black]
    C --> D[End Rotation & Balancing]

    

    B -->|True| E{k.Parent == k.Parent.Parent.Right}

    

    %% RIGHT SIDE UNCLE LOGIC
    E -->|True| F[Identify Uncle: u = k.Parent.Parent.Left]
    F --> G{u.Color == Color.Red}
    G -->|True: Case A| H[Recolor: u & k.Parent to Black, Grandparent to Red]
    H --> H2[Move Pointer: k = k.Parent.Parent]
    H2 --> B

    

    G -->|False: Case B| I{k == k.Parent.Left}
    I -->|True| J[Move Pointer: k = k.Parent -> Execute RightRotate k]
    I -->|False| K[Recolor: k.Parent to Black, Grandparent to Red]
    J --> K
    K --> L[Execute LeftRotate k.Parent.Parent]
    L --> M{k == root}
    M -->|True| C
    M -->|False| B



    %% LEFT SIDE UNCLE LOGIC
    E -->|False| N[Identify Uncle: u = k.Parent.Parent.Right]
    N --> O{u.Color == Color.Red}
    O -->|True: Case A| P[Recolor: u & k.Parent to Black, Grandparent to Red]
    P --> P2[Move Pointer: k = k.Parent.Parent]
    P2 --> B

    

    O -->|False: Case B| Q{k == k.Parent.Right}
    Q -->|True| R[Move Pointer: k = k.Parent -> Execute LeftRotate k]
    Q -->|False| S[Recolor: k.Parent to Black, Grandparent to Red]
    R --> S
    S --> T[Execute RightRotate k.Parent.Parent]
    T --> U{k == root}
    U -->|True| C
    U -->|False| B

```
</details>

### 3. Splay Tree (`SplayTree.cs`)

* A self-adjusting search tree that dynamically optimizes around the **Locality of Reference** principle.

* Every operation triggers a recursive `Splay` mechanism, violently cascading the targeted key up to the root using custom **Zig-Zig** and **Zig-Zag** double rotation vectors.





<details>

<summary><b>📐 3. SplayTree Tree (SplayTree.cs) - Balancing Logic</b></summary>

    

```mermaid

graph TD
    A[Start: Splay / Node root, int key] --> B{root == null OR root.Key == key}
    B -->|True| C[Return root]
    
    B -->|False| D{key < root.Key}
    
    %% LEFT SUBTREE SPLAYING
    D -->|True: Left Subtree| E{root.Left == null}
    E -->|True| C
    E -->|False| F{key < root.Left.Key}
    F -->|True: Sol-Sol Zig-Zig| G[Recurse: root.Left.Left = Splay -> Execute RightRotate root]
    F -->|False: Sol-Sağ Zig-Zag| H{key > root.Left.Key}
    H -->|True| I[Recurse: root.Left.Right = Splay]
    I --> J{root.Left.Right != null}
    J -->|True| K[Execute LeftRotate root.Left]
    J -->|False| L{root.Left == null}
    K --> L
    G --> L
    H -->|False| L
    L -->|True| M[Return root]
    L -->|False| N[Return RightRotate root]
    
    %% RIGHT SUBTREE SPLAYING
    D -->|False: Right Subtree| O{root.Right == null}
    O -->|True| C
    O -->|False| P{key < root.Right.Key}
    P -->|True: Sağ-Sol Zag-Zig| Q[Recurse: root.Right.Left = Splay]
    Q --> R{root.Right.Left != null}
    R -->|True| S[Execute RightRotate root.Right]
    R --> T{root.Right == null}
    S --> T
    P -->|False: Sağ-Sağ Zag-Zag| U[Recurse: root.Right.Right = Splay -> Execute LeftRotate root]
    U --> T
    T -->|True| V[Return root]
    T -->|False| W[Return LeftRotate root]

    M --> X[Increment rotationsCount via Rotate Engines]
    N --> X
    V --> X
    W --> X

```

</details>

### 4. B+ Tree ('BPlusTree.cs')

* An $m$-way balanced search tree designed explicitly for database structural indexing loops.
* Restricts records strictly inside the external leaves while internal pages hold directory values, executing automated Split-Child mutations on saturation boundaries.

<details>

<summary><b>📐 3. SplayTree Tree (SplayTree.cs) - Balancing Logic</b></summary>

```mermaid

```

</details>


### 5. Quadtree ('QuadTree.cs')
* A spatial partitioning tree structure optimizing two-dimensional grid lookups.
* Maps numerical keys to structural Point('X, Y') planes, dividing geographical space recursively into four quadrants ('NorthWest, NorthEast, SouthWest, SouthEast') when node capacities are reached.

<details>
<summary><b>📐 3. SplayTree Tree (SplayTree.cs) - Balancing Logic</b></summary>

```mermaid

```
---

</details>

## 🚀 Installation & Getting Started

### Prerequisites
*  [.NET SDK](https://dotnet.microsoft.com/download) (Version 6.0 or higher recommended)
* Microsoft SQL Server (LocalDB or SQLEXPRESS)



### 1. Repository Cloning
Clone the project architecture to your localized workstation using Git:

```bash
git clone https://github.com/EgeSul/TreeBench.BR
cd TreeBench.BR
```



### 2. Database Provisioning
Run the following script inside SQL Server Management Studio (SSMS) to instantiate the database pipeline:

```sql

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



-- Populate with 100,000 random records for mock testing
SET NOCOUNT ON;
DECLARE @i INT = 1;
WHILE @i <= 100000
BEGIN
    INSERT INTO TestNumbers (Value, DataType)
    VALUES (CAST(RAND() * 1000000 AS INT), 'Production');
    SET @i = @i + 1;
END;
GO

```

### 2. Configure Connection String
Open Services/DataGenerator.cs and configure your localized SQL Server connection string properties:
```C#
Open Services/DataGenerator.cs and configure your localized SQL Server connection string properties:
```
### 3. Run Benchmark Compilation
Execute the following native commands in your terminal or compile directly via Visual Studio:

```Bash
dotnet build
dotnet run --configuration Release
```


### Future Releases / Roadmap:

<br>[x] v1.0.0 - AVL & Red-Black Tree benchmarking with advanced memory profiling.
<br>[X] v1.5.0 - .NET Dependency Injection
<br>[ ] v2.0.0 - Transitioning to multi-way structures (B+ Tree) and spatial indexing (Quadtree) for enterprise data simulation.

---

### 📄 License
This architecture is completely open-source and released under the MIT License. 

