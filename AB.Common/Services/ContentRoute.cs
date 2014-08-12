using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AB.Common.Services
{
	public class ContentRouteInfo<TResult>
	{
		public ContentRouteInfo(int contentRouteHash, string pattern, TResult value, IDictionary<string, string> variables)
		{
			ContentRouteHash = contentRouteHash;
			MatchPattern = pattern;
			Value = value;
			Variables = variables;
		}

		public string MatchPattern { get; private set; }

		public TResult Value { get; private set; }

		public IDictionary<string, string> Variables { get; private set; }

		public int ContentRouteHash { get; private set; }
	}

	public class ContentRoute<TResult>
	{
		private readonly int hashCode;
		private readonly bool patternContainsDns;
		private readonly IList<string> urlSegments;
		private readonly string urlToMatch;
		private readonly TResult value;
		private Dictionary<string, Func<string, bool>> constraints;

		public ContentRoute(string urlToMatch, TResult value) : this(urlToMatch, value, null) {}

		public ContentRoute(string urlToMatch, TResult value, IDictionary<string, string> constraintsExpressions)
		{
			if (IsNullOrWhiteSpace(urlToMatch))
			{
				throw new ArgumentNullException("urlToMatch");
			}
			this.urlToMatch = urlToMatch;
			this.value = value;
			urlSegments = SplitUrlToPathSegmentStrings(urlToMatch);
			patternContainsDns = ContainsDns(urlToMatch);
			Dictionary<string, string> tmpConstraints = ConfigureVariablesConstraints(constraintsExpressions);
			hashCode = CalculateHashCode(urlSegments, tmpConstraints.Values);
		}

		public string Pattern
		{
			get { return urlToMatch; }
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ContentRoute<TResult>);
		}

		public bool Equals(ContentRoute<TResult> other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return GetHashCode() == other.GetHashCode();
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public TResult GetRouteValue(Uri url)
		{
			return GetRouteValue(url, null);
		}

		public TResult GetRouteValue(Uri url, object dynamicConstraintsValues)
		{
			if (Match(url, dynamicConstraintsValues))
			{
				return value;
			}
			throw new ArgumentOutOfRangeException();
		}

		public ContentRouteInfo<TResult> GetRouteInfo(Uri uri)
		{
			return GetRouteInfo(uri, null);
		}

		public ContentRouteInfo<TResult> GetRouteInfo(Uri uri, object dynamicConstraintsValues)
		{
			IDictionary<string, string> variables;
			if (Match(uri, ObjectToDictionary(dynamicConstraintsValues), out variables))
			{
				return new ContentRouteInfo<TResult>(GetHashCode(), Pattern, value, variables);
			}
			throw new ArgumentOutOfRangeException();
		}

		public bool TryGetRouteInfo(Uri uri, out ContentRouteInfo<TResult> contentRouteInfo)
		{
			return TryGetRouteInfo(uri, null, out contentRouteInfo);
		}

		public bool TryGetRouteInfo(Uri uri, object dynamicConstraintsValues, out ContentRouteInfo<TResult> contentRouteInfo)
		{
			IDictionary<string, object> dynValues = ObjectToDictionary(dynamicConstraintsValues);
			IDictionary<string, string> variables;
			if (Match(uri, dynValues, out variables))
			{
				contentRouteInfo = new ContentRouteInfo<TResult>(GetHashCode(), Pattern, value, variables);
				return true;
			}
			contentRouteInfo = null;
			return false;
		}

		public bool TryGetRouteValue(Uri url, out TResult routeData)
		{
			return TryGetRouteValue(url, null, out routeData);
		}

		public bool TryGetRouteValue(Uri url, object dynamicConstraintsValues, out TResult routeData)
		{
			if (Match(url, dynamicConstraintsValues))
			{
				routeData = value;
				return true;
			}
			routeData = default(TResult);
			return false;
		}

		public bool Match(Uri url)
		{
			IDictionary<string, string> variables;
			return Match(url, null, out variables);
		}

		public bool Match(Uri url, object dynamicConstraintsValues)
		{
			IDictionary<string, object> dynValues = ObjectToDictionary(dynamicConstraintsValues);

			IDictionary<string, string> variables;
			return Match(url, dynValues, out variables);
		}

		private bool Match(Uri uri, IEnumerable<KeyValuePair<string, object>> dynamicVariables, out IDictionary<string, string> variables)
		{
			variables = new Dictionary<string, string>(20);
			if (uri == null)
			{
				return false;
			}
			string urlToCheck = PurgeDnsIfNeeded(uri);
			if (urlToMatch.Equals(urlToCheck))
			{
				return true;
			}
			IList<string> toMatchSegments = SplitUrlToPathSegmentStrings(urlToCheck);
			if (urlSegments.Count > toMatchSegments.Count)
			{
				return false;
			}
			for (int i = 0; i < urlSegments.Count; i++)
			{
				bool segmenIsAVariable = IsVariableSegment(urlSegments[i]);
				string matchSegment = toMatchSegments[i];
				if (!urlSegments[i].Equals(matchSegment) && !segmenIsAVariable)
				{
					variables.Clear();
					return false;
				}
				if (segmenIsAVariable)
				{
					string variableName = GetVariableName(urlSegments[i]);
					variables[variableName] = !matchSegment.EndsWith(".aspx") ? matchSegment : matchSegment.Substring(0, matchSegment.Length - 5);
				}
			}
			if (dynamicVariables != null)
			{
				foreach (var dynamicVariable in dynamicVariables)
				{
					string variableValue = Convert.ToString(dynamicVariable.Value, CultureInfo.InvariantCulture);
					variables[dynamicVariable.Key] = variableValue;
				}
			}

			foreach (var constraint in constraints)
			{
				string variableValue;
				if (!variables.TryGetValue(constraint.Key, out variableValue))
				{
					variableValue = "";
				}
				if (!constraint.Value(variableValue))
				{
					return false;
				}
			}
			return true;
		}

		private string GetVariableName(string urlSegment)
		{
			return urlSegment.Trim('{', '}');
		}

		private static IDictionary<string, object> ObjectToDictionary(object dynamicConstraintsValues)
		{
			IDictionary<string, object> dynValues = null;
			if (dynamicConstraintsValues != null)
			{
				dynValues = dynamicConstraintsValues as IDictionary<string, object>;
				if (dynValues == null)
				{
					dynValues = new Dictionary<string, object>();
					foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicConstraintsValues))
					{
						dynValues[propertyDescriptor.Name] = propertyDescriptor.GetValue(dynamicConstraintsValues);
					}
				}
			}
			return dynValues;
		}

		private string PurgeDnsIfNeeded(Uri url)
		{
			if (patternContainsDns)
			{
				return url.ToString();
			}

			return url.PathAndQuery;
		}

		private static IList<string> SplitUrlToPathSegmentStrings(string url)
		{
			var list = new List<string>();
			if (IsNullOrWhiteSpace(url))
			{
				return list;
			}

			string urlToCheck;
			IEnumerable<string> dnsSegments = AdjustWithDnsSegments(url, out urlToCheck);
			string[] urlSegments = urlToCheck.Split('/').Where(s => s != null && !string.Empty.Equals(s) && !"http:".Equals(s) && !"https:".Equals(s)).ToArray();
			IEnumerable<string> queryStringSegments = AdjustWithQueryStringSegments(urlSegments);

			list.AddRange(dnsSegments);
			list.AddRange(urlSegments);
			list.AddRange(queryStringSegments);
			return list;
		}

		private static bool IsNullOrWhiteSpace(string value)
		{
			if (value == null)
			{
				return true;
			}

			return value.All(Char.IsWhiteSpace);
		}

		private static IEnumerable<string> AdjustWithDnsSegments(string url, out string urlToCheck)
		{
			if (!ContainsDns(url))
			{
				urlToCheck = url;
				return Enumerable.Empty<string>();
			}
			string protocolCleaned = url;
			if (url.StartsWith("http://"))
			{
				protocolCleaned = url.Substring(7);
			}
			else if (url.StartsWith("https://"))
			{
				protocolCleaned = url.Substring(8);
			}
			int indexOfFirstSlash = protocolCleaned.IndexOf('/');
			bool isJustDns = indexOfFirstSlash < 0;

			string dns = isJustDns ? protocolCleaned : protocolCleaned.Substring(0, indexOfFirstSlash);
			urlToCheck = isJustDns ? string.Empty : protocolCleaned.Substring(indexOfFirstSlash);
			return dns.Split('.').Where(s => s != null && !string.Empty.Equals(s));
		}

		private static IEnumerable<string> AdjustWithQueryStringSegments(string[] urlSegments)
		{
			if (urlSegments.Length > 0)
			{
				string lastSegment = urlSegments[urlSegments.Length - 1];
				int indexOfQueryStringStart = lastSegment.IndexOf('?');
				if (indexOfQueryStringStart >= 0)
				{
					string lastSegmentCleaned = lastSegment.Substring(0, indexOfQueryStringStart);
					urlSegments[urlSegments.Length - 1] = lastSegmentCleaned;
					return lastSegment.Substring(indexOfQueryStringStart + 1).Split('&').Where(s => s != null && !string.Empty.Equals(s));
				}
			}
			return Enumerable.Empty<string>();
		}

		private bool IsVariableSegment(string urlSegment)
		{
			return urlSegment.StartsWith("{") && urlSegment.EndsWith("}");
		}

		private static bool ContainsDns(string urlToMatch)
		{
			string toMatch = urlToMatch;
			if (toMatch.StartsWith("http://"))
			{
				return true;
			}
			if (toMatch.StartsWith("https://"))
			{
				return true;
			}
			int indexOfFirstDot = toMatch.IndexOf('.');
			int indexOfFirstSlash = toMatch.IndexOf('/');
			bool isJustDns = indexOfFirstSlash < 0;
			return indexOfFirstDot > 0 && (indexOfFirstDot < indexOfFirstSlash || isJustDns);
		}

		private Dictionary<string, string> ConfigureVariablesConstraints(IDictionary<string, string> constraintsExpressions)
		{
			Dictionary<string, string> tmpConstraints = constraintsExpressions != null ? new Dictionary<string, string>(constraintsExpressions) : new Dictionary<string, string>();
			constraints = new Dictionary<string, Func<string, bool>>(urlSegments.Count + tmpConstraints.Count);
			foreach (string segment in urlSegments)
			{
				if (IsVariableSegment(segment))
				{
					string configuredExpression;
					string variableName = GetVariableName(segment);
					if (tmpConstraints.TryGetValue(variableName, out configuredExpression))
					{
						AddConstraintForVariable(variableName, configuredExpression);
					}
					else
					{
						// no specific constraint then match any value
						constraints[variableName] = s => true;
					}
				}
			}
			// add dynamic constraints
			foreach (var constraint in tmpConstraints.Where(x => !constraints.ContainsKey(x.Key) && !IsNullOrWhiteSpace(x.Key)))
			{
				AddConstraintForVariable(constraint.Key, constraint.Value);
			}
			return tmpConstraints;
		}

		private void AddConstraintForVariable(string variableName, string configuredExpression)
		{
			if (!IsNullOrWhiteSpace(configuredExpression))
			{
				var exp = new Regex(configuredExpression, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				constraints[variableName] = segmentValue => exp.IsMatch(segmentValue);
			}
			else
			{
				// no specific constraint then match any value
				constraints[variableName] = s => true;
			}
		}

		private int CalculateHashCode(IEnumerable<string> list, IEnumerable<string> constraintsValues)
		{
			int hash = 61;
			foreach (string segment in list)
			{
				if (IsVariableSegment(segment))
				{
					hash ^= 761;
				}
				else
				{
					hash ^= segment.GetHashCode();
				}
			}
			unchecked
			{
				foreach (string constraint in constraintsValues)
				{
					hash ^= constraint.GetHashCode();
					hash++;
				}
			}
			return hash;
		}
	}
}