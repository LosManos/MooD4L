Hey! Thanks for checking this out!
===

This is a T4 template that will generate POCO's (Plain Old CLR Objects) from your database tables and columns.

Here is all you need to do:

1. Add references to the Micorosoft SQL Server SMO assemblies.  You can easily get them through the Web Platform Installer.  After installing I found mine in "C:\Program Files\Microsoft SQL Server\100\SDK\Assemblies".  Here are the specific ones you need to add to your project:
	1. Microsoft.SqlServer.ConnectionInfo
	2. Microsoft.SqlServer.Management.Sdk.Sfc
	3. Microsoft.SqlServer.Smo
2. Drop the 2 T4 templates into your Visual Studio Project.
3. Open up the "Pocos.tt" template.
4. Set your own values for the namespaceName, serverName, and databaseName.
5. Save the template and it should promt you to run!  Just hit OK and watch the magic happen!  **Note: when the files are generate, they may not be actually added to your project.  Click the "Show All Files" button to see them and include them.  We're working to fix this.**

After that, you can continue to run the T4 template as you update the database, or you can delete the templates and keep the POCO's.

Limitations:
---
1. Only works with Windows Authentication and configuration values are hardcoded into the template.  Ideally this could work off AppSettings (Web.config/app.config) and allow for SQL Server authentication as well.
2. Generates POCO's for all tables in the database (no automatic filtering).  Ideally a schema, prefix, or regex filter would be nice.
3. The only Data Annotation attribute that is currently added is the Key attribute.  Ideally we will support all related validation attributes.
4. Currently the C# class files are generated and added to the same directory as the T4 template, but they're not added to the project.  Ideally they should be nested under the T4 Template or at least added to the project.