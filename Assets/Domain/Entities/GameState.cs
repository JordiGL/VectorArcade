// Assets/Domain/Services/GameState.cs
using System.Collections.Generic;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Domain.Services
{
    public sealed class GameState
    {
        public Player Player = new();
        public List<Asteroid> Asteroids = new();
        public List<Planet> Planets = new();
        public List<Bullet> Bullets = new();
        public List<Missile> Missiles = new List<Missile>();
        public List<Item> Items = new List<Item>();
        public List<Comet> Comets = new();
        public int Score = 0;
        public float TimeSinceStart = 0f;
        public bool GameOver = false;
        public CometRules CometRules; // referencia a reglas para acceso desde casos de uso
    }
}
