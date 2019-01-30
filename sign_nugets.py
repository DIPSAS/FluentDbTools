from DockerBuildSystem import TerminalTools, VersionTools
from SwarmManagement import SwarmTools
import glob
import os
import sys


NUGET_FOLDER = 'src/output/nuget/'
CERTIFICATE_FILE_PATH = 'CodeSign/CodeSign.pfx'
TIMESTAMPER_URL = 'http://sha256timestamp.ws.symantec.com/sha256/timestamp'
OUTPOUT_DIRECTORY = 'src/output/nuget/signed/'
CHANGELOG_PATH = 'CHANGELOG.md'


def GetNugets(nugetFolder, version):
    return glob.glob(os.path.join(nugetFolder, '*.%s.nupkg' % (version)))


def SignNugetPackage(nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, outputDirectory):
    terminalCommand = 'nuget sign %s -CertificatePath %s -CertificatePassword %s -Timestamper %s -OutputDirectory %s' % (
        nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, outputDirectory)
    TerminalTools.ExecuteTerminalCommands([terminalCommand], True)


def VerifySignedNugetPackage(nugetPackage):
    terminalCommand = 'nuget verify -Signatures %s' % (
        nugetPackage)
    TerminalTools.ExecuteTerminalCommands([terminalCommand], True)


def SignNugetPackages(
                    certificateFilePath,
                    certificatePassword, 
                    nugetFolder=NUGET_FOLDER,
                    timestamperUrl=TIMESTAMPER_URL,
                    outputDirectory=OUTPOUT_DIRECTORY,
                    changelogPath=CHANGELOG_PATH):
    version = VersionTools.GetVersionFromChangelog(changelogPath)
    nugetPackages = GetNugets(nugetFolder, version)
    for nugetPackage in nugetPackages:
        SignNugetPackage(nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, outputDirectory)
        signedNugetPackage = os.path.join(outputDirectory, os.path.basename(nugetPackage))
        VerifySignedNugetPackage(signedNugetPackage)


def GetPasswordFromArguments(arguments):
    passwords = SwarmTools.GetArgumentValues(arguments, '-p')
    if len(passwords) == 0:
        return 'xxxx'
    return passwords[0]


def GetCertificatePathFromArguments(arguments):
    certificates = SwarmTools.GetArgumentValues(arguments, '-c')
    if len(certificates) == 0:
        return CERTIFICATE_FILE_PATH
    return certificates[0]


if __name__ == "__main__":
    arguments = sys.argv
    certificateFilePath = GetCertificatePathFromArguments(arguments)
    certificatePassword = GetPasswordFromArguments(arguments)
    SignNugetPackages(certificateFilePath, certificatePassword)
