using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace FileRenamer;
class TvRenamer
{
    const string formatPattern = "\\(\\d+\\s+x\\s+\\d+\\)";

    private readonly ILogger logger;

    public TvRenamer(ILogger<TvRenamer> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void RenameAndMove(string folderPath)
    {
        int counter = 0;
        int season = 0;
        string seasonString = String.Empty;

        foreach (string fileName in System.IO.Directory.GetFiles(folderPath))
        {
            string thisSeason = fileName.Substring(fileName.LastIndexOf("\\") + 1, 3);
            if (!Directory.Exists(folderPath + "\\" + thisSeason))
            {
                Directory.CreateDirectory(folderPath + "\\" + thisSeason);
            }
            File.Move(fileName, folderPath + "\\" + thisSeason + "\\" + fileName.Substring(fileName.LastIndexOf("\\") + 1));
        }
    }

    public void Rename(string folderPath)
    {
        int counter = 0;
        int season = 0;
        string seasonString = String.Empty;

        logger.LogDebug("Opening Directory {0}", folderPath);
        foreach (string fileName in Directory.GetFiles(folderPath))
        {
            try
            {
                logger.LogDebug("Handling File {0}", fileName);
                var thisFileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                var theMatch = System.Text.RegularExpressions.Regex.Match(thisFileName, formatPattern);
                if (theMatch != null && theMatch?.Value?.Length > 0)
                {
                    var toSplit = theMatch.Value.Substring(1, theMatch.Value.Length - 2);
                    var splits = toSplit.Split('x');
                    var theSeason = splits[0].Trim();
                    var theEpisode = splits[1].Trim();
                    int intSeason = int.Parse(theSeason);
                    int intEpisode = int.Parse(theEpisode);
                    theSeason = intSeason < 10 ? "0" + intSeason.ToString() : intSeason.ToString();
                    theEpisode = intEpisode < 10 ? "0" + intEpisode.ToString() : intEpisode.ToString();
                    var newTag = "S" + theSeason + "E" + theEpisode;
                    thisFileName = thisFileName.Replace(theMatch.Value, newTag);
                    logger.LogDebug("New File Name is {0}", thisFileName);
                    var newFilePath = fileName.Substring(0, fileName.LastIndexOf("\\") + 1) + thisFileName;
                    newFilePath = newFilePath.Replace("_", "-");
                    while (newFilePath.EndsWith("-"))
                    {
                        newFilePath = newFilePath.Substring(0, newFilePath.Length - 1);
                    }
                    logger.LogDebug("Renaming File {0} to {1}", fileName, newFilePath);
                    File.Move(fileName, newFilePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error with file {0}", fileName);
            }
        }

        foreach (string dir in Directory.GetDirectories(folderPath))
        {
            Rename(dir);
        }
    }
}

