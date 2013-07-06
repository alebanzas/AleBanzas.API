using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public class PollingFrequencer : IPollingFrequencer
	{
		private const int CeilingMultiplier = 30;
		private const int FloorMultiplier = 2;

		private readonly int ceiling;
		private readonly int floor;
		private readonly bool useGradualDecrease;
		private int current;

		public static PollingFrequencer For(TimeSpan estimatedTimeToProcessMessageBlock, bool useGradualDecrease = true)
		{
			return new PollingFrequencer(estimatedTimeToProcessMessageBlock, useGradualDecrease);
		}
		/// <summary>
		/// Create a new instance of <see cref="PollingFrequencer"/>
		/// </summary>
		/// <param name="estimatedTimeToProcessMessageBlock">Estimated time to process a "MessageBlock"</param>
		/// <param name="useGradualDecrease">False to decrease to floor immediately.</param>
		/// <remarks>
		/// Uses <paramref name="estimatedTimeToProcessMessageBlock"/> to calculate the ceiling and the floor of the frequence.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Where <paramref name="estimatedTimeToProcessMessageBlock"/> execeed one hour. </exception>
		public PollingFrequencer(TimeSpan estimatedTimeToProcessMessageBlock, bool useGradualDecrease = true)
		{
			if(estimatedTimeToProcessMessageBlock.TotalHours > 1)
			{
				throw new ArgumentOutOfRangeException("estimatedTimeToProcessMessageBlock", "Max value allowed is one hour.");
			}
			this.useGradualDecrease = useGradualDecrease;
			var estimatedMilliseconds = Convert.ToInt32(estimatedTimeToProcessMessageBlock.TotalMilliseconds);
			ceiling = estimatedMilliseconds*CeilingMultiplier;
			floor = estimatedMilliseconds*FloorMultiplier;
			current = floor;
		}

		/// <summary>
		/// Create a new instance of <see cref="PollingFrequencer"/>
		/// </summary>
		/// <param name="floor">minimum milliseconds</param>
		/// <param name="ceiling">max milliseconds</param>
		/// <param name="useGradualDecrease">False to decrease to <paramref name="floor"/> immediately.</param>
		public PollingFrequencer(int floor, int ceiling, bool useGradualDecrease = true)
		{
			if(floor > ceiling)
			{
				throw new ArgumentOutOfRangeException("floor", "The floor should be less than ceiling.");
			}
			this.useGradualDecrease = useGradualDecrease;
			this.ceiling = ceiling;
			this.floor = floor;
			current = floor;
		}

		/// <summary>
		/// The current milliseconds.
		/// </summary>
		/// <remarks>> it increase at each call.</remarks>
		public int Current
		{
			get
			{
				var actual = current;
				if (current < ceiling)
				{
					current *= 2;
				}
				return actual;
			}
		}

		public void Decrease()
		{
			if (useGradualDecrease && current > floor)
			{
				current /= 2;
				return;
			}
			current = floor;
		}
	}
}