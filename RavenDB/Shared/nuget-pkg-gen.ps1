task CreateNugetPackages {

	Remove-Item $base_dir\RavenDB\RavenDB*.nupkg
	
	$nuget_dir = "$build_dir\NuGet"
	Remove-Item $nuget_dir -Force -Recurse -ErrorAction SilentlyContinue
	New-Item $nuget_dir -Type directory | Out-Null
	
	New-Item $nuget_dir\RavenDB.Client\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Client\lib\net45 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Client\lib\sl50 -Type directory | Out-Null
	Copy-Item $base_dir\RavenDB\Shared\NuGet\RavenDB.Client.nuspec $nuget_dir\RavenDB.Client\RavenDB.Client.nuspec
	
	@("Raven.Abstractions.???", "Raven.Client.Lightweight.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Client\lib\net40 }
	@("Raven.Abstractions-4.5.???", "Raven.Client.Lightweight-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Client\lib\net45 }
	@("Raven.Client.Silverlight.???", "AsyncCtpLibrary_Silverlight5.???") |% { Copy-Item "$build_dir\sl5\$_" $nuget_dir\RavenDB.Client\lib\sl50	}
	
	New-Item $nuget_dir\RavenDB.Client.MvcIntegration\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Client.MvcIntegration\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Client.MvcIntegration.nuspec $nuget_dir\RavenDB.Client.MvcIntegration\RavenDB.Client.MvcIntegration.nuspec
	@("Raven.Client.MvcIntegration.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Client.MvcIntegration\lib\net40 }
	@("Raven.Client.MvcIntegration-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Client.MvcIntegration\lib\net45 }
		
	New-Item $nuget_dir\RavenDB.Database\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Database\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Database.nuspec $nuget_dir\RavenDB.Database\RavenDB.Database.nuspec
	@("Raven.Abstractions.???", "Raven.Database.???", "BouncyCastle.Crypto.???",
		 "Esent.Interop.???", "ICSharpCode.NRefactory.???", "ICSharpCode.NRefactory.CSharp.???", "Mono.Cecil.???", "Lucene.Net.???", "Lucene.Net.Contrib.Spatial.NTS.???",
		 "Jint.Raven.???", "Spatial4n.Core.NTS.???", "GeoAPI.dll", "NetTopologySuite.dll", "PowerCollections.dll", "AWS.Extensions.???", "AWSSDK.???") `
		 |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Database\lib\net40 }
	@("Raven.Abstractions-4.5.???", "Raven.Database-4.5.???", "BouncyCastle.Crypto.???",
		 "Esent.Interop.???", "ICSharpCode.NRefactory.???", "ICSharpCode.NRefactory.CSharp.???", "Mono.Cecil.???", "Lucene.Net.???", "Lucene.Net.Contrib.Spatial.NTS.???",
		 "Jint.Raven.???", "Spatial4n.Core.NTS.???", "GeoAPI.dll", "NetTopologySuite.dll", "PowerCollections.dll", "AWS.Extensions.???", "AWSSDK.???") `
		 |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Database\lib\net45 }
	
	New-Item $nuget_dir\RavenDB.Server -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Server.nuspec $nuget_dir\RavenDB.Server\RavenDB.Server.nuspec
	New-Item $nuget_dir\RavenDB.Server\tools -Type directory | Out-Null
	@("Esent.Interop.???", "ICSharpCode.NRefactory.???", "ICSharpCode.NRefactory.CSharp.???", "Mono.Cecil.???", "Lucene.Net.???", "Lucene.Net.Contrib.Spatial.NTS.???",
		"Spatial4n.Core.NTS.???", "GeoAPI.dll", "NetTopologySuite.dll", "PowerCollections.dll",	"NewtonSoft.Json.???", "NLog.???", "Jint.Raven.???",
		"Raven.Abstractions.???", "Raven.Database.???", "Raven.Server.???", "Raven.Smuggler.???", "AWS.Extensions.???", "AWSSDK.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Server\tools }
	Copy-Item (Get-DependencyPackageFiles 'Microsoft.CompilerServices.AsyncTargetingPack' -FrameworkVersion net40) $nuget_dir\RavenDB.Server\tools
	Copy-Item $base_dir\DefaultConfigs\RavenDb.exe.config $nuget_dir\RavenDB.Server\tools\Raven.Server.exe.config

	New-Item $nuget_dir\RavenDB.Embedded\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Embedded\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Embedded.nuspec $nuget_dir\RavenDB.Embedded\RavenDB.Embedded.nuspec
	@("Raven.Client.Embedded.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Embedded\lib\net40 }
	@("Raven.Client.Embedded-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Embedded\lib\net45 }
	
	# Client packages
	@("Authorization", "UniqueConstraints") | Foreach-Object { 
		$name = $_;
		New-Item $nuget_dir\RavenDB.Client.$name\lib\net40 -Type directory | Out-Null
		New-Item $nuget_dir\RavenDB.Client.$name\lib\net45 -Type directory | Out-Null
		Copy-Item $base_dir\NuGet\RavenDB.Client.$name.nuspec $nuget_dir\RavenDB.Client.$name\RavenDB.Client.$name.nuspec
		@("Raven.Client.$_.???") |% { Copy-Item $build_dir\$_ $nuget_dir\RavenDB.Client.$name\lib\net40 }
		@("Raven.Client.$_-4.5.???") |% { Copy-Item $build_dir\net45\$_ $nuget_dir\RavenDB.Client.$name\lib\net45 }
	}
	
	New-Item $nuget_dir\RavenDB.Bundles.Authorization\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Bundles.Authorization\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Bundles.Authorization.nuspec $nuget_dir\RavenDB.Bundles.Authorization\RavenDB.Bundles.Authorization.nuspec
	@("Raven.Bundles.Authorization.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Bundles.Authorization\lib\net40 }
	@("Raven.Bundles.Authorization-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Bundles.Authorization\lib\net45 }
	
	New-Item $nuget_dir\RavenDB.Bundles.CascadeDelete\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Bundles.CascadeDelete\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Bundles.CascadeDelete.nuspec $nuget_dir\RavenDB.Bundles.CascadeDelete\RavenDB.Bundles.CascadeDelete.nuspec
	@("Raven.Bundles.CascadeDelete.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Bundles.CascadeDelete\lib\net40 }
	@("Raven.Bundles.CascadeDelete-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Bundles.CascadeDelete\lib\net45 }
	
	New-Item $nuget_dir\RavenDB.Bundles.IndexReplication\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Bundles.IndexReplication\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Bundles.IndexReplication.nuspec $nuget_dir\RavenDB.Bundles.IndexReplication\RavenDB.Bundles.IndexReplication.nuspec
	@("Raven.Bundles.IndexReplication.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Bundles.IndexReplication\lib\net40 }
	@("Raven.Bundles.IndexReplication-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Bundles.IndexReplication\lib\net45 }

	New-Item $nuget_dir\RavenDB.Bundles.UniqueConstraints\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Bundles.UniqueConstraints\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Bundles.UniqueConstraints.nuspec $nuget_dir\RavenDB.Bundles.UniqueConstraints\RavenDB.Bundles.UniqueConstraints.nuspec
	@("Raven.Bundles.UniqueConstraints.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Bundles.UniqueConstraints\lib\net40 }
	@("Raven.Bundles.UniqueConstraints-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Bundles.UniqueConstraints\lib\net45 }
	
	New-Item $nuget_dir\RavenDB.AspNetHost\content -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.AspNetHost\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.AspNetHost\lib\net45 -Type directory | Out-Null
	@("Raven.Web.???") |% { Copy-Item "$build_dir\web\$_" $nuget_dir\RavenDB.AspNetHost\lib\net40 }
	@("Raven.Web-4.5.???") |% { Copy-Item "$build_dir\web\net45\$_" $nuget_dir\RavenDB.AspNetHost\lib\net45 }
	Copy-Item $base_dir\NuGet\RavenDB.AspNetHost.nuspec $nuget_dir\RavenDB.AspNetHost\RavenDB.AspNetHost.nuspec
	Copy-Item $base_dir\DefaultConfigs\NuGet.AspNetHost.Web.config $nuget_dir\RavenDB.AspNetHost\content\Web.config.transform
	
	New-Item $nuget_dir\RavenDB.Tests.Helpers\lib\net40 -Type directory | Out-Null
	New-Item $nuget_dir\RavenDB.Tests.Helpers\lib\net45 -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenDB.Tests.Helpers.nuspec $nuget_dir\RavenDB.Tests.Helpers\RavenDB.Tests.Helpers.nuspec
	@("Raven.Tests.Helpers.???", "Raven.Server.???") |% { Copy-Item "$build_dir\$_" $nuget_dir\RavenDB.Tests.Helpers\lib\net40 }
	@("Raven.Tests.Helpers-4.5.???", "Raven.Server-4.5.???") |% { Copy-Item "$build_dir\net45\$_" $nuget_dir\RavenDB.Tests.Helpers\lib\net45 }
	New-Item $nuget_dir\RavenDB.Tests.Helpers\content -Type directory | Out-Null
	Copy-Item $base_dir\NuGet\RavenTests $nuget_dir\RavenDB.Tests.Helpers\content\RavenTests -Recurse
	
	$nugetVersion = "$version.$env:buildlabel"
	if ($global:uploadCategory -and $global:uploadCategory.EndsWith("-Unstable")){
		$nugetVersion += "-Unstable"
	}
	
	# Sets the package version in all the nuspec as well as any RavenDB package dependency versions
	$packages = Get-ChildItem $nuget_dir *.nuspec -recurse
	$packages |% { 
		$nuspec = [xml](Get-Content $_.FullName)
		$nuspec.package.metadata.version = $nugetVersion
		$nuspec | Select-Xml '//dependency' |% {
			if($_.Node.Id.StartsWith('RavenDB')){
				$_.Node.Version = "[$nugetVersion]"
			}
		}
		$nuspec.Save($_.FullName);
		Exec { &"$base_dir\.nuget\nuget.exe" pack $_.FullName }
	}
	
	# Upload packages
	$accessPath = "$base_dir\..\Nuget-Access-Key.txt"
	$sourceFeed = "https://nuget.org/"
	
	if ($global:uploadMode -eq "Vnext3") {
		$accessPath = "$base_dir\..\MyGet-Access-Key.txt"
		$sourceFeed = "http://www.myget.org/F/ravendb3/api/v2/package"
	}
	
	if ( (Test-Path $accessPath) ) {
		$accessKey = Get-Content $accessPath
		$accessKey = $accessKey.Trim()
		
		# Push to nuget repository
		$packages | ForEach-Object {
			Exec { &"$base_dir\.nuget\NuGet.exe" push "$($_.BaseName).$nugetVersion.nupkg" $accessKey -Source $sourceFeed }
		}
	}
	else {
		Write-Host "$accessPath does not exit. Cannot publish the nuget package." -ForegroundColor Yellow
	}
}