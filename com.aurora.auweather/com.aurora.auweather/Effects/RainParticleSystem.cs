﻿using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Com.Aurora.AuWeather.Effects
{
    // 简单雨滴是一个矩形，从画布顶部出发，以一个固定角度和初速度，以 g(?) 的加速度下落，同时，雨滴纹理的旋转角度应与运动方向相同
    // 下落到画布底部后，粒子被回收。同时，上方的出发位置是随机的，雨的规模(计算加速度、速度、缩放和角度)可以被用户设置
    public class RainParticleSystem : ParticleSystem
    {
        private RainLevel rainLevel;
        private CanvasBitmap snowbitmap;
        private CanvasBitmap rainbitmap;
        private Vector2 snowCenter;
        private Rect snowBounds;
        private Rect rainBounds;
        private Vector2 rainCenter;

        /// <summary>
        /// 根据雨的规模设置初始化参数
        /// </summary>
        /// <param name="rainLevel"></param>
        public RainParticleSystem(RainLevel rainLevel)
        {
            this.rainLevel = rainLevel;
            this.InitializeConstants();
        }
        protected override void InitializeConstants()
        {
            bitmap = rainbitmap;
            bitmapCenter = rainCenter;
            bitmapBounds = rainBounds;
            minLifetime = 0;
            maxLifetime = 0;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            switch (rainLevel)
            {
                case RainLevel.light:
                    InitializeLight();
                    break;
                case RainLevel.moderate:
                    Initializemoderate();
                    break;
                case RainLevel.heavy:
                    Initializeheavy();
                    break;
                case RainLevel.extreme:
                    Initializeextreme();
                    break;
                case RainLevel.sSnow:
                    InitializesSnow();
                    break;
                case RainLevel.lSnow:
                    InitializelSnow();
                    break;
                default:
                    InitializeLight();
                    break;
            }
            blendState = CanvasBlend.Add;
        }

        private void InitializelSnow()
        {
            bitmap = snowbitmap;
            bitmapCenter = snowCenter;
            bitmapBounds = snowBounds;
            minRotationAngle = 1;
            maxRotationAngle = 2;

            minInitialSpeed = 80;
            maxInitialSpeed = 120;

            minAcceleration = 0;
            maxAcceleration = 40;

            minScaleX = 0.3f;
            maxScaleX = 0.6f;

            minScaleY = 0.3f;
            maxScaleY = 0.6f;

            minNumParticles = 2;
            maxNumParticles = 3;
        }

        private void InitializesSnow()
        {
            bitmap = snowbitmap;
            bitmapCenter = snowCenter;
            bitmapBounds = snowBounds;
            minRotationAngle = 1;
            maxRotationAngle = 2;

            minInitialSpeed = 50;
            maxInitialSpeed = 150;

            minAcceleration = 0;
            maxAcceleration = 25;

            minScaleX = 0.2f;
            maxScaleX = 0.4f;

            minScaleY = 0.2f;
            maxScaleY = 0.4f;

            minNumParticles = 0;
            maxNumParticles = 2;
        }

        private void Initializeextreme()
        {
            minRotationAngle = 1.5708f;
            maxRotationAngle = 1.5708f;

            minInitialSpeed = 1800;
            maxInitialSpeed = 2000;

            minAcceleration = 1200;
            maxAcceleration = 1500;

            minScaleX = 0.6f;
            maxScaleX = 0.9f;

            minScaleY = 0.9f;
            maxScaleY = 4;

            minNumParticles = 11;
            maxNumParticles = 12;
        }

        private void Initializeheavy()
        {
            minRotationAngle = 1.54f;
            maxRotationAngle = 1.57f;

            minInitialSpeed = 1400;
            maxInitialSpeed = 1600;

            minAcceleration = 900;
            maxAcceleration = 1000;

            minScaleX = 0.6f;
            maxScaleX = 0.9f;

            minScaleY = 0.9f;
            maxScaleY = 2;

            minNumParticles = 6;
            maxNumParticles = 7;
        }

        private void Initializemoderate()
        {
            minRotationAngle = 1.45f;
            maxRotationAngle = 1.52f;

            minInitialSpeed = 900;
            maxInitialSpeed = 1200;

            minAcceleration = 600;
            maxAcceleration = 800;

            minScaleX = 0.6f;
            maxScaleX = 0.9f;

            minScaleY = 0.9f;
            maxScaleY = 1.3f;

            minNumParticles = 2;
            maxNumParticles = 3;
        }

        private void InitializeLight()
        {
            minRotationAngle = 1.404f;
            maxRotationAngle = 1.484f;

            minInitialSpeed = 600;
            maxInitialSpeed = 950;

            minAcceleration = 400;
            maxAcceleration = 600;

            minScaleX = 0.6f;
            maxScaleX = 0.9f;

            minScaleY = 0.7f;
            maxScaleY = 1;

            minNumParticles = 0;
            maxNumParticles = 2;
        }

        public override async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            snowbitmap = await CanvasBitmap.LoadAsync(resourceCreator, "Assets/snow.png");
            rainbitmap = await CanvasBitmap.LoadAsync(resourceCreator, "Assets/rain.png");
            snowCenter = snowbitmap.Size.ToVector2() / 2;
            snowBounds = snowbitmap.Bounds;
            rainCenter = rainbitmap.Size.ToVector2() / 2;
            rainBounds = rainbitmap.Bounds;
            ChangeConstants(this.rainLevel);
        }

        /// <summary>
        /// 根据旋转角度确定下落角度
        /// </summary>
        /// <returns></returns>
        private Vector2 PickDirection(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// 更新粒子物理属性，如果粒子超过边界，将其回收
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <param name="size"></param>
        public void Update(float elapsedTime, Vector2 size)
        {
            for (int i = ActiveParticles.Count - 1; i >= 0; i--)
            {
                var p = ActiveParticles[i];
                if (p.Position.X > 0 - size.Y * (float)Math.Tan(1.5708 - (minRotationAngle + maxRotationAngle) / 2) && p.Position.X <= size.X && p.Position.Y <= size.Y)
                {
                    p.Update(elapsedTime);
                }
                else
                {
                    ActiveParticles.RemoveAt(i);
                    FreeParticles.Push(p);
                }
            }
        }

        /// <summary>
        /// 获得画布尺寸，在画布顶部生成粒子
        /// </summary>
        /// <param name="size"></param>
        public void AddRainDrop(Vector2 size)
        {
            int numParticles = Tools.Random.Next(minNumParticles, maxNumParticles);
            for (int i = 0; i < numParticles; i++)
            {
                Particle particle = (FreeParticles.Count > 0) ? FreeParticles.Pop() : new Particle();
                float x = Tools.RandomBetween(0 - size.Y * (float)Math.Tan(1.5708 - (minRotationAngle + maxRotationAngle) / 2), size.X);
                InitializeParticle(particle, new Vector2(x, -5));
                ActiveParticles.Add(particle);
            }
        }

        protected override void InitializeParticle(Particle particle, Vector2 where)
        {
            float velocity = Tools.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration = Tools.RandomBetween(minAcceleration, maxAcceleration);
            float lifetime = Tools.RandomBetween(minLifetime, maxLifetime);
            float scaleX = Tools.RandomBetween(minScaleX, maxScaleX);
            float scaleY = Tools.RandomBetween(minScaleY, maxScaleY);
            float rotationSpeed = Tools.RandomBetween(minRotationSpeed, maxRotationSpeed);
            float rotation = Tools.RandomBetween(minRotationAngle, maxRotationAngle);
            Vector2 direction = PickDirection(rotation);
            particle.Initialize(where, velocity * direction, acceleration * direction, lifetime, scaleX, scaleY, rotation, rotationSpeed);
        }

        internal void ChangeConstants(RainLevel rainLevel)
        {
            bitmap = rainbitmap;
            bitmapCenter = rainCenter;
            bitmapBounds = rainBounds;
            switch (rainLevel)
            {
                case RainLevel.light:
                    InitializeLight();
                    break;
                case RainLevel.moderate:
                    Initializemoderate();
                    break;
                case RainLevel.heavy:
                    Initializeheavy();
                    break;
                case RainLevel.extreme:
                    Initializeextreme();
                    break;
                case RainLevel.sSnow:
                    InitializesSnow();
                    break;
                case RainLevel.lSnow:
                    InitializelSnow();
                    break;
                default:
                    InitializeLight();
                    break;
            }
        }
    }
}
