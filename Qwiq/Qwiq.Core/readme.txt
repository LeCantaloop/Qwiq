NOTE for projects deploying to Azure

If you are installing Qwiq to a project which is part of a package which will be deployed to Azure,
additional steps must be taken to avoid runtime exceptions. Qwiq takes a dependency on
TeamFoundationServer packages which in turn use native binaries loaded at runtime. These binaries
are Microsoft.WITDataStore32.dll and Microsoft.WITDataStore64.dll. These two binaries exist within
the TeamFoundataionServer Extended Client package. Without additional manual setup these two
binaries are not included in the package step of an Azure service build/deployment, due to lack of
dependency detection to runtime loaded binaries. To ensure inclusion in the .cspkg the binaries
must be added to the projects containing the entry points for each of the service's roles which use
Qwiq.

To add to project:
1. Right click project, select add existing item
2. Navigate to the binaries
  2a. Go to your packages folder choose the currently installed verion of the
      TeamFoundataionServer Extended Client package
  2b. Locate where the WITDataStore*.dll binaries are stored. At the time of writing this, for
      Microsoft.TeamFoundationServer.ExtendedClient.14.89.0 the binaries are located under
      \Microsoft.TeamFoundationServer.ExtendedClient.14.89.0\lib\native, x86 and amd64 respectively
3. Select the binary and choose Add as a Link from the Add button dropdown
4. Do this for both the x86 and amd64 binary


Alternatively, for those comfortable editing a .csproj file manually, the below can be used with a
little editing. Add the following to the csproj, updating the include path to the appropriate
location within your installed TFS ExtendedClient package.
The <Visible> tag can be ommitted or included depending on if you would like the binaries to be
visible in the project or be hidden.

<ItemGroup>
    <Content Include="..\packages\Microsoft.TeamFoundationServer.ExtendedClient.14.89.0\lib\native\x86\Microsoft.WITDataStore32.dll">
      <Link>Microsoft.WITDataStore32.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <Visible>False</Visible>
    </Content>
    <Content Include="..\packages\Microsoft.TeamFoundationServer.ExtendedClient.14.89.0\lib\native\amd64\Microsoft.WITDataStore64.dll">
      <Link>Microsoft.WITDataStore64.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <Visible>False</Visible>
    </Content>
</ItemGroup>
