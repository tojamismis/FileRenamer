using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileRenamer
{
  class DateRenamer
  {
    private readonly ILogger logger;
    public DateRenamer(ILogger<DateRenamer> logger)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string newPathFolder = @"C:\Users\tojam\Videos\Audials\Audials Movies\AtiaICloud";
    public void Rename(string folderPath)
    {
      foreach (string fileName in System.IO.Directory.GetFiles(@"C:\Users\tojam\iCloudPhotos\Photos"))
      {
        MoveFile(fileName, newPathFolder);
      }
    }

    private void MoveFile(string fileName, string folderPath)
    {
      try
      {
        logger.LogDebug($"Starting file {fileName}");
        var fileDate = File.GetLastWriteTime(fileName);
        var newName = CreateFileName(fileName, folderPath);
        if (FilesEqual(fileName, newName))
        {
          logger.LogDebug("New File Name of {0} already exists.  Will move to next file.", newName);
        }
        logger.LogDebug($"File {fileName} has new Destination {newName}");
        if (!File.Exists(newName))
        {
          logger.LogDebug($"Copying {fileName} to {newName}");
          File.Copy(fileName, newName);
          logger.LogDebug($"Copy of {fileName} complete");
        }
      }
      catch(Exception ex)
      {
        logger.LogError(ex, "Error with file {0}", fileName);
      }
    }

    private string CreateFileName(string existingFileName, string directoryPath)
    {
      var ext = Path.GetExtension(existingFileName);
      var newFileBaseName = CreateBaseFileDateName(existingFileName);
      var newBasePath = Path.Combine(directoryPath, newFileBaseName);
      var returnFileName = newBasePath + ext;
      if (!File.Exists(returnFileName)) return returnFileName;
      int i = 1;
      while(File.Exists(returnFileName))
      {
        if (FilesEqual(existingFileName, returnFileName)) return returnFileName;
        returnFileName = $"{newBasePath}-{i}{ext}";
        i++;
      }
      return returnFileName;
    }

    private string CreateBaseFileDateName(string fileName)
    {
      var fileDate = File.GetLastWriteTime(fileName);
      return fileDate.ToString("s").Replace("T", "_").Replace(":", ".");
    }

    private bool FilesEqual(string fileName, string newFileName)
    {
      if (!File.Exists(newFileName)) return false;
      var fileHash = CalcHash(fileName);
      var newFileHash = CalcHash(newFileName);
      var equal = fileHash.SequenceEqual(newFileHash);
      Array.Clear(fileHash, 0, fileHash.Length);
      Array.Clear(newFileHash, 0, newFileHash.Length);
      return equal;
    }

    private byte[] CalcHash(string fileName)
    {
      using var md5 = MD5.Create();
      using var stream = File.OpenRead(fileName);
      return md5.ComputeHash(stream);
    }

    private void Rename(string folderPath, string findString, string replaceString)
    {
      foreach (string fileName in System.IO.Directory.GetFiles(folderPath))
      {
        File.Move(fileName, fileName.Replace(findString, replaceString));
      }
    }
  }
}
