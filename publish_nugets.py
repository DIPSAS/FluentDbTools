from DockerBuildSystem import TerminalTools, VersionTools
from SwarmManagement import SwarmTools
import glob
import os
import sys


NUGET_FOLDER = 'src/output/nuget/signed/'
CHANGELOG_PATH = 'CHANGELOG.md'
SOURCE_FEED = 'https://api.nuget.org/v3/index.json'


def GetNugets(nugetFolder, version):
    return glob.glob(os.path.join(nugetFolder, '*.%s.nupkg' % (version)))


def PublishNugetPackage(apiKey, sourceFeed, nugetPackage):
    terminalCommand = 'dotnet nuget push -k %s -s %s %s' % (
        apiKey, sourceFeed, nugetPackage)
    TerminalTools.ExecuteTerminalCommands([terminalCommand], True)


def PublishNugetPackages(
                        apiKey, 
                        sourceFeed=SOURCE_FEED,
                        nugetFolder=NUGET_FOLDER,
                        changelogPath=CHANGELOG_PATH):
    version = VersionTools.GetVersionFromChangelog(changelogPath)
    nugetPackages = GetNugets(nugetFolder, version)
    for nugetPackage in nugetPackages:
        PublishNugetPackage(apiKey, sourceFeed, nugetPackage)



def GetApiKeyFromArguments(arguments):
    apiKey = SwarmTools.GetArgumentValues(arguments, '-p')
    if len(apiKey) == 0:
        return 'xxxx'
    return apiKey[0]


if __name__ == "__main__":
    arguments = sys.argv
    apiKey = GetApiKeyFromArguments(arguments)
    PublishNugetPackages(apiKey)
