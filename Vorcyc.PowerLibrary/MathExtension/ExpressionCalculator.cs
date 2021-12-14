
using System;
using System.Text.RegularExpressions;
using SysMath = System.Math;


namespace Vorcyc.PowerLibrary.MathExtension
{
    /// <summary>
    /// 表达式求值
    /// </summary>
    /// <example>
    /// 以下代码演示一个表达式计算
    /// <code>
    /// var y = "sqrt(4)*(2-3)"
    /// </code>
    /// </example>
    public static class ExpressionCalculator
    {

        public static double Evaluate(string expr)
        {
            Regex reEval = new Regex(@"((?<fone>(exp|log|log10|abs|sqr|sqrt|sin|cos|tan|asin|acos|atan))\s*\((?<fone1>\s*[+-]?\d+\.?\d*\b\s*)\)|(?<ftwo>(min|max)\s*)\((?<ftwo1>\s*[+-]?\d+\.?\d*\b\s*),(?<ftwo2>\s*[+-]?\d+\.?\d*\b\s*)\)|(?<!\^\s*)(?<mod1>\s*[+-]?\d+\.?\d*\b\s*)\s+mod\s+(?<mod2>\s*[+-]?\d+\.?\d*\b\s*)(?!\s*\^)|(?<pow1>\s*[+-]?\d+\.?\d*\b\s*)\^(?<pow2>\s*[+-]?\d+\.?\d*\b\s*)|(?<!\^\s*)(?<div1>\s*[+-]?\d+\.?\d*\b\s*)\/(?<div2>\s*[+-]?\d+\.?\d*\b\s*)(?!\s*\^)|(?<!\^\s*)(?<mul1>\s*[+-]?\d+\.?\d*\b\s*)\*(?<mul2>\s*[+-]?\d+\.?\d*\b\s*)(?!\s*\^)|(?<![*/^]\s*)(?<sub1>\s*[+-]?\d+\.?\d*\b\s*)\-(?<sub2>\s*[+-]?\d+\.?\d*\b\s*)(?!\s*[*/^])|(?<![*/^]\s*)(?<add1>\s*[+-]?\d+\.?\d*\b\s*)\+(?<add2>\s*[+-]?\d+\.?\d*\b\s*)(?!\s*[*/^])|\s*\((?<nump>\s*[+-]?\d+\.?\d*\b\s*)\)\s*)", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"(?<=[0-9)]\s*)[+-](?=[0-9(])", "$0 ");
            Regex reNumber = new Regex(@"^\s*[+-]?\d+\.?\d*\b\s*$");
            while (!reNumber.IsMatch(expr))
            {
                string newExpr = reEval.Replace(expr, new MatchEvaluator(ExpressionCalculator.PerformOperation), 1);
                if (expr == newExpr)
                {
                    throw new ArgumentException("无效表达式");
                }
                expr = newExpr;
            }
            return double.Parse(expr);
        }


        private static string PerformOperation(Match m)
        {
            double result = 0;

            if (m.Groups["nump"].Length > 0)
            {
                return m.Groups["nump"].Value.Trim();
            }
            if (m.Groups["neg"].Length > 0)
            {
                return "+";
            }
            if (m.Groups["add1"].Length > 0)
            {
                result = double.Parse(m.Groups["add1"].Value) + double.Parse(m.Groups["add2"].Value);
            }
            else if (m.Groups["sub1"].Length > 0)
            {
                result = double.Parse(m.Groups["sub1"].Value) - double.Parse(m.Groups["sub2"].Value);
            }
            else if (m.Groups["mul1"].Length > 0)
            {
                result = double.Parse(m.Groups["mul1"].Value) * double.Parse(m.Groups["mul2"].Value);
            }
            else if (m.Groups["mod1"].Length > 0)
            {
                result = SysMath.IEEERemainder(double.Parse(m.Groups["mod1"].Value), double.Parse(m.Groups["mod2"].Value));
            }
            else if (m.Groups["div1"].Length > 0)
            {
                result = double.Parse(m.Groups["div1"].Value) / double.Parse(m.Groups["div2"].Value);
            }
            else if (m.Groups["pow1"].Length > 0)
            {
                result = SysMath.Pow(double.Parse(m.Groups["pow1"].Value), double.Parse(m.Groups["pow2"].Value));
            }
            else if (m.Groups["fone"].Length > 0)
            {
                double operand = double.Parse(m.Groups["fone1"].Value);
                switch (m.Groups["fone"].Value.ToLower())
                {
                    case "exp":
                        result = SysMath.Exp(operand);
                        break;

                    case "log":
                        result = SysMath.Log(operand);
                        break;

                    case "log10":
                        result = SysMath.Log10(operand);
                        break;

                    case "abs":
                        result = SysMath.Abs(operand);
                        break;

                    case "sqrt":
                        result = SysMath.Sqrt(operand);
                        break;

                    case "sin":
                        result = SysMath.Sin(operand);
                        break;

                    case "cos":
                        result = SysMath.Cos(operand);
                        break;

                    case "tan":
                        result = SysMath.Tan(operand);
                        break;

                    case "asin":
                        result = SysMath.Asin(operand);
                        break;

                    case "acos":
                        result = SysMath.Acos(operand);
                        break;

                    case "atan":
                        result = SysMath.Atan(operand);
                        break;
                }
            }
            else if (m.Groups["ftwo"].Length > 0)
            {
                double operand1 = double.Parse(m.Groups["ftwo1"].Value);
                double operand2 = double.Parse(m.Groups["ftwo2"].Value);
                switch (m.Groups["ftwo"].Value.ToLower())
                {
                    case "min":
                        result = SysMath.Min(operand1, operand2);
                        break;

                    case "max":
                        result = SysMath.Max(operand1, operand2);
                        break;
                }
            }
            return result.ToString();
        }
    }
}
