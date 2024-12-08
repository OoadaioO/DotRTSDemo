using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameHandler : MonoBehaviour, IDisposable {

    public Mesh Mesh {
        get => mesh;
    }
    public Material Material {
        get => material;
    }

    public int EntityCount => entityCount;

    public static GameHandler Instance;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;

    [SerializeField] int entityCount;


    public ComputeBuffer uvBuffer;
    public ComputeBuffer matrixBuffer;


    private void Awake() {
        Instance = this;
    }

    private void Start() {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(typeof(LocalTransform), typeof(TestTag), typeof(SpriteSheetAnimation_Data));

        uvBuffer = new ComputeBuffer(entityCount, sizeof(float) * 4);
        matrixBuffer = new ComputeBuffer(entityCount, sizeof(float) * 16);


        NativeArray<Entity> entityArray = new NativeArray<Entity>(entityCount, Allocator.Temp);

        entityManager.CreateEntity(entityArchetype, entityArray);
        foreach (Entity entity in entityArray) {
            entityManager.SetComponentData(entity, new LocalTransform {
                Position = new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-3f, 3f), 0f),
                Rotation = quaternion.identity,
                Scale = UnityEngine.Random.Range(1f, 3f)
            });

            entityManager.SetComponentData(entity, new SpriteSheetAnimation_Data {
                currentFrame = UnityEngine.Random.Range(0, 15),
                frameCount = 16,
                frameTimer = UnityEngine.Random.Range(0f, 1f),
                frameTimerMax = 0.05f
            });
        }
        entityArray.Dispose();

    }

    public void Dispose() {

        uvBuffer?.Release();
        uvBuffer = null;
        matrixBuffer?.Release();
        matrixBuffer = null;
    }
}
