using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MaxSumLineFinder.ApplicationLogic
{
    /// <summary>
    /// Class that helps to parse text files by the custom logic
    /// </summary>
    public class FileParser
    {
        /// <summary>
        /// Separator symbol that is used to split numbers inside of same line
        /// </summary>
        public const char NumberSeparator = ',';

        /// <summary>
        /// Number format that is ised to parse numbers by en-Us culture (dot as decimal separator)
        /// </summary>
        public static IFormatProvider NumberFormat => new CultureInfo("en-Us");

        /// <summary>
        /// Parses file located by the profided <paramref name="path"/>
        /// <list>In order to read file this method is using <see cref="FileParser.ReadAllFileLines(string)"/></list>
        /// <list>To get invalid lines uses <see cref="FileParser.GetInvalidLinesNumbers(Dictionary{int, string})"/></list>
        /// <list>To get lines with max sum uses <see cref="FileParser.GetMaxSumLinesNumbers(Dictionary{int, string}, IEnumerable{int})"/></list>
        /// </summary>
        /// <param name="path">Path to file to parse</param>
        /// <returns>
        /// <list><see cref="FileParseResult"/> with the lists of line numbers with invalid characters and line numbers with maximum sum of contained elements</list>
        /// <list>empty <see cref="FileParseResult"/> in case file by provided <paramref name="path"/> is empty or contains only empty lines</list>
        /// </returns>
        public FileParseResult Parse(string path)
        {
            var lines = ReadAllFileLines(path);

            if (!lines.Any() || lines.All(x => string.IsNullOrWhiteSpace(x.Value.Trim())))
                return FileParseResult.CreateEmptyResult();

            var invalidLinesNumbers = GetInvalidLinesNumbers(lines);
            var maxSumLinesNumbers = GetMaxSumLinesNumbers(lines, invalidLinesNumbers);

            return FileParseResult.Create(maxSumLinesNumbers, invalidLinesNumbers);
        }

        /// <summary>
        /// Method to get list of line numbers that are containing invalid characters
        /// </summary>
        /// <param name="lines">Dictionary of lines to filter</param>
        /// <returns>
        /// List of line numbers that are not satisfiyng conditions defined in <see cref="FileParser.LineInvalid(string)"/> method
        /// </returns>
        public IEnumerable<int> GetInvalidLinesNumbers(Dictionary<int, string> lines)
        {
            return lines.Where(x => LineInvalid(x.Value)).Select(x => x.Key).AsEnumerable();
        }

        /// <summary>
        /// Method to get list of line numbers that are contains max sum of elements
        /// </summary>
        /// <param name="lines">Dictionary of lines to calculate sum and find the one with highest sum</param>
        /// <param name="exceptLines">List of line numbers to exclude from consideration</param>
        /// <returns>
        /// <list>List of line numbers with maximum sum of it's parsed elements.</list>
        /// <list>Returns empty list in case <paramref name="lines"/> prameter does not contain any line</list>
        /// <list>or all of them were skipped by <paramref name="exceptLines"/> parameter</list>
        /// <list>or all of them were filtered out by <see cref="FileParser.LineInvalid(string)"/> method</list>
        /// </returns>
        public IEnumerable<int> GetMaxSumLinesNumbers(Dictionary<int, string> lines, IEnumerable<int> exceptLines = null)
        {
            var linesWithSum = lines.Where(x => !exceptLines.Contains(x.Key) && !LineInvalid(x.Value))
                .ToDictionary(
                    line => line.Key,
                    line => line.Value.Split(FileParser.NumberSeparator, StringSplitOptions.RemoveEmptyEntries)
                                      .Aggregate(0M, (current, next) => current + decimal.Parse(next, NumberStyles.Any, FileParser.NumberFormat)));

            if (!linesWithSum.Any())
                return new List<int>().AsEnumerable();

            var maxSum = linesWithSum.Max(x => x.Value);
            return linesWithSum.Where(line => line.Value == maxSum).Select(x => x.Key).AsEnumerable();
        }

        /// <summary>
        /// Checks if input <see cref="string"/> <paramref name="line"/> is a valid line to parse and calculate sum.
        /// <list>As numbers separator used <see cref="FileParser.NumberSeparator"/></list>
        /// <list>As numbers format used <see cref="IFormatProvider"/> <see cref="FileParser.NumberFormat"/></list>
        /// <list>As numbers styles used <see cref="NumberStyles.Any"/></list>
        /// </summary>
        /// <param name="line">Input line</param>
        /// <returns>false in case described above conditions are not satisfied at least once otherwise true</returns>
        public bool LineInvalid(string line)
        {
            return line.Split(FileParser.NumberSeparator, StringSplitOptions.RemoveEmptyEntries).Any(x => !decimal.TryParse(x, NumberStyles.Any, FileParser.NumberFormat, out var result));
        }

        /// <summary>
        /// Reads all lines from the file specified by <paramref name="path"/>
        /// </summary>
        /// <param name="path">File path</param>
        /// <exception cref="System.ArgumentException">Thrown when provided path is invalid, or file does not exists by provided path, or caller does not have permissions to access it</exception>
        /// <returns>Dictionary where Key is a line number from provided path and Value is line Content</returns>
        public Dictionary<int, string> ReadAllFileLines(string path)
        {
            if (!PathValid(path))
                throw new ArgumentException($"{nameof(path)} contains invalid path characters");

            if (!File.Exists(path))
                throw new ArgumentException($"File \"{Path.GetFileName(path)}\" does not exists by the provided path {path} or you do not have permissions to access it.");

            int lineNumber = 1;
            return File.ReadAllLines(path).ToDictionary(_ => lineNumber++, y => y);
        }

        /// <summary>
        /// Checks input string <paramref name="path"/> for valid path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>true if provided is valid othervise false</returns>
        public bool PathValid(string path)
        {
            bool valid = true;

            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(path);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

            if (fileInfo is null)
                return false;

            return valid && Path.IsPathRooted(path) && !Path.GetInvalidPathChars().Any(x => path.ToCharArray().Contains(x));
        }
    }
}
