using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MatchResult
{
    public List<Symbol> connectedSymbols;
    public MatchDirection direction;
}

public enum MatchDirection
{
    None,
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super
}

public enum BoardState
{
    Idle,
    ProcessingMove
}

public class Board : MonoBehaviour
{
    [Header("References")]
    public Symbol[] symbolPrefabs;
    public Transform symbolsParent;
    public GameObject boardObject;

    [Header("Settings")]
    public int width = 6;
    public int height = 8;
    public int maxTriesToGenerateBoard = 100;
    public float symbolSwapDuration = 0.2f;
    public ArrayLayout arrayLayout;

    [Header("Debug")]
    [SerializeField, ReadOnly] private  BoardState state;
    [SerializeField, ReadOnly] private Symbol selectedSymbol;

    private Tile[,] board;
    private float spacingX;
    private float spacingY;


    public BoardState State => state;

    public static Board Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state = BoardState.Idle;

        // try to generate a board with no matches on start
        int triesToGenerateBoard = 0;
        while (triesToGenerateBoard < maxTriesToGenerateBoard)
        {
            GenerateBoard();

            // check for matches
            // we want to find a board that has no matches
            if (!BoardContainsMatch())
                break;

            Debug.Log("Found matches when generating the board, regenerating new board.");
            triesToGenerateBoard++;

            ClearBoard();
        }
    }

    private void Update()
    {
        // detect player click
        if (Input.GetMouseButtonDown(0))
        {
            // send a raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // we clicked on a symbol
            if (hit.collider != null && hit.collider.GetComponentInParent<Symbol>())
            {
                // we're in the process of moving tiles around, so don't do anything
                if (state == BoardState.ProcessingMove)
                    return;

                // select
                Symbol clickedSymbol = hit.collider.GetComponentInParent<Symbol>();
                OnSymbolClicked(clickedSymbol);
            }
        }
    }

    #region BOARD
    private void ClearBoard()
    {
        // destroy all the spawned symbols
        // all the spawned symbols are under the symbols parent
        foreach (Transform symbol in symbolsParent)
            Destroy(symbol.gameObject);
    }

    private void GenerateBoard()
    {
        // create a new empty board of chosen size
        board = new Tile[width, height];
        
        // calculate spacing between tiles
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        // generate symbols inside the board
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // spawn symbol in usable tiles
                // (unticked in inspector)
                if (arrayLayout.rows[y].row[x] == false)
                {
                    // choose a random symbol
                    int randomIndex = Random.Range(0, symbolPrefabs.Length);
                    Symbol symbolPrefab = symbolPrefabs[randomIndex];

                    // spawn the symbol
                    Vector2 spawnPosition = new(x - spacingX, y - spacingY);
                    Symbol symbolInstance = SpawnSymbol(new Vector2Int(x, y), spawnPosition, symbolPrefab);

                    // create a new tile in the board for this symbol
                    board[x, y] = new Tile(true, symbolInstance);
                }
                // create unusable tiles
                // (ticked in inspector)
                else
                {
                    board[x, y] = new Tile(false, null);
                }
            }
        }

        Debug.Log("Generated new board.");
    }

    private Symbol SpawnSymbol(Vector2Int indices, Vector2 spawnPosition, Symbol symbolPrefab)
    {
        Symbol symbolInstance = Instantiate(symbolPrefab, spawnPosition, Quaternion.identity);
        symbolInstance.transform.SetParent(symbolsParent, false);
        symbolInstance.SetIndices(indices);
        return symbolInstance;
    }
    #endregion

    #region CHECKING
    public bool BoardContainsMatch()
    {
        Debug.Log("Checking for matches...");

        bool hasMatched = false;

        List<Symbol> symbolsToRemove = new();
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                // check if usable tile
                if (!board[x, y].isUsable)
                    continue;

                // get the symbol in this tile
                Symbol symbol = board[x, y].symbol;

                // check if it's not already matched
                if (symbol.IsMatched)
                    continue;

                // run some matching logic
                MatchResult matchResult = IsConnected(symbol);

                // we found a match
                if (matchResult.connectedSymbols.Count >= 3)
                {
                    // TOOD: complex matching (supers etc.)

                    // add the connected symbols to the list of symbols to consume
                    symbolsToRemove.AddRange(matchResult.connectedSymbols);

                    // mark connected symbols as matched to avoid using them again for matching logic
                    foreach (Symbol connectedSymbol in matchResult.connectedSymbols)
                        connectedSymbol.SetIsMatched(true);

                    hasMatched = true;
                }
            }
        }

        return hasMatched;
    }

    private MatchResult IsConnected(Symbol symbol)
    {
        List<Symbol> connectedSymbols = new();
        SymbolType symbolType = symbol.Type;

        // read our initial symbol
        connectedSymbols.Add(symbol);

        // check for horizontal connections
        CheckDirection(symbol, Vector2Int.right, connectedSymbols);
        CheckDirection(symbol, Vector2Int.left, connectedSymbols);
        // found horizontal match (3)
        if (connectedSymbols.Count == 3)
        {
            Debug.Log($"Normal horizontal 3 match. Type: {connectedSymbols[0].Type.name}");
            return new MatchResult
            {
                connectedSymbols = connectedSymbols,
                direction = MatchDirection.Horizontal
            };
        }
        // found long horizontal match (> 3)
        else if (connectedSymbols.Count > 3)
        {
            Debug.Log($"Long horizontal match. Type: {connectedSymbols[0].Type.name}");
            return new MatchResult
            {
                connectedSymbols = connectedSymbols,
                direction = MatchDirection.LongHorizontal
            };
        }

        // clear out the connected symbols to avoid accidental matches
        connectedSymbols.Clear();
        // read our initial symbol
        connectedSymbols.Add(symbol);

        // check for vertical connections
        CheckDirection(symbol, Vector2Int.up, connectedSymbols);
        CheckDirection(symbol, Vector2Int.down, connectedSymbols);
        // found vertical match (3)
        if (connectedSymbols.Count == 3)
        {
            Debug.Log($"Normal vertical 3 match. Type: {connectedSymbols[0].Type.name}");
            return new MatchResult
            {
                connectedSymbols = connectedSymbols,
                direction = MatchDirection.Vertical
            };
        }
        // found long vertical match (> 3)
        else if (connectedSymbols.Count > 3)
        {
            Debug.Log($"Long vertical match. Type: {connectedSymbols[0].Type.name}");
            return new MatchResult
            {
                connectedSymbols = connectedSymbols,
                direction = MatchDirection.LongVertical
            };
        }

        // we did not find any match
        return new MatchResult
        {
            connectedSymbols = connectedSymbols,
            direction = MatchDirection.None
        };
    }

    private void CheckDirection(Symbol symbol, Vector2Int direction, List<Symbol> connectedSymbols)
    {
        SymbolType symbolType = symbol.Type;

        int x = symbol.X + direction.x;
        int y = symbol.Y + direction.y;

        // check that we're within the boundaries of the board
        while (
            x >= 0 && x < width &&
            y >= 0 && y < height)
        {
            // is a usable tile?
            if (!board[x, y].isUsable)
                break;

            // get the neighbour symbol
            Symbol neighbourSymbol = board[x, y].symbol;

            // do the symbol type match?
            // and is not already matched?
            if (neighbourSymbol.IsMatched || neighbourSymbol.Type != symbolType)
                break;

            // it's a connected symbol
            // add it to the connection list
            connectedSymbols.Add(neighbourSymbol);

            x += direction.x;
            y += direction.y;
        }
    }

    #endregion

    #region SWAPPING
    public void OnSymbolClicked(Symbol symbol)
    {
        // if we don't have any symbol currently selected, then select it
        if (selectedSymbol == null)
        {
            Debug.Log(symbol);
            SelectSymbol(symbol);
        }
        // if we selected the same symbol twice, deselect it
        else if (selectedSymbol == symbol)
        {
            DeselectCurrentSymbol();
        }
        // if we selected a different symbol while we have a symbol selected, swap them
        else if (selectedSymbol != symbol)
        {
            SwapSymbol(selectedSymbol, symbol);

            DeselectCurrentSymbol();
        }
    }

    private void SelectSymbol(Symbol symbol) => selectedSymbol = symbol;
    private void DeselectCurrentSymbol() => selectedSymbol = null;

    private void SwapSymbol(Symbol firstSymbol, Symbol secondSymbol)
    {
        // check if the symbols are adjacent to eachother
        if (!IsAdjacent(firstSymbol, secondSymbol))
            return;

        state = BoardState.ProcessingMove;

        DoSwap(firstSymbol, secondSymbol);

        StartCoroutine(ProcessMatches(firstSymbol, secondSymbol));
    }

    private bool IsAdjacent(Symbol firstSymbol, Symbol secondSymbol)
    {
        return Mathf.Abs(firstSymbol.X - secondSymbol.X) + Mathf.Abs(firstSymbol.Y - secondSymbol.Y) == 1;
    }

    private void DoSwap(Symbol firstSymbol, Symbol secondSymbol)
    {
        Symbol temp = board[firstSymbol.X, firstSymbol.Y].symbol;

        // swap symbols
        board[firstSymbol.X, firstSymbol.Y].symbol = board[secondSymbol.X, secondSymbol.Y].symbol;
        board[secondSymbol.X, secondSymbol.Y].symbol = temp;

        // update indices
        int tempX = firstSymbol.X;
        int tempY = firstSymbol.Y;
        firstSymbol.SetIndices(new Vector2Int(secondSymbol.X, secondSymbol.Y));
        secondSymbol.SetIndices(new Vector2Int(tempX, tempY));

        // move the symbols to their new position
        var firstSymbolPos = board[firstSymbol.X, firstSymbol.Y].symbol.transform.position;
        var secondSymbolPos = board[secondSymbol.X, secondSymbol.Y].symbol.transform.position;
        firstSymbol.MoveToPosition(secondSymbolPos);
        secondSymbol.MoveToPosition(firstSymbolPos);
    }

    private IEnumerator ProcessMatches(Symbol firstSymbol, Symbol secondSymbol)
    {
        // wait for the symbols to finish swapping their position
        yield return new WaitForSeconds(symbolSwapDuration);

        // if there was no match, move the symbols back to their starting tile
        bool hasMatch = BoardContainsMatch();
        if (!hasMatch)
        {
            DoSwap(firstSymbol, secondSymbol);
        }

        state = BoardState.Idle;
    }
    #endregion
}
