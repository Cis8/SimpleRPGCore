using ElectricDrill.SimpleRpgCore;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    public class ExpSource : MonoBehaviour, IExpSource
    {
        [SerializeField] private long exp;
        private bool _harvested = false;
    
        public long Exp {
            get {
                if (_harvested)  {
                    return 0;
                }
                
                _harvested = true;
                return exp;
            }
        }

        void Start() {
        
        }
    
        void Update() {
        
        }
    }   
}
