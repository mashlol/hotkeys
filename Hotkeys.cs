using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoreAudio;

public class TrayOnlyApplicationContext : ApplicationContext {
    private readonly NotifyIcon trayIcon;

    public TrayOnlyApplicationContext() {
        trayIcon = new NotifyIcon() {
            Icon = new Icon(SystemIcons.Exclamation, 40, 40),
            ContextMenu = new ContextMenu(new MenuItem[] {
                new("Set to speakers", (sender, e) => SetToSpeakers()),
                new("Set to headphones", (sender, e) => SetToHeadphones()),
                new("Exit", Exit),
            }),
            Visible = true
        };
    }

    private void Exit(object sender, EventArgs e) {
        trayIcon.Visible = false;

        Application.Exit();
    }

    public static void SetToSpeakers() {
        SetAudioDevice("Speakers (Realtek(R) Audio)");
    }

    public static void SetToHeadphones() {
        SetAudioDevice("Speakers (Yeti X)");
    }

    private static void SetAudioDevice(string deviceName) {
        var deviceEnumerator = new MMDeviceEnumerator(Guid.NewGuid());
        var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        foreach (var device in devices) {
            if (device.DeviceFriendlyName == deviceName) {
                deviceEnumerator.SetDefaultAudioEndpoint(device);
                return;
            }
        }
    }
}

public class Hotkeys {

    public static int Main(string[] args) {

        Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>() {
            {Combination.FromString("Alt+Shift+Q"), () => {
                Application.SetSuspendState(PowerState.Suspend, false, false);
            }},
            {Combination.FromString("Alt+Shift+W"), () => {
                TrayOnlyApplicationContext.SetToSpeakers();
            }},
            {Combination.FromString("Alt+Shift+E"), () => {
                TrayOnlyApplicationContext.SetToHeadphones();
            }},
        });

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TrayOnlyApplicationContext());

        return 0;
    }


}