using GraphicsLibrary.Core;

namespace Resim.Logic
{
	/* Projectiles worden nog niet gebruikt
	 */
	public class Projectile:Entity
	{
		public bool ExplodeOnImpact;

		public Projectile(string name, Mesh mesh, bool explodeOnImpact)
			: base(name)
		{
			ExplodeOnImpact = explodeOnImpact;
		}
	}
	public class ProjectileWeapon:Weapon
	{
		public Projectile Projectile;

		public ProjectileWeapon(string name, int price, Projectile projectile, float fireRate)
			: base(WeaponType.Projectile, name, price, fireRate)
		{
			Projectile = projectile;
		}
	}
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
	/* De basisclass voor een weapon, alle andere weapons overriden deze class
	 */
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