namespace AspNetCoreIdentity.Permisson
{
	public static  class Permisson
	{
		public static class Stock 
		{
			public const string Read = "Permisson.Stock.Read";
			public const string Create = "Permisson.Stock.Create";
			public const string Update = "Permisson.Stock.Update";
			public const string Delete = "Permisson.Stock.Delete";
		}
		public static class Order
		{
			public const string Read = "Permisson.Order.Read";
			public const string Create = "Permisson.Order.Create";
			public const string Update = "Permisson.Order.Update";
			public const string Delete = "Permisson.Order.Delete";
		}

		public static class Catalog
		{
			public const string Read = "Permisson.Catalog.Read";
			public const string Create = "Permisson.Catalog.Create";
			public const string Update = "Permisson.Catalog.Update";
			public const string Delete = "Permisson.Catalog.Delete";
		}
	}
}
