using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DifferLib.Tests
{
    [TestClass]
    public sealed class HighlighterTests
    {
        private const string DeleteLineStart = "<dls>";
        private const string DeleteLineEnd = "</dle>";
        private const string DeleteBlockStart = "<dbs>";
        private const string DeleteBlockEnd = "</dbe>";
        private const string InsertLineStart = "<ils>";
        private const string InsertLineEnd = "</ile>";
        private const string InsertBlockStart = "<ibs>";
        private const string InsertBlockEnd = "</ibe>";

        private static readonly HighlighterSettings _settingsDelete = new HighlighterSettings(DeleteLineStart, DeleteLineEnd, DeleteBlockStart, DeleteBlockEnd);
        private static readonly HighlighterSettings _settingsInsert = new HighlighterSettings(InsertLineStart, InsertLineEnd, InsertBlockStart, InsertBlockEnd);
        private static readonly HighlighterSettings _settingsEmpty = new HighlighterSettings(string.Empty, string.Empty, string.Empty, string.Empty);

        private static readonly SettingsSuite _settingsSuite = new SettingsSuite(_settingsDelete, _settingsInsert);
        private static readonly SettingsSuite _settingsSuiteEmpty = new SettingsSuite(_settingsEmpty, _settingsEmpty);

        [TestMethod]
        public void TestInsertEmptyDescriptors()
        {
            var highlighted = _settingsSuite.Highlighter.HighlightDelete("_", new List<SubstringDescriptor>());
            Assert.AreEqual("_", highlighted);
        }

        [TestMethod]
        public void TestDeleteEmptyDescriptors()
        {
            var highlighted = _settingsSuite.Highlighter.HighlightInsert("_", new List<SubstringDescriptor>());
            Assert.AreEqual("_", highlighted);
        }

        [TestMethod]
        public void TestLineEndings()
        {
            Run(_settingsSuiteEmpty);
            Run(_settingsSuite);

            void Run(SettingsSuite settingsSuite)
            {
                var highlighted = settingsSuite.Highlighter.HighlightDelete("_\r\n.", new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1) });
                var line1 = settingsSuite.WrapperFactoryDelete.Create("_").Block().Append("\r\n").Line().ToString();
                var line2 = settingsSuite.WrapperFactoryDelete.Create(".").Block().Line().ToString();
                Assert.AreEqual(line1 + line2, highlighted);
            }
        }

        [TestMethod]
        public void TestDifferentLineEndings()
        {
            Run(_settingsSuiteEmpty);
            Run(_settingsSuite);

            void Run(SettingsSuite settingsSuite)
            {
                var highlighted = settingsSuite.Highlighter.HighlightDelete("_\r\n.\n*", new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1) });
                var line1 = settingsSuite.WrapperFactoryDelete.Create("_").Block().Append("\r\n").Line().ToString();
                var line2 = settingsSuite.WrapperFactoryDelete.Create(".").Block().Append("\n").Line().ToString();
                Assert.AreEqual(line1 + line2 + "*", highlighted);
            }
        }

        [TestMethod]
        public void TestFullLineDelete()
        {
            Run(_settingsSuiteEmpty);
            Run(_settingsSuite);
            
            void Run(SettingsSuite settingsSuite)
            {
                var highlighted = settingsSuite.Highlighter.HighlightDelete("_", new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1) });
                var line = settingsSuite.WrapperFactoryDelete.Create("_").Block().Line().ToString();
                Assert.AreEqual(line, highlighted);
            }
        }

        [TestMethod]
        public void TestInterlineDelete()
        {
            Run(_settingsSuiteEmpty);
            Run(_settingsSuite);

            void Run(SettingsSuite settingsSuite)
            {
                var descriptors = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 4) };
                var highlighted = settingsSuite.Highlighter.HighlightDelete("_\r\n.*", descriptors);
                var line1 = settingsSuite.WrapperFactoryDelete.Create("_\r\n").Block().Line().ToString();
                var line2 = settingsSuite.WrapperFactoryDelete.Create(".").Block().Append("*").Line().ToString();
                Assert.AreEqual(line1 + line2, highlighted);
            }
        }

        private class SettingsSuite
        {
            private readonly HighlighterSettings _settingsInsert;
            private readonly HighlighterSettings _settingsDelete;

            public SettingsSuite(HighlighterSettings settingsInsert, HighlighterSettings settingsDelete)
            {
                _settingsInsert = settingsInsert;
                _settingsDelete = settingsDelete;
            }

            public Highlighter Highlighter => new Highlighter(_settingsDelete, _settingsInsert);
            public WrapperFactory WrapperFactoryDelete => new WrapperFactory(_settingsDelete);
            public WrapperFactory WrapperFactoryInsert => new WrapperFactory(_settingsInsert);
        }

        private interface IWrapper
        {
            IWrapper Append(string suffix);
            IWrapper Block();
            IWrapper Line();
        }

        private class WrapperFactory
        {
            private readonly HighlighterSettings _settings;
            public WrapperFactory(HighlighterSettings settings) { _settings = settings; }
            public IWrapper Create(string source) => new Wrapper(_settings, source);

            private class Wrapper : IWrapper
            {
                private readonly HighlighterSettings _settings;
                private string _data;
                public Wrapper(HighlighterSettings settings, string source)
                {
                    _settings = settings;
                    _data = source;
                }
                public IWrapper Append(string suffix)
                {
                    _data += suffix;
                    return this;
                }
                public IWrapper Block()
                {
                    _data = _settings.BlockStart + _data + _settings.BlockEnd;
                    return this;
                }
                public IWrapper Line()
                {
                    _data = _settings.LineStart + _data + _settings.LineEnd;
                    return this;
                }
                public override string ToString() => _data;
            }
        }
    }
}
