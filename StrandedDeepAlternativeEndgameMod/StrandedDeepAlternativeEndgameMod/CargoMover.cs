using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beam;

namespace StrandedDeepAlternativeEndgameMod
{
    public class CargoMover : MonoBehaviour
    {
        public DateTime lastTime = DateTime.MinValue;
        public List<Vector3> Positions { get; set; }
        public Vector3 currentDestination = new Vector3();
        public float speedPerHour = 50.0f;

        void Start()
        {
            //Transform cargoTransform = this.transform.parent;
        }

        void Update()
        {
            return;

            Transform cargoTransform = this.transform.parent;

            DateTime newTime = GameTime.Now;
            TimeSpan delta = newTime.Subtract(lastTime);

            //transform.position

            //delta.TotalHours * speedPerHour;
            Debug.Log("Stranded Deep AlternativeEndgame Mod : try move the endgame cargo");
            cargoTransform.position = currentDestination; 

            for( int i = 0; i < Positions.Count; i++)
            {
                if (Mathf.Approximately(cargoTransform.position.x, Positions[i].x)
                    && Mathf.Approximately(cargoTransform.position.z, Positions[i].z))
                {
                    if (i < Positions.Count - 1)
                    {
                        currentDestination = Positions[i + 1];
                    }
                    else
                    {
                        currentDestination = Positions[0];
                    }
                    break;
                }
            }

            lastTime = newTime;
        }
    }
}
