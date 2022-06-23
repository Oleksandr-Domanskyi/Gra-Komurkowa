using System;


public delegate void ShowBox(int x, int y, int ball);
public delegate void PlayCut();

public class Lines
{
    public const int SIZE= 9;
    public const int BALLS = 7;
    const int ADD_BALLS = 3;
    Random random = new Random();
    ShowBox showBox;
    PlayCut playCut;
    ScoreManager score;

    int[,]map;
    int fromX, fromY;
    bool isBallSelected;

    public Lines(ShowBox showBox, PlayCut playCut)
    {
        this.showBox = showBox;
        this.playCut = playCut;
        map = new int[SIZE, SIZE];
    }
    
    public void Start()
    {
        ClearMap();
        AddRandromBalls();
        isBallSelected = false;
    }

    public void Click(int x, int y) 
    {
        if (IsGameOver())
            Start();
        else
             if (map[x, y] == 0)
            TakeBall(x, y);
            else
            MoveBall(x, y);

    }
    private void TakeBall(int x, int y)
    {
        if (!isBallSelected) return;
        if (!CanMove(x, y)) return;
        SetMap(x, y, map[fromX, fromY]);
        SetMap(fromX, fromY, 0);
        isBallSelected = false;
        if (!Cutlines())
        {
            AddRandromBalls();
            Cutlines();
        }

    }
    private void MoveBall(int x,int y)
    {
        fromX = x;
        fromY = y;
        isBallSelected = true;
       



    }

    public bool IsGameOver()
    {
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                if (map[x, y] == 0)
                    return false;
        return true;
    }
    private void ClearMap()
    {
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                SetMap(x,y,0);
    }
    private bool OnMap(int x, int y)
    {
        return x >= 0 && x < SIZE && y >= 0 && y < SIZE;
    }
    private int GetMap(int x, int y)
    {
        if (!OnMap(x, y)) return 0;
        return map[x, y];
    }
    private void SetMap(int x, int y,int ball)
    {
        map[x, y] = ball;
        showBox(x, y, ball);
    }
    private void AddRandromBalls()
    {
        for (int j = 0; j < ADD_BALLS; j++)
            AddRandromBall();
    }

    private void AddRandromBall()
    {
        int x, y;
        int loop = SIZE * SIZE;
        do
        {
            x = random.Next(SIZE);
            y = random.Next(SIZE);
            if (--loop <= 0) return;
        } while (map[x, y] > 0);
        int ball = 1 + random.Next(BALLS - 1);
        SetMap(x, y, ball);
    }

    private bool[,] mark;

    public bool Cutlines()
    {
        int balls = 0;
        mark = new bool[SIZE, SIZE];
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
            {
                balls += Cutlines(x, y, 1, 0);
                balls += Cutlines(x, y, 0, 1);
                balls += Cutlines(x, y, 1, 1);
                balls += Cutlines(x, y, -1, 1);
            }
        if(balls>0)
        {
            
            playCut();
            for (int x = 0; x < SIZE; x++)
                for (int y = 0; y < SIZE; y++)
                    if (mark[x, y])
                        SetMap(x, y, 0);
            return true;
        }
        return false;
    }
    private int Cutlines(int x0, int y0,int sx,int sy)
    {
        int ball = map[x0, y0];
        if (ball == 0) return 0;
        int count = 0;
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
            count++;
        if (count < 5)
            return 0;
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
            mark[x,y]=true;
        return count;
    }

    private bool[,] used;
    private bool CanMove(int toX, int toY)
    {
        used = new bool[SIZE, SIZE];
        Walk(fromX, fromY, true);
        return used[toX, toY];
    }
    private void Walk(int x, int y, bool start = false)
    {
        if (!start)
        {
            if (!OnMap(x, y)) return;
            if (map[x, y] > 0) return;
            if (used[x, y]) return;
        }
        used[x, y] = true;
        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }
}
