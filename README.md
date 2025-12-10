# Chess Programming - Algorithms & Data Structures

<img width="1638" alt="Chess Board Visualization" src="https://github.com/user-attachments/assets/f6b9c1f9-8ca5-4eb4-8f28-b2379f399d4a" />

## 📋 Overview

A high-performance chess engine implementation in C# (.NET 9.0) featuring two distinct board representations: a traditional 2D array approach and an optimized BitBoard implementation using Magic Bitboards for move generation.

This project serves as both a functional chess engine and an educational exploration of algorithm optimization and data structure choices in chess programming.

## ⚡ Features

- **Dual Board Implementations**
  - Traditional 2D Array Board representation
  - BitBoard with Magic Bitboard move generation
- **Complete Chess Rules**
  - All standard piece movements
  - Special moves (castling, en passant, pawn promotion)
  - Check and checkmate detection
- **Multiple Interfaces**
  - Blazor web UI for interactive play
  - Console applications for both implementations
  - Comprehensive test suite
- **Performance Benchmarking**
  - BenchmarkDotNet integration
  - Detailed performance comparisons

## 📊 Performance Comparison

Comprehensive benchmarks comparing the 2D Array Board vs BitBoard (Magic) implementations:

### Benchmark Results

**Environment:**
- **Platform:** Windows 11 (10.0.26100.7171)
- **CPU:** AMD Ryzen 7 7800X3D (8 physical cores, 16 logical cores)
- **Runtime:** .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
- **Configuration:** ShortRun (IterationCount=3, LaunchCount=1, WarmupCount=3)

| Method             | Categories         | Mean         | Error         | StdDev     | Rank | Gen0   | Gen1   | Allocated |
|------------------- |------------------- |-------------:|--------------:|-----------:|-----:|-------:|-------:|----------:|
| **Board (2D Array)** | ApplyUndo          |     29.91 ns |      4.607 ns |   0.253 ns |    1 |      - |      - |         - |
| **BitBoard (Magic)** | ApplyUndo          |     34.46 ns |      6.028 ns |   0.330 ns |    1 | 0.0017 |      - |      88 B |
|                    |                    |              |               |            |      |        |        |           |
| **BitBoard (Magic)** | Clone              |     88.08 ns |      7.391 ns |   0.405 ns |    1 | 0.0064 |      - |     328 B |
| **Board (2D Array)** | Clone              |    405.30 ns |    147.016 ns |   8.058 ns |    2 | 0.0353 |      - |    1776 B |
|                    |                    |              |               |            |      |        |        |           |
| **Board (2D Array)** | GetPiecesForColor  |    179.07 ns |    152.008 ns |   8.332 ns |    1 | 0.0186 |      - |     936 B |
| **BitBoard (Magic)** | GetPiecesForColor  |    501.03 ns |     67.923 ns |   3.723 ns |    2 | 0.0639 |      - |    3240 B |
|                    |                    |              |               |            |      |        |        |           |
| **BitBoard (Magic)** | IsInCheck          |     10.24 ns |      0.692 ns |   0.038 ns |    1 | 0.0005 |      - |      24 B |
| **Board (2D Array)** | IsInCheck          |    583.38 ns |    137.848 ns |   7.556 ns |    2 | 0.0629 |      - |    3160 B |
|                    |                    |              |               |            |      |        |        |           |
| **BitBoard (Magic)** | LoadFEN            |    430.36 ns |    102.683 ns |   5.628 ns |    1 | 0.0253 |      - |    1288 B |
| **Board (2D Array)** | LoadFEN            |    661.34 ns |    103.354 ns |   5.665 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| **BitBoard (Magic)** | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| **Board (2D Array)** | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| **BitBoard (Magic)** | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| **Board (2D Array)** | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

### Key Insights

- **Move Generation:** BitBoard is **~55-97x faster** for move generation (most critical operation)
- **IsInCheck:** BitBoard is **~57x faster** with significantly less memory allocation
- **Clone:** BitBoard is **~4.6x faster** and uses **~5.4x less memory**
- **Trade-offs:** 2D Array performs slightly better in simple operations like ApplyUndo and GetPiecesForColor

## Project Structure

```
chess-programming-algo/
├── ChessEngine/              # Core chess engine library
│   ├── Core/                 # Board implementations & game logic
│   │   ├── Board.cs          # Traditional 2D array implementation
│   │   ├── BitBoard.cs       # Magic bitboard implementation
│   │   └── ...
│   ├── Pieces/               # Chess piece definitions
│   ├── ChessEngines/         # AI player implementations
│   └── Game/                 # Game management logic
├── ChessBlazor/              # Blazor web UI
├── ChessConsole/             # Console app (2D Array)
├── ChessConsole.BitBoard/    # Console app (BitBoard)
├── ChessEngine.Tests/        # Unit tests
└── ChessEngine.Benchmarks/   # Performance benchmarks
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- IDE: Visual Studio 2022, Rider, or VS Code

### Running the Applications

**Blazor Web UI:**
```bash
cd ChessBlazor
dotnet run
```
Navigate to `https://localhost:5001` in your browser.

**Console Application (2D Array):**
```bash
cd ChessConsole
dotnet run
```

**Console Application (BitBoard):**
```bash
cd ChessConsole.BitBoard
dotnet run
```

### Running Benchmarks

```bash
cd ChessEngine.Benchmarks
dotnet run -c Release
```

### Running Tests

```bash
cd ChessEngine.Tests
dotnet test
```

## Technical Highlights

### Magic Bitboards
The BitBoard implementation uses [Magic Bitboards](https://www.chessprogramming.org/Magic_Bitboards) for efficient sliding piece (rooks, bishops, queens) move generation. This technique uses perfect hashing to quickly lookup pre-computed attack tables.

### Key Optimizations
- Bitwise operations for piece movement and attack detection
- Pre-computed attack tables for sliding pieces
- Efficient check detection using bitboard masks
- Minimal memory allocations during move generation

## Testing

The project includes comprehensive unit tests covering:
- Move generation for all piece types
- Special moves (castling, en passant, promotion)
- Check and checkmate detection
- FEN string parsing and board serialization
- Game state management

This project is part of an academic assignment for HAN University of Applied Sciences.

## Acknowledgments

- Chess programming community at [Chess Programming Wiki](https://www.chessprogramming.org/)

---

**Performance matters.** This project demonstrates how algorithmic choices and data structure selection can lead to dramatic performance improvements in computational chess.
