using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [CreateAssetMenu(fileName = "New Characteristic", menuName = "Simple RPG/Characteristic")]
    [Serializable]
    public class Characteristic : BoundedValue
    {

    }
}
