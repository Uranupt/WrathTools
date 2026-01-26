using System.Collections.Generic;

namespace WrathTools
{
	public static class ListPool<T>
	{

		private static readonly Stack<List<T>> _pool = new();

		public static List<T> Get()
		{
			return _pool.Count > 0 ? _pool.Pop() : new List<T>();
		}

		public static void Store(List<T> list)
		{
			list.Clear();
			_pool.Push(list);
		}

		public static LeaseScope<List<T>> Lease()
		{
			return new LeaseScope<List<T>>(Get(), Store, true);
		}

	}

}