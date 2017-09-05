using System.Text.RegularExpressions;

namespace Shriek.WebApi.Proxy.UriTemplates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UriTemplate
    {
        private static Dictionary<char, OperatorInfo> _Operators = new Dictionary<char, OperatorInfo>() {
                                        {'\0', new OperatorInfo {Default = true, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = false}},
                                        {'+', new OperatorInfo {Default = false, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}},
                                        {'.', new OperatorInfo {Default = false, First = ".", Seperator = '.', Named = false, IfEmpty = "",AllowReserved = false}},
                                        {'/', new OperatorInfo {Default = false, First = "/", Seperator = '/', Named = false, IfEmpty = "",AllowReserved = false}},
                                        {';', new OperatorInfo {Default = false, First = ";", Seperator = ';', Named = true, IfEmpty = "",AllowReserved = false}},
                                        {'?', new OperatorInfo {Default = false, First = "?", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
                                        {'&', new OperatorInfo {Default = false, First = "&", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
                                        {'#', new OperatorInfo {Default = false, First = "#", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}}
                                        };

        private readonly string _template;
        private readonly Dictionary<string, object> _Parameters;

        private enum States { CopyingLiterals, ParsingExpression }

        private readonly bool _resolvePartially;

        public UriTemplate(string template, bool resolvePartially = false, bool caseInsensitiveParameterNames = false)
        {
            _resolvePartially = resolvePartially;
            _template = template;
            _Parameters = caseInsensitiveParameterNames
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return _template;
        }

        public void SetParameter(string name, object value)
        {
            _Parameters[name] = value;
        }

        public void ClearParameter(string name)
        {
            _Parameters.Remove(name);
        }

        public void SetParameter(string name, string value)
        {
            _Parameters[name] = value;
        }

        public void SetParameter(string name, IEnumerable<string> value)
        {
            _Parameters[name] = value;
        }

        public void SetParameter(string name, IDictionary<string, string> value)
        {
            _Parameters[name] = value;
        }

        public IEnumerable<string> GetParameterNames()
        {
            Result result = ResolveResult();
            return result.ParameterNames;
        }

        public string Resolve()
        {
            var result = ResolveResult();
            return result.ToString();
        }

        private Result ResolveResult()
        {
            var currentState = States.CopyingLiterals;
            var result = new Result();
            StringBuilder currentExpression = null;
            foreach (var character in _template.ToCharArray())
            {
                switch (currentState)
                {
                    case States.CopyingLiterals:
                        if (character == '{')
                        {
                            currentState = States.ParsingExpression;
                            currentExpression = new StringBuilder();
                        }
                        else if (character == '}')
                        {
                            throw new ArgumentException("Malformed template, unexpected } : " + result.ToString());
                        }
                        else
                        {
                            result.Append(character);
                        }
                        break;

                    case States.ParsingExpression:
                        if (character == '}')
                        {
                            ProcessExpression(currentExpression, result);

                            currentState = States.CopyingLiterals;
                        }
                        else
                        {
                            currentExpression.Append(character);
                        }

                        break;
                }
            }
            if (currentState == States.ParsingExpression)
            {
                result.Append("{");
                result.Append(currentExpression.ToString());

                throw new ArgumentException("Malformed template, missing } : " + result.ToString());
            }

            if (result.ErrorDetected)
            {
                throw new ArgumentException("Malformed template : " + result.ToString());
            }
            return result;
        }

        private void ProcessExpression(StringBuilder currentExpression, Result result)
        {
            if (currentExpression.Length == 0)
            {
                result.ErrorDetected = true;
                result.Append("{}");
                return;
            }

            OperatorInfo op = GetOperator(currentExpression[0]);

            var firstChar = op.Default ? 0 : 1;
            bool multivariableExpression = false;

            var varSpec = new VarSpec(op);
            for (int i = firstChar; i < currentExpression.Length; i++)
            {
                char currentChar = currentExpression[i];
                switch (currentChar)
                {
                    case '*':
                        varSpec.Explode = true;
                        break;

                    case ':':  // Parse Prefix Modifier
                        var prefixText = new StringBuilder();
                        currentChar = currentExpression[++i];
                        while (currentChar >= '0' && currentChar <= '9' && i < currentExpression.Length)
                        {
                            prefixText.Append(currentChar);
                            i++;
                            if (i < currentExpression.Length) currentChar = currentExpression[i];
                        }
                        varSpec.PrefixLength = int.Parse(prefixText.ToString());
                        i--;
                        break;

                    case ',':
                        multivariableExpression = true;
                        var success = ProcessVariable(varSpec, result, multivariableExpression);
                        bool isFirst = varSpec.First;
                        // Reset for new variable
                        varSpec = new VarSpec(op);
                        if (success || !isFirst || _resolvePartially) varSpec.First = false;
                        if (!success && _resolvePartially) { result.Append(","); }
                        break;

                    default:
                        if (IsVarNameChar(currentChar))
                        {
                            varSpec.VarName.Append(currentChar);
                        }
                        else
                        {
                            result.ErrorDetected = true;
                        }
                        break;
                }
            }

            ProcessVariable(varSpec, result, multivariableExpression);
            if (multivariableExpression && _resolvePartially) result.Append("}");
        }

        private bool ProcessVariable(VarSpec varSpec, Result result, bool multiVariableExpression = false)
        {
            var varname = varSpec.VarName.ToString();
            result.ParameterNames.Add(varname);

            if (!_Parameters.ContainsKey(varname)
                || _Parameters[varname] == null
                || (_Parameters[varname] is IList && ((IList)_Parameters[varname]).Count == 0)
                || (_Parameters[varname] is IDictionary && ((IDictionary)_Parameters[varname]).Count == 0))
            {
                if (_resolvePartially == true)
                {
                    if (multiVariableExpression)
                    {
                        if (varSpec.First)
                        {
                            result.Append("{");
                        }

                        result.Append(varSpec.ToString());
                    }
                    else
                    {
                        result.Append("{");
                        result.Append(varSpec.ToString());
                        result.Append("}");
                    }
                    return false;
                }
                return false;
            }

            if (varSpec.First)
            {
                result.Append(varSpec.OperatorInfo.First);
            }
            else
            {
                result.Append(varSpec.OperatorInfo.Seperator);
            }

            object value = _Parameters[varname];

            // Handle Strings
            if (value is string)
            {
                var stringValue = (string)value;
                if (varSpec.OperatorInfo.Named)
                {
                    result.AppendName(varname, varSpec.OperatorInfo, string.IsNullOrEmpty(stringValue));
                }
                result.AppendValue(stringValue, varSpec.PrefixLength, varSpec.OperatorInfo.AllowReserved);
            }
            else
            {
                // Handle Lists
                var list = value as IList;
                if (list == null && value is IEnumerable<string>)
                {
                    list = ((IEnumerable<string>)value).ToList<string>();
                };
                if (list != null)
                {
                    if (varSpec.OperatorInfo.Named && !varSpec.Explode)  // exploding will prefix with list name
                    {
                        result.AppendName(varname, varSpec.OperatorInfo, list.Count == 0);
                    }

                    result.AppendList(varSpec.OperatorInfo, varSpec.Explode, varname, list);
                }
                else
                {
                    // Handle associative arrays
                    var dictionary = value as IDictionary<string, string>;
                    if (dictionary != null)
                    {
                        if (varSpec.OperatorInfo.Named && !varSpec.Explode)  // exploding will prefix with list name
                        {
                            result.AppendName(varname, varSpec.OperatorInfo, dictionary.Count() == 0);
                        }
                        result.AppendDictionary(varSpec.OperatorInfo, varSpec.Explode, dictionary);
                    }
                    else
                    {
                        // If above all fails, convert the object to string using the default object.ToString() implementation
                        var stringValue = value.ToString();
                        if (varSpec.OperatorInfo.Named)
                        {
                            result.AppendName(varname, varSpec.OperatorInfo, string.IsNullOrEmpty(stringValue));
                        }
                        result.AppendValue(stringValue, varSpec.PrefixLength, varSpec.OperatorInfo.AllowReserved);
                    }
                }
            }
            return true;
        }

        private static bool IsVarNameChar(char c)
        {
            return ((c >= 'A' && c <= 'z') //Alpha
                    || (c >= '0' && c <= '9') // Digit
                    || c == '_'
                    || c == '%'
                    || c == '.');
        }

        private static OperatorInfo GetOperator(char operatorIndicator)
        {
            OperatorInfo op;
            switch (operatorIndicator)
            {
                case '+':
                case ';':
                case '/':
                case '#':
                case '&':
                case '?':
                case '.':
                    op = _Operators[operatorIndicator];
                    break;

                default:
                    op = _Operators['\0'];
                    break;
            }
            return op;
        }

        private const string varname = "[a-zA-Z0-9_]*";
        private const string op = "(?<op>[+#./;?&]?)";
        private const string var = "(?<var>(?:(?<lvar>" + varname + ")[*]?,?)*)";
        private const string varspec = "(?<varspec>{" + op + var + "})";

        // (?<varspec>{(?<op>[+#./;?&]?)(?<var>[a-zA-Z0-9_]*[*]?|(?:(?<lvar>[a-zA-Z0-9_]*[*]?),?)*)})

        private Regex _ParameterRegex = null;

        public IDictionary<string, object> GetParameters(Uri uri)
        {
            if (_ParameterRegex == null)
            {
                var matchingRegex = CreateMatchingRegex(_template);
                lock (this)
                {
                    _ParameterRegex = new Regex(matchingRegex);
                }
            }

            var match = _ParameterRegex.Match(uri.OriginalString);
            var parameters = new Dictionary<string, object>();

            for (int x = 1; x < match.Groups.Count; x++)
            {
                if (match.Groups[x].Success)
                {
                    var paramName = _ParameterRegex.GroupNameFromNumber(x);
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        parameters.Add(paramName, Uri.UnescapeDataString(match.Groups[x].Value));
                    }
                }
            }
            return match.Success ? parameters : null;
        }

        public static string CreateMatchingRegex(string uriTemplate)
        {
            var findParam = new Regex(varspec);

            var template = new Regex(@"([^{]|^)\?").Replace(uriTemplate, @"$+\?"); ;//.Replace("?",@"\?");
            var regex = findParam.Replace(template, delegate (Match m)
            {
                var paramNames = m.Groups["lvar"].Captures.Cast<Capture>().Where(c => !string.IsNullOrEmpty(c.Value)).Select(c => c.Value).ToList();
                var op = m.Groups["op"].Value;
                switch (op)
                {
                    case "?":
                        return GetQueryExpression(paramNames, prefix: "?");

                    case "&":
                        return GetQueryExpression(paramNames, prefix: "&");

                    case "#":
                        return GetExpression(paramNames, prefix: "#");

                    case "/":
                        return GetExpression(paramNames, prefix: "/");

                    case "+":
                        return GetExpression(paramNames);

                    default:
                        return GetExpression(paramNames);
                }
            });

            return regex + "$";
        }

        public static string CreateMatchingRegex2(string uriTemplate)
        {
            var findParam = new Regex(varspec);
            //split by host/path/query/fragment

            var template = new Regex(@"([^{]|^)\?").Replace(uriTemplate, @"$+\?"); ;//.Replace("?",@"\?");
            var regex = findParam.Replace(template, delegate (Match m)
            {
                var paramNames = m.Groups["lvar"].Captures.Cast<Capture>().Where(c => !string.IsNullOrEmpty(c.Value)).Select(c => c.Value).ToList();
                var op = m.Groups["op"].Value;
                switch (op)
                {
                    case "?":
                        return GetQueryExpression(paramNames, prefix: "?");

                    case "&":
                        return GetQueryExpression(paramNames, prefix: "&");

                    case "#":
                        return GetExpression(paramNames, prefix: "#");

                    case "/":
                        return GetExpression(paramNames, prefix: "/");

                    case "+":
                        return GetExpression(paramNames);

                    default:
                        return GetExpression(paramNames);
                }
            });

            return regex + "$";
        }

        private static string GetQueryExpression(List<String> paramNames, string prefix)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var paramname in paramNames)
            {
                sb.Append(@"\" + prefix + "?");
                if (prefix == "?") prefix = "&";

                sb.Append("(?:");
                sb.Append(paramname);
                sb.Append("=");

                sb.Append("(?<");
                sb.Append(paramname);
                sb.Append(">");
                sb.Append("[^/?&]+");
                sb.Append(")");
                sb.Append(")?");
            }

            return sb.ToString();
        }

        private static string GetExpression(List<String> paramNames, string prefix = null)
        {
            StringBuilder sb = new StringBuilder();

            string paramDelim;

            switch (prefix)
            {
                case "#":
                    paramDelim = "[^,]+";
                    break;

                case "/":
                    paramDelim = "[^/?]+";
                    break;

                case "?":
                case "&":
                    paramDelim = "[^&#]+";
                    break;

                case ";":
                    paramDelim = "[^;/?#]+";
                    break;

                case ".":
                    paramDelim = "[^./?#]+";
                    break;

                default:
                    paramDelim = "[^/?&]+";
                    break;
            }

            foreach (var paramname in paramNames)
            {
                if (string.IsNullOrEmpty(paramname)) continue;

                if (prefix != null)
                {
                    sb.Append(@"\" + prefix + "?");
                    if (prefix == "#") { prefix = ","; }
                }
                sb.Append("(?<");
                sb.Append(paramname);
                sb.Append(">");
                sb.Append(paramDelim); // Param Value
                sb.Append(")?");
            }

            return sb.ToString();
        }
    }
}