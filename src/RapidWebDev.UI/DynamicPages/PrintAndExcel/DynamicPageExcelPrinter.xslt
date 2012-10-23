<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	xmlns:d="http://www.rapidwebdev.org/schemas/dynamicpage/printandexcel"
	exclude-result-prefixes="msxsl">
	
    <xsl:output method="html" indent="yes" doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd" />

	<xsl:template match="/d:DataSource">
		<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40">
			<head>
				<title>RapidWebDev Solution</title>
				<META HTTP-EQUIV="Content-Type" Content="application/vnd.ms-excel; charset=UTF-8"></META>
				<style type="text/css">
					body
					{
						margin: 0px;
						font-size: 12px;
						font-family: Arial, Sans-Serif, sans-serif;
						color:black;
					}

					@page
					{
						mso-page-orientation:landscape;
						margin:.25in .25in .5in .25in;
						mso-header-margin:.5in;
						mso-footer-margin:.25in;
						mso-horizontal-page-align:center;
						mso-vertical-page-align:center;
					}

					br
					{
						mso-data-placement:same-cell;
					}

					td
					{
						vertical-align: top;
					}
				</style>
			</head>
			<body>
				<xsl:variable name="ColumnNumber" select="count(d:Schema)" />
				<table border="1" width="100%">
					<tr bgcolor="#003366" height="33">
						<td>
							<xsl:attribute name="colspan"><xsl:value-of select="$ColumnNumber"/></xsl:attribute>
							<img border="0" alt="RapidWebDev Logo">
								<xsl:attribute name="src"><xsl:value-of select="@LogoUrl"/></xsl:attribute>
							</img>
							&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
							<span style="font-size:16pt; color: white"><b><xsl:value-of select="@Name" /></b></span>
						</td>
					</tr>
					<tr bgcolor="#e8e8e8">
						<td>
							<xsl:attribute name="colspan"><xsl:value-of select="$ColumnNumber"/></xsl:attribute>
							<xsl:value-of select="@Description"/>
						</td>
					</tr>
					<tr bgcolor="f0f0f0">
						<xsl:for-each select="d:Schema">
							<td class="colHeaderLink" width="1%" valign="top" nowrap="true" style="vnd.ms-excel.numberformat:@">
								<b><xsl:value-of select="."/>&#160;</b>
							</td>
						</xsl:for-each>
					</tr>
					<xsl:for-each select="d:Data">
						<tr bgcolor="ffffff">
							<xsl:for-each select="d:Property">
								<td class="nav" nowrap="true" style="vnd.ms-excel.numberformat:@"><xsl:value-of select="@Value" disable-output-escaping="yes"/>&#160;</td>
							</xsl:for-each>
						</tr>
					</xsl:for-each>
				</table>				
			</body>
		</html>        
    </xsl:template>
</xsl:stylesheet>
