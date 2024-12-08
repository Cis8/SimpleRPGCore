using ElectricDrill.SimpleRpgCore;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    public class ExpSource : MonoBehaviour, IExpSource
    {
        [SerializeField] private long exp;
        public bool Harvested { get; set; } = false;
    
        public long Exp {
            get {
                if (Harvested)  {
                    return 0;
                }
                
                Harvested = true;
                return exp;
            }
        }

        void Start() {
        
        }
    
        void Update() {
        
        }
    }   
}
