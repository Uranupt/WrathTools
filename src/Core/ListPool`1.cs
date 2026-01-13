using System.Collections.Generic;

namespace WrathTools
{
	public static class ListPool<T>
	{

		private static readonly Stack<List<T>> _pool = new();

		public static List<T> Get()
		{
			if(_pool.Count > 0)
			{
				return _pool.Pop();
			}
			return new List<T>();
		}

		public static void Store(List<T> list)
		{
			list.Clear();
			_pool.Push(list);
		}

	}

	public static class ListPoolExtensions
	{

		public static void StoreInPool<T>(this List<T> list)
		{
			ListPool<T>.Store(list);
		}

	}

}