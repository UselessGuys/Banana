using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class CharacterStats : MonoBehaviour
    {
        public int Health;
        public int Mana;
        public int Rage;

        public float MoveSpeed;
        public int JumpHeight;

        public int AttackSpeed;

        // Enemies Stats
        public int HearRange;
        public float VisionRange;
        public float VisionAngle;
    }
}
