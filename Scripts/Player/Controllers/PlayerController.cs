using System;
using Godot;
using IntoTheCrypt.Helpers;
using IntoTheCrypt.Messages;
using IntoTheCrypt.Models;

namespace IntoTheCrypt.Player.Controllers
{
	public class PlayerController : Spatial
	{
		#region Public

		#region Members
		public StatMenuController StatMenu { get; private set; }
		public Stats Stats;
		#endregion

		#region Member Methods
		public override void _Input(InputEvent @event)
		{
			switch (@event)
			{
				case InputEventKey key:
					switch (key.Scancode)
					{
						case (uint)KeyList.Escape:
							if (key.IsPressed())
							{
								GetTree().ChangeSceneTo(_mainMenuScene);
							}
							break;
						case (uint)KeyList.Tab:
							if (key.IsPressed())
							{
								StatMenu.ToggleActive();
							}
							break;
						default:
							break;
					}
					break;
				default:
					break;
			}
		}
		
		public override void _Process(float delta)
		{
			UpdateBleed(delta);
			UpdateToxic(delta);
			UpdateMenus(delta);
		}
		
		public override void _Ready()
		{
			LoadScenes();
			StatMenu = GetNode<StatMenuController>("StatMenuControl");

			Stats = new Stats(
				maxArmorRating: 1,
				maxHp: 100
			);
			StatMenu.SetActive(false);
			Stats.ArmorRating = Stats.MaxArmorRating;
			Stats.HP = Stats.MaxHP;
		}
		
		public void HandleDamage(DamagePlayerMessage damage)
		{
			DamageHelper.HandleDamage(Stats, damage);
		}
		#endregion

		#endregion
		
		#region Private

		#region Members
		protected float _bleedElapsedTime = 0f;
		private PackedScene _mainMenuScene;
		protected float _toxicElapsedTime = 0f;
		#endregion

		#region Member Methods
		private void LoadScenes()
		{
			_mainMenuScene = GD.Load<PackedScene>("res://Scenes/UI/MainMenu/MainMenu.tscn");
			if (_mainMenuScene == null)
			{
				throw new Exception("MainMenu scene did not load correctly");
			}
		}

		private void UpdateBleed(float delta)
		{
			if (Stats.Bleed == 0f)
			{
				_bleedElapsedTime = 0f;
				return;
			}
			_bleedElapsedTime += delta;
			// Accumulate bleed damage
			uint accumulatedDamage = 0;
			for (int i = 1; i <= _bleedElapsedTime; ++i)
			{
				accumulatedDamage += DamageHelper.DamageFromBleed(Stats);
				Stats.Bleed *= Stats.BleedReductionRatio;
			}
			if (Stats.Bleed <= 1f)
			{
				Stats.Bleed = 0f;
			}
			// Remove excess seconds that have passed since last update
			_bleedElapsedTime %= 1f;
			DamageHelper.Damage(Stats, accumulatedDamage);
		}

		private void UpdateMenus(float delta)
		{
			UpdateStatMenu(delta);
		}

		private void UpdateStatMenu(float delta)
		{
			StatMenu.UpdateStats(Stats);
		}

		private void UpdateToxic(float delta)
		{
			var transientToxic = Stats.TransientToxic;
			if (transientToxic == 0f)
			{
				_toxicElapsedTime = 0f;
				return;
			}
			_toxicElapsedTime += delta;
			// Reduce toxic
			uint toxicToRemove = 0;
			for (int i = 1; i <= _toxicElapsedTime; ++i)
			{
				toxicToRemove += Constants.TOXIC_DROP_RATE;
			}
			if (toxicToRemove > transientToxic)
			{
				toxicToRemove = transientToxic;
			}
			// Remove the transient toxicity
			Stats.Toxic -= toxicToRemove;
			// Remove excess seconds that have passed since last update
			_toxicElapsedTime %= 1f;
		}
		#endregion

		#endregion
	}
}