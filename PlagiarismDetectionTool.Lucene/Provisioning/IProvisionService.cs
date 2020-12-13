using PlagiarismDetectionTool.LuceneTools.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Provisioning
{
    public interface IProvisionService
    {
        public void ProvisionFromDirectory(string directoryName);
        public void ProvisionFromDirectory(DirectoryInfo directory);
    }
}
