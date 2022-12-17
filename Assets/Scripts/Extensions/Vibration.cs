using UnityEngine;

public static class Vibration
{
    public static void Vibrate()
    {
        Handheld.Vibrate();
    }

    public static void VibrateIfOn()
    {
        //if (SettingsMapper.GetVibration())
        //{
            Handheld.Vibrate();
        //}
    }
}
