using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpriteSheetAnimation_Data : IComponentData {
    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;


    public float4 uv;

    public float4x4 matrix;

}

partial struct SpriteSheetAnimationSystem : ISystem {

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        new SpriteSheetAnimationJob() {
            deltaTime = SystemAPI.Time.DeltaTime
        }
        .ScheduleParallel();

    }

    [BurstCompile]
    public partial struct SpriteSheetAnimationJob : IJobEntity {
        public float deltaTime;
        public void Execute(ref SpriteSheetAnimation_Data spriteSheetAnimationData, in LocalTransform localTransform) {
            spriteSheetAnimationData.frameTimer += deltaTime;
            while (spriteSheetAnimationData.frameTimer > spriteSheetAnimationData.frameTimerMax) {
                spriteSheetAnimationData.frameTimer -= spriteSheetAnimationData.frameTimerMax;
                spriteSheetAnimationData.currentFrame = (spriteSheetAnimationData.currentFrame + 1) % spriteSheetAnimationData.frameCount;


                float uvWidth = 1f / spriteSheetAnimationData.frameCount;
                float uvHeight = 1f;
                float uvOffsetX = uvWidth * spriteSheetAnimationData.currentFrame;
                float uvOffsetY = 0f;
                spriteSheetAnimationData.uv = new float4(uvWidth, uvHeight, uvOffsetX, uvOffsetY);

                float3 position = localTransform.Position;
                position.z = position.y * 0.01f;

                spriteSheetAnimationData.matrix = float4x4.TRS(
                    position,
                    localTransform.Rotation,
                    localTransform.Scale
                );
            }
        }
    }

}

