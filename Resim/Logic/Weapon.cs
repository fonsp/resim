namespace Resim.Logic
{
	public class MeleeWeapon:Weapon
	{
		public float Range;
		public float Damage;

		public MeleeWeapon(
			string name,
			int price,
			float damage,
			float range,
			float fireRate
			)
			: base(WeaponType.Melee, name, price, fireRate)
		{
			Range = range;
			Damage = damage;
		}
	}

	public class HitScanWeapon:Weapon
	{
		public bool Automatic;
		public float Damage;
		public bool RequiresReload;
		public int BulletsPerMagazine;

		public HitScanWeapon(
			string name,
			int price,
			float damage,
			float fireRate,
			bool automatic,
			bool requiresReload,
			int bulletsPerMagazine)
			: base(WeaponType.Hitscan, name, price, fireRate)
		{
			Automatic = automatic;
			Damage = damage;
			RequiresReload = requiresReload;
			BulletsPerMagazine = bulletsPerMagazine;
		}
	}

	public class Weapon
	{
		public float FireRate;
		public int Price;
		public WeaponType WeaponType;
		public string Name;

		public Weapon(WeaponType type, string name, int price, float fireRate)
		{
			FireRate = fireRate;
			Price = price;
			WeaponType = type;
			Name = name;
		}
	}

	public enum WeaponType
	{
		Melee,
		Hitscan,
		Projectile
	}
}