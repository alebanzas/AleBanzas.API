using System.Threading.Tasks;

namespace ABServicios.Api
{
	public static class TaskHelpers
	{
		private static readonly Task<object> _completedTaskReturningNull = FromResult<object>(null);

		private static readonly Task _defaultCompleted = FromResult<AsyncVoid>(default(AsyncVoid));

		public static Task Completed()
		{
			return _defaultCompleted;
		}

		public static Task<TResult> FromResult<TResult>(
			TResult result)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetResult(result);
			return tcs.Task;
		}

		public static Task<object> NullResult()
		{
			return _completedTaskReturningNull;
		}

		private struct AsyncVoid {}
	}
}