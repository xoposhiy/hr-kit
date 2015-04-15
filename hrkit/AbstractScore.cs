using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HrKit
{
	public class AbstractScore
	{
		public Dictionary<string, double> points = new Dictionary<string, double>();

		public virtual double Total
		{
			get
			{
				return points.Where(kv => !kv.Key.StartsWith("#"))
					.Select(kv => kv.Value)
					.Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
					.Sum();
			}
		}

		public virtual void UpdateWithAllScores(List<AbstractScore> allScores)
		{
		}

		public IEnumerable<string> GetDataRowHeader()
		{
			var keys = points.OrderBy(kv => kv.Key).Select(kv => kv.Key);
			return new[] { GetType().Name.Replace("Score", "") + "Total" }.Concat(keys);
		}

		public IEnumerable<string> GetDataRow()
		{
			var values = points.OrderBy(kv => kv.Key).Select(kv => kv.Value);
			return new[] { Total }.Concat(values).Select(v => v.ToString(CultureInfo.InvariantCulture));
		}
	}
}