<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="RapidWebDev.Web.Default" AutoEventWireup="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><%= Resources.Common.Copyright %></title>
    <link rel="Stylesheet" type="text/css" href="/resources/css/ext-all.css" />
    <link rel="Stylesheet" type="text/css" href="/resources/css/global.css" />
    <style type="text/css">
		p { margin: 4px 0 8px 0; }
		ul { margin-left: 24px; }
		ul li { list-style-image: url(resources/images/dot.png); padding: 2px 0 2px 0; }
    </style>
</head>
<body>
    <div style="vertical-align:top; padding: 6px;">
		<p><b>"Everything is prepared here for your development. You only need to focus on your core business!" - the mission of <a href="http://www.rapidwebdev.org" target="_blank">RapidWebDev org</a>.</b></p>
		<p>
			RapidWebDev is for software companies who are seeking for an efficient, reusable, extendable and maintainable platform to speed up development of their .NET applications. 
			It integrates almost reusable APIs, common components, available services and UI framework on developing .NET applications. 
			It focuses on productivity and quality improvement through integration and innovation which can save more than 50% code with much higher performance and quality than any other solutions in our practice. It's grown out from our innovation and rich experience on more than 10 outsourcing projects since 2004, and been proven again and again. 
		</p>
		<ul>
			<li><a href="http://rapidwebdev.codeplex.com/Release/ProjectReleases.aspx" target="_blank">Get the latest release.</a></li>
			<li><a href="http://rapidwebdev.codeplex.com/Thread/List.aspx" target="_blank">Join in RapidWebDev discussions.</a></li>
			<li><a href="http://www.rapidwebdev.org/Content/ByUniqueKey/Documentation" target="_blank">Go to official documentation.</a></li>
			<li><a href="http://www.rapidwebdev.org/ListViewer/Blog" target="_blank">Look at RapidWebDeb latest news.</a></li>
		</ul>
		
		<hr />
		<p><i>Currently you're running RapidWebDev version <% =this.AssemblyVersion %></i></p>
	</div>
</body>
</html>