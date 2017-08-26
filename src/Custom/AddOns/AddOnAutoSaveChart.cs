#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Gui.Tools;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

#endregion

//This namespace holds Add ons in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.AddOns
{
    public class AutoSaveChartAddOn : NinjaTrader.NinjaScript.AddOnBase
    {

        private NTMenuItem addOnFrameworkMenuItem;
        private NTMenuItem existingMenuItemInControlCenter;

        private static List<Chart> visibleChart = new List<Chart>();

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"This add-on will save all chart in one folder while user send an order to system.";
                Name = "AddOnAutoSaveChart";
            }
            else if (State == State.Configure)
            {
            }
        }

        
        // Will be called as a new NTWindow is created. It will be called in the thread of that window
        protected override void OnWindowCreated(Window window)
        {

            //1. Add a menu item in the control windows
            addMenuItem(window);

            //2. Add all chart which is visible in this workspace.
            Chart chart = window as Chart;

            //Save each chart
            if (chart != null)
            {
                if( !visibleChart.Contains(chart))
                {
                    visibleChart.Add(chart);
                }
                 
            }

        }
        // Will be called as a new NTWindow is destroyed. It will be called in the thread of that window
        protected override void OnWindowDestroyed(Window window)
        {
            //Print("AddOnBase.OnWindowDestroyed");
            //1. 
            if (addOnFrameworkMenuItem != null && window is ControlCenter)
            {
                if (existingMenuItemInControlCenter != null && existingMenuItemInControlCenter.Items.Contains(addOnFrameworkMenuItem))
                    existingMenuItemInControlCenter.Items.Remove(addOnFrameworkMenuItem);

                addOnFrameworkMenuItem.Click -= OnMenuItemClick;
                addOnFrameworkMenuItem = null;
            }
            //2. Remove all chart
            Chart chart = window as Chart;

            if (chart != null)
            {
                visibleChart.Remove(chart);
            }

        }

        // Open our AddOn's window when the menu item is clicked on
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            //Print("AddOnBase.OnMenuItemClick");
            Core.Globals.RandomDispatcher.BeginInvoke(new Action(() => new AutoSaveChartWindow().Show() ));
        }

        //Key function in this Addon
        static public void saveCharts(String folderName)
        {
            NinjaTrader.Code.Output.Process("Folder:" + folderName, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            if (!System.IO.Directory.Exists(folderName))
            {
                System.IO.Directory.CreateDirectory(folderName);
            }
            foreach (Chart chart in visibleChart)
            {
                if( chart.Dispatcher.CheckAccess() )
                {
                  
                    saveOneChart(chart, folderName);
                }
                else
                {
                   
                    chart.Dispatcher.InvokeAsync( () =>saveOneChart(chart, folderName));
                }
            }
        }

        static private void saveOneChart(Chart chart, String folderName)
        {
            try
            {
                if (chart.IsVisible)
                {
                    String fileName = folderName + chart.Title + "_"+ chart.IsActive + ".png";
                    
                    System.IO.FileStream file = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
                    BitmapSource control = chart.Form.GetBitmapSource();
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(chart.Form.GetBitmapSource()));
                    
                    encoder.Save(file);
                    NinjaTrader.Code.Output.Process("Chart:" + chart.IsFocused + "File:" + fileName, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                    file.Flush();
                    file.Dispose();
                    
                }
                   
            } catch ( Exception ex )
            {
                NinjaTrader.Code.Output.Process(ex.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);

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
            addOnFrameworkMenuItem = new NTMenuItem { Header = "AutoSaveChartAddOn", Style = Application.Current.TryFindResource("MainMenuItem") as Style };

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

        

    }

    public class AutoSaveChartWindow : NTWindow, IWorkspacePersistence
    {
        
        private AddOnAutoSaveChartFactory factory = null;

        public AutoSaveChartWindow()
        {
            NinjaTrader.Code.Output.Process("AutoSaveChartWindow.AutoSaveChartWindow()", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            AddOnSaveChartTab tabPage = getTab();

            Loaded += (o, e) =>
            {
                if (WorkspaceOptions == null)
                    WorkspaceOptions = new WorkspaceOptions("AddOnAutoSaveChart-" + Guid.NewGuid().ToString("N"), this);
            };
        }
      
        private AddOnSaveChartTab getTab()
        {
            // set Caption property (not Title), since Title is managed internally to properly combine selected Tab Header and Caption for display in the windows taskbar
            // This is the name displayed in the top-left of the window
            Caption = "AutoSaveChart";

            // TabControl should be created for window content if tab features are wanted
            TabControl tc = new TabControl();

            // Attached properties defined in TabControlManager class should be set to achieve tab moving, adding/removing tabs
            TabControlManager.SetIsMovable(tc, true);
            TabControlManager.SetCanAddTabs(tc, true);
            TabControlManager.SetCanRemoveTabs(tc, true);

            // if ability to add new tabs is desired, TabControl has to have attached property "Factory" set.
            factory = new AddOnAutoSaveChartFactory();
            TabControlManager.SetFactory(tc, factory);
            Content = tc;

            /* In order to have link buttons functionality, tab control items must be derived from Tools.NTTabPage
            They can be added using extention method AddNTTabPage(NTTabPage page) */

            AddOnSaveChartTab tabPage = new AddOnSaveChartTab();

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
            NinjaTrader.Code.Output.Process("AutoSaveChartWindow.Restore", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if (MainTabControl != null)
                MainTabControl.RestoreFromXElement(element);

            
        }

        public void Save(XDocument document, XElement element)
        {
            NinjaTrader.Code.Output.Process("AutoSaveChartWindow.Save", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if (MainTabControl != null)
                MainTabControl.SaveToXElement(element);
        
        }
    }

    /* Class which implements Tools.INTTabFactory must be created and set as an attached property for TabControl
    in order to use tab page add/remove/move/duplicate functionality */
    public class AddOnAutoSaveChartFactory : INTTabFactory
    {

        public AddOnAutoSaveChartFactory()
        {
            NinjaTrader.Code.Output.Process("AddOnAutoSaveChartFactory.AddOnAutoSaveChartFactory", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }
      
        // INTTabFactory member. Required to create parent window
        public NTWindow CreateParentWindow()
        {
            NinjaTrader.Code.Output.Process("AddOnAutoSaveChartFactory.CreateParentWindow", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            return new AutoSaveChartWindow();
        }

        // INTTabFactory member. Required to create tabs
        public NTTabPage CreateTabPage(string typeName, bool isTrue)
        {
            NinjaTrader.Code.Output.Process("AddOnAutoSaveChartFactory.CreateTabPage", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            AddOnSaveChartTab tagPage =  new AddOnSaveChartTab();
            
            return tagPage;
        }
    }

    /* This is where we define the actual content of the tabs for our AddOn window.
        Note: Class derived from Tools.NTTabPage has to be created if instrument link or interval link functionality is desired.
        Tools.IInstrumentProvider and/or Tools.IIntervalProvider interface(s) should be implemented.
        Also NTTabPage provides additional functionality for properly naming tab headers using properties and variables such as @FUNCTION, @INSTRUMENT, etc. */
    public class AddOnSaveChartTab : NTTabPage, NinjaTrader.Gui.Tools.IInstrumentProvider, NinjaTrader.Gui.Tools.IIntervalProvider
    {
        String outputFolder = "E:\\新增資料夾\\TEST";
        const String orderFolder = "/Order";
        const String transactionFolder = "/Transaction";
        Button imageSaveButton = null;
        Button startButton = null;
        Button clearButton = null;
       
        TextBox txtOutputFolder = null;
        private AccountSelector accountSelector = null;
        TextBox txtLog = null;

        

        public AddOnSaveChartTab()
        {
            NinjaTrader.Code.Output.Process("AddOnSaveChartTab.AddOnSaveChartTab", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            Content = loadUI();

        }

        private DependencyObject loadUI()
        {
            using (System.IO.Stream assemblyResourceStream = GetManifestResourceStream("AddOns.AddOnAutoSaveChart.xaml"))
            {
                if (assemblyResourceStream == null)
                    return null;
                System.IO.StreamReader streamReader = new System.IO.StreamReader(assemblyResourceStream);
                Page page = System.Windows.Markup.XamlReader.Load(streamReader.BaseStream) as Page;
                DependencyObject pageContent = null;
                if (page != null)
                {
                    pageContent = page.Content as DependencyObject;

                    imageSaveButton = LogicalTreeHelper.FindLogicalNode(pageContent, "imageSaveButton") as Button;
                    imageSaveButton.Click += OnImageSaveButtonClick;
                    startButton = LogicalTreeHelper.FindLogicalNode(pageContent, "startButton") as Button;
                    startButton.Click += OnStartButtonClick;

                    clearButton = LogicalTreeHelper.FindLogicalNode(pageContent, "clearButton") as Button;
                    clearButton.Click += OnClearButtonClick;

                    txtOutputFolder = LogicalTreeHelper.FindLogicalNode(pageContent, "outputFolder") as TextBox;
                    txtOutputFolder.Text = outputFolder;

                    accountSelector = LogicalTreeHelper.FindLogicalNode(pageContent, "accountSelector") as AccountSelector;
                    addEvent(accountSelector, true);
                    // When the account selector's selection changes, unsubscribe and resubscribe
                    //accountSelector.SelectionChanged += (o, args) =>
                    //{
                    //    addEvent(accountSelector, false);
                    //    addEvent(accountSelector, true);

                    //};

                    txtLog = LogicalTreeHelper.FindLogicalNode(pageContent, "outputBox") as TextBox;

                }

                return pageContent;
            }
        }

        

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {

            String start = (String)startButton.Content;
           
            if( start.Equals("Start"))
            {
                startButton.Content = "Stop";
               
                if (accountSelector != null)
                {
                    addEvent(accountSelector, true);

                }
            } else
            {
                startButton.Content = "Start";

                addEvent(accountSelector, false);

            }

        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {

            if( txtLog != null)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    txtLog.Clear();
                });
                
            }

        }
        private void addEvent(AccountSelector accountSelector, bool add)
        {
            
            if (accountSelector.SelectedAccount != null)
            {
                
                if( add )
                {
                    // Subscribe to new account subscriptions
                    accountSelector.SelectedAccount.AccountItemUpdate += OnAccountItemUpdate;
                    accountSelector.SelectedAccount.ExecutionUpdate += OnExecutionUpdate;
                    accountSelector.SelectedAccount.OrderUpdate += OnOrderUpdate;
                    accountSelector.SelectedAccount.PositionUpdate += OnPositionUpdate;
                } else
                {
                    // Unsubscribe to any prior account subscriptions
                    accountSelector.SelectedAccount.AccountItemUpdate -= OnAccountItemUpdate;
                    accountSelector.SelectedAccount.ExecutionUpdate -= OnExecutionUpdate;
                    accountSelector.SelectedAccount.OrderUpdate -= OnOrderUpdate;
                    accountSelector.SelectedAccount.PositionUpdate -= OnPositionUpdate;
                }   
         
            }
        }

        private void OnImageSaveButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = dialog.SelectedPath;
                
            }
        }

        public BarsPeriod BarsPeriod
        {
            get;
            set;
        }

        public Instrument Instrument
        {
            get;
            set;
           
        }

        
        protected override string GetHeaderPart(string variable)
        {
            return "AutoSaveChart";
        }

        protected override void Restore(XElement element)
        {
            
        }

        protected override void Save(XElement element)
        {
           
        }

        #region Account Update Handler
        private void OnAccountItemUpdate(object sender, AccountItemEventArgs e)
        {
             //Dispatcher.InvokeAsync(() =>
             //{
             //    txtLog.AppendText("OnAccountItemUpdate\n");
             //});

            /* Dispatcher.InvokeAsync() is needed for multi-threading considerations. When processing events outside of the UI thread, and we want to
                influence the UI .InvokeAsync() allows us to do so. It can also help prevent the UI thread from locking up on long operations. */
            //Dispatcher.InvokeAsync(() =>
            //{
            //    txtLog.AppendText(string.Format("{0}Account: {1}{0}AccountItem: {2}{0}Value: {3}",
            //        Environment.NewLine,
            //        e.Account.Name,
            //        e.AccountItem,
            //        e.Value));
            //});
        }

        // This method is fired as new executions come in, an existing execution is amended (e.g. by the broker's back office), or an execution is removed (e.g. by the broker's back office)
        private void OnExecutionUpdate(object sender, ExecutionEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                txtLog.AppendText("OnExecutionUpdate sender = " + sender + "e= " + e + "\n");
            });
            
            /* Dispatcher.InvokeAsync() is needed for multi-threading considerations. When processing events outside of the UI thread, and we want to
                influence the UI .InvokeAsync() allows us to do so. It can also help prevent the UI thread from locking up on long operations. */
            //Dispatcher.InvokeAsync(() =>
            //{
            //    txtLog.AppendText(string.Format("{0}Instrument: {1}{0}Quantity: {2}{0}Price: {3}{0}Time: {4}{0}ExecutionID: {5}{0}Exchange: {6}{0}MarketPosition: {7}{0}Operation: {8}{0}OrderID: {9}{0}Name: {10}{0}Commission: {11}{0}Rate: {12}{0}",
            //        Environment.NewLine,
            //        e.Execution.Instrument.FullName,
            //        e.Quantity,
            //        e.Price,
            //        e.Time,
            //        e.ExecutionId,
            //        e.Exchange,
            //        e.MarketPosition,
            //        e.Operation,
            //        e.OrderId,
            //        e.Execution.Name,
            //        e.Execution.Commission,
            //        e.Execution.Rate));
            //});
        }

        // This method is fired as the status of an order changes
        private void OnOrderUpdate(object sender, OrderEventArgs e)
        {

            Dispatcher.InvokeAsync(() =>
            {
               
                txtLog.AppendText("OnOrderUpdate sender = " + sender + "e= " + e + "\n");

                if (accountSelector == null || !e.Order.Account.DisplayName.Equals(accountSelector.SelectedAccount.DisplayName))
                {
                    txtLog.AppendText("OnOrderUpdate Account = " + e.Order.Account.DisplayName + "Account Selector= " + accountSelector.SelectedAccount + "\n");
                    return;
                }

                if (e.OrderState == OrderState.Submitted || e.OrderState == OrderState.ChangeSubmitted || e.OrderState == OrderState.Cancelled || e.OrderState == OrderState.Filled )
                {

                    String folerName = txtOutputFolder.Text + orderFolder + "/" + e.OrderId + "/";
                    //2017-02-19_07_51_41_Buy_Limit
                    folerName += DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + "_" + e.Order.OrderAction + "_" + e.Order.OrderType + "_" + e.OrderState +"/";
                    AutoSaveChartAddOn.saveCharts(folerName);


                }

                //if( e.OrderState == OrderState.Cancelled)
                //{
                //    moveToTransactionFolder(e.OrderId);
                //}
            });

            /* Dispatcher.InvokeAsync() is needed for multi-threading considerations. When processing events outside of the UI thread, and we want to
               influence the UI .InvokeAsync() allows us to do so. It can also help prevent the UI thread from locking up on long operations. */
            //Dispatcher.InvokeAsync(() =>
            //{
            //    txtLog.AppendText(string.Format("{0}Instrument: {1}{0}OrderAction: {2}{0}OrderType: {3}{0}Quantity: {4}{0}LimitPrice: {5}{0}StopPrice: {6}{0}OrderState: {7}{0}Filled: {8}{0}AverageFillPrice: {9}{0}Name: {10}{0}OCO: {11}{0}TimeInForce: {12}{0}OrderID: {13}{0}Time: {14}{0}",
            //        Environment.NewLine,
            //        e.Order.Instrument.FullName,
            //        e.Order.OrderAction,
            //        e.Order.OrderType,
            //        e.Quantity,
            //        e.LimitPrice,
            //        e.StopPrice,
            //        e.OrderState,
            //        e.Filled,
            //        e.AverageFillPrice,
            //        e.Order.Name,
            //        e.Order.Oco,
            //        e.Order.TimeInForce,
            //        e.OrderId,
            //        e.Time));
            //});
        }

        private void moveToTransactionFolder(string orderId)
        {

            string transactionFolderName = txtOutputFolder.Text + transactionFolder;
            if (!System.IO.Directory.Exists(transactionFolderName))
            {
                System.IO.Directory.CreateDirectory(transactionFolderName);
            }
            String[] directory = System.IO.Directory.GetDirectories(transactionFolderName);
            int dsize = directory.Length;

            int lastDirectory = 0;
            if (dsize != 0)
            {
                lastDirectory = Int32.Parse(directory[dsize - 1].Substring(directory[dsize - 1].LastIndexOf('\\') + 1));
            }
            String dest = transactionFolderName + "/" + (lastDirectory + 1) + "/";
            
            String src = txtOutputFolder.Text + orderFolder + "/" + orderId + "/";

            txtLog.AppendText("Move " + src + " To " + dest + "\n");
            try
            {
                System.IO.Directory.Move(src, dest);
            } catch( Exception e)
            {
                txtLog.AppendText(e.ToString());
            }
            


        }

        // This method is fired as a position changes
        private void OnPositionUpdate(object sender, PositionEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                txtLog.AppendText("OnPositionUpdate\n");
            });



        }

        #endregion

        public override void Cleanup()
        {
           
            if (accountSelector.SelectedAccount != null)
            {
                accountSelector.SelectedAccount.AccountItemUpdate -= OnAccountItemUpdate;
                accountSelector.SelectedAccount.ExecutionUpdate -= OnExecutionUpdate;
                accountSelector.SelectedAccount.OrderUpdate -= OnOrderUpdate;
                accountSelector.SelectedAccount.PositionUpdate -= OnPositionUpdate;
            }
            accountSelector.Cleanup();

        }

        #region TESTCode


        private void OnTestButtonClick(object sender, RoutedEventArgs e)
        {
           

            //MarketReplay replay = new MarketReplay(Instrument, new DateTime(2017, 3, 11));
            ////replay.OnMarketData(MarketDataType.Ask, 100, 1, new DateTime(2017, 3, 11, 13, 0, 0));
            //replay.Start();
            //int i = 0;
            //while (replay.MoveNext())
            //{
            //    i++;
            //}
            //NinjaTrader.Code.Output.Process("Count = " + i, NinjaTrader.NinjaScript.PrintTo.OutputTab1);


            //while (replay.MoveNext())
            //{

            //    NinjaTrader.Code.Output.Process("ReplayData = " + replay.CurrentMarketData + " ReplayDepth = " + replay.CurrentMarketDepth, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            //    return;
            //}
            //replay.Stop();
            //MarketReplay.DumpMarketData(Instrument, new DateTime(2017, 3, 9), new DateTime(2017, 3, 10), outputFolder + "/test.txt");
        }

        
        #endregion
    }

  
}
