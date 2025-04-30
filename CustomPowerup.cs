using System;
using System.Collections.Generic;
using UnityEngine;
using PowerupAPI.Base;
using System.Collections;
using System.Numerics;

namespace PowerupAPI.Powerups
{
    public class CustomPowerup
    {
        public UnityEngine.GameObject prefab = null;
        public UnityEngine.AudioClip sound = null;
        public string displayName;
        public string description;
        public string unlockText;
        public PowerupScript.Identifier identifier;

        public PowerupScript.Category category { get; set; } = PowerupScript.Category.normal;
        public PowerupScript.Archetype archetype { get; set; } = PowerupScript.Archetype.generic;
        public bool isInstantPowerup { get; set; } = false;
        public int maxBuyTimes { get; set; } = -1;
        public float storeRerollChance { get; set; } = 0f;
        public int startingPrice { get; set; } = 1;
        public BigInteger unlockPrice { get; set; } = -1L;
        public PowerupScript.PowerupEvent onEquip = null;
        public PowerupScript.PowerupEvent onUnequip = null;
        public PowerupScript.PowerupEvent onPutInDrawer = null;
        public PowerupScript.PowerupEvent onThrowAway = null;

        public CustomPowerup(
            PowerupScript.Category category = PowerupScript.Category.normal,
            PowerupScript.Archetype archetype = PowerupScript.Archetype.generic,
            bool isInstantPowerup = false,
            int maxBuyTimes = -1,
            float storeRerollChance = 0f,
            int startingPrice = 1,
            BigInteger unlockPrice = default
        )
        {
            this.category = category;
            this.archetype = archetype;
            this.isInstantPowerup = isInstantPowerup;
            this.maxBuyTimes = maxBuyTimes;
            this.storeRerollChance = storeRerollChance;
            this.startingPrice = startingPrice;
            this.unlockPrice = unlockPrice == default ? new BigInteger(-1) : unlockPrice;
        }

       /* public void SetDisplayName(string nameKey, string displayName, string language){
            NameKey = nameKey;
            //CustomPowerupAPI.AddI2Term(nameKey, displayName, language);
        }

        public void SetDescription(string descKey, string description, string language){
            DescKey = descKey;
            //CustomPowerupAPI.AddI2Term(descKey, description, language);
        }

        public void SetUnlockText(string unlockKey, string unlockText, string language){
            UnlockKey = unlockKey;
            //CustomPowerupAPI.AddI2Term(unlockKey, unlockText, language);
        }*/
    }
}