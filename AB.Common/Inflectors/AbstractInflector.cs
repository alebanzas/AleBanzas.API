using System.Collections.Generic;
using System.Linq;

namespace AB.Common.Inflectors
{
	/// <summary>
	/// Base implementation.
	/// </summary>
	/// <remarks>
	/// Originally implemented by http://andrewpeters.net/inflectornet/
	/// </remarks>
	public abstract class AbstractInflector : IInflector
	{
		private readonly List<IRuleApplier> plurals = new List<IRuleApplier>();
		private readonly List<IRuleApplier> singulars = new List<IRuleApplier>();
		private readonly HashSet<string> uncountables = new HashSet<string>();

		protected virtual string ApplyFirstMatchRule(IEnumerable<IRuleApplier> rules, string word)
		{
			string result = word;

			if (!uncountables.Contains(word.ToLower()))
			{
				rules.Reverse().First(r => (result = r.Apply(word)) != null);
			}
			return result;
		}

		protected virtual string ApplyRules(IEnumerable<IRuleApplier> rules, string word)
		{
			string result = word;
			foreach (var rule in rules)
			{
				result = rule.Apply(result);
			}
			return result;
		}

		protected virtual void AddIrregular(string singular, string plural)
		{
			AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
			AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
		}

		protected virtual void AddUncountable(string word)
		{
			uncountables.Add(word.ToLower());
		}

		protected void AddPlural(string rule, string replacement)
		{
			plurals.Add(new NounsRule(rule, replacement));
		}

		protected void AddSingular(string rule, string replacement)
		{
			singulars.Add(new NounsRule(rule, replacement));
		}

		public virtual string Pluralize(string word)
		{
			return ApplyFirstMatchRule(plurals, word);
		}

		public virtual string Singularize(string word)
		{
			return ApplyFirstMatchRule(singulars, word);
		}

		public abstract string Ordinalize(string number);
	}
}