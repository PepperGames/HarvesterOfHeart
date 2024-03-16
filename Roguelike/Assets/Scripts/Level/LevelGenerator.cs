using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    public static float LVL = 1;
    public enum gridSpace { empty, floor, wall, l, u, r, d, luc, ldc, ruc, rdc, rd, ru, ld, lu };
    public gridSpace[,] grid;
    private int roomHeight, roomWidth;
    private Vector2 roomSizeWorldUnits;
    private float worldUnitsInOneGridCell = 1;
      
    enum Mobs { mob1, boss1, boss2, boss3 };
    public GameObject portal, mob1, boss1, boss2, boss3;
    private GameObject player;

    public float distance;
    public int MobCountOnLvl;
    private int floorCount;
    private int mobCount;
    private int floorNumber;
    public int[,] floorForSpawnMob;
    //для теста сколько полов
    private int floorC = 0;
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    private float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
    private float chanceWalkerDestoy = 0.05f;
    private int maxWalkers = 10;
    private float percentToFill = 0.2f;
    public GameObject floor1, floor2, floor3, wall, l, u, r, d, luc, ldc, ruc, rdc, rd, ru, ld, lu, menorahL, menorahU, menorahR, menorahD;

    NormalGenerationCheck normalGenerationCheck;
    //вспомогательные массивы(словари)
    private int[,] LwallForMenorah;
    private int[,] UwallForMenorah;
    private int[,] RwallForMenorah;
    private int[,] DwallForMenorah;
    public float MenorahSpawnRange;
    public LayerMask whatIsMenorah;

    public AudioSource[] audioSources;

    [SerializeField]
    private AnalyticsComponent analytics;
    public void Start()
    {
        RestartLVL();
        audioSources[0].Play();
    }

    public void RestartLVL()
    {

        Setup();
        CreateFloors();
        CreateWalls();
        SpawnLevel();

        normalGenerationCheck = GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<NormalGenerationCheck>();
        if (!NormalGeneration())
        {
            Restarter r = new Restarter();
            r.Restart();
        }
        analytics.OnLevelStart((int)LVL);
        SpawnMob();
        SpawnMenorah();
    }
    void Setup()
    {
        floorC = 0;
        float hw = 13;
        if (LVL % 3 != 0)
        {
            hw = 12 + LVL * 2;
            if (hw > 20)
                hw = 20;
        }
        else
        {
            hw = 15;
        }
        roomSizeWorldUnits = new Vector2(hw, hw);

        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
 
        grid = new gridSpace[roomWidth, roomHeight];
 
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                grid[x, y] = gridSpace.empty;
            }
        }

        walkers = new List<walker>();
        //создать ходока
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //центр сетки
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f),
                                        Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;

        walkers.Add(newWalker);
    }
    void CreateFloors()
    {
        if (LVL % 3 != 0)
        {
            int iterations = 0;//цикл не будет работать вечно
            do
            {
                //создать пол на каждом шагу
                foreach (walker myWalker in walkers)
                {
                    grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
                }
                //шанс: уничтожить ходока
                int numberChecks = walkers.Count; //может изменить счетчик в этом цикле
                for (int i = 0; i < numberChecks; i++)
                {
                    //только если он не единственный и с малой вероятностью
                    if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                    {
                        walkers.RemoveAt(i);
                        break; // уничтожить только одного за итерацию
                    }
                }
                //шанс: ходок выберет новое направление
                for (int i = 0; i < walkers.Count; i++)
                {
                    if (Random.value < chanceWalkerChangeDir)
                    {
                        walker thisWalker = walkers[i];
                        thisWalker.dir = RandomDirection();
                        walkers[i] = thisWalker;
                    }
                }
                //шанс: создать нового ходока
                numberChecks = walkers.Count; //может изменить счетчик в этом цикле
                for (int i = 0; i < numberChecks; i++)
                {
                    // только если количество ходоков < макс, и с низким шансом
                    if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                    {
                        walker newWalker = new walker();
                        newWalker.dir = RandomDirection();
                        newWalker.pos = walkers[i].pos;
                        walkers.Add(newWalker);
                    }
                }
                //перемещать ходока
                for (int i = 0; i < walkers.Count; i++)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.pos += thisWalker.dir;
                    walkers[i] = thisWalker;
                }
                //избегать границы сетки
                for (int i = 0; i < walkers.Count; i++)
                {
                    walker thisWalker = walkers[i];
                    //clamp x, y, чтобы оставить границу 1 пробела: оставьте место для стен
                    thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                    thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                    walkers[i] = thisWalker;
                }
                //проверьте, чтобы выйти из цикла
                if ((float)NumberOfFloors() / (float)grid.Length > percentToFill)
                {
                    break;
                }
                iterations++;
            } while (iterations < 100000);
        }
        else
        {
            for (int x = 1; x < roomWidth - 1; x++)
            {
                for (int y = 1; y < roomHeight - 1; y++)
                {
                    grid[x, y] = gridSpace.floor;
                }
            }
            grid[2, 12] = gridSpace.empty;
            grid[12, 12] = gridSpace.empty;
            grid[12, 2] = gridSpace.empty;
            grid[2, 2] = gridSpace.empty;
        }
    }
    void CreateWalls()
    {
        //левая стенка
        for (int x = 0, y = 0; y < roomWidth; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 1, y = 0; y < roomWidth; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //нижнее
        for (int x = 0, y = 0; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 0, y = 1; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //верх
        for (int x = 0, y = roomHeight - 1; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 0, y = roomHeight - 2; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //низ
        for (int x = roomWidth - 1, y = 0; y < roomHeight; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = roomWidth - 2, y = 0; y < roomHeight; y++)
        {
            grid[x, y] = gridSpace.empty;
        }


        //пройти через каждую клетку сетки
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //Если есть пол, проверьте пространство вокруг него
                if (grid[x, y] == gridSpace.floor)
                {
                    //стены лево\право, верх\низ
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }
                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }
                }
            }

        }

        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //Если есть пол, проверьте пространство вокруг него
                if (grid[x, y] == gridSpace.floor)
                {

                    // углы
                    if (grid[x - 1, y + 1] == gridSpace.empty)
                    {
                        grid[x - 1, y + 1] = gridSpace.wall;
                    }
                    if (grid[x - 1, y - 1] == gridSpace.empty)
                    {
                        grid[x - 1, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y + 1] == gridSpace.empty)
                    {
                        grid[x + 1, y + 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y - 1] == gridSpace.empty)
                    {
                        grid[x + 1, y - 1] = gridSpace.wall;
                    }
                }
            }
        }
        //убираем внутренние стенки
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //Если есть пол, проверить пространство вокруг него
                if (grid[x, y] == gridSpace.wall)
                {
                    if (grid[x - 1, y + 1] == gridSpace.empty || grid[x, y + 1] == gridSpace.empty ||
                        grid[x + 1, y + 1] == gridSpace.empty || grid[x + 1, y] == gridSpace.empty ||
                        grid[x + 1, y - 1] == gridSpace.empty || grid[x, y - 1] == gridSpace.empty ||
                        grid[x - 1, y - 1] == gridSpace.empty || grid[x - 1, y] == gridSpace.empty)
                    {

                    }
                    else
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
            }
        }

        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //Если есть пол, проверьте пространство вокруг него
                if (grid[x, y] == gridSpace.wall)
                {

                    // стороны буквы там де полы
                    if (grid[x - 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    {
                        grid[x, y] = gridSpace.l;
                    }
                    //кастыль убрать
                    if (grid[x - 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.r;
                    }
                    if (grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.d;
                    }
                    if (grid[x, y - 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.u;
                    }
                    //наружные углы буквы - где пустота
                    if (grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x - 1, y] == gridSpace.empty && grid[x, y + 1] == gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x + 1, y - 1] == gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.luc;
                    }
                    if (grid[x + 1, y] == gridSpace.empty && grid[x, y + 1] == gridSpace.empty && grid[x - 1, y - 1] == gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ruc;
                    }
                    if (grid[x - 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.floor && grid[x + 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ldc;
                    }
                    //if (grid[x + 1, y] == gridSpace.empty && grid[x, y - 1] == gridSpace.empty && grid[x - 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    if (grid[x + 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.floor && grid[x - 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.rdc;
                    }
                    
                    //внутренние углы буквы - где полы
                    if (grid[x - 1, y] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ld;
                    }
                    if (grid[x + 1, y] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.rd;
                    }
                    if (grid[x + 1, y] == gridSpace.floor && grid[x, y + 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ru;
                    }
                    if (grid[x, y + 1] == gridSpace.floor && grid[x - 1, y] == gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.lu;
                    }
                }
            }
        }
    }

    //void RemoveSingleWalls()
    //{
    //    //loop though every grid space
    //    for (int x = 0; x < roomWidth - 1; x++)
    //    {
    //        for (int y = 0; y < roomHeight - 1; y++)
    //        {
    //            //if theres a wall, check the spaces around it
    //            if (grid[x, y] == gridSpace.wall)
    //            {
    //                //assume all space around wall are floors
    //                bool allFloors = true;
    //                //check each side to see if they are all floors
    //                for (int checkX = -1; checkX <= 1; checkX++)
    //                {
    //                    for (int checkY = -1; checkY <= 1; checkY++)
    //                    {
    //                        if (x + checkX < 0 || x + checkX > roomWidth - 1 ||
    //                            y + checkY < 0 || y + checkY > roomHeight - 1)
    //                        {
    //                            //skip checks that are out of range
    //                            continue;
    //                        }
    //                        if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
    //                        {
    //                            //skip corners and center
    //                            continue;
    //                        }
    //                        if (grid[x + checkX, y + checkY] != gridSpace.floor)
    //                        {
    //                            allFloors = false;
    //                        }
    //                    }
    //                }
    //                if (allFloors)
    //                {
    //                    grid[x, y] = gridSpace.floor;
    //                }
    //            }
    //        }
    //    }
    //}
    void SpawnLevel()
    {
        int t = 0;
        int L = 0;
        int U = 0;
        int R = 0;
        int D = 0;
        int tLength = 0;
        int LLength = 0;
        int ULength = 0;
        int RLength = 0;
        int DLength = 0;
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.floor:
                        tLength++;
                        break;
                    case gridSpace.l:
                        LLength++;
                        break;
                    case gridSpace.u:
                        ULength++;
                        break;
                    case gridSpace.r:
                        RLength++;
                        break;
                    case gridSpace.d:
                        DLength++;
                        break;
                    case gridSpace.ld:
                        ULength++;
                        break;
                    case gridSpace.rd:
                        ULength++;
                        break;
                }
            }
        }
        floorForSpawnMob = new int[tLength, 2];
        LwallForMenorah = new int[LLength, 2];
        UwallForMenorah = new int[ULength, 2];
        RwallForMenorah = new int[RLength, 2];
        DwallForMenorah = new int[DLength, 2];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        break;
                    case gridSpace.floor:
                        float f = Random.Range(0f, 3f);
                        if (f < 1)
                        {
                            Spawn(x, y, floor1);
                        }
                        else if(f < 2)
                        {
                            Spawn(x, y, floor2);
                        }
                        else
                        {
                            Spawn(x, y, floor3);
                        }                      
                        floorForSpawnMob[t,0] = x;
                        floorForSpawnMob[t,1] = y;
                        floorC++;
                        t++;
                        break;

                    case gridSpace.wall:
                        Spawn(x, y, wall);
                        break;
                    case gridSpace.l:
                        LwallForMenorah[L, 0] = x;
                        LwallForMenorah[L, 1] = y;
                        L++;
                        Spawn(x, y, l);
                        break;
                    case gridSpace.u:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, u);
                        break;
                    case gridSpace.r:
                        RwallForMenorah[R, 0] = x;
                        RwallForMenorah[R, 1] = y;
                        R++;
                        Spawn(x, y, r);
                        break;
                    case gridSpace.d:
                        DwallForMenorah[D, 0] = x;
                        DwallForMenorah[D, 1] = y;
                        D++;
                        Spawn(x, y, d);
                        break;
                    case gridSpace.luc:
                        Spawn(x, y, luc);
                        break;
                    case gridSpace.ldc:
                        Spawn(x, y, ldc);
                        break;
                    case gridSpace.ruc:
                        Spawn(x, y, ruc);
                        break;
                    case gridSpace.rdc:
                        Spawn(x, y, rdc);
                        break;
                    case gridSpace.ld:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, ld);
                        break;
                    case gridSpace.rd:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, rd);
                        break;
                    case gridSpace.ru:
                        Spawn(x, y, ru);
                        break;
                    case gridSpace.lu:
                        Spawn(x, y, lu);
                        break;
                }
            }
        }
    }
    Vector2 RandomDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //выбрать направление
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }
    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }
    void Spawn(float x, float y, GameObject toSpawn)
    {
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell;
        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity, gameObject.transform);
    }

    bool NormalGeneration()
    {
        if (LVL % 3 != 0)
        {
            int achievable = 0;
            for (int i = 0; i < floorC; i++)
            {
                achievable += normalGenerationCheck.GetAvailable(new Vector2(floorForSpawnMob[0, 0], floorForSpawnMob[0, 1]), new Vector2(floorForSpawnMob[i, 0], floorForSpawnMob[i, 1]));
            }
            if (achievable == floorC)
            {
                return true;
            }
            else return false;
        }
        return true;
    }

    public void SpawnMob()
    {
        MobCountOnLvl = 0;
        if (LVL % 3 != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            mobCount = (int)Random.Range(LevelGenerator.LVL, LevelGenerator.LVL + 4f);

            floorCount = floorForSpawnMob.GetLength(0);
            floorNumber = (int)Random.Range(0f, floorCount);

            floorNumber = (int)Random.Range(0f, floorCount);

            Vector2 floorPos = new Vector2(floorForSpawnMob[floorNumber, 0], floorForSpawnMob[floorNumber, 1]);

            player.transform.position = new Vector3(floorPos.x, floorPos.y, -90);

            for (int i = 0; i < mobCount; i++)
            {
                for (int j = 0; j < floorCount; j++)
                {
                    floorNumber = (int)Random.Range(0f, floorCount);
                    floorPos = new Vector2(floorForSpawnMob[floorNumber, 0], floorForSpawnMob[floorNumber, 1]);
                    if (Vector2.Distance(floorPos, player.transform.position) >= distance)
                    {
                        Instantiate(mob1, new Vector3(floorPos.x, floorPos.y, -90), Quaternion.identity);
                        MobCountOnLvl++;
                        break;
                    }
                }
            }
            //print(MobCountOnLvl);
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");

            player.transform.position = new Vector3(3, 3, -90);

            if (LVL == 3)
            {
                Instantiate(boss2, new Vector3(7, 7, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else if (LVL == 6)
            {
                Instantiate(boss1, new Vector3(10, 10, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else if (LVL == 9)
            {
                Instantiate(boss3, new Vector3(10, 10, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else
            {
                float r = Random.Range(0f, 2f);

                if (r < 1)
                {
                    Instantiate(boss1, new Vector3(10, 10, -89), Quaternion.identity);
                    MobCountOnLvl++;
                }

                else if (r < 2)
                {
                    Instantiate(boss2, new Vector3(7, 7, -89), Quaternion.identity);
                    MobCountOnLvl++;
                }
            }
            
        }       
    }
        
    
    public void DecreaseMobCountOnLvl()
    {
        MobCountOnLvl--;
        if (MobCountOnLvl == 0)
        {
            GameObject Enemy = GameObject.FindGameObjectWithTag("Enemy");
            GameObject Portal = Instantiate(portal, new Vector3(Enemy.transform.position.x, Enemy.transform.position.y + 0.3f, -80), Quaternion.identity);
            Portal.GetComponent<NextLvl>().levelGenerator = gameObject;
        }
    }

    public float[] GetPos1Pos2()
    {
        int r1 = Random.Range(0, floorForSpawnMob.GetLength(0));
        float[] f = new float[2];
        f[0] = floorForSpawnMob[r1, 0];
        f[1] = floorForSpawnMob[r1, 1];
        return f;
    }
    //
    private void SpawnMenorah()
    {

        for (int i= 0; i< UwallForMenorah.GetLength(0); i++)
        {
        Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(UwallForMenorah[i, 0], UwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahU, new Vector3(UwallForMenorah[i, 0], UwallForMenorah[i, 1], -10), Quaternion.identity, gameObject.transform);
        }


        for (int i = 0; i < LwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(LwallForMenorah[i, 0], LwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahL, new Vector3(LwallForMenorah[i, 0], LwallForMenorah[i, 1], -10), Quaternion.identity, gameObject.transform);
        }
        

        for (int i = 0; i < RwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(RwallForMenorah[i, 0], RwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahR, new Vector3(RwallForMenorah[i, 0], RwallForMenorah[i, 1], -10), Quaternion.identity, gameObject.transform);
        }


        for (int i = 0; i < DwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(DwallForMenorah[i, 0], DwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahD, new Vector3(DwallForMenorah[i, 0], DwallForMenorah[i, 1], -10), Quaternion.identity, gameObject.transform);
        }

    }
}
