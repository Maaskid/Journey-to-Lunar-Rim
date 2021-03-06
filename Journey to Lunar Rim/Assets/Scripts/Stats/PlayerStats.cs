using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "New Player Stats")]
    public class PlayerStats : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("___Tank Einstellungen___")]
        public float tankInit;
        // [NonSerialized]
        public float TankRuntime;
        // [NonSerialized]
        public float verbrauchsZeit = 2f;

        [Header("___Schild Einstellungen___")]
        public float schadenMax;
        public float selbstReparaturZeit = 5f;
        private float schadenInit = 0;
        // [NonSerialized]
        public float SchadenRuntime;
        

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            TankRuntime = tankInit;
            SchadenRuntime = schadenInit;
        }
    }
}