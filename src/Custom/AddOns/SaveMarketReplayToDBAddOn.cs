#region Using declarations
using System;
using System.Collections.Generic;
using System.Windows;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using System.Xml.Linq;
using System.Windows.Controls;
using Infragistics.Windows.Editors;
using MongoDB.Driver;
using NinjaTrader.Server;
using System.Reflection;
using NinjaTrader.Custom.MongoDB;
using System.IO;
using NinjaTrader.Custom.MongoDB.Table;
using System.Windows.Threading;
using NinjaTrader.Custom.Thread;
using NinjaTrader.Custom.DataOperation;
#endregion

//This namespace holds Add ons in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.AddOns
{
    public class SaveMarketReplayToDB : NinjaTrader.NinjaScript.AddOnBase
	{
        private NTMenuItem addOnFrameworkMenuItem;
        private NTMenuItem existingMenuItemInControlCenter;

        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Add on here.";
				Name										= "SaveMarketReplayToDBAddOn";
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnWindowCreated(Window window)
		{
            //1. Add a menu item in the control windows
            addMenuItem(window);

        }

		protected override void OnWindowDestroyed(Window window)
		{
            //1. 
            if (addOnFrameworkMenuItem != null && window is ControlCenter)
            {
                if (existingMenuItemInControlCenter != null && existingMenuItemInControlCenter.Items.Contains(addOnFrameworkMenuItem))
                    existingMenuItemInControlCenter.Items.Remove(addOnFrameworkMenuItem);

                addOnFrameworkMenuItem.Click -= OnMenuItemClick;
                addOnFrameworkMenuItem = null;
            }
        }

        private void addMenuItem(Window window)
        {

            // We want to place our AddOn in the Control Center's menus
            ControlCenter cc = window as ControlCenter;
            if (cc == null)
                return;

            Print("AddOnBase.addMenuItem");
            /* Determine we want to place our AddOn in the Control Center's "New" menu
            Other menus can be accessed via the control's "Automation ID". For example: toolsMenuItem, workspacesMenuItem, connectionsMenuItem, helpMenuItem. */
            existingMenuItemInControlCenter = cc.FindFirst("ControlCenterMenuItemNew") as NTMenuItem;
            if (existingMenuItemInControlCenter == null)
                return;

            // 'Header' sets the name of our AddOn seen in the menu structure
            addOnFrameworkMenuItem = new NTMenuItem { Header = "SaveMarketReplayToDBAddOn", Style = Application.Current.TryFindResource("MainMenuItem") as Style };

            existingMenuItemInControlCenter.Items.Add(addOnFrameworkMenuItem);
            // Add our AddOn into the "New" menu
            if (cc != null)
            {
                //foreach (ItemsControl m in existingMenuItemInControlCenter.Items)
                //{
                //    Print("MenuItem menu = " + m);
                //}
            }

            //// Subscribe to the event for when the user presses our AddOn's menu item
            addOnFrameworkMenuItem.Click += OnMenuItemClick;
        }

        // Open our AddOn's window when the menu item is clicked on
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            //Print("AddOnBase.OnMenuItemClick");
            Core.Globals.RandomDispatcher.BeginInvoke(new Action(() => new SaveMarketReplayToDBWindow().Show()));
        }
    }

    public class SaveMarketReplayToDBWindow : NTWindow, IWorkspacePersistence
    {
        public SaveMarketReplayToDBWindow()
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBWindow.AutoSaveChartWindow()", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            SaveMarketReplayToDBTab tabPage = getTab();

            Loaded += (o, e) =>
            {
                if (WorkspaceOptions == null)
                    WorkspaceOptions = new WorkspaceOptions("AddOnAutoSaveChart-" + Guid.NewGuid().ToString("N"), this);
            };
        }
        private SaveMarketReplayToDBTab getTab()
        {
            // set Caption property (not Title), since Title is managed internally to properly combine selected Tab Header and Caption for display in the windows taskbar
            // This is the name displayed in the top-left of the window
            Caption = "SaveMarketReplayToDB";

            // TabControl should be created for window content if tab features are wanted
            TabControl tc = new TabControl();

            // Attached properties defined in TabControlManager class should be set to achieve tab moving, adding/removing tabs
            TabControlManager.SetIsMovable(tc, true);
            TabControlManager.SetCanAddTabs(tc, true);
            TabControlManager.SetCanRemoveTabs(tc, true);

            // if ability to add new tabs is desired, TabControl has to have attached property "Factory" set.
            SaveMarketReplayToDBFactory factory = new SaveMarketReplayToDBFactory();
            TabControlManager.SetFactory(tc, factory);
            Content = tc;

            /* In order to have link buttons functionality, tab control items must be derived from Tools.NTTabPage
            They can be added using extention method AddNTTabPage(NTTabPage page) */

            SaveMarketReplayToDBTab tabPage = new SaveMarketReplayToDBTab();

            tc.AddNTTabPage(tabPage);
            return tabPage;
        }
        public WorkspaceOptions WorkspaceOptions
        {
            get;
            set;
        }

        public void Restore(XDocument document, XElement element)
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBWindow.Restore", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if (MainTabControl != null)
                MainTabControl.RestoreFromXElement(element);
        }

        public void Save(XDocument document, XElement element)
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBWindow.Save", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if (MainTabControl != null)
                MainTabControl.SaveToXElement(element);
        }
    }

    /* Class which implements Tools.INTTabFactory must be created and set as an attached property for TabControl
    in order to use tab page add/remove/move/duplicate functionality */
    public class SaveMarketReplayToDBFactory : INTTabFactory
    {

        public SaveMarketReplayToDBFactory()
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBFactory.SaveMarketReplayToDBFactory", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }

        // INTTabFactory member. Required to create parent window
        public NTWindow CreateParentWindow()
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBFactory.CreateParentWindow", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            return new SaveMarketReplayToDBWindow();
        }

        // INTTabFactory member. Required to create tabs
        public NTTabPage CreateTabPage(string typeName, bool isTrue)
        {
            NinjaTrader.Code.Output.Process("AddOnAutoSaveChartFactory.CreateTabPage", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            SaveMarketReplayToDBTab tagPage = new SaveMarketReplayToDBTab();

            return tagPage;
        }
    }
    /* This is where we define the actual content of the tabs for our AddOn window.
        Note: Class derived from Tools.NTTabPage has to be created if instrument link or interval link functionality is desired.
        Tools.IInstrumentProvider and/or Tools.IIntervalProvider interface(s) should be implemented.
        Also NTTabPage provides additional functionality for properly naming tab headers using properties and variables such as @FUNCTION, @INSTRUMENT, etc. */
    public class SaveMarketReplayToDBTab : NTTabPage, NinjaTrader.Gui.Tools.IInstrumentProvider, NinjaTrader.Gui.Tools.IIntervalProvider
    {
        String connectionString = "mongodb://localhost:27017/";
        String tempLocation = "E:\\新增資料夾\\TEST\\tempLoc.txt";
        string database = "Develop";
        TextBox txtConnStr = null;
        TextBox tempLoc = null;
        InstrumentSelector instrument = null;
        CheckBox cbDownloadAll = null;
        XamDateTimeEditor beginDate = null;
        XamDateTimeEditor endDate = null;
        Button loadDateButton = null;
        Button loadMarketReplayButton = null;
        Button downloadToDBButton = null;
        TextBox txtLog = null;

        public SaveMarketReplayToDBTab()
        {
            NinjaTrader.Code.Output.Process("SaveMarketReplayToDBTab.SaveMarketReplayToDBTab", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            Content = loadUI();
        }

        private DependencyObject loadUI()
        {
            using (System.IO.Stream assemblyResourceStream = GetManifestResourceStream("AddOns.SaveMarketReplayToDBAddOn.xaml"))
            {
                if (assemblyResourceStream == null)
                    return null;

                System.IO.StreamReader streamReader = new System.IO.StreamReader(assemblyResourceStream);
                Page page = System.Windows.Markup.XamlReader.Load(streamReader.BaseStream) as Page;
                DependencyObject pageContent = null;

                if( page != null )
                {
                    pageContent = page.Content as DependencyObject;

                    txtConnStr = LogicalTreeHelper.FindLogicalNode(pageContent, "conStr") as TextBox;
                    txtConnStr.Text = connectionString;

                    tempLoc = LogicalTreeHelper.FindLogicalNode(pageContent, "tempLocation") as TextBox;
                    tempLoc.Text = tempLocation;

                    instrument = LogicalTreeHelper.FindLogicalNode(pageContent, "instrumentSelector") as InstrumentSelector;
                    instrument = LogicalTreeHelper.FindLogicalNode(pageContent, "instrumentSelector") as InstrumentSelector;
                    if (instrument != null)
                        instrument.InstrumentChanged += OnInstrumentChanged;

                    cbDownloadAll = LogicalTreeHelper.FindLogicalNode(pageContent, "downloadAll") as CheckBox;
                    loadDateButton = LogicalTreeHelper.FindLogicalNode(pageContent, "loadButton") as Button;
                    loadDateButton.Click += OnLoadDateButtonClick;

                    loadMarketReplayButton = LogicalTreeHelper.FindLogicalNode(pageContent, "loadMarketReplayButton") as Button;
                    loadMarketReplayButton.Click += OnLoadMarketReplayButtonClick;

                    beginDate = LogicalTreeHelper.FindLogicalNode(pageContent, "beginDate") as XamDateTimeEditor;
                    endDate = LogicalTreeHelper.FindLogicalNode(pageContent, "endDate") as XamDateTimeEditor;

                    downloadToDBButton = LogicalTreeHelper.FindLogicalNode(pageContent, "downloadToDBButton") as Button;
                    downloadToDBButton.Click += OnDownloadToDBButtonClick;

                    txtLog = LogicalTreeHelper.FindLogicalNode(pageContent, "outputBox") as TextBox;
                }

                return pageContent;
            }
        }

        private void OnLoadDateButtonClick(object sender, RoutedEventArgs e)
        {
            loadDate();
        }

        private void loadDate(string marketName = null)
        {

            //1. Load Date from C:\Users\Peter12\Documents\NinjaTrader 8
            string profile = Environment.GetEnvironmentVariable("USERPROFILE");
            string dbLocation = profile + "\\Documents\\NinjaTrader 8\\db\\replay\\" + Instrument.FullName + "\\";
            string[] files = Directory.GetFiles(dbLocation);
            Array.Sort(files);

            beginDate.Value = DataParser.parseDate(files[0]);
            endDate.Value = DataParser.parseDate(files[files.Length - 1]);

            instrumentHasLoaded = Instrument.FullName;
        }
        private void OnLoadMarketReplayButtonClick(object sender, RoutedEventArgs e)
        {
            if( beginDate.Value == null || endDate.Value == null )
            {
                MessageBox.Show("You should set Date between which we dowload from server.");
            } else
            {
                loadHistoricalData((DateTime)beginDate.Value, (DateTime)endDate.Value);
            }
        }

        double total = 0;
        int datalength = 0;
        private void loadHistoricalData(DateTime begin, DateTime end)
        {
            datalength = 0;
            foreach (Connection c in Connection.Connections)
            {

                HdsClient client = typeof(Connection).GetProperty("HistoricalDataClient", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as HdsClient;
                NinjaTrader.Code.Output.Process(String.Format("Hds:{0} ", client), NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                Action<NinjaTrader.Cbi.ErrorCode, string, object> callback = MarketReplayCallBack;
                DateTime index = begin;
                total = (end - begin).TotalDays;
                while (index.CompareTo(end) <= 0) 
                {
                    client.RequestMarketReplay(Instrument, index, callback, null, index);
                    index = index.AddDays(1);
                }
            }
        }

        private void MarketReplayCallBack(NinjaTrader.Cbi.ErrorCode error, string str, object obj)
        {
            NinjaTrader.Code.Output.Process(String.Format("Error:{0}  str = {1}, obj = {2}, {3} / {4}", error, str, obj, datalength, total), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            DateTime date = (DateTime)obj;

            if (endDate.Value.Equals(date))
            {
                txtLog.AppendText("Load MarketReplay successfully.");
            }
        }

  
        private void OnDownloadToDBButtonClick(object sender, RoutedEventArgs e)
        {

            #region Right Code
            MongoDBMethod.registerClass();
            MongoClient connection = new MongoClient(txtConnStr.Text);

            //1. Create and read market name
            string marketName = MongoDBMethod.readMarketId(connection, database, Instrument);

            //2. Create and read contract
            Contracts contract = null;
            if (cbDownloadAll.IsChecked == true)
            {
                //TODO Perform download all
            }
            else
            {
                //1. load beginDate and endDate if needed
                if (beginDate.Value == null || endDate.Value == null || !instrumentHasLoaded.Equals(Instrument.FullName))
                {
                    loadDate(marketName);
                }

                //2. Store Contracts 
                DateTime begin = (DateTime)beginDate.Value;
                DateTime end = (DateTime)endDate.Value;

                contract = MongoDBMethod.readContract(connection, database, marketName, Instrument, begin, end);

                //3. Create threads to download MarketReplay Data to database
                //ThreadManager.createThread(this, "testThread", BindingFlags.Public | BindingFlags.Static, false, begin, end);
                //ThreadManager.createThread(this, "download", BindingFlags.Public | BindingFlags.Static, false, begin, end, 
                //    connection, database, Instrument, contract, tempLoc.Text);
                download(begin, end, connection, database, Instrument, contract, tempLoc.Text);

            }
            #endregion

        }

        [ThreadMethod("download")]
        public static void download(DateTime beginDate, DateTime endDate, MongoClient connection, string dbName, Instrument Instrument, Contracts contract, string tmpFolder)
        {
            NinjaTrader.Code.Output.Process("Begin = " + beginDate + " End = " + endDate, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            beginDate = new DateTime(2017, 8, 31);
            DateTime afterBeginDate = beginDate.AddDays(1);
            Core.Globals.RandomDispatcher.BeginInvoke(new Action(() =>
            {
                string file = tmpFolder;
                NinjaTrader.Code.Output.Process(String.Format("Dump Data: {0} Time={1} millisecond = {2} ", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                //MarketReplay.DumpMarketDepth(Instrument, beginDate, afterBeginDate, file);
                DateTime firstDate = DataParser.parseDate(file, false);
                DateTime lastDate = DataParser.parseDate(file, true);

                NinjaTrader.Code.Output.Process("First Date of File" + file + " = " + firstDate, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                NinjaTrader.Code.Output.Process("Last Date of File" + file + " = " + lastDate, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                IDictionary<DateTime, List<Figure>> dataInDB = MongoDBMethod.searchPrice(connection, dbName, firstDate, lastDate);

                NinjaTrader.Code.Output.Process(String.Format("Load Data: {0} Time={1} millisecond = {2}", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                List<Figure> data = DataParser.parse(contract.Id.MarketName, contract.Id.ContractName, file, dataInDB);
                NinjaTrader.Code.Output.Process(String.Format("Finish Data: {0} Time={1} millisecond = {2}", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                MongoDBMethod.insertPrice(connection, dbName, data);


            }), DispatcherPriority.SystemIdle);
            NinjaTrader.Code.Output.Process("Date=" + new DateTime(2017, 3, 9), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }
        [ThreadMethod("testThread")]
        public static void testThread(DateTime begin , DateTime end)
        {  

           

        }
        private string instrumentHasLoaded = "";
        private void OnInstrumentChanged(object sender, EventArgs e)
        {
            if( Instrument != null )
            {
                instrumentHasLoaded = Instrument.FullName;
            }
            
            NinjaTrader.Code.Output.Process("OnInstrumentChanged() OldInstrument: " + instrumentHasLoaded, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            Instrument = sender as Cbi.Instrument;
        }


        //private string exportLastData(Instrument instrument)
        //{
        //    HistoricalData historical = new HistoricalData();

        //    Type type = typeof(HistoricalData);
        //    FieldInfo info = type.GetField("cbxInsSelExport", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        ComboBox cbxInstrument = info.GetValue(historical) as ComboBox;
        //        cbxInstrument.Text = instrument.FullName;
        //    }

        //    info = type.GetField("dtpExportFrom", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        XamDateTimeEditor startDate = info.GetValue(historical) as XamDateTimeEditor;

        //        startDate.Value = Instrument.Expiry.AddYears(-8);
        //    }

        //    info = type.GetField("dtpExportTo", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        XamDateTimeEditor endDate = info.GetValue(historical) as XamDateTimeEditor;

        //        endDate.Value = Instrument.Expiry.AddYears(8);
        //    }

        //    info = type.GetField("cboExportInterval", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        ComboBox cbxInterval = info.GetValue(historical) as ComboBox;

        //        cbxInterval.Text = "Day";
        //    }

        //    info = type.GetField("cboExportDataType", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        ComboBox cbxType = info.GetValue(historical) as ComboBox;

        //        cbxType.Text = "Last";
        //    }


        //    info = type.GetField("btnExport", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        Button btnExport = info.GetValue(historical) as Button;

        //        MethodInfo method = typeof(Button).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);
        //        if (method != null)
        //        {
        //            method.Invoke(btnExport, null);
        //        }

        //    }

        //    info = type.GetField("exportFileName", BindingFlags.NonPublic | BindingFlags.Instance);

        //    if (info != null)
        //    {
        //        string file = info.GetValue(historical) as string;

        //        return file;



        //    } else
        //    {
        //        return "";
        //    }
        //}

        
        public Instrument Instrument
        {
            get;
            set;
        }

        public BarsPeriod BarsPeriod
        {
            get;
            set;
        }

        public override void Cleanup()
        {
            if( instrument != null ) 
                instrument.Cleanup();
        }
        protected override string GetHeaderPart(string variable)
        {
            return "SaveMarketReplayToDB";
        }

        protected override void Restore(XElement element)
        {
            
        }

        protected override void Save(XElement element)
        {
            
        }
       
    }
}
