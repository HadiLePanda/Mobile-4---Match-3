using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [Header("References")]
    public GameObject[] symbolPrefabs;
    public GameObject boardObject;
    public Transform symbolsParent;

    [Header("Settings")]
    public int width = 6;
    public int height = 8;
    public ArrayLayout arrayLayout;

    private Node[,] board;
    private float spacingX;
    private float spacingY;

    public static Board Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // create a new empty board of chosen size
        board = new Node[width, height];
        
        // calculate spacing between nodes
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        // generate symbols inside the board
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // get random position
                Vector2 position = new(x - spacingX, y - spacingY);

                // in case it's a usable position (not ticked in the inspector)
                // let's spawn a symbol
                if (arrayLayout.rows[y].row[x] == false)
                {
                    // choose a random symbol
                    int randomIndex = Random.Range(0, symbolPrefabs.Length);
                    GameObject symbolPrefab = symbolPrefabs[randomIndex];

                    // spawn the symbol
                    GameObject symbolInstance = Instantiate(symbolPrefab, position, Quaternion.identity, symbolsParent);
                    symbolInstance.GetComponent<Symbol>().SetIndices(x, y);

                    // create a new node in the board for this symbol
                    board[x, y] = new Node(true, symbolInstance);
                }
                // otherwise if it's a blocked position (ticked in the inspector)
                // create an unusable node at this position
                else
                {
                    // create an unusable node
                    board[x, y] = new Node(false, null);
                }
            }
        }
    }
}
