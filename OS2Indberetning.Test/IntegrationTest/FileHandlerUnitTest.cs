﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OS2Indberetning.BuisnessLogic;


namespace OS2Indberetning.IntegrationTest
{
    [TestClass]
    public class FileHandlerUnitTest
    {
        [TestMethod]
        public void FileHandler_WriteToFileInSpecificFolder_AssertThatReadFromFileIsEqualToWrittenToFile()
        {
            FileHandler.WriteFileContent("test", "test", "test").ContinueWith((result) =>
            {
                FileHandler.ReadFileContent("test", "test").ContinueWith((sender) =>
                {
                    Assert.AreEqual(sender.Result, "test");
                });
            });
        }

        [TestMethod]
        public void FileHandler_WriteToFileInSpecificFolderTwice_AssertThatReadFromFileIsEqualToWrittenToFileLast()
        {
            FileHandler.WriteFileContent("test", "test", "test").ContinueWith((result1) =>
            {
                FileHandler.WriteFileContent("test", "test", "twice").ContinueWith((result2) =>
                {
                    FileHandler.ReadFileContent("test", "test").ContinueWith((sender) =>
                    {
                        Assert.AreEqual(sender.Result, "twice");
                    });
                });
            });
        }

        [TestMethod]
        public void FileHandler_ReadFromFolderThatDoesntExist_AssertThatReadFromFileIsNull()
        {
            FileHandler.ReadFileContent("something", "something").ContinueWith((sender) =>
            {
                Assert.IsNull(sender);
            });
        }
    }
}
