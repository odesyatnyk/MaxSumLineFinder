using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxSumLineFinder.ApplicationLogic
{
    /// <summary>
    /// Class that represents results of Parsed file by <see cref="FileParser"/> class <see cref="FileParser.Parse(string)"/> method
    /// </summary>
    public class FileParseResult
    {
        public static FileParseResult Create(IEnumerable<int> linesWithMaxSum, IEnumerable<int> invalidLines)
            => new FileParseResult(linesWithMaxSum, invalidLines);

        public static FileParseResult CreateEmptyResult()
            => new FileParseResult(null, null);

        protected FileParseResult(IEnumerable<int> linesWithMaxSum, IEnumerable<int> invalidLines)
        {
            LinesWithMaxSum = linesWithMaxSum ?? new List<int>();
            InvalidLines = invalidLines ?? new List<int>();
        }

        /// <summary>
        /// Contains list of line numbers with maximum sum in line
        /// </summary>
        public IEnumerable<int> LinesWithMaxSum { get; }

        /// <summary>
        /// Contains list of line numbers that contains invalid characters
        /// </summary>
        public IEnumerable<int> InvalidLines { get; }

        /// <summary>
        /// Indicates whether current <see cref="FileParseResult"/> object contains any line number with sum
        /// </summary>
        public bool AnyValidLine => LinesWithMaxSum.Any();

        /// <summary>
        /// Indicates whether current <see cref="FileParseResult"/> object contains any line number with invalid characters
        /// </summary>
        public bool AnyInvalidLine => InvalidLines.Any();
    }
}
