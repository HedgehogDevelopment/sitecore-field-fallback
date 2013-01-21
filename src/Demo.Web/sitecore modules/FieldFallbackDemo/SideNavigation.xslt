<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: Test.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 1/16/2013 4:44:52 PM                                               
    Copyright notice at bottom of file
==============================================================-->

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:sc="http://www.sitecore.net/sc"
  xmlns:dot="http://www.sitecore.net/dot"
  exclude-result-prefixes="dot sc">

    <!-- output directives -->
    <xsl:output method="html" indent="no" encoding="UTF-8" />

    <!-- parameters -->
    <xsl:param name="lang" select="'en'"/>
    <xsl:param name="id" select="''"/>
    <xsl:param name="sc_item"/>
    <xsl:param name="sc_currentitem"/>

    <!-- variables -->
    <!-- Uncomment one of the following lines if you need a "home" variable in you code -->
    <xsl:variable name="home" select="sc:item('/sitecore/content/FieldFallbackDemo',.)" />
    <!--<xsl:variable name="home" select="/*/item[@key='content']/item[@key='home']" />-->
    <!--<xsl:variable name="home" select="$sc_currentitem/ancestor-or-self::item[@template='site root']" />-->


    <!-- entry point -->
    <xsl:template match="*">
        <xsl:apply-templates select="$sc_item" mode="main"/>
    </xsl:template>

    <!--==============================================================-->
    <!-- main                                                         -->
    <!--==============================================================-->
    <xsl:template match="*" mode="main">
        <xsl:call-template name="menu">
            <xsl:with-param name="menuparent" select="$home" />
        </xsl:call-template>
    </xsl:template>


    <xsl:template name="menu">
        <xsl:param name="menuparent" />
        <ul class="nav nav-list">
            <xsl:for-each select="$menuparent/item">
                <li>
                    <sc:link text="{@name}" select="." />
                </li>
                    <xsl:call-template name="menu">
                        <xsl:with-param name="menuparent" select="." />
                    </xsl:call-template>
            </xsl:for-each>
        </ul>
    </xsl:template>

</xsl:stylesheet>