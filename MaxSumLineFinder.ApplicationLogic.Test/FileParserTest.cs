using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MaxSumLineFinder.ApplicationLogic.Test
{
    [TestClass]
    public class FileParserTest
    {
        #region Init and Cleanup

        FileParser parser = null;
        string root = null;
        #region FileNames

        string allLinesInvalidFileName = null;
        string allLinesValidFileName = null;
        string emptyFileName = null;
        string mixedLinesFileName = null;
        string notExistingFileName = null;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            parser = new FileParser();
            root = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            allLinesInvalidFileName = "AllLinesInvalid.txt";
            allLinesValidFileName = "AllLinesValid.txt";
            emptyFileName = "EmptyFile.txt";
            mixedLinesFileName = "MixedLines.txt";
            notExistingFileName = "IAmNothing.txt";
        }

        [TestCleanup]
        public void Cleanup()
        {
            parser = null;
            root = null;
            allLinesInvalidFileName = null;
            allLinesValidFileName = null;
            emptyFileName = null;
            mixedLinesFileName = null;
            notExistingFileName = null;
        }

        #endregion

        [TestMethod]
        public void Parse_EmptyFile_EmptyResult()
        {
            string path = GetFilePath(emptyFileName);

            var result = parser.Parse(path);

            Assert.IsFalse(result.AnyValidLine);
            Assert.IsFalse(result.AnyInvalidLine);
        }

        [TestMethod]
        public void Parse_AllLinesInvalidFile_NoLinesWithMaxSumAllLinesAreInvalidExceptEmptyOnes()
        {
            string path = GetFilePath(allLinesInvalidFileName);

            var result = parser.Parse(path);

            Assert.IsFalse(result.AnyValidLine);
            Assert.IsTrue(result.AnyInvalidLine);
            Assert.IsTrue(new [] { 1, 2, 3, 5, 6 }.SequenceEqual(result.InvalidLines));
        }

        [TestMethod]
        public void Parse_AllLinesValidFile_NoInvalidLinesOneLineWithMaxSum()
        {
            string path = GetFilePath(allLinesValidFileName);

            var result = parser.Parse(path);

            Assert.IsTrue(result.AnyValidLine);
            Assert.IsFalse(result.AnyInvalidLine);
            Assert.IsTrue(new[] { 4 }.SequenceEqual(result.LinesWithMaxSum));
        }

        [TestMethod]
        public void Parse_TryParseNotExistingFile_ExceptionThrown()
        {
            string path = GetFilePath(notExistingFileName);

            Assert.ThrowsException<ArgumentException>(() => parser.Parse(path));
        }

        [TestMethod]
        public void Parse_TryParseFileByInvalidPath_ExceptionThrown()
        {
            string path = "*?/.s[";

            Assert.ThrowsException<ArgumentException>(() => parser.Parse(path));
        }

        [TestMethod]
        public void LineInvalidMethodLineContainsSymbol_TrueResult()
        {
            var line = "12,a";

            var result = parser.LineInvalid(line);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LineInvalidMethodLineContainsDecimalNegativeOverflow_TrueResult()
        {
            var line = "-7922816251426433759354395033555555";

            var result = parser.LineInvalid(line);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LineInvalidMethodLineContainsDecimalPositiveOverflow_TrueResult()
        {
            var line = "7922816251426433759354395033555555";

            var result = parser.LineInvalid(line);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LineInvalidMethodOnePositiveNumberProvided_FalseResult()
        {
            var line = "33";

            var result = parser.LineInvalid(line);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LineInvalidMethodOneNegativeNumberProvided_FalseResult()
        {
            var line = "-33";

            var result = parser.LineInvalid(line);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LineInvalidMethodOneDecimalNumberProvided_FalseResult()
        {
            var line = "33.55";

            var result = parser.LineInvalid(line);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LineInvalidMethodSeriesOfNumbersProvided_FalseResult()
        {
            var line = "33.55,44,52,23123312.52,23";

            var result = parser.LineInvalid(line);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReadAllFileLinesMethod_ValidPathExistingFile_FileReadToEnd()
        {
            var path = GetFilePath(allLinesValidFileName);

            var result = parser.ReadAllFileLines(path);

            Assert.AreEqual(6, result.Count);

            Assert.AreEqual("1", result[1]);
            Assert.AreEqual("2,3,4,6,7", result[2]);
            Assert.AreEqual("123.5532,55", result[3]);
            Assert.AreEqual("10000,24", result[4]);
            Assert.AreEqual("", result[5]);
            Assert.AreEqual("86.666", result[6]);
        }

        [TestMethod]
        public void ReadAllFileLinesMethod_InvalidPath_ExceptionThrown()
        {
            string path = "*?/.s[";

            Assert.ThrowsException<ArgumentException>(() => parser.ReadAllFileLines(path));
        }

        [TestMethod]
        public void ReadAllFileLinesMethod_ValidPathNotExistingFile_ExceptionThrown()
        {
            string path = GetFilePath(notExistingFileName);

            Assert.ThrowsException<ArgumentException>(() => parser.ReadAllFileLines(path));
        }

        [TestMethod]
        public void PathValidMethod_ValidPath_TrueResult()
        {
            var path = GetFilePath(mixedLinesFileName);

            var result = parser.PathValid(path);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PathValidMethod_InvalidPath1_FalseResult()
        {
            var path = "s[d23\\?";

            var result = parser.PathValid(path);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PathValidMethod_InvalidPath2_FalseResult()
        {
            var path = @"c:\*.txt";

            var result = parser.PathValid(path);
            
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetInvalidLinesNumbersMethod_EmptyInput_EmptyResultList()
        {
            var input = new Dictionary<int, string>();

            var result = parser.GetInvalidLinesNumbers(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetInvalidLinesNumbersMethod_OnlyValidLines_EmptyResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, string.Empty },
                {2, "1,2" },
                {3, "" },
                {4, "            " },
                {5, "9565.4,2" }

            };

            var result = parser.GetInvalidLinesNumbers(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetInvalidLinesNumbersMethod_ContainsInvalidLines_ResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, string.Empty },
                {2, "1,2" },
                {3, "" },
                {4, "            " },
                {5, "9565.4,2" },
                {6, "9565.4,2,as" },
            };

            var result = parser.GetInvalidLinesNumbers(input);

            Assert.IsTrue(result.Any());
            Assert.IsTrue(new[] { 6 }.SequenceEqual(result));
        }

        [TestMethod]
        public void GetMaxSumLinesNumbersMethod_EmptyInput_EmptyResultList()
        {
            var input = new Dictionary<int, string>();

            var result = parser.GetMaxSumLinesNumbers(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetMaxSumLinesNumbersMethod_OnlyInvalidLines_EmptyResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, "sasd" },
                {2, "1,2,ere" }
            };

            var result = parser.GetMaxSumLinesNumbers(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetMaxSumLinesNumbersMethod_OnlyValidLines_ResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, string.Empty },
                {2, "1,2" },
                {3, "" },
                {4, "            " },
                {5, "9565.4,2" }

            };

            var result = parser.GetMaxSumLinesNumbers(input);

            Assert.IsTrue(result.Any());
            Assert.AreEqual(5, result.Single());
        }

        [TestMethod]
        public void GetMaxSumLinesNumbersMethod_ContainsInvalidLines_ResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, string.Empty },
                {2, "1,2" },
                {3, "Hel" },
                {4, "            " },
                {5, "9565.4,2" },
                {6, "9565.4,2,as" },
            };

            var result = parser.GetMaxSumLinesNumbers(input);

            Assert.IsTrue(result.Any());
            Assert.AreEqual(5, result.Single());
        }

        [TestMethod]
        public void GetMaxSumLinesNumbersMethod_ContainsInvalidLinesWithExcludingLines_ResultList()
        {
            var input = new Dictionary<int, string>()
            {
                {1, string.Empty },
                {2, "1,2" },
                {3, "Hel" },
                {4, "            " },
                {5, "9565.4,2" },
                {6, "9565.4,2,as" },
            };

            var exclude = new List<int>() { 5 };
            var result = parser.GetMaxSumLinesNumbers(input, exclude);

            Assert.IsTrue(result.Any());
            Assert.AreEqual(2, result.Single());
        }

        #region Private Methods

        private string GetFilePath(string file)
        {
            return Path.Combine(root, file);
        }

        #endregion
    }
}
