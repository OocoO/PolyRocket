namespace NinjaTower.Battle
{
	public abstract class EnumBase
	{
	}
	
	public class EnemyType : EnumBase
	{
		public readonly int ID;
		public readonly string Address;
		public int MaxHp = 10;
		
		public EnemyType (string address, int id)
		{
			Address = address;
			ID = id;
		}

		public static EnemyType Red = new EnemyType(null, 10001);
	}
}