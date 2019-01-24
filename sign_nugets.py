from DockerBuildSystem import TerminalTools
import glob
import os

NUGET_FOLDER = 'src/output/nuget/'
CERTIFICATE_FILE_PATH = 'X509/IntermediateCA.crt'
OUTPOUT_DIRECTORY = 'src/output/nuget/signed/'

def GetNugets(nugetFolder):
    return glob.glob(os.path.join(nugetFolder, '*.nupkg'))


def SignNugetPackage(nugetPackage, certificateFilePath, outputDirectory):
    terminalCommand = 'nuget sign %s -CertificatePath %s -OutputDirectory %s' % (
        nugetPackage, certificateFilePath, outputDirectory)
    TerminalTools.ExecuteTerminalCommands([terminalCommand])


def VerifySignedNugetPackage(nugetPackage):
    terminalCommand = 'nuget verify -Signatures %s' % (
        nugetPackage)
    TerminalTools.ExecuteTerminalCommands([terminalCommand])


def SignNugetPackages(nugetFolder=NUGET_FOLDER, 
                    certificateFilePath=CERTIFICATE_FILE_PATH,
                    outputDirectory=OUTPOUT_DIRECTORY):
    nugetPackages = GetNugets(nugetFolder)
    for nugetPackage in nugetPackages:
        SignNugetPackage(nugetPackage, certificateFilePath, outputDirectory)
        signedNugetPackage = os.path.join(outputDirectory, os.path.basename(nugetPackage))
        VerifySignedNugetPackage(signedNugetPackage)


if __name__ == "__main__":
    SignNugetPackages()
