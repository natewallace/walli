/*
 * Copyright (c) 2014 Nathaniel Wallace
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using SalesForceLanguage;
using SalesForceLanguage.Apex.CodeModel;
using SalesForceLanguage.Apex.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SalesForceLanguage.Apex
{
    /// <summary>
    /// Used to do code completions.
    /// </summary>
    public class LanguageCompletion
    {
        #region Fields

        /// <summary>
        /// Used to do code completions.
        /// </summary>
        private LanguageManager _language;

        /// <summary>
        /// Holds generic code completions.
        /// </summary>
        private static List<Symbol> _genericCompletions;

        /// <summary>
        /// Tokens for which code completions should be ignored.
        /// </summary>
        private static Tokens[] _tokensToIgnore = new Tokens[] 
        {
            Tokens.COMMENT_BLOCK,
            Tokens.COMMENT_DOC,
            Tokens.COMMENT_DOCUMENTATION,
            Tokens.COMMENT_INLINE,
            Tokens.SOQL,
            Tokens.SOSL,
            Tokens.LITERAL_STRING
        };

        /// <summary>
        /// Holds all of the visual force symbols.
        /// </summary>
        private static Dictionary<string, string[]> _visualForceSymbols;

        /// <summary>
        /// Holds the visual force tag names.
        /// </summary>
        private static string[] _visualForceTags;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LanguageCompletion()
        {
            _genericCompletions = new List<Symbol>();

            _genericCompletions.Add(new Keyword("abstract"));
            _genericCompletions.Add(new Keyword("after"));
            _genericCompletions.Add(new Keyword("before"));
            _genericCompletions.Add(new Keyword("break"));
            _genericCompletions.Add(new Keyword("catch"));
            _genericCompletions.Add(new Keyword("class"));
            _genericCompletions.Add(new Keyword("continue"));
            _genericCompletions.Add(new Keyword("delete"));
            _genericCompletions.Add(new Keyword("do"));
            _genericCompletions.Add(new Keyword("double"));
            _genericCompletions.Add(new Keyword("else"));
            _genericCompletions.Add(new Keyword("enum"));
            _genericCompletions.Add(new Keyword("extends"));
            _genericCompletions.Add(new Keyword("false"));
            _genericCompletions.Add(new Keyword("final"));
            _genericCompletions.Add(new Keyword("finally"));
            _genericCompletions.Add(new Keyword("for"));
            _genericCompletions.Add(new Keyword("get"));
            _genericCompletions.Add(new Keyword("global"));
            _genericCompletions.Add(new Keyword("if"));
            _genericCompletions.Add(new Keyword("implements"));
            _genericCompletions.Add(new Keyword("insert"));
            _genericCompletions.Add(new Keyword("interface"));
            _genericCompletions.Add(new Keyword("merge"));
            _genericCompletions.Add(new Keyword("new"));
            _genericCompletions.Add(new Keyword("null"));
            _genericCompletions.Add(new Keyword("on"));
            _genericCompletions.Add(new Keyword("override"));
            _genericCompletions.Add(new Keyword("private"));
            _genericCompletions.Add(new Keyword("protected"));
            _genericCompletions.Add(new Keyword("public"));
            _genericCompletions.Add(new Keyword("return"));
            _genericCompletions.Add(new Keyword("rollback"));
            _genericCompletions.Add(new Keyword("set"));
            _genericCompletions.Add(new Keyword("static"));
            _genericCompletions.Add(new Keyword("super"));
            _genericCompletions.Add(new Keyword("testmethod"));
            _genericCompletions.Add(new Keyword("this"));
            _genericCompletions.Add(new Keyword("throw"));
            _genericCompletions.Add(new Keyword("transient"));
            _genericCompletions.Add(new Keyword("trigger"));
            _genericCompletions.Add(new Keyword("true"));
            _genericCompletions.Add(new Keyword("try"));
            _genericCompletions.Add(new Keyword("undelete"));
            _genericCompletions.Add(new Keyword("update"));
            _genericCompletions.Add(new Keyword("upsert"));
            _genericCompletions.Add(new Keyword("value"));
            _genericCompletions.Add(new Keyword("virtual"));
            _genericCompletions.Add(new Keyword("void"));
            _genericCompletions.Add(new Keyword("webservice"));
            _genericCompletions.Add(new Keyword("while"));
            _genericCompletions.Add(new Keyword("with sharing"));
            _genericCompletions.Add(new Keyword("without sharing"));

            _genericCompletions = _genericCompletions.OrderBy(s => s.Name).ToList();

            _visualForceSymbols = new Dictionary<string, string[]>();
            _visualForceSymbols.Add("analytics:reportChart", new string[] { "cacheAge", "cacheResults", "developerName", "filter", "hideOnError", "rendered", "reportId", "showRefreshButton", "size" });
            _visualForceSymbols.Add("apex:actionFunction", new string[] { "action", "focus", "id", "immediate", "name", "namespace", "onbeforedomupdate", "oncomplete", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:actionPoller", new string[] { "action", "enabled", "id", "interval", "oncomplete", "onsubmit", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:actionRegion", new string[] { "id", "immediate", "rendered", "renderRegionOnly" });
            _visualForceSymbols.Add("apex:actionStatus", new string[] { "dir", "for", "id", "lang", "layout", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onstart", "onstop", "rendered", "startStyle", "startStyleClass", "startText", "stopStyle", "stopStyleClass", "stopText", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:actionSupport", new string[] { "action", "disabled", "diableDefault", "event", "focus", "id", "immediate", "onbeforedomupdate", "oncomplete", "onsubmit", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:areaSeries", new string[] { "axis", "colorSet", "highlight", "highlightLineWidth", "highlightOpacity", "highlightStrokeColor", "id", "opacity", "rendered", "rendererFn", "showInLegend", "tips", "title", "xField", "yField" });
            _visualForceSymbols.Add("apex:attribute", new string[] { "access", "assignTo", "default", "description", "encode", "id", "name", "required", "type" });
            _visualForceSymbols.Add("apex:axis", new string[] { "dashSize", "fields", "grid", "gridFill", "id", "margin", "maximum", "minimum", "position", "rendered", "steps", "title", "type" });
            _visualForceSymbols.Add("apex:barSeries", new string[] { "axis", "colorSet", "colorsProgressWithinSeries", "groupGutter", "gutter", "highlight", "highlightColor", "highlightLineWidth", "highlightOpacity", "highlightStroke", "id", "orientation", "rendered", "rendererFn", "showInLegend", "stacked", "tips", "title", "xField", "xPadding", "yField", "yPadding" });
            _visualForceSymbols.Add("apex:canvasApp", new string[] { "applicationName", "border", "canvasId", "containerId", "developerName", "entityFields", "height", "id", "maxHeight", "maxWidth", "namespacePrefix", "onCanvasAppError", "onCanvasAppLoad", "parameters", "rendered", "scrolling", "width" });
            _visualForceSymbols.Add("apex:chart", new string[] { "animate", "background", "colorSet", "data", "floating", "height", "hidden", "id", "legend", "name", "rendered", "renderTo", "resizable", "theme", "width" });
            _visualForceSymbols.Add("apex:chartLabel", new string[] { "color", "display", "field", "font", "id", "minMargin", "orientation", "rendered", "rendererFn", "rotate" });
            _visualForceSymbols.Add("apex:chartTips", new string[] { "height", "id", "labelField", "rendered", "rendererFn", "trackMouse", "valueField", "width" });
            _visualForceSymbols.Add("apex:column", new string[] { "breakBefore", "colspan", "dir", "footerClass", "footercolspan", "footerdir", "footerlang", "footeronclick", "footerondblclick", "footeronkeydown", "footeronkeypress", "footeronkeyup", "footeronmousedown", "footeronmousemove", "footeronmouseout", "footeronmouseover", "footeronmouseup", "footerstyle", "footertitle", "footerValue", "headerClass", "headercolspan", "headerdir", "headerlang", "headeronclick", "headerondblclick", "headeronkeydown", "headeronkeypress", "headeronkeyup", "headeronmousedown", "headeronmousemove", "headeronmouseout", "headeronmouseover", "headeronmouseup", "headerstyle", "headertitle", "headerValue", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "rowspan", "style", "styleClass", "title", "value", "width" });
            _visualForceSymbols.Add("apex:commandButton", new string[] { "accesskey", "action", "alt", "dir", "disabled", "id", "image", "immediate", "lang", "onblur", "onclick", "oncomplete", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "reRender", "status", "style", "styleClass", "tabindex", "timeout", "title", "value" });
            _visualForceSymbols.Add("apex:commandLink", new string[] { "accesskey", "action", "charset", "coords", "dir", "hreflang", "id", "immediate", "lang", "onblur", "onclick", "oncomplete", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rel", "rendered", "reRender", "rev", "shape", "status", "style", "styleClass", "tabindex", "target", "timeout", "title", "type", "value" });
            _visualForceSymbols.Add("apex:component", new string[] { "access", "allowDML", "controller", "extensions", "id", "language", "layout", "rendered", "selfClosing" });
            _visualForceSymbols.Add("apex:componentBody", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("apex:composition", new string[] { "rendered", "rendered", "template" });
            _visualForceSymbols.Add("apex:dataList", new string[] { "dir", "first", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "rows", "style", "styleClass", "title", "type", "value", "var" });
            _visualForceSymbols.Add("apex:dataTable", new string[] { "align", "bgcolor", "border", "captionClass", "captionStyle", "cellpadding", "cellspacing", "columnClasses", "columns", "columnsWidth", "dir", "first", "footerClass", "frame", "headerClass", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onRowClick", "onRowDblClick", "onRowMouseDown", "onRowMouseMove", "onRowMouseOut", "onRowMouseOver", "onRowMouseUp", "rendered", "rowClasses", "rows", "rules", "style", "styleClass", "summary", "title", "value", "var", "width" });
            _visualForceSymbols.Add("apex:define", new string[] { "name" });
            _visualForceSymbols.Add("apex:detail", new string[] { "id", "inlineEdit", "oncomplete", "relatedList", "relatedListHover", "rendered", "rerender", "showChatter", "subject", "title" });
            _visualForceSymbols.Add("apex:dynamicComponent", new string[] { "componentValue", "id", "invokeAfterAction", "rendered" });
            _visualForceSymbols.Add("apex:emailPublisher", new string[] { "autoCollapseBody", "bccVisibility", "ccVisibility", "emailBody", "emailBodyFormat", "emailBodyHeight", "enableQuickText", "entityId", "expandableHeader", "fromAddresses", "fromVisibility", "id", "onSubmitFailure", "onSubmitSuccess", "rendered", "reRender", "sendButtonName", "showAdditionalFields", "showAttachments", "showSendButton", "showTemplates", "subject", "subjectVisibility", "submitFunctionName", "title", "toAddresses", "toVisibility", "verticalResize", "width" });
            _visualForceSymbols.Add("apex:enhancedList", new string[] { "customizable", "height", "id", "listId", "oncomplete", "rendered", "reRender", "rowsPerPage", "type", "width" });
            _visualForceSymbols.Add("apex:facet", new string[] { "name" });
            _visualForceSymbols.Add("apex:flash", new string[] { "flashvars", "height", "id", "loop", "play", "rendered", "src", "width" });
            _visualForceSymbols.Add("apex:form", new string[] { "accept", "acceptcharset", "dir", "enctype", "forceSSL", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onreset", "onsubmit", "prependId", "rendered", "style", "styleClass", "target", "title" });
            _visualForceSymbols.Add("apex:gaugeSeries", new string[] { "colorSet", "dataField", "donut", "highlight", "id", "labelField", "needle", "rendered", "rendererFn", "tips" });
            _visualForceSymbols.Add("apex:iframe", new string[] { "frameborder", "height", "id", "rendered", "scrolling", "src", "title", "width" });
            _visualForceSymbols.Add("apex:image", new string[] { "alt", "dir", "height", "id", "ismap", "lang", "longdesc", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "style", "styleClass", "title", "url", "usemap", "value", "width" });
            _visualForceSymbols.Add("apex:include", new string[] { "id", "pageName", "rendered" });
            _visualForceSymbols.Add("apex:includeScript", new string[] { "id", "loadOnReady", "onload", "value" });
            _visualForceSymbols.Add("apex:inlineEditSupport", new string[] { "changedStyleClass", "disabled", "event", "hideOnEdit", "id", "rendered", "resetFunction", "showOnEdit" });
            _visualForceSymbols.Add("apex:input", new string[] { "accesskey", "alt", "dir", "disabled", "id", "label", "lang", "list", "list", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "required", "size", "style", "styleClass", "tabindex", "title", "type", "type", "type", "value" });
            _visualForceSymbols.Add("apex:inputCheckbox", new string[] { "accesskey", "dir", "disabled", "id", "immediate", "label", "lang", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "rendered", "required", "selected", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:inputField", new string[] { "id", "label", "list", "list", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "rendered", "required", "showDatePicker", "type", "style", "styleClass", "taborderhint", "type", "type", "type", "value" });
            _visualForceSymbols.Add("apex:inputFile", new string[] { "accept", "accessKey", "alt", "contentType", "dir", "disabled", "fileName", "fileSize", "id", "lang", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "rendered", "required", "size", "style", "styleclass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:inputHidden", new string[] { "id", "immediate", "rendered", "required", "value" });
            _visualForceSymbols.Add("apex:inputSecret", new string[] { "accesskey", "alt", "dir", "disabled", "id", "immediate", "label", "lang", "maxlength", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "readonly", "redisplay", "rendered", "required", "size", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:inputText", new string[] { "accesskey", "alt", "dir", "disabled", "id", "label", "lang", "list", "list", "maxlength", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "required", "size", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:inputTextarea", new string[] { "accesskey", "cols", "dir", "disabled", "id", "label", "lang", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "readonly", "rendered", "required", "richText", "rows", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:insert", new string[] { "name" });
            _visualForceSymbols.Add("apex:legend", new string[] { "font", "id", "padding", "position", "rendered", "spacing" });
            _visualForceSymbols.Add("apex:lineSeries", new string[] { "axis", "fill", "fillColor", "highlight", "highlightStrokeWidth", "id", "markerFill", "markerSize", "markerType", "opacity", "rendered", "rendererFn", "showInLegend", "smooth", "strokeColor", "strokeWidth", "tips", "title", "xField", "yField" });
            _visualForceSymbols.Add("apex:listViews", new string[] { "id", "rendered", "type" });
            _visualForceSymbols.Add("apex:logCallPublisher", new string[] { "autoCollapseBody", "entityId", "id", "logCallBody", "logCallBodyHeight", "onSubmitFailure", "onSubmitSuccess", "rendered", "reRender", "showAdditionalFields", "showSubmitButton", "submitButtonName", "submitFunctionName", "title", "width" });
            _visualForceSymbols.Add("apex:message", new string[] { "dir", "for", "id", "lang", "rendered", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:messages", new string[] { "dir", "globalOnly", "id", "lang", "layout", "rendered", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:milestoneTracker", new string[] { "entityId", "id", "rendered" });
            _visualForceSymbols.Add("apex:outputField", new string[] { "dir", "id", "label", "lang", "rendered", "style", "styleClass", "title", "value" });
            _visualForceSymbols.Add("apex:outputLabel", new string[] { "accesskey", "dir", "escape", "for", "id", "lang", "onblur", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:outputLink", new string[] { "accesskey", "charset", "coords", "dir", "disabled", "hreflang", "id", "lang", "onblur", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rel", "rendered", "rev", "shape", "style", "styleClass", "tabindex", "target", "title", "type", "value" });
            _visualForceSymbols.Add("apex:outputPanel", new string[] { "dir", "id", "lang", "layout", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:outputText", new string[] { "dir", "escape", "id", "label", "lang", "rendered", "style", "styleClass", "title", "value" });
            _visualForceSymbols.Add("apex:page", new string[] { "action", "apiVersion", "applyBodyTag", "applyHtmlTag", "cache", "contentType", "controller", "deferLastCommandUntilReady", "docType", "expires", "extensions", "id", "label", "language", "manifest", "name", "pageStyle", "readOnly", "recordSetName", "recordSetVar", "renderAs", "rendered", "setup", "showChat", "showHeader", "sidebar", "standardController", "standardStylesheets", "tabStyle", "title", "title", "applyHtmlTag", "applyBodyTag", "title", "showHeader", "false", "wizard" });
            _visualForceSymbols.Add("apex:pageBlock", new string[] { "dir", "helpTitle", "helpUrl", "id", "lang", "mode", "detail", "maindetail", "edit", "inlineEdit", "detail", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "tabStyle", "title" });
            _visualForceSymbols.Add("apex:pageBlockButtons", new string[] { "dir", "id", "lang", "location", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:pageBlockSection", new string[] { "collapsible", "columns", "dir", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "showHeader", "title" });
            _visualForceSymbols.Add("apex:pageBlockSectionItem", new string[] { "dataStyle", "dataStyleClass", "dataTitle", "dir", "helpText", "id", "labelStyle", "labelStyleClass", "labelTitle", "lang", "onDataclick", "onDatadblclick", "onDatakeydown", "onDatakeypress", "onDatakeyup", "onDatamousedown", "onDatamousemove", "onDatamouseout", "onDatamouseover", "onDatamouseup", "onLabelclick", "onLabeldblclick", "onLabelkeydown", "onLabelkeypress", "onLabelkeyup", "onLabelmousedown", "onLabelmousemove", "onLabelmouseout", "onLabelmouseover", "onLabelmouseup", "rendered" });
            _visualForceSymbols.Add("apex:pageBlockTable", new string[] { "align", "bgcolor", "border", "captionClass", "captionStyle", "cellpadding", "cellspacing", "columnClasses", "columns", "columnsWidth", "dir", "first", "footerClass", "frame", "headerClass", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onRowClick", "onRowDblClick", "onRowMouseDown", "onRowMouseMove", "onRowMouseOut", "onRowMouseOver", "onRowMouseUp", "rendered", "rowClasses", "rows", "rules", "style", "styleClass", "summary", "title", "value", "var", "width" });
            _visualForceSymbols.Add("apex:pageMessage", new string[] { "detail", "escape", "id", "rendered", "severity", "strength", "summary", "title" });
            _visualForceSymbols.Add("apex:pageMessages", new string[] { "escape", "id", "rendered", "showDetail" });
            _visualForceSymbols.Add("apex:panelBar", new string[] { "contentClass", "contentStyle", "headerClass", "headerClassActive", "headerStyle", "headerStyleActive", "height", "id", "items", "rendered", "style", "styleClass", "switchType", "value", "var", "width" });
            _visualForceSymbols.Add("apex:panelBarItem", new string[] { "contentClass", "contentStyle", "expanded", "headerClass", "headerClassActive", "headerStyle", "headerStyleActive", "id", "label", "name", "onenter", "onleave", "rendered" });
            _visualForceSymbols.Add("apex:panelGrid", new string[] { "bgcolor", "border", "captionClass", "captionStyle", "cellpadding", "cellspacing", "columnClasses", "columns", "dir", "footerClass", "frame", "headerClass", "id", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "rowClasses", "rules", "style", "styleClass", "summary", "title", "width" });
            _visualForceSymbols.Add("apex:panelGroup", new string[] { "id", "layout", "rendered", "style", "styleClass" });
            _visualForceSymbols.Add("apex:param", new string[] { "assignTo", "id", "name", "value" });
            _visualForceSymbols.Add("apex:pieSeries", new string[] { "colorSet", "dataField", "donut", "highlight", "id", "labelField", "rendered", "rendererFn", "showInLegend", "tips" });
            _visualForceSymbols.Add("apex:radarSeries", new string[] { "fill", "highlight", "id", "markerFill", "markerSize", "markerType", "opacity", "rendered", "showInLegend", "strokeColor", "strokeWidth", "tips", "title", "xField", "yField" });
            _visualForceSymbols.Add("apex:relatedList", new string[] { "id", "list", "pageSize", "rendered", "subject", "title" });
            _visualForceSymbols.Add("apex:remoteObjectField", new string[] { "id", "jsShorthand", "name", "rendered" });
            _visualForceSymbols.Add("apex:remoteObjectModel", new string[] { "create", "delete", "fields", "id", "jsShorthand", "name", "rendered", "retrieve", "update" });
            _visualForceSymbols.Add("apex:remoteObjects", new string[] { "create", "delete", "id", "jsNamespace", "rendered", "retrieve", "update" });
            _visualForceSymbols.Add("apex:repeat", new string[] { "first", "id", "rendered", "rows", "value", "var" });
            _visualForceSymbols.Add("apex:scatterSeries", new string[] { "axis", "highlight", "id", "markerFill", "markerSize", "markerType", "rendered", "rendererFn", "showInLegend", "tips", "title", "xField", "yField" });
            _visualForceSymbols.Add("apex:scontrol", new string[] { "controlName", "height", "id", "rendered", "scrollbars", "subject", "width" });
            _visualForceSymbols.Add("apex:sectionHeader", new string[] { "description", "help", "id", "printUrl", "rendered", "subtitle", "title" });
            _visualForceSymbols.Add("apex:selectCheckboxes", new string[] { "accesskey", "border", "borderVisible", "false", "dir", "disabled", "disabledClass", "enabledClass", "id", "immediate", "label", "lang", "layout", "legendInvisible", "false", "true", "legendText", "legendText", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "readonly", "rendered", "required", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:selectList", new string[] { "accesskey", "dir", "disabled", "disabledClass", "enabledClass", "id", "label", "lang", "multiselect", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "readonly", "rendered", "required", "size", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:selectOption", new string[] { "dir", "id", "itemDescription", "itemDisabled", "itemEscaped", "itemLabel", "itemValue", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "style", "styleClass", "title", "value" });
            _visualForceSymbols.Add("apex:selectOptions", new string[] { "id", "rendered", "value" });
            _visualForceSymbols.Add("apex:selectRadio", new string[] { "accesskey", "border", "borderVisible", "false", "dir", "disabled", "disabledClass", "enabledClass", "id", "immediate", "label", "lang", "layout", "legendInvisible", "false", "true", "legendText", "legendText", "onblur", "onchange", "onclick", "ondblclick", "onfocus", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onselect", "readonly", "rendered", "required", "style", "styleClass", "tabindex", "title", "value" });
            _visualForceSymbols.Add("apex:stylesheet", new string[] { "id", "value" });
            _visualForceSymbols.Add("apex:tab", new string[] { "disabled", "focus", "id", "immediate", "label", "labelWidth", "name", "onclick", "oncomplete", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "ontabenter", "ontableave", "rendered", "reRender", "status", "style", "styleClass", "switchType", "timeout", "title" });
            _visualForceSymbols.Add("apex:tabPanel", new string[] { "activeTabClass", "contentClass", "contentStyle", "dir", "disabledTabClass", "headerAlignment", "headerClass", "headerSpacing", "height", "id", "immediate", "inactiveTabClass", "lang", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "reRender", "selectedTab", "style", "styleClass", "switchType", "tabClass", "title", "value", "width" });
            _visualForceSymbols.Add("apex:toolbar", new string[] { "contentClass", "contentStyle", "height", "id", "itemSeparator", "onclick", "ondblclick", "onitemclick", "onitemdblclick", "onitemkeydown", "onitemkeypress", "onitemkeyup", "onitemmousedown", "onitemmousemove", "onitemmouseout", "onitemmouseover", "onitemmouseup", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "separatorClass", "style", "styleClass", "width" });
            _visualForceSymbols.Add("apex:toolbarGroup", new string[] { "id", "itemSeparator", "location", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "rendered", "separatorClass", "style", "styleClass" });
            _visualForceSymbols.Add("apex:variable", new string[] { "id", "rendered", "value", "var" });
            _visualForceSymbols.Add("apex:vote", new string[] { "id", "objectId", "rendered", "rerender" });
            _visualForceSymbols.Add("chatter:feed", new string[] { "entityId", "feedItemType", "id", "onComplete", "rendered", "reRender", "showPublisher" });
            _visualForceSymbols.Add("chatter:feedWithFollowers", new string[] { "entityId", "id", "onComplete", "rendered", "reRender", "showHeader" });
            _visualForceSymbols.Add("chatter:follow", new string[] { "entityId", "id", "onComplete", "rendered", "reRender" });
            _visualForceSymbols.Add("chatter:followers", new string[] { "entityId", "id", "rendered" });
            _visualForceSymbols.Add("chatter:newsfeed", new string[] { "id", "onComplete", "rendered", "reRender" });
            _visualForceSymbols.Add("chatter:userPhotoUpload", new string[] { "id", "rendered", "showOriginalPhoto" });
            _visualForceSymbols.Add("chatteranswers:aboutme", new string[] { "communityId", "id", "noSignIn", "rendered" });
            _visualForceSymbols.Add("chatteranswers:allfeeds", new string[] { "articleLanguage", "communityId", "filterOptions", "forceSecureCustomWebAddress", "id", "jsApiVersion", "noSignIn", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:changepassword", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("chatteranswers:datacategoryfilter", new string[] { "communityId", "id", "rendered" });
            _visualForceSymbols.Add("chatteranswers:feedfilter", new string[] { "filterOptions", "id", "rendered" });
            _visualForceSymbols.Add("chatteranswers:feeds", new string[] { "articleLanguage", "communityId", "id", "jsApiVersion", "noSignIn", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:forgotpassword", new string[] { "id", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:forgotpasswordconfirm", new string[] { "id", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:guestsignin", new string[] { "id", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:help", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("chatteranswers:login", new string[] { "id", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:registration", new string[] { "hideTerms", "id", "profileId", "registrationClassName", "rendered", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:searchask", new string[] { "communityId", "id", "noSignIn", "rendered", "searchLanguage", "useUrlRewriter" });
            _visualForceSymbols.Add("chatteranswers:singleitemfeed", new string[] { "entityId", "id", "rendered" });
            _visualForceSymbols.Add("flow:interview", new string[] { "buttonLocation", "buttonStyle", "finishLocation", "id", "interview", "name", "rendered", "rerender", "showHelp" });
            _visualForceSymbols.Add("ideas:detailOutputLink", new string[] { "id", "ideaId", "page", "pageNumber", "pageOffset", "rendered", "style", "styleClass" });
            _visualForceSymbols.Add("ideas:listOutputLink", new string[] { "category", "communityId", "id", "page", "pageNumber", "pageOffset", "rendered", "sort", "status", "stickyAttributes", "style", "styleClass" });
            _visualForceSymbols.Add("ideas:profileListOutputLink", new string[] { "communityId", "id", "page", "pageNumber", "pageOffset", "rendered", "sort", "stickyAttributes", "style", "styleClass", "userId" });
            _visualForceSymbols.Add("knowledge:articleCaseToolbar", new string[] { "articleId", "caseId", "id", "includeCSS", "rendered" });
            _visualForceSymbols.Add("knowledge:articleList", new string[] { "articleTypes", "articleVar", "categories", "hasMoreVar", "id", "keyword", "language", "pageNumber", "pageSize", "rendered", "sortBy" });
            _visualForceSymbols.Add("knowledge:articleRendererToolbar", new string[] { "articleId", "canVote", "id", "includeCSS", "rendered", "showChatter" });
            _visualForceSymbols.Add("knowledge:articleTypeList", new string[] { "articleTypeVar", "id", "rendered" });
            _visualForceSymbols.Add("knowledge:categoryList", new string[] { "ancestorsOf", "categoryGroup", "categoryVar", "id", "level", "rendered", "rootCategory" });
            _visualForceSymbols.Add("liveAgent:clientChat", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatAlertMessage", new string[] { "agentsUnavailableLabel", "connectionErrorLabel", "dismissLabel", "id", "noCookiesLabel", "noFlashLabel", "noHashLabel", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatEndButton", new string[] { "id", "label", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatFileTransfer", new string[] { "fileTransferCanceledLabel", "fileTransferCancelFileLabel", "fileTransferDropFileLabel", "fileTransferFailedLabel", "fileTransferSendFileLabel", "fileTransferSuccessfulLabel", "fileTransferUploadLabel", "id", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatInput", new string[] { "autoResizeElementId", "id", "rendered", "useMultiline" });
            _visualForceSymbols.Add("liveAgent:clientChatLog", new string[] { "agentTypingLabel", "chatEndedByAgentLabel", "chatEndedByVisitorLabel", "chatTransferredLabel", "id", "rendered", "visitorNameLabel" });
            _visualForceSymbols.Add("liveAgent:clientChatMessages", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatQueuePosition", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatSaveButton", new string[] { "id", "label", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatSendButton", new string[] { "id", "label", "rendered" });
            _visualForceSymbols.Add("liveAgent:clientChatStatusMessage", new string[] { "id", "reconnectingLabel", "rendered" });
            _visualForceSymbols.Add("messaging:attachment", new string[] { "filename", "id", "inline", "renderAs", "rendered" });
            _visualForceSymbols.Add("messaging:emailHeader", new string[] { "id", "name", "rendered" });
            _visualForceSymbols.Add("messaging:emailTemplate", new string[] { "id", "language", "recipientType", "relatedToType", "rendered", "replyTo", "subject" });
            _visualForceSymbols.Add("messaging:htmlEmailBody", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("messaging:plainTextEmailBody", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("site:googleAnalyticsTracking", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("site:previewAsAdmin", new string[] { "id", "rendered" });
            _visualForceSymbols.Add("social:profileViewer", new string[] { "entityId", "id", "rendered" });
            _visualForceSymbols.Add("support:caseArticles", new string[] { "articleTypes", "attachToEmailEnabled", "bodyHeight", "caseId", "categories", "categoryMappingEnabled", "defaultKeywords", "defaultSearchType", "id", "insertLinkToEmail", "language", "logSearch", "mode", "onSearchComplete", "rendered", "reRender", "searchButtonName", "searchFieldWidth", "searchFunctionName", "showAdvancedSearch", "title", "titlebarStyle", "width" });
            _visualForceSymbols.Add("support:caseFeed", new string[] { "caseId", "id", "rendered" });
            _visualForceSymbols.Add("support:caseUnifiedFiles", new string[] { "entityId", "id", "rendered" });
            _visualForceSymbols.Add("support:clickToDial", new string[] { "entityId", "id", "number", "params", "rendered" });
            _visualForceSymbols.Add("support:portalPublisher", new string[] { "answerBody", "answerBodyHeight", "autoCollapseBody", "entityId", "id", "onSubmitFailure", "onSubmitSuccess", "rendered", "reRender", "showSendEmailOption", "showSubmitButton", "submitButtonName", "submitFunctionName", "title", "width" });
            _visualForceSymbols.Add("topics:widget", new string[] { "customUrl", "entity", "id", "rendered", "renderStyle" });

            _visualForceSymbols.Add("a", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "download", "draggable", "dropzone", "hidden", "href", "hreflang", "id", "lang", "media", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "rel", "spellcheck", "style", "tabindex", "target", "title", "translate", "type" });
            _visualForceSymbols.Add("abbr", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("address", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("area", new string[] { "accesskey", "alt", "class", "contenteditable", "contextmenu", "coords", "dir", "download", "draggable", "dropzone", "hidden", "href", "hreflang", "id", "lang", "media", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "rel", "shape", "spellcheck", "style", "tabindex", "target", "title", "translate", "type" });
            _visualForceSymbols.Add("article", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("aside", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("audio", new string[] { "accesskey", "autoplay", "class", "contenteditable", "contextmenu", "controls", "dir", "draggable", "dropzone", "hidden", "id", "lang", "loop", "muted", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "preload", "spellcheck", "src", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("b", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("base", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "href", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "target", "title", "translate" });
            _visualForceSymbols.Add("bdi", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("bdo", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("blockquote", new string[] { "accesskey", "cite", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("body", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("br", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("button", new string[] { "accesskey", "autofocus", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "formaction", "formenctype", "formmethod", "formnovalidate", "formtarget", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "type", "value" });
            _visualForceSymbols.Add("canvas", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "height", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "width" });
            _visualForceSymbols.Add("caption", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("cite", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("code", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("col", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "span", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("colgroup", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "span", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("datalist", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("dd", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("del", new string[] { "accesskey", "cite", "class", "contenteditable", "contextmenu", "datetime", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("details", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "open", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("dfn", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("dialog", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "open", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("div", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("dl", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("dt", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("em", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("embed", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "height", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "src", "style", "tabindex", "title", "translate", "type", "width" });
            _visualForceSymbols.Add("fieldset", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("figcaption", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("figure", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("footer", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("form", new string[] { "accesskey", "action", "autocomplete", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "enctype", "hidden", "id", "lang", "method", "name", "novalidate", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "target", "title", "translate" });
            _visualForceSymbols.Add("h1", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("h2", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("h3", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("h4", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("h5", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("h6", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("head", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("header", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("hr", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("html", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "manifest", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "xmlns" });
            _visualForceSymbols.Add("i", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("iframe", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "height", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "sandbox", "seamless", "spellcheck", "src", "srcdoc", "style", "tabindex", "title", "translate", "width" });
            _visualForceSymbols.Add("img", new string[] { "accesskey", "alt", "class", "crossorigin", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "height", "hidden", "id", "ismap", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "src", "style", "tabindex", "title", "translate", "usemap", "width" });
            _visualForceSymbols.Add("input", new string[] { "accept", "accesskey", "align", "alt", "autocomplete", "autofocus", "checked", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "formaction", "formenctype", "formmethod", "formnovalidate", "formtarget", "height", "hidden", "id", "lang", "list", "max", "maxlength", "min", "multiple", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "pattern", "placeholder", "readonly", "required", "size", "spellcheck", "src", "step", "style", "tabindex", "title", "translate", "type", "value", "width" });
            _visualForceSymbols.Add("ins", new string[] { "accesskey", "cite", "class", "contenteditable", "contextmenu", "datetime", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("kbd", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("keygen", new string[] { "accesskey", "autofocus", "challenge", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "hidden", "id", "keytype", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("label", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "for", "form", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("legend", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("li", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "value" });
            _visualForceSymbols.Add("link", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "href", "hreflang", "id", "lang", "media", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "rel", "sizes", "spellcheck", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("main", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("map", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("mark", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("menu", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "label", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("menuitem", new string[] { "accesskey", "checked", "class", "command", "contenteditable", "contextmenu", "default", "dir", "disabled", "draggable", "dropzone", "hidden", "icon", "id", "label", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "radiogroup", "spellcheck", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("meta", new string[] { "accesskey", "charset", "class", "content", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "name", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("meter", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "form", "hidden", "high", "id", "lang", "low", "max", "min", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "optimum", "spellcheck", "style", "tabindex", "title", "translate", "value" });
            _visualForceSymbols.Add("nav", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("noscript", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("object", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "data", "dir", "draggable", "dropzone", "form", "height", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "type", "usemap", "width" });
            _visualForceSymbols.Add("ol", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "reversed", "spellcheck", "start", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("optgroup", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "hidden", "id", "label", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("option", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "hidden", "id", "label", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "selected", "spellcheck", "style", "tabindex", "title", "translate", "value" });
            _visualForceSymbols.Add("output", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "for", "form", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("p", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("param", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "value" });
            _visualForceSymbols.Add("pre", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("progress", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "max", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate", "value" });
            _visualForceSymbols.Add("q", new string[] { "accesskey", "cite", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("rp", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("rt", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("ruby", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("s", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("samp", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("script", new string[] { "accesskey", "async", "charset", "class", "contenteditable", "contextmenu", "defer", "dir", "draggable", "dropzone", "hidden", "id", "lang", "spellcheck", "src", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("section", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("select", new string[] { "accesskey", "autofocus", "class", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "hidden", "id", "lang", "multiple", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "required", "size", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("small", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("source", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "media", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "src", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("span", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("strong", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("style", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "media", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "scoped", "spellcheck", "style", "tabindex", "title", "translate", "type" });
            _visualForceSymbols.Add("sub", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("summary", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("sup", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("table", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "sortable", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("tbody", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("td", new string[] { "accesskey", "class", "colspan", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "headers", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "rowspan", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("textarea", new string[] { "accesskey", "autofocus", "class", "cols", "contenteditable", "contextmenu", "dir", "disabled", "draggable", "dropzone", "form", "hidden", "id", "lang", "maxlength", "name", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "placeholder", "readonly", "required", "rows", "spellcheck", "style", "tabindex", "title", "translate", "wrap" });
            _visualForceSymbols.Add("tfoot", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("th", new string[] { "abbr", "accesskey", "class", "colspan", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "headers", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "rowspan", "scope", "sorted", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("thead", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("time", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "datetime", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("title", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("tr", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("track", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "default", "dir", "draggable", "dropzone", "hidden", "id", "kind", "label", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "src", "srclang", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("u", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("ul", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("var", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });
            _visualForceSymbols.Add("video", new string[] { "accesskey", "autoplay", "class", "contenteditable", "contextmenu", "controls", "dir", "draggable", "dropzone", "height", "hidden", "id", "lang", "loop", "muted", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "poster", "preload", "spellcheck", "src", "style", "tabindex", "title", "translate", "width" });
            _visualForceSymbols.Add("wbr", new string[] { "accesskey", "class", "contenteditable", "contextmenu", "dir", "draggable", "dropzone", "hidden", "id", "lang", "onabort", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncuechange", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus", "onformchange", "onforminput", "oninput", "oninvalid", "onkeydown", "onkeypress", "onkeyup", "onloadeddata", "onloadedmetadata", "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onpause", "onplay", "onplaying", "onprogress", "onratechange", "onscroll", "onseeked", "onseeking", "onselect", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "onvolumechange", "onwaiting", "spellcheck", "style", "tabindex", "title", "translate" });

            _visualForceTags = _visualForceSymbols.Keys.OrderBy(tag => tag).ToArray();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="language">Language.</param>
        public LanguageCompletion(LanguageManager language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _language = language;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get visualforce tags for the current position.
        /// </summary>
        /// <param name="text">The text to inspect to determine tags.</param>
        /// <returns>The tags to use for completions.</returns>
        public string[] GetVisualForceCompletionsTags(Stream text)
        {
            return _visualForceTags;
        }

        /// <summary>
        /// Get visualforce tags for the current position.
        /// </summary>
        /// <param name="text">The text to inspect to determine tags.</param>
        /// <returns>The tags to use for completions.</returns>
        public string[] GetVisualForceCompletionsAttributes(Stream text)
        {
            string line = null;
            bool openString = false;
            int startAttributeIndex = 0;
            int insertIndex = (int)text.Position;

            if (text.Position > 0)
            {
                // get line to calculate completions for
                StringBuilder lineBuilder = new StringBuilder();
                bool stop = false;

                while (text.Position > 0)
                {
                    text.Position = text.Position - 1;
                    char c = (char)text.ReadByte();

                    switch (c)
                    {
                        case '>':
                            return new string[0];

                        case '<':
                        case '/':
                            bool firstCharFound = false;
                            int b = text.ReadByte();
                            while (b != -1)
                            {
                                char c2 = (char)b;
                                switch (c2)
                                {
                                    case ' ':
                                    case '\t':
                                    case '\n':
                                    case '\r':
                                        if (firstCharFound)
                                        {
                                            stop = true;
                                            startAttributeIndex = (int)text.Position;
                                        }
                                        break;

                                    default:
                                        firstCharFound = true;
                                        lineBuilder.Append(c2);
                                        break;
                                }
                                if (stop)
                                    break;

                                b = text.ReadByte();
                            }
                            stop = true;
                            break;

                        case '"':
                            openString = !openString;
                            break;

                        default:
                            break;
                    }

                    if (stop)
                        break;

                    text.Position = text.Position - 1;
                }

                line = lineBuilder.ToString();
            }

            if (openString || 
                insertIndex < startAttributeIndex || 
                String.IsNullOrWhiteSpace(line))
                return new string[0];

            if (_visualForceSymbols.ContainsKey(line))
                return _visualForceSymbols[line];
            else
                return new string[0];
        }

        /// <summary>
        /// Check to see if the given position is within a comment.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="text">The text to check.</param>
        /// <returns>true if the position is within a comment.</returns>
        private bool IsInToken(TextPosition position, Stream text, Tokens[] tokens)
        {
            long currentPosition = text.Position;

            bool result = false;
            text.Position = 0;
            ApexLexer lexer = new ApexLexer(text, true);
            while (lexer.yylex() > 2)
            {
                if (lexer.yylval != null)
                {
                    if (lexer.yylval.TextSpan.Contains(position) ||
                        lexer.yylval.TextSpan.StartLine > position.Line)
                        break;
                }
            }

            text.Position = currentPosition;

            if (lexer.yylval != null &&
                lexer.yylval.TextSpan.Contains(position) &&
                tokens.Contains(lexer.yylval.Token))
                return true;

            return result;
        }

        /// <summary>
        /// Break the given line in the given text down to distinct parts.
        /// </summary>
        /// <param name="text">The text to get parts for.</param>
        /// <returns>The parts to process.</returns>
        private string[] GetLineParts(Stream text)
        {
            StringBuilder lineBuilder = new StringBuilder();

            if (text.Position > 0)
            {
                // find start of the line
                int openDelimiterCount = 0;
                bool stop = false;

                while (text.Position > 0)
                {
                    text.Position = text.Position - 1;
                    char c = (char)text.ReadByte();

                    switch (c)
                    {
                        case ')':
                        case ']':
                        case '>':
                            openDelimiterCount++;
                            break;

                        case '{':
                        case '}':
                        case '(':
                        case '[':
                        case '<':
                            if (openDelimiterCount == 0)
                                stop = true;
                            else
                                openDelimiterCount--;
                            break;

                        case ';':
                        case ',':
                        case '=':
                        case '-':
                        case '+':
                        case '*':
                        case ':':
                        case '&':
                        case '|':
                        case '!':
                        case '^':
                        case '?':
                            if (openDelimiterCount == 0)
                                stop = true;
                            break;

                        // special case for inline comments
                        case '/':
                            if (openDelimiterCount == 0)
                            {
                                stop = true;
                                if (text.Position > 1)
                                {
                                    text.Position = text.Position - 2;
                                    c = (char)text.ReadByte();
                                    if (c == '/')
                                        lineBuilder.Insert(0, "//");
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    if (stop)
                        break;

                    lineBuilder.Insert(0, c);
                    text.Position = text.Position - 1;
                }
            }

            // lex the line
            List<string> parts = new List<string>();
            StringBuilder delimiterBuilder = new StringBuilder();
            StringBuilder templateBuilder = new StringBuilder();
            int openBracket = 0;
            int openParens = 0;
            int openTemplate = 0;

            using (MemoryStream lineReader = new MemoryStream(Encoding.ASCII.GetBytes(lineBuilder.ToString())))
            {
                ApexLexer lexer = new ApexLexer(lineReader);
                while (lexer.yylex() > 2)
                {
                    if (lexer.yylval != null)
                    {
                        switch (lexer.yylval.Token)
                        {
                            case Tokens.SEPARATOR_BRACKET_EMPTY:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openBracket == 0 && openParens == 0)
                                {
                                    if (parts.Count == 0)
                                        parts.Add("[]");
                                    else
                                        parts[parts.Count - 1] = String.Format("{0}[]", parts[parts.Count - 1]);
                                }
                                break;

                            case Tokens.OPERATOR_LESS_THAN:
                                if (openBracket == 0 && openParens == 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                    openTemplate++;
                                }
                                break;

                            case Tokens.OPERATOR_GREATER_THAN:
                            case Tokens.OPERATOR_GREATER_THAN_A:
                            case Tokens.OPERATOR_GREATER_THAN_B:
                            case Tokens.OPERATOR_GREATER_THAN_C:
                                if (openBracket == 0 && openParens == 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                    openTemplate--;

                                    if (openTemplate == 0)
                                    {
                                        if (parts.Count == 0)
                                            parts.Add(templateBuilder.ToString());
                                        else
                                            parts[parts.Count - 1] = String.Format("{0}{1}", parts[parts.Count - 1], templateBuilder.ToString());

                                        templateBuilder.Clear();
                                    }
                                }
                                break;

                            case Tokens.SEPARATOR_BRACKET_LEFT:                            
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openParens == 0)
                                {
                                    if (openBracket == 0)
                                    {
                                        if (parts.Count == 0)
                                            parts.Add("[]");
                                        else
                                            parts[parts.Count - 1] = String.Format("{0}[]", parts[parts.Count - 1]);
                                    }

                                    openBracket++;
                                }
                                break;

                            case Tokens.SEPARATOR_BRACKET_RIGHT:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openParens == 0)
                                {
                                    openBracket--;
                                }
                                break;

                            case Tokens.SEPARATOR_PARENTHESES_LEFT:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openBracket == 0)
                                {
                                    if (openParens == 0)
                                    {
                                        if (parts.Count == 0)
                                            parts.Add("()");
                                        else
                                            parts[parts.Count - 1] = String.Format("{0}()", parts[parts.Count - 1]);
                                    }

                                    openParens++;
                                }
                                break;

                            case Tokens.SEPARATOR_PARENTHESES_RIGHT:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openBracket == 0)
                                {
                                    openParens--;
                                }
                                break;

                            case Tokens.SEPARATOR_DOT:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                break;

                            case Tokens.IDENTIFIER:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openBracket == 0 && openParens == 0)
                                {
                                    parts.Add(lexer.yytext);
                                }
                                break;

                            case Tokens.SOQL:
                            case Tokens.SOSL:
                                if (lexer.yylval.Text != null)
                                {
                                    Match match = Regex.Match(lexer.yylval.Text, "FROM[ \t\n\r]+[A-Za-z_0-9]+", RegexOptions.IgnoreCase);
                                    if (match != null)
                                    {
                                        int index = match.Value.IndexOfAny(new char[] { ' ', '\t', '\n', '\r' });
                                        if (index != -1)
                                        {
                                            parts.Add("_select");
                                            parts.Add(match.Value.Substring(index).Trim());
                                        }
                                    }
                                }
                                break;

                            default:
                                if (openTemplate > 0)
                                {
                                    templateBuilder.Append(lexer.yytext);
                                }
                                else if (openBracket == 0 && openParens == 0)
                                {
                                    parts.Add(lexer.yytext);
                                }
                                break;
                        }
                    }
                }
            }

            return parts.ToArray();
        }

        /// <summary>
        /// Get the symbols leading up to the current text position.
        /// </summary>
        /// <param name="text">The text to get the symbols from.</param>
        /// <param name="className">The name of the class the text is in.</param>
        /// <param name="position">The position in the class text to start from.</param>
        /// <param name="includeIncompleteMethods">If true, incomplete methods will be included in matches.</param>
        /// <returns>The symbols that matched or null if a match can't be made.</returns>
        private Symbol[] MatchSymbols(Stream text, string className, TextPosition position, bool includeIncompleteMethods)
        {
            List<Symbol> result = new List<Symbol>();

            // get parts and do some pre-processing
            List<string> parts = new List<string>(GetLineParts(text));

            // special process for triggers
            if (parts.Count > 0 && (parts[0].ToLower() == "trigger" || parts[0].ToLower() == "system.trigger"))
            {
                SymbolTable trigger = _language.GetSymbols("trigger");
                if (trigger != null)
                {
                    result.Add(trigger);

                    if (parts.Count > 1 && parts[1].ToLower() == "new")
                        result.Add(new Field(new TextPosition(0, 0), "new", null, SymbolModifier.Public | SymbolModifier.Static, "List<sObject>"));
                }
            }

            // tack on method symbol to last part when specified
            if (includeIncompleteMethods && parts.Count > 0)
                parts[parts.Count - 1] = String.Format("{0}()", parts[parts.Count - 1]);
            
            bool isNewStatement = false;

            for (int i = result.Count; i < parts.Count; i++)
            {
                parts[i] = parts[i].ToLower();

                // check for new statement
                if (parts[i] == "new")
                    isNewStatement = true;

                // remove generic symbols
                if (parts[i] != "this" && 
                    parts[i] != "super" &&
                    _genericCompletions.Any(s => s.Id == parts[i]))
                {
                    parts.RemoveAt(i);
                    i--;
                }
                // remove if and for statements
                else if (parts[i] == "if()" ||
                         parts[i] == "for()")
                {
                    parts.RemoveAt(i);
                    i--;
                }
                // replace list indexer with it's method counterpart
                else if (parts[i].EndsWith("[]"))
                {
                    int index = i;
                    while (parts[index].EndsWith("[]"))
                    {
                        parts[index] = parts[index].Substring(0, parts[index].Length - 2);
                        parts.Insert(index + 1, "get()");
                        i++;
                    }
                }
            }

            // check for constructor
            if (result.Count == 0 && isNewStatement && parts.Count > 0)
            {
                StringBuilder constructorBuilder = new StringBuilder();
                foreach (string part in parts)
                    constructorBuilder.AppendFormat("{0}.", part);

                if (constructorBuilder.Length > 0)
                {
                    constructorBuilder.Length = constructorBuilder.Length - 1;
                    if (includeIncompleteMethods || constructorBuilder.ToString().EndsWith("()"))
                    {
                        constructorBuilder.Length = constructorBuilder.Length - 2;

                        SymbolTable constructorClass = _language.GetSymbols(constructorBuilder.ToString());
                        if (constructorClass != null)
                            return new Symbol[] { constructorClass, new Constructor(new TextPosition(0, 0), constructorClass.Type, null, SymbolModifier.Public, null) };
                    }
                }
            }

            // match parts to types
            SymbolTable classSymbol = _language.GetSymbols(className);
            TypedSymbol matchedSymbol = (result.Count == 0) ? null : (TypedSymbol)result[result.Count - 1];
            bool partFound = (matchedSymbol != null);

            if (classSymbol != null)
            {
                bool typeSearchDone = false;

                for (int i = result.Count; i < parts.Count; i++)
                {
                    string part = parts[i];
                    partFound = false;

                    // SOQL or SOSL part
                    if (parts[i] == "_select" && i + 1 < parts.Count)
                    {
                        i++;
                        matchedSymbol = new Field(
                            new TextPosition(0, 0),
                            "soql",
                            null,
                            SymbolModifier.Public | SymbolModifier.Static,
                            String.Format("{0}[]", parts[1]));
                        partFound = true;
                    }
                    // method
                    else if (parts[i].EndsWith("()"))
                    {
                        string methodName = part.Substring(0, part.IndexOf('('));
                        SymbolTable externalClass = (i == 0) ? classSymbol : _language.GetSymbols(matchedSymbol.FullType);
                        while (externalClass != null && !partFound)
                        {
                            foreach (Method m in GetMethods(externalClass, false))
                            {
                                if (m.Id == methodName)
                                {
                                    if (m.Type != "void" || i == parts.Count - 1)
                                    {
                                        matchedSymbol = m;
                                        partFound = true;
                                    }
                                    break;
                                }
                            }

                            if (!partFound)
                                externalClass = _language.GetSymbols(externalClass.Extends);
                        }
                    }
                    // variables
                    else
                    {
                        SymbolTable externalClass = null;
                        if (i == 0)
                            externalClass = classSymbol;
                        else if (matchedSymbol != null) 
                            externalClass = _language.GetSymbols(matchedSymbol.FullType);

                        if (i == 0)
                        {
                            // this keyword
                            if (part == "this")
                            {
                                matchedSymbol = new Field(new TextPosition(0, 0), "this", null, SymbolModifier.None, classSymbol.FullType);
                                partFound = true;
                            }
                            // super keyword
                            else if (part == "super")
                            {
                                matchedSymbol = new Field(new TextPosition(0, 0), "super", null, SymbolModifier.None, classSymbol.Extends);
                                partFound = true;
                            }
                            else
                            {
                                // look for method parameters
                                foreach (Method method in classSymbol.Methods)
                                {
                                    if (method.Contains(position))
                                    {
                                        foreach (Parameter parameter in method.Parameters)
                                        {
                                            if (parameter.Id == part)
                                            {
                                                matchedSymbol = parameter;
                                                partFound = true;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                // look for constructor parameters
                                if (!partFound)
                                {
                                    foreach (Constructor constructor in classSymbol.Constructors)
                                    {
                                        if (constructor.Contains(position))
                                        {
                                            foreach (Parameter parameter in constructor.Parameters)
                                            {
                                                if (parameter.Id == part)
                                                {
                                                    matchedSymbol = parameter;
                                                    partFound = true;
                                                    break;
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }

                                // look for local variable
                                if (!partFound)
                                {
                                    foreach (VariableScope scope in classSymbol.VariableScopes)
                                    {
                                        if (scope.Span.Contains(position))
                                        {
                                            foreach (Field variable in scope.Variables)
                                            {
                                                if (variable.Id == part && variable.Location.CompareTo(position) < 0)
                                                {
                                                    matchedSymbol = variable;
                                                    partFound = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (partFound)
                                            break;
                                    }
                                }
                            }
                        }

                        if (externalClass != null)
                        {
                            // look for fields
                            if (!partFound)
                            {
                                foreach (Field field in GetFields(externalClass, false))
                                {
                                    if (field.Id == part)
                                    {
                                        matchedSymbol = field;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }

                            // look for properties
                            if (!partFound)
                            {
                                foreach (Property property in GetProperties(externalClass, false))
                                {
                                    if (property.Id == part)
                                    {
                                        matchedSymbol = property;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // check for inner class
                    if (!partFound && matchedSymbol != null)
                    {
                        SymbolTable symbolType = _language.GetSymbols(matchedSymbol.FullType);
                        if (symbolType != null)
                        {
                            foreach (SymbolTable innerClass in symbolType.InnerClasses)
                            {
                                if (part == innerClass.Id)
                                {
                                    matchedSymbol = innerClass;
                                    partFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    // check for type reference
                    if (!partFound && !typeSearchDone && result.Count == 0)
                    {
                        TypedSymbol tempSymbol = matchedSymbol;
                        int tempIndex = i;

                        StringBuilder typeNameBuilder = new StringBuilder();
                        for (i = 0; i < parts.Count; i++)
                        {
                            if (i == 0)
                                typeNameBuilder.Append(parts[i]);
                            else
                                typeNameBuilder.AppendFormat(".{0}", parts[i]);

                            matchedSymbol = _language.GetSymbols(typeNameBuilder.ToString());
                            if (matchedSymbol != null)
                            {
                                partFound = true;
                                break;
                            }
                        }

                        if (!partFound)
                        {
                            matchedSymbol = tempSymbol;
                            i = tempIndex;
                        }

                        typeSearchDone = true;
                    }

                    // collect the matched symbol
                    if (partFound && matchedSymbol != null)
                        result.Add(matchedSymbol);
                    else if (matchedSymbol == null)
                        return null;
                }
            }

            if (result.Count == 0 || !partFound)
                return null;
            else
                return result.ToArray();
        }

        /// <summary>
        /// Get generic code completions.
        /// </summary>
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Generic code completions.</returns>
        public Symbol[] GetCodeCompletionsLetter(Stream text, string className, TextPosition position)
        {
            if (IsInToken(position, text, _tokensToIgnore))
                return new Symbol[0];

            string word = null;
            bool isOpenBracket = false;

            if (text.Position > 0)
            {
                // get the char directly before the insertion point
                text.Position = text.Position - 1;
                char prevChar = (char)text.ReadByte();
                if (Char.IsLetterOrDigit(prevChar) || prevChar == '_' || prevChar == '\'' || prevChar == '.')
                    return new Symbol[0];
                if (prevChar == '[')
                    isOpenBracket = true;
            }

            // process the words
            string[] parts = GetLineParts(text);
            if (parts != null && parts.Length > 0)
            {
                // check the last word
                string lastWord = parts[parts.Length - 1];
                switch (lastWord)
                {
                    case "abstract":
                    case "delete":
                    case "extends":
                    case "final":
                    case "global":
                    case "implements":
                    case "insert":
                    case "merge":
                    case "new":
                    case "override":
                    case "private":
                    case "protected":
                    case "public":
                    case "return":
                    case "static":
                    case "testmethod":
                    case "throw":
                    case "transient":
                    case "undelete":
                    case "update":
                    case "upsert":
                    case "virtual":
                    case "webservice":
                    case "with sharing":
                    case "without sharing":
                        break;

                    default:
                        if (_genericCompletions.Any(s => s.Id == word))
                            return new Symbol[0];
                        break;
                }

                // check for a type starting at the end
                StringBuilder typeBuilder = new StringBuilder();
                bool done = false;
                for (int i = parts.Length - 1; i > -1; i--)
                {
                    switch (parts[i])
                    {
                        case "abstract":
                        case "delete":
                        case "extends":
                        case "final":
                        case "global":
                        case "implements":
                        case "insert":
                        case "merge":
                        case "new":
                        case "override":
                        case "private":
                        case "protected":
                        case "public":
                        case "return":
                        case "static":
                        case "testmethod":
                        case "throw":
                        case "transient":
                        case "undelete":
                        case "update":
                        case "upsert":
                        case "virtual":
                        case "webservice":
                        case "with sharing":
                        case "without sharing":
                            done = true;
                            break;

                        default:
                            if (_genericCompletions.Any(s => s.Id == parts[i]))
                            {
                                if (i == parts.Length - 1)
                                    return new Symbol[0];
                                else
                                    done = true;
                            }
                            else
                            {
                                if (typeBuilder.Length == 0)
                                    typeBuilder.Append(parts[i]);
                                else
                                    typeBuilder.Insert(0, String.Format("{0}.", parts[i]));

                                if (_language.GetSymbols(typeBuilder.ToString()) != null)
                                    return new Symbol[0];
                            }
                            break;
                    }

                    if (done)
                        break;
                }
            }

            // if we get to this point then return symbols for completions
            List<Symbol> result = new List<Symbol>();
            result.AddRange(_genericCompletions);
            result.AddRange(_language.GetAllSymbols());

            // if the first non-whitespace char before the insertion point is a bracket add the SELECT and FIND keywords
            if (isOpenBracket)
            {
                result.Add(new Keyword("SELECT"));
                result.Add(new Keyword("FIND"));
            }

            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol != null)
            {
                // add variables from the current scope
                foreach (VariableScope scope in classSymbol.VariableScopes)
                {
                    if (scope.Span.Contains(position))
                    {
                        foreach (Field field in scope.Variables)
                            if (field.Location.CompareTo(position) < 0)
                                result.Add(field);
                    }
                }

                // add parameters from current method or constructor
                bool containerFound = false;
                foreach (Method m in classSymbol.Methods)
                {
                    if (m.Contains(position))
                    {
                        result.AddRange(m.Parameters);
                        containerFound = true;
                        break;
                    }
                }

                if (!containerFound)
                {
                    foreach (Constructor c in classSymbol.Constructors)
                    {
                        if (c.Contains(position))
                        {
                            result.AddRange(c.Parameters);
                            containerFound = true;
                            break;
                        }
                    }
                }

                // add local members
                result.AddRange(GetFields(classSymbol, false));
                result.AddRange(GetProperties(classSymbol, false));
                result.AddRange(GetMethods(classSymbol, false));
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Parse the text and give possible symbols that can be added to the end of the text.
        /// </summary>
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Valid symbols that can be used for the code completion.</returns>
        public Symbol[] GetCodeCompletionsDot(Stream text, string className, TextPosition position)
        {
            if (IsInToken(position, text, _tokensToIgnore))
                return new Symbol[0];

            List<Symbol> result = new List<Symbol>();

            // get class definition
            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol == null)
                return result.ToArray();

            // get symbol matches
            Symbol[] matchedSymbols = MatchSymbols(text, className, position, false);
            if (matchedSymbols == null || matchedSymbols.Length == 0)
                return result.ToArray();

            // get the symbol definitions
            Symbol symbol = matchedSymbols[matchedSymbols.Length - 1];
            SymbolTable typeSymbol = null;
            bool isTypeReference = false;
            if (symbol is SymbolTable)
            {
                isTypeReference = true;
                typeSymbol = symbol as SymbolTable;
            }
            else
            {
                isTypeReference = false;
                if (symbol is Constructor && matchedSymbols[0] is SymbolTable)
                    typeSymbol = matchedSymbols[0] as SymbolTable;
                else if (symbol is TypedSymbol)
                    typeSymbol = _language.GetSymbols((symbol as TypedSymbol).FullType);
            }

            if (typeSymbol == null)
                return result.ToArray();

            // determine if the symbol is an external reference or not
            bool isExternal = true;
            SymbolTable classCheck = classSymbol;
            while (classCheck != null)
            {
                isExternal = (classCheck.Id != symbol.Id);
                if (!isExternal)
                    break;

                classCheck = _language.GetSymbols(classCheck.Extends);
            }

            // build the results
            result.AddRange(GetFields(typeSymbol, isExternal));
            result.AddRange(GetProperties(typeSymbol, isExternal));
            result.AddRange(GetMethods(typeSymbol, isExternal));

            // filter out values
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] is ModifiedSymbol)
                {
                    SymbolModifier modifier = (result[i] as ModifiedSymbol).Modifier;
                    if ((modifier.HasFlag(SymbolModifier.Static) && !isTypeReference) ||
                        (!modifier.HasFlag(SymbolModifier.Static) && isTypeReference))
                    {
                        result.RemoveAt(i);
                        i--;
                    }
                }
            }

            // add inner classes
            if (isTypeReference)
            {
                foreach (SymbolTable innerClass in typeSymbol.InnerClasses)
                    if (innerClass.Modifier.HasFlag(SymbolModifier.Public))
                        result.Add(innerClass);
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Get the methods that match for the current text.
        /// </summary>
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the method completions.</param>
        /// <returns>The methods that match for the current position.</returns>
        public Method[] GetMethodCompletions(Stream text, string className, TextPosition position)
        {
            List<Method> result = new List<Method>();

            // get class definition
            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol == null)
                return result.ToArray();

            // get symbol matches
            Symbol[] matchedSymbols = MatchSymbols(text, className, position, true);
            if (matchedSymbols == null || matchedSymbols.Length == 0)
                return result.ToArray();

            // get the matched method
            Method method = matchedSymbols[matchedSymbols.Length - 1] as Method;
            if (method == null)
                return result.ToArray();

            // get the parent
            SymbolTable parent = null;
            bool isTypeReference = false;
            if (matchedSymbols.Length == 1)
            {
                parent = classSymbol;
                isTypeReference = true;
            }
            else
            {
                if (matchedSymbols[matchedSymbols.Length - 2] is SymbolTable)
                    parent = matchedSymbols[matchedSymbols.Length - 2] as SymbolTable;
                else if (matchedSymbols[matchedSymbols.Length - 2] is TypedSymbol)
                    parent = _language.GetSymbols((matchedSymbols[matchedSymbols.Length - 2] as TypedSymbol).FullType);

                isTypeReference = !(matchedSymbols[matchedSymbols.Length - 1] is Constructor) &&
                                  (matchedSymbols[matchedSymbols.Length - 2] is SymbolTable);
            }

            if (parent == null)
                return result.ToArray();

            bool isExternal = (classSymbol.Id != parent.Id);

            // build results
            HashSet<string> signatures = new HashSet<string>();
            SymbolTable currentClass = parent;

            if (currentClass != null)
            {
                // check for constructors
                foreach (Constructor c in currentClass.Constructors)
                {
                    if (c.Id == method.Id && !signatures.Contains(c.Signature))
                    {
                        result.Add(c);
                        signatures.Add(c.Signature);
                    }
                }

                if (result.Count == 0)
                {
                    // check for methods
                    while (currentClass != null)
                    {
                        foreach (Method m in currentClass.Methods)
                        {
                            if (m.Id == method.Id && !signatures.Contains(m.Signature))
                            {
                                result.Add(m);
                                signatures.Add(m.Signature);
                            }
                        }

                        currentClass = _language.GetSymbols(currentClass.Extends);
                    }
                }
            }

            // filter out methods based on visibility
            if (isExternal)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i] is ModifiedSymbol)
                    {
                        SymbolModifier modifier = (result[i] as ModifiedSymbol).Modifier;
                        if ((modifier.HasFlag(SymbolModifier.Static) && !isTypeReference) ||
                            (!modifier.HasFlag(SymbolModifier.Static) && isTypeReference))
                        {
                            result.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get the fields for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the fields for.</param>
        /// <param name="isExternal">When true, only fields accessible to external code will be returned.</param>
        /// <returns>The fields for the given symbol table.</returns>
        private List<Field> GetFields(SymbolTable symbolTable, bool isExternal)
        {
            List<Field> result = new List<Field>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetFieldsWithModifiers(SymbolModifier.Public));
                GetInheritedFields(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Fields);
                GetInheritedFields(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the fields for the given table and all fields that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the fields for.</param>
        /// <param name="modifiers">Only get fields that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting fields are added to this list.</param>
        private void GetInheritedFields(SymbolTable symbolTable, SymbolModifier modifiers, List<Field> result)
        {
            if (symbolTable == null)
                return;

            foreach (Field field in symbolTable.Fields)
            {
                if ((modifiers & field.Modifier) > 0)
                    result.Add(field);
            }

            GetInheritedFields(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Get the properties for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the properties for.</param>
        /// <param name="isExternal">When true, only properties accessible to external code will be returned.</param>
        /// <returns>The properties for the given symbol table.</returns>
        private List<Property> GetProperties(SymbolTable symbolTable, bool isExternal)
        {
            List<Property> result = new List<Property>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetPropertiesWithModifiers(SymbolModifier.Public));
                GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Properties);
                GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the properties for the given table and all properties that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the properties for.</param>
        /// <param name="modifiers">Only get properties that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting properties are added to this list.</param>
        private void GetInheritedProperties(SymbolTable symbolTable, SymbolModifier modifiers, List<Property> result)
        {
            if (symbolTable == null)
                return;

            foreach (Property property in symbolTable.Properties)
            {
                if ((modifiers & property.Modifier) > 0)
                    result.Add(property);
            }

            GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Get the methods for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="isExternal">When true, only methods accessible to external code will be returned.</param>
        /// <returns>The methods for the given symbol table.</returns>
        private List<Method> GetMethods(SymbolTable symbolTable, bool isExternal)
        {
            List<Method> result = new List<Method>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetMethodsWithModifiers(SymbolModifier.Public));
                GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Methods);
                GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            if (symbolTable.TableType == SymbolTableType.Interface)
                foreach (string interfaceName in symbolTable.Interfaces)
                    GetInterfaceMethods(_language.GetSymbols(interfaceName), result);

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the methods for the given table and all methods that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="modifiers">Only get methods that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting methods are added to this list.</param>
        private void GetInheritedMethods(SymbolTable symbolTable, SymbolModifier modifiers, List<Method> result)
        {
            if (symbolTable == null)
                return;

            foreach (Method method in symbolTable.Methods)
            {
                if ((method.Modifier & modifiers) > 0)
                    result.Add(method);
            }

            GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Recursive call that gets the methods for the given interface and all interfaces that it implements. 
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="result">The resulting methods are added to this list.</param>
        private void GetInterfaceMethods(SymbolTable symbolTable, List<Method> result)
        {
            if (symbolTable == null)
                return;

            foreach (Method method in symbolTable.Methods)
                result.Add(method);

            foreach (string interfaceName in symbolTable.Interfaces)
                GetInterfaceMethods(_language.GetSymbols(interfaceName), result);
        }

        #endregion
    }
}
