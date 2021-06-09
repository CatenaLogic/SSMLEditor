namespace SSMLEditor.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    public class BreakAnalyzer : IAnalyzer
    {
        private readonly Regex _regex = new Regex(@"^(\d+\.?\d*)s$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(AnalyzerContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var document = context.Document;

            await foreach (var item in AnalyzeAsync(context, document.Root))
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(AnalyzerContext context, XElement element)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            foreach (var childElement in element.Elements())
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                if (childElement.Name?.LocalName == "break")
                {
                    var timeAttribute = childElement.Attribute("time");
                    if (timeAttribute is not null)
                    {
                        var value = timeAttribute.Value?.Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            var match = _regex.Match(value);
                            if (match.Success)
                            {
                                var numberAsString = match.Groups[1].Value;
                                var number = double.Parse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture);
                                if (number > 5.0)
                                {
                                    var lineNumber = ((IXmlLineInfo)timeAttribute).LineNumber;
                                    var position = ((IXmlLineInfo)timeAttribute).LinePosition;

                                    for (var i = 0; i < lineNumber - 1; i++)
                                    {
                                        position += context.DocumentLines[i].Length;

                                        if (i < lineNumber - 2)
                                        {
                                            position += "\r\n".Length;
                                        }
                                    }

                                    var length = timeAttribute.ToString().Length;

                                    yield return new AnalyzerResult
                                    {
                                        Name = "Break too long",
                                        Description = "Break element has a maximum of 5 seconds",
                                        StartIndex = position,
                                        Length = length,
                                    };
                                }
                            }
                        }
                    }
                }
                else
                {
                    await foreach (var item in AnalyzeAsync(context, childElement))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
