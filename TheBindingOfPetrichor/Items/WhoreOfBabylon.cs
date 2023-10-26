using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TheBindingOfPetrichor.Utils;
using static TheBindingOfPetrichor.Main;

namespace TheBindingOfPetrichor.Items
{
    class WhoreOfBabylon : ItemBase<WhoreOfBabylon>
    {
        public ConfigEntry<float> DamageBoost;


        public override string ItemName => "Whore of Babylon";

        public override string ItemLangTokenName => "WHORE_OF_BABYLON";

        public override string ItemPickupDesc => "Gain a significant damage boost at low health";

        public override string ItemFullDescription => "Upon reaching low health, gain a 500% damage boost";

        public override string ItemLore => "And the woman was arrayed in purple and scarlet colour, and decked with gold and precious stones and pearls, having a golden cup in her hand full of abominations and filthiness of her fornication";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Whore.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Whore_Of_Babylon_Icon_29.png");

        public static GameObject itemBodyModelPrefab;

        public ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

        public int ChatAmount = 0;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            itemBodyModelPrefab = ItemModel;
            var itemDisplay = itemBodyModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(itemBodyModelPrefab);


            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("md1CommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule()
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0.01f,0.01f,0.01f),
                }
            });

            return rules;
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            DamageBoost = config.Bind<float>("Item " + ItemName, "Damage Boost At Low Health", 5.0f, "Percentage of damage boost");

        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage; 
        }

        private void AddDamage(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs args)
        {

            var stack = GetCount(self);
            var healthComponent = self.healthComponent;
            if (healthComponent.combinedHealthFraction <= 0.50f)
            {
                args.damageMultAdd += 5.0f * stack;
                self.AddBuff()

                if (ChatAmount == 0)
                {
                    ChatAmount = 1;
                    Chat.AddMessage("What a horrible night to have a curse...");
                }
            }
            if (healthComponent.combinedHealthFraction > 0.50f) 
            {
                ChatAmount = 0;
            }

        }
    }
}
