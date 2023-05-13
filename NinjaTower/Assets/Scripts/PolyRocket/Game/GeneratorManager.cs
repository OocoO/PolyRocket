using System.Collections.Generic;
using PolyRocket.Game.Actor;
using PolyRocket.SO;
using UnityEngine;

namespace PolyRocket.Game
{
    public class GeneratorManager
    {
        private PrPlayer _player;
        private List<ElementGenerator> _generators;

        public GeneratorManager(PrPlayer player)
        {
            _player = player;
            var level = player.Level;
            _generators = new List<ElementGenerator>();

            foreach (var element in level.Config.m_Elements)
            {
                var generator = Object.Instantiate(element);
                generator.Init(level);
                _generators.Add(generator);
            }
        }

        public void Update()
        {
            foreach (var generator in _generators)
            {
                generator.Update();
            }
        }

        public void OnDestroy()
        {
            foreach (var generator in _generators)
            {
                Object.Destroy(generator);
            }
        }
    }
}