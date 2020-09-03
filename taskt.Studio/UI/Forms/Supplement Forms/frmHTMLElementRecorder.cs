﻿using Gecko;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using taskt.Core.Script;
using taskt.Utilities;

namespace taskt.UI.Forms.Supplement_Forms
{
    public partial class frmHTMLElementRecorder : UIForm
    {
        public List<ScriptElement> ScriptElements { get; set; }
        public Dictionary<string, object> SearchParameters { get; set; }
        public string LastItemClicked { get; set; }
        public string StartURL { get; set; }
        private string _homeURL = "https://www.google.com/"; //TODO replace with openbots url;
        private string _xPath;
        private string _name;
        private string _id;
        private string _tagName;
        private string _className;
        private string _linkText;
        private List<string> _cssSelectors;
        private bool _isRecording = false;

        public frmHTMLElementRecorder(string startURL)
        {
            if (string.IsNullOrEmpty(startURL))
                StartURL = _homeURL;
            else
                StartURL = startURL;

            InitializeComponent();

            Xpcom.Initialize("Firefox");
            wbElementRecorder.Navigate(StartURL);
            tbURL.Text = StartURL;
            tbURL.Refresh();
        }

        private void frmHTMLElementRecorder_Load(object sender, EventArgs e)
        {
        }

        private void pbRecord_Click(object sender, EventArgs e)
        {
            if (!_isRecording)
            {
                _isRecording = true;
                TopMost = true;
                if (!chkStopOnClick.Checked)
                    lblDescription.Text = "Recording. Press F2 to save and close.";

                SearchParameters = new Dictionary<string, object>();            

                //start global hook and wait for left mouse down event
                GlobalHook.StartEngineCancellationHook(Keys.F2);
                GlobalHook.HookStopped += GlobalHook_HookStopped;
                GlobalHook.StartElementCaptureHook(chkStopOnClick.Checked);
                wbElementRecorder.DomClick += wbElementRecorder_DomClick;
            }
            else
            {
                _isRecording = false;
                if (!chkStopOnClick.Checked)
                    lblDescription.Text = "Recording has stopped. Press F2 to save and close.";

                //remove wait for left mouse down event
                wbElementRecorder.DomClick -= wbElementRecorder_DomClick;
            }
        }

        private void GlobalHook_HookStopped(object sender, EventArgs e)
        {
            wbElementRecorder_DomClick(null, null);
            Close();
        }

        private void wbElementRecorder_DomClick(object sender, DomMouseEventArgs e)
        {
            //mouse down has occured
            if (e != null)
            {
                try
                {
                    GeckoElement element = wbElementRecorder.DomDocument.ElementFromPoint(e.ClientX, e.ClientY);

                    string savedId = element.GetAttribute("id");
                    string uniqueId = Guid.NewGuid().ToString();
                    element.SetAttribute("id", uniqueId);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(element.OwnerDocument.GetElementsByTagName("html")[0].OuterHtml);
                    element.SetAttribute("id", savedId);
                    HtmlNode node = doc.GetElementbyId(uniqueId);

                    _xPath = node.XPath.Replace("[1]", "");
                    _name = element.GetAttribute("name") == null ? "" : element.GetAttribute("name");
                    _id = element.GetAttribute("id") == null ? "" : element.GetAttribute("id"); ;
                    _tagName = element.TagName;
                    _className = element.GetAttribute("class") == null ? "" : element.GetAttribute("class");
                    _linkText = element.TagName.ToLower() != "a" ? "" : element.TextContent;
                    _cssSelectors = GetCSSSelectors(element);

                    string cssSelectorString = string.Join(", ", GetCSSSelectors(element));

                    LastItemClicked = $"[XPath:{_xPath}].[ID:{_id}].[Name:{_name}].[Tag Name:{_tagName}].[Class:{_className}].[Link Text:{_linkText}].[CSS Selector:{cssSelectorString}]";
                    lblSubHeader.Text = LastItemClicked;

                    SearchParameters.Clear();
                    SearchParameters.Add("XPath", _xPath);
                    SearchParameters.Add("ID", _id);
                    SearchParameters.Add("Name", _name);
                    SearchParameters.Add("Tag Name", _tagName);
                    SearchParameters.Add("Class Name", _className);
                    SearchParameters.Add("CSS Selector", _cssSelectors);
                    SearchParameters.Add("Link Text", _linkText);
                }
                catch (Exception)
                {
                    lblDescription.Text = "Error cloning element. Please Try Again.";
                }
            }

            if (chkStopOnClick.Checked)
                Close();
        }

        private void pbHome_Click(object sender, EventArgs e)
        {
            wbElementRecorder.Navigate(_homeURL);
        }

        private void pbRefresh_Click(object sender, EventArgs e)
        {
            wbElementRecorder.Reload();
        }

        private void uiBtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void pbGo_Click(object sender, EventArgs e)
        {
            wbElementRecorder.Navigate(tbURL.Text);
        }

        private void pbBack_Click(object sender, EventArgs e)
        {
            wbElementRecorder.GoBack();
        }

        private void pbForward_Click(object sender, EventArgs e)
        {
            wbElementRecorder.GoForward();
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            Dictionary<ScriptElementType, object> elementValueDict = new Dictionary<ScriptElementType, object>()
            {
                { ScriptElementType.XPath, _xPath },
                { ScriptElementType.Name, _name },
                { ScriptElementType.ID, _id },
                { ScriptElementType.TagName, _tagName },
                { ScriptElementType.ClassName, _className },
                { ScriptElementType.LinkText, _linkText },
                { ScriptElementType.CSSSelector, _cssSelectors }
            };

            frmAddElement addElementForm = new frmAddElement();
            addElementForm.ScriptElements = ScriptElements;
            addElementForm.ElementValueDict = elementValueDict;
            addElementForm.ShowDialog();

            if (addElementForm.DialogResult == DialogResult.OK)
            {
                ScriptElementType elementType = (ScriptElementType)Enum.Parse(typeof(ScriptElementType),
                                                addElementForm.cbxElementType.SelectedItem.ToString().Replace(" ", ""));


                ScriptElement newElement = new ScriptElement()
                {
                    ElementName = addElementForm.txtElementName.Text.Replace("<", "").Replace(">", ""),
                    ElementType = elementType,
                    ElementValue = elementType == ScriptElementType.CSSSelector ? addElementForm.cbxDefaultValue.Text 
                                                                                : addElementForm.txtDefaultValue.Text
                };

                ScriptElements.Add(newElement);
            }
        }

        private void pbElements_Click(object sender, EventArgs e)
        {
            frmScriptElements scriptElementForm = new frmScriptElements();
            scriptElementForm.ScriptElements = ScriptElements;
            scriptElementForm.ShowDialog();

            if (scriptElementForm.DialogResult == DialogResult.OK)
            {
                ScriptElements = scriptElementForm.ScriptElements;
            }
        }

        private void tbURL_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                wbElementRecorder.Navigate(tbURL.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void frmHTMLElementRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            StartURL = wbElementRecorder.Url.ToString();
            DialogResult = DialogResult.Cancel;
        }

        private void wbElementRecorder_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            tbURL.Text = wbElementRecorder.Url.ToString();
        }

        private void wbElementRecorder_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            //force popups to open in the same browser window
            ((GeckoWebBrowser)sender).Navigate(e.Uri);
            e.Cancel = true;
        }

        private List<string> GetCSSSelectors(GeckoElement element)
        {
            var attributes = element.Attributes;
            string tagName = element.TagName.ToLower();
            List<string> attributeList = new List<string>();
            foreach (var attribute in attributes)
                attributeList.Add($"{tagName}[{attribute.NodeName}='{attribute.NodeValue}']");

            return attributeList;
        }
    }
}