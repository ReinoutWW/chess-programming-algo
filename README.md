<img width="1638" height="1256" alt="image" src="https://github.com/user-attachments/assets/f6b9c1f9-8ca5-4eb4-8f28-b2379f399d4a" />

## Bitboard Chess Engine Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                           CHESS ENGINE                               │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐        │
│  │    SEARCH    │────▶│  EVALUATION  │     │   INTERFACE  │        │
│  │              │     │              │     │  (UCI/Console)│        │
│  │  - Minimax   │     │  - Material  │     │              │        │
│  │  - AlphaBeta │     │  - Mobility  │     └──────────────┘        │
│  │  - Ordering  │     │  - Position  │              │               │
│  └──────┬───────┘     └──────────────┘              │               │
│         │                    ▲                      │               │
│         ▼                    │                      ▼               │
│  ┌──────────────────────────────────────────────────────┐          │
│  │                    GAME STATE                         │          │
│  │  - Board (BitBoards)    - Side to Move               │          │
│  │  - Castling Rights      - En Passant Square          │          │
│  │  - Move History         - Halfmove Clock             │          │
│  └──────────────────────────┬───────────────────────────┘          │
│                             │                                       │
│         ┌───────────────────┼───────────────────┐                  │
│         ▼                   ▼                   ▼                  │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐            │
│  │  MAKE MOVE  │    │   UNMAKE    │    │    MOVE     │            │
│  │             │    │    MOVE     │    │ GENERATION  │            │
│  └─────────────┘    └─────────────┘    └──────┬──────┘            │
│                                               │                    │
└───────────────────────────────────────────────┼────────────────────┘
                                                │
                    ┌───────────────────────────┼───────────────────┐
                    ▼                           ▼                   ▼
             ┌─────────────┐           ┌─────────────┐    ┌─────────────┐
             │   ATTACK    │           │   BITBOARD  │    │    MOVE     │
             │   TABLES    │           │    CORE     │    │   STRUCT    │
             │             │           │             │    │             │
             │ - Knights   │           │ - Pieces[]  │    │ - From      │
             │ - Kings     │           │ - White/Blk │    │ - To        │
             │ - Pawns     │           │ - Occupied  │    │ - Flags     │
             │ - Magic BB  │           │ - Set/Clear │    │ - Piece     │
             └─────────────┘           └─────────────┘    └─────────────┘
```

### BitBoard Core

```
┌─────────────────────────────────────┐
│           BITBOARD CORE             │
├─────────────────────────────────────┤
│  Data:                              │
│  ├── pieces[2, 6]  (color, type)   │
│  ├── whitePieces   (aggregate)     │
│  ├── blackPieces   (aggregate)     │
│  └── occupied      (aggregate)     │
├─────────────────────────────────────┤
│  Methods:                           │
│  ├── SetBit()                      │
│  ├── ClearBit()                    │
│  ├── IsBitSet()                    │
│  ├── PlacePiece()                  │
│  └── RemovePiece()                 │
└─────────────────────────────────────┘
```

### Attack Tables

```
┌─────────────────────────────────────────────────────────────┐
│                      ATTACK TABLES                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   KNIGHT    │  │    KING     │  │    PAWN     │        │
│  │  ATTACKS[]  │  │  ATTACKS[]  │  │  ATTACKS[]  │        │
│  │             │  │             │  │             │        │
│  │  64 entries │  │  64 entries │  │ 64×2 entries│        │
│  │  (1 per sq) │  │  (1 per sq) │  │ (per color) │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
│         │                │                │                │
│         └────────────────┼────────────────┘                │
│                          ▼                                  │
│                  ┌─────────────────┐                       │
│                  │  MAGIC BITBOARDS │                       │
│                  │   (Sliders)      │                       │
│                  ├─────────────────┤                       │
│                  │ ┌─────┐ ┌─────┐ │                       │
│                  │ │ROOK │ │BISH.│ │                       │
│                  │ │Magic│ │Magic│ │                       │
│                  │ └─────┘ └─────┘ │                       │
│                  │                 │                       │
│                  │ Queen = R | B   │                       │
│                  └─────────────────┘                       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Move Generation Pipeline

```
┌──────────────────────────────────────────────────────────────────┐
│                     MOVE GENERATION                               │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│   Input: Current Position                                         │
│      │                                                            │
│      ▼                                                            │
│   ┌─────────────────────────────────────────────────────────┐   │
│   │  For each piece type:                                    │   │
│   │                                                          │   │
│   │  PAWNS ──▶ Shift forward, captures diagonal, promotions │   │
│   │  KNIGHTS ▶ Lookup attack table, mask friendly pieces    │   │
│   │  BISHOPS ▶ Magic bitboard lookup                        │   │
│   │  ROOKS ──▶ Magic bitboard lookup                        │   │
│   │  QUEENS ─▶ Rook attacks | Bishop attacks                │   │
│   │  KING ───▶ Lookup attack table + castling check         │   │
│   └─────────────────────────────────────────────────────────┘   │
│      │                                                            │
│      ▼                                                            │
│   ┌─────────────────────────────────────────────────────────┐   │
│   │  Filter: Remove moves that leave king in check          │   │
│   └─────────────────────────────────────────────────────────┘   │
│      │                                                            │
│      ▼                                                            │
│   Output: List<Move>                                              │
│                                                                   │
└──────────────────────────────────────────────────────────────────┘
```

### Move Structure

```
┌────────────────────────────────────┐
│            MOVE (16-32 bits)       │
├────────────────────────────────────┤
│                                    │
│  ┌──────┬──────┬──────┬────────┐  │
│  │ From │  To  │Flags │ Piece  │  │
│  │ 6bit │ 6bit │ 4bit │ 4bit   │  │
│  └──────┴──────┴──────┴────────┘  │
│                                    │
│  Flags:                            │
│  ├── 0001 = Capture                │
│  ├── 0010 = En Passant             │
│  ├── 0100 = Castling               │
│  ├── 1000 = Promotion              │
│  └── .... = Promotion piece type   │
│                                    │
└────────────────────────────────────┘
```
