using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 10.5f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float GROUND_MOVE_SPEED = 30f;
    private const float CLOUD_MOVE_SPEED = 0.2f;
    private const float PIPE_DESTROY_LIMIT = -100f;
    private const float PIPE_SPAWN_LIMIT = 100f;
    private const float GROUND_DESTROY_LIMIT = -200f;
    private const float GROUND_SPAWN_LIMIT = 100f;
    private const float CLOUD_DESTROY_LIMIT = -160f;
    private const float CLOUD_SPAWN_LIMIT = 160f;
    private const float BIRD_X_POSITION = 0f;

    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float cloudSpawnTimer;
    private float gapSize;

    private int pipesSpawned;
    private int pipesPassedCount;

    private List<Pipe> pipeList;
    private List<Transform> groundList;
    private List<Transform> cloudList;

    private static Level Instance;

    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead
    }

    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }
    }

    private void CreatePipe(float Height, float xPos, bool createBottom)
    {
        //Setup Pipe Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().PFPipeHead);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + Height - PIPE_HEAD_HEIGHT * 0.5f;
        }
        else
        {
            pipeHeadYPosition = CAMERA_ORTHO_SIZE - Height + PIPE_HEAD_HEIGHT * 0.5f;
        }
        pipeHead.position = new Vector3(xPos, pipeHeadYPosition);

        //Setup Pipe Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().PFPipeBody);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = CAMERA_ORTHO_SIZE - Height;
        }
        pipeBody.position = new Vector3(xPos, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, Height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, Height);
        pipeBodyBoxCollider.offset = new Vector2(0f, Height * 0.5f);

        Pipe pipeObject = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipeObject);
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPos)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPos, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * 0.5f, xPos, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            bool isToTheRightOfTheBird = pipeList[i].GetXPosition() > BIRD_X_POSITION;
            pipeList[i].Move();
            if (isToTheRightOfTheBird && pipeList[i].GetXPosition() <= BIRD_X_POSITION)
            {
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }
            if (pipeList[i].GetXPosition() < PIPE_DESTROY_LIMIT)
            {
                pipeList[i].DestroySelf();
                pipeList.Remove(pipeList[i]);
                i--;
            }
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float MinHeight = gapSize * 0.5f + heightEdgeLimit;
            float TotalHeight = CAMERA_ORTHO_SIZE * 2f;
            float MaxHeight = TotalHeight - gapSize * 0.5f - heightEdgeLimit;
            float RandomHeight = Random.Range(MinHeight, MaxHeight);
            int RandomGapSize = 0;
            switch (gapSize)
            {
                case 50:
                    RandomGapSize = Random.Range(50, 40);
                    break;

                case 40:
                    RandomGapSize = Random.Range(40, 30);
                    break;

                case 30:
                    RandomGapSize = Random.Range(30, 20);
                    break;

                case 20:
                    RandomGapSize = Random.Range(25, 20);
                    break;
            }
            CreateGapPipes(RandomHeight, RandomGapSize, PIPE_SPAWN_LIMIT);
        }
    }

    private Transform GetCloudPrefabTransform()
    {
        switch (Random.Range(0, 3))
        {
            case 0: return GameAssets.GetInstance().PFCloud1;

            case 1: return GameAssets.GetInstance().PFCloud2;

            case 2: return GameAssets.GetInstance().PFCloud3;
        }
        return GameAssets.GetInstance().PFCloud1;
    }

    private void SpawnInitialClouds()
    {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        float cloudY = 30f;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, cloudY, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private void HandleCloudsMovement()
    {
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0)
        {
            float cloudSpawnTimerMax = Random.Range(10, 20);
            cloudSpawnTimer = cloudSpawnTimerMax;
            float cloudY = 30f;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_LIMIT, cloudY, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        for (int i = 0; i < cloudList.Count; i++)
        {
            cloudList[i].position += new Vector3(-1, 0, 0) * CLOUD_MOVE_SPEED;
            if (cloudList[i].position.x < CLOUD_DESTROY_LIMIT)
            {
                Destroy(cloudList[i].gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 192f;
        groundTransform = Instantiate(GameAssets.GetInstance().PFGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().PFGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().PFGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGroundMovement()
    {
        foreach (Transform groundTransform in groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * GROUND_MOVE_SPEED * Time.deltaTime;

            if (groundTransform.position.x < GROUND_DESTROY_LIMIT)
            {
                float rightMostXPosition = -120f;
                for (int i = 0; i < groundList.Count; i++)
                {
                    if (groundList[i].position.x > rightMostXPosition)
                    {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }

                float groundWidth = 192f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth-0.5063f, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                pipeSpawnTimerMax = 1.4f;
                gapSize = 50f;
                break;

            case Difficulty.Medium:
                pipeSpawnTimerMax = 1.3f;
                gapSize = 40f;
                break;

            case Difficulty.Hard:
                pipeSpawnTimerMax = 1.2f;
                gapSize = 30f;
                break;

            case Difficulty.Impossible:
                pipeSpawnTimerMax = 1.1f;
                gapSize = 20f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30)
        {
            return Difficulty.Impossible;
        }
        if (pipesSpawned >= 20)
        {
            return Difficulty.Hard;
        }
        if (pipesSpawned >= 10)
        {
            return Difficulty.Medium;
        }
        return Difficulty.Easy;
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount / 2;
    }

    public static Level GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        Instance = this;
        pipeList = new List<Pipe>();
        SpawnInitialGround();
        SpawnInitialClouds();
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    // Start is called before the first frame update
    private void Start()
    {
        BirdControl.GetInstance().OnDied += Bird_OnDied;
        BirdControl.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        state = State.BirdDead;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGroundMovement();
            HandleCloudsMovement();
        }
    }
}
