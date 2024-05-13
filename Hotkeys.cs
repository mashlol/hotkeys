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
                new("Exit", Exit)
            }),
            Visible = true
        };
    }

    void Exit(object sender, EventArgs e) {
        trayIcon.Visible = false;

        Application.Exit();
    }
}

public class Hotkeys {

    public static int Main(string[] args) {

        Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>() {
            {Combination.FromString("Alt+Shift+Q"), () => {
                Application.SetSuspendState(PowerState.Suspend, false, false);
            }},
            {Combination.FromString("Alt+Shift+W"), () => {
                SetAudioDevice("Speakers (Realtek(R) Audio)");
            }},
            {Combination.FromString("Alt+Shift+E"), () => {
                SetAudioDevice("Speakers (Yeti X)");
            }},
        });

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TrayOnlyApplicationContext());

        return 0;
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