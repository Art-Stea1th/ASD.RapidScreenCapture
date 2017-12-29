using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASD.RSC.ViewModels {

    using Helpers;
    using Services;

    internal enum State { Stopped, Started }

    internal sealed class ShellViewModel : Observable {

        private readonly string title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        private ScreenCaptureService screenCaptureService;

        private RelayCommand startCommand, pauseCommand;
        private RelayCommand startPauseCommand;
        private State state;

        private DispatcherTimer timer;

        public string Title => $"{title} : {state}";
        public ImageSource Screen => screenCaptureService.ScreenBuffer;

        public ICommand StartPause {
            get => startPauseCommand;
            set => Set(ref startPauseCommand, (RelayCommand)value);
        }

        public ShellViewModel() {
            InitializeScreenCapture();
            InitializeCommands();
        }

        private void InitializeScreenCapture() {
            screenCaptureService = new ScreenCaptureService();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(1);
            timer.Tick += (s, e) => screenCaptureService.UpdateScreen();
        }

        private void InitializeCommands() {
            startCommand = new RelayCommand(
                () => ChangeState(StartCapture, pauseCommand),
                () => state == State.Stopped, "START");
            pauseCommand = new RelayCommand(
                () => ChangeState(PauseCapture, startCommand),
                () => state == State.Started, "PAUSE");
            ChangeState(PauseCapture, startCommand);
        }

        private void StartCapture() {
            timer.Start();
            state = State.Started;
        }

        private void PauseCapture() {
            timer.Stop();
            state = State.Stopped;
        }

        private void ChangeState(Action action, RelayCommand newStartPauseCommand) {
            action?.Invoke();
            StartPause = newStartPauseCommand;
            OnPropertyChanged(nameof(Title));
        }
    }
}