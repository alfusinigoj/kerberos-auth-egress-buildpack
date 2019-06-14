using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using FileMode = System.IO.FileMode;
using ZipFile = System.IO.Compression.ZipFile;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public enum StackType
    {
        Windows,
        Linux
    }
    public static int Main() => Execute<Build>(x => x.Publish);
    const string BuildpackProjectName = "Pivotal.RouteService.Auth.Egress.Buildpack";
    string PackageZipName => $"{BuildpackProjectName}-{Runtime}-{GitVersion.MajorMinorPatch}.zip";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Target CF stack type - 'windows' or 'linux'. Determines buildpack runtime (Framework or Core). Default is 'windows'")]
    readonly StackType Stack = StackType.Windows;

    [Parameter("GitHub personal access token with access to the repo")]
    string GitHubToken;

    string Runtime => Stack == StackType.Windows ? "win-x64" : "linux-x64";
    string Framework => Stack == StackType.Windows ? "net47" : "netcoreapp2.2";


    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Description("Cleans up **/bin and **/obj folders")
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteFile);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteFile);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Description("Restores NuGet dependencies for the buildpack")
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetRuntime(Runtime));
        });

    Target Compile => _ => _
        .Description("Compiles the buildpack")
        .DependsOn(Restore)
        .Executes(() =>
        {
            Logger.Info(Stack);
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetFramework(Framework)
                .SetRuntime(Runtime)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .Description("Execute tests")
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .Description("Packages buildpack in Cloud Foundry expected format into /artifacts directory")
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetFramework(Framework)
                .SetRuntime(Runtime)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            var workDirectory = TemporaryDirectory / "pack";
            EnsureCleanDirectory(TemporaryDirectory);
            var buildpackProject = Solution.GetProject(BuildpackProjectName);
            var publishDirectory = buildpackProject.Directory / "bin" / Configuration / Framework / Runtime / "publish";
            var workBinDirectory = workDirectory / "bin";
            var scriptsDirectory = RootDirectory / "scripts";

            var requiredAssembliesSourceDirectory = SourceDirectory / "requiredAssemblies";
            var requiredAssembliesTargetDirectory = workBinDirectory / "requiredAssemblies";

            CopyDirectoryRecursively(publishDirectory, workBinDirectory, DirectoryExistsPolicy.Merge);
            CopyDirectoryRecursively(scriptsDirectory, workBinDirectory, DirectoryExistsPolicy.Merge);
            CopyDirectoryRecursively(requiredAssembliesSourceDirectory, requiredAssembliesTargetDirectory, DirectoryExistsPolicy.Merge);

            var tempZipFile = TemporaryDirectory / PackageZipName;

            ZipFile.CreateFromDirectory(workDirectory, tempZipFile);
            MakeFilesInZipUnixExecutable(tempZipFile);
            CopyFileToDirectory(tempZipFile, ArtifactsDirectory, FileExistsPolicy.Overwrite);
            Logger.Block(ArtifactsDirectory / PackageZipName);

        });


    Target Release => _ => _
        .Description("Creates a GitHub release (or ammends existing) and uploads buildpack artifact")
        .DependsOn(Publish)
        .Requires(() => GitHubToken)
        .Executes(async () =>
        {
            if (!GitRepository.IsGitHubRepository())
                throw new Exception("Only supported when git repo remote is github");

            var client = new GitHubClient(new ProductHeaderValue(BuildpackProjectName))
            {
                Credentials = new Credentials(GitHubToken, AuthenticationType.Bearer)
            };
            var gitIdParts = GitRepository.Identifier.Split("/");
            var owner = gitIdParts[0];
            var repoName = gitIdParts[1];

            var releaseName = $"v{GitVersion.MajorMinorPatch}";
            Release release;
            try
            {
                release = await client.Repository.Release.Get(owner, repoName, releaseName);
            }
            catch (NotFoundException)
            {
                var newRelease = new NewRelease(releaseName)
                {
                    Name = releaseName,
                    Draft = false,
                    Prerelease = false
                };
                release = await client.Repository.Release.Create(owner, repoName, newRelease);
            }

            var existingAsset = release.Assets.FirstOrDefault(x => x.Name == PackageZipName);
            if (existingAsset != null)
            {
                await client.Repository.Release.DeleteAsset(owner, repoName, existingAsset.Id);
            }

            var zipPackageLocation = ArtifactsDirectory / PackageZipName;
            var releaseAssetUpload = new ReleaseAssetUpload(PackageZipName, "application/zip", File.OpenRead(zipPackageLocation), null);
            var releaseAsset = await client.Repository.Release.UploadAsset(release, releaseAssetUpload);

            Logger.Block(releaseAsset.BrowserDownloadUrl);
        });

    public static void MakeFilesInZipUnixExecutable(AbsolutePath zipFile)
    {
        var tmpFileName = zipFile + ".tmp";
        using (var input = new ZipInputStream(File.Open(zipFile, FileMode.Open)))
        using (var output = new ZipOutputStream(File.Open(tmpFileName, FileMode.Create)))
        {
            output.SetLevel(9);
            ZipEntry entry;

            while ((entry = input.GetNextEntry()) != null)
            {
                var outEntry = new ZipEntry(entry.Name);
                outEntry.HostSystem = (int)HostSystemID.Unix;
                outEntry.ExternalFileAttributes = -2115174400;
                output.PutNextEntry(outEntry);
                input.CopyTo(output);
            }
            output.Finish();
            output.Flush();
        }

        DeleteFile(zipFile);
        RenameFile(tmpFileName, zipFile, FileExistsPolicy.Overwrite);
    }
}
