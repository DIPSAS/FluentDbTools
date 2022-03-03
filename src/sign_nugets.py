from DockerBuildSystem import TerminalTools, VersionTools
from SwarmManagement import SwarmTools
import glob
import os
import sys
import traceback
import shutil

PACKAGE_INPUT_UNSIGNED_FOLDER = 'output/nuget/'
PACKAGE_OUTPUT_SIGNED_FOLDER = 'output/nuget/signed/'
CERTIFICATE_FILE_PATH = '../CodeSign/CodeSign.pfx'
TIMESTAMPER_URL = 'http://sha256timestamp.ws.symantec.com/sha256/timestamp'
CHANGELOG_PATH = '../CHANGELOG.md'

def touch(fname, times=None):
    with open(fname, 'a'):
        os.utime(fname, times)

def GetNugets(packageInputUnsignedFolder, version):
    nugetPackages = glob.glob(os.path.join(packageInputUnsignedFolder, '*.%s.nupkg' % (version)))
    nugetPackages += glob.glob(os.path.join(packageInputUnsignedFolder, '*.%s.snupkg' % (version)))
    return nugetPackages


def SignNugetPackage(nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, packageOutputSignedFolder, allowMissingCertificateFilePath):
    verbosity = "quiet"
    retries = 0;  
    limit = 3  
    exception = Exception("...")
    print("")
    while retries != limit:
        try:
            if os.path.exists(certificateFilePath):
                terminalCommand = 'nuget sign %s -CertificatePath %s -CertificatePassword %s -Timestamper %s -OutputDirectory %s -Verbosity %s' % (
                                            nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, packageOutputSignedFolder, verbosity)
                # print("Try Sign terminalCommand:{}".format(terminalCommand))
                TerminalTools.ExecuteTerminalCommands([terminalCommand], True)
                print("Signed Package({}) Completed".format(nugetPackage))
                return 1
            else:
                if allowMissingCertificateFilePath == False:
                    retries = limit
                    raise Exception("certificateFilePath not found, not able to sign. Set -allowMissingCertificateFilePath true to allow it.")

                print("Missing certificateFilePath ({}). Copy Package({}) to Output folder({}) instead.".format(certificateFilePath, nugetPackage, packageOutputSignedFolder))
                if (os.path.exists(packageOutputSignedFolder)):
                    shutil.copy(nugetPackage, packageOutputSignedFolder)
                else:
                    os.mkdir(packageOutputSignedFolder, 0o755)    
                    shutil.copy(nugetPackage, packageOutputSignedFolder)

                touch(os.path.join(packageOutputSignedFolder,"_notsigned-missing-certificateFilePath.txt"))
                return 0

            return 1
        except Exception as err:
            exception = err
            if retries != limit:
                print(">>> Retry Sign Package({}) {} of {}".format(nugetPackage,1 + retries, limit))
                verbosity = "normal"
                retries = retries + 1
    raise exception


def VerifySignedNugetPackage(nugetPackage):
    retries = 0;  
    limit = 3  
    verbosity = "quiet"
    exception = Exception("...")

    while retries != limit:
        try:
            terminalCommand = 'nuget verify -Signatures %s -Verbosity %s' % (nugetPackage, verbosity)
            TerminalTools.ExecuteTerminalCommands([terminalCommand], True)
            print("--> Verify Signed Package({}) Succeeded".format(nugetPackage))
            return
        except Exception as err:
            exception = err
            if retries != limit:
                print(">>> Retry Verify Signed Package({}) {} of {}".format(nugetPackage,1 + retries, limit))
                verbosity = "normal"
                retries = retries + 1
    raise exception

def SignNugetPackages(
                    certificateFilePath,
                    certificatePassword, 
                    packageInputUnsignedFolder=PACKAGE_INPUT_UNSIGNED_FOLDER,
                    packageOutputSignedFolder=PACKAGE_OUTPUT_SIGNED_FOLDER,
                    allowMissingCertificateFilePath=False,
                    timestamperUrl=TIMESTAMPER_URL,
                    changelogPath=CHANGELOG_PATH):

    version = VersionTools.GetVersionFromChangelog(changelogPath)
    nugetPackages = GetNugets(packageInputUnsignedFolder, version)
    if len(nugetPackages) == 0:
        print("SignNugetPackages: No packages found in {}".format(packageInputUnsignedFolder))
        
    for nugetPackage in nugetPackages:
        ok = SignNugetPackage(nugetPackage, certificateFilePath, certificatePassword, timestamperUrl, packageOutputSignedFolder, allowMissingCertificateFilePath)
        if ok == 0:
            if allowMissingCertificateFilePath:
                continue
            else:
                raise Exception("certificateFilePath not found, not able to sign. Set -allowMissingCertificateFilePath true to allow it.")

        signedNugetPackage = os.path.join(packageOutputSignedFolder, os.path.basename(nugetPackage))
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

def GetPackageInputUnsignedFolderFromArguments(arguments):
    argValues = SwarmTools.GetArgumentValues(arguments, '-packageInputUnsignedFolder')
    if len(argValues) == 0:
        return PACKAGE_INPUT_UNSIGNED_FOLDER
    return argValues[0]

def GetPackageOutputSignedFolderFromArguments(arguments):
    argValues = SwarmTools.GetArgumentValues(arguments, '-packageOutputSignedFolder')
    if len(argValues) == 0:
        return PACKAGE_OUTPUT_SIGNED_FOLDER
    return argValues[0]

def GetAllowMissingCertificateFilePathFromArguments(arguments):
    argValues = SwarmTools.GetArgumentValues(arguments, '-allowMissingCertificateFilePath')
    if len(argValues) == 0:
        return False
    value = str.lower(argValues[0])    
    return value == "true" or value == "1" or value == "on" or value == "yes" or value == "enabled"


if __name__ == "__main__":
    arguments = sys.argv
    certificateFilePath = GetCertificatePathFromArguments(arguments)
    certificatePassword = GetPasswordFromArguments(arguments)
    packageInputUnsignedFolder = GetPackageInputUnsignedFolderFromArguments(arguments)
    packageOutputSignedFolder = GetPackageOutputSignedFolderFromArguments(arguments)
    allowMissingCertificateFilePath = GetAllowMissingCertificateFilePathFromArguments(arguments)
    SignNugetPackages(certificateFilePath, certificatePassword, packageInputUnsignedFolder, packageOutputSignedFolder,allowMissingCertificateFilePath)
