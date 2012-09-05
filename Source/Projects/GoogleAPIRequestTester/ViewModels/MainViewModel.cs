using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GoogleAPI;
using GoogleAPIRequestTester.Models;

namespace GoogleAPIRequestTester.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _debugText;
        private ObservableCollection<ReaderMethod> _readerMethods;
        private string _requestText;
        private string _responseText;
        private ReaderMethod _selectedMethod;

        public MainViewModel()
        {
            CreateReaderAPI();

            ReaderMethods = new ObservableCollection<ReaderMethod>
            {
                new ReaderMethod("Selected Method", () => WriteToDebug("Select a valid method")),
                new ReaderMethod("Get Auth Token", GoogleReaderAPI.GetToken),
                new ReaderMethod("Refresh Auth Token", () => WriteToDebug("TO BE DONE!!!!!!"))
            };

            SelectedMethod = ReaderMethods
                .Where(i => i.Name == "Selected Method")
                .FirstOrDefault();

            RunCommand = new RelayCommand(OnRun);
            ClearData = new RelayCommand(OnClearData);
        }

        private GoogleReaderAPI GoogleReaderAPI { get; set; }

        public string DebugText
        {
            get { return _debugText; }
            set
            {
                _debugText = value;
                RaisePropertyChanged("DebugText");
            }
        }

        public string RequestText
        {
            get { return _requestText; }
            set
            {
                _requestText = value;
                RaisePropertyChanged("RequestText");
            }
        }

        public string ResponseText
        {
            get { return _responseText; }
            set
            {
                _responseText = value;
                RaisePropertyChanged("ResponseText");
            }
        }

        public ObservableCollection<ReaderMethod> ReaderMethods
        {
            get { return _readerMethods; }
            set
            {
                _readerMethods = value;
                RaisePropertyChanged("ReaderMethods");
            }
        }

        public ReaderMethod SelectedMethod
        {
            get { return _selectedMethod; }
            set
            {
                _selectedMethod = value;
                RaisePropertyChanged("SelectedMethod");
            }
        }

        public ICommand RunCommand { get; set; }
        public ICommand ClearData { get; set; }

        private void WriteToDebug(string message, params object[] args)
        {
            DebugText += message.FormatWithNewLine(args);
        }

        private void WriteResponse(string message, params object[] args)
        {
            ResponseText += message.FormatWithNewLine(args);
        }

        private void WriteRequest(string message, params object[] args)
        {
            RequestText += message.FormatWithNewLine(args);
        }

        private void CreateReaderAPI()
        {
            GoogleReaderAPI = new GoogleAPIHelper();

            GoogleReaderAPI.OnDebugMessage += WriteToDebug;
            GoogleReaderAPI.OnRequest += WriteRequest;
            GoogleReaderAPI.OnResponse += WriteResponse;
        }

        private void OnClearData()
        {
            WriteToDebug("Going to clear all user saved data");
        }

        private void OnRun()
        {
            WriteToDebug(string.Format("Going to run method {0}", SelectedMethod.Name));

            try
            {
                SelectedMethod.Method();
            }
            catch (System.Exception ex)
            {
                WriteToDebug("ERROR: {0}", ex.Message);
                WriteToDebug("Stacktrace: {0}", ex.StackTrace);
            }
        }
    }
}