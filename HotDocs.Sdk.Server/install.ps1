param($installPath, $toolsPath, $package, $project)

$vsRef = $project.Object.References.Item("HotDocs.Server.Interop")
if ($vsRef -and $vsRef.EmbedInteropTypes)
{
    $vsRef.EmbedInteropTypes = $false
}