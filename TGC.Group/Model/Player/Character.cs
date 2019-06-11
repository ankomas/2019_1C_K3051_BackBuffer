using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using BulletSharp;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Consumables;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Recipes;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Player
{
    public class Character
    {
        private static readonly Stats BaseStats = new Stats(35, 100);

        public Stats MaxStats => BaseStats + this.equipment.ExtraStats();

        public Stats ActualStats { get; }
        public Inventory Inventory { get; } = new Inventory(30);

        private Equipment equipment = new Equipment();
        
        public Character()
        {
            this.ActualStats = this.MaxStats;
        }

        public void UpdateStats(Stats newStats)
        {
            if (isAsphyxiating(newStats)) newStats.Life += newStats.Oxygen * 10;
            
            this.ActualStats.Update(newStats, this.MaxStats);
        }

        private bool isAsphyxiating(Stats newStats)
        {
            return this.ActualStats.Oxygen <= 0 && newStats.Oxygen < 0;
        }

        public bool IsDead()
        {
            //return this.ActualStats.Life <= 0;
            return false;
        }

        public void GiveItem(IItem item)
        {
            this.Inventory.AddItem(item);
        }

        public void Equip(IEquipable equipable)
        {
            this.Inventory.RemoveItem(equipable);
            this.equipment.AddEquipable(equipable);
        }

        public void RemoveIngredients(IEnumerable<Ingredient> recipeIngredients)
        {
            this.Inventory.RemoveIngredients(recipeIngredients);
        }

        public void RemoveItem(IItem item)
        {
            this.Inventory.RemoveItem(item);
        }

        public void Hit(int quantity)
        {
            this.ActualStats.Update(new Stats(0, -quantity), this.MaxStats);
        }

        public void Consume(IConsumable consumable)
        {
            this.ActualStats.Update(consumable.stats, MaxStats);
            RemoveItem(consumable);
        }

        public bool CanCraft(ICrafteable item)
        {
            return item.Recipe.CanCraft(this.Inventory.AsIngredients());
        }
    }
}
